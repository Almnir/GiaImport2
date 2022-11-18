using Dapper;
using GiaImport2.Enumerations;
using GiaImport2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GiaImport2.Services
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly ICommonRepository CommonRepository;
        //Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public InterviewRepository(ICommonRepository commonRepository)
        {
            this.CommonRepository = commonRepository;
        }

        public async Task<IEnumerable<string>> GetExamDates()
        {
            IEnumerable<string> examDates = null;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(CommonRepository.GetConnection()))
                {
                    sqlConnection.Open();
                    examDates = await sqlConnection.QueryAsync<string>(@"select ExamDate from dat_Exams where TestTypeCode = 9 and SubjectCode = 20");
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                //log.Error(status);
                CommonRepository.GetLogger().Error(status);
            }
            finally
            {
            }
            return examDates;
            //return await Task.FromResult(examDates);
        }

        public async Task<List<Governmentinfo>> GetParticipantsExamsData(string examDate)
        {
            List<Governmentinfo> result = new List<Governmentinfo>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(CommonRepository.GetConnection()))
                {
                    sqlConnection.Open();
                    var participantsExamsQuery = await sqlConnection.QueryAsync<Models.ParticipantsExamsModel>(
                                                        @"select 
                                                                        g.GovernmentCode,
                                                                        g.GovernmentName,
                                                                        s.SchoolID,
                                                                        s.SchoolCode,
                                                                        s.ShortName,
                                                                        p.ParticipantID,
                                                                        p.pClass,
                                                                        p.Surname,
                                                                        p.[Name],
                                                                        p.SecondName,
                                                                        p.DocumentSeries,
                                                                        p.DocumentNumber,
																		p.ParticipantCode,
																		CAST(COALESCE(pp.PValue, '0') AS INT) as Property
													from rbd_ParticipantsExamsOnSchool as peos
                                                    join rbd_Schools as s
                                                      on s.SchoolID = peos.SchoolID
                                                    join rbd_Governments as g
													  on g.GovernmentID = s.GovernmentID
                                                    join rbd_ParticipantsExams as pe
                                                      on pe.ParticipantsExamsID = peos.ParticipantsExamsID
                                                    join rbd_Participants as p
                                                      on p.ParticipantID = pe.ParticipantID
                                                    join dat_Exams as e
                                                      on e.ExamGlobalID = pe.ExamGlobalID
                                                    left join rbd_ParticipantProperties as pp
													  on pp.ParticipantId = p.ParticipantID and
													     pp.Property = 7
                                                    where g.DeleteType = 0 and 
													      s.DeleteType = 0 and 
														  p.DeleteType = 0 and 
														  ISNULL(pe.IsDeleted, 0) = 0 and
														  peos.IsDeleted = 0 and 
														  e.ExamDate = @ExamDate 
                                                    order by g.GovernmentCode, s.SchoolCode
                                                    ", new { @ExamDate = examDate });
                    var loaderSheets = await sqlConnection.QueryAsync<Models.SheetsModel>(
                        @"select ParticipantID, SheetID, Barcode, Reserve01, PackageFK
                             from loader.sht_sheets_R
                             where TestTypeCode = 9 and
                                   SubjectCode = 20 and
                                   ExamDate = @ExamDate", new { @ExamDate = examDate });

                    foreach (var item in participantsExamsQuery)
                    {
                        FileStatus fileStatus = FileStatus.Exported;
                        Guid sheetRid = Guid.Empty;
                        Guid packageId = Guid.Empty;
                        string barcode = null, kimcode = null;
                        // если уже выгружались, то помечаем как повторную выгрузку
                        var shrdto = loaderSheets.Where(a => a.ParticipantID == item.ParticipantID).FirstOrDefault();
                        if (shrdto != null)
                        {
                            packageId = shrdto.PackageId;
                            fileStatus = FileStatus.ConditionReexport;
                            sheetRid = shrdto.SheetID;
                            barcode = shrdto.Barcode;
                            kimcode = shrdto.Kimcode;
                        }
                        Participantinfo participantinfo = new Participantinfo(
                            item.ParticipantID,
                            item.pClass,
                            item.Surname,
                            item.Name,
                            item.SecondName,
                            item.DocumentSeries,
                            item.DocumentNumber,
                            fileStatus,
                            sheetRid,
                            barcode,
                            kimcode,
                            packageId,
                            item.ParticipantCode,
                            item.Property
                            );

                        Governmentinfo governmentInfo = result.Where(x=>x.GovernmentCode == item.GovernmentCode).FirstOrDefault();
                        Schoolinfo school = null;
                        bool newGov = false;
                        if (governmentInfo != null)
                        {
                            school = governmentInfo.FindSchoolByID(item.SchoolID);
                        }
                        else
                        {
                            newGov = true;
                            governmentInfo = new Governmentinfo
                            {
                                GovernmentCode = item.GovernmentCode,
                                // а вот теперь будут коды ОИВ(МСУ)
                                GovernmentName = item.GovernmentCode.ToString()
                            };
                        }
                        // если школа не найдена
                        if (school == null)
                        {
                            Schoolinfo schoolinfo = new Schoolinfo(
                                item.SchoolID,
                                item.SchoolCode,
                                "ОО №" + item.SchoolCode.ToString(),
                                item.ShortName,
                                new List<Participantinfo>() { participantinfo }
                                );
                            governmentInfo.Schools.Add(schoolinfo);
                        }
                        // если школа есть уже 
                        else
                        {
                            school.Participants.Add(participantinfo);
                        }
                        if (newGov == true) { result.Add(governmentInfo); }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
            return await Task.FromResult(result);
        }
    }
}
