using DevExpress.Utils.Extensions;
using GiaImport2.DataModels;
using GiaImport2.Enumerations;
using GiaImport2.Models;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using MFtcFinalInterview;
using MFtcPck;
using MFtcPck.FtcXml;
using MFtcUtils.Digest.Enumerators;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container = SimpleInjector.Container;

namespace GiaImport2.Services
{
    public class ExportFinalInterviewService
    {
        //private string examDate = "2018.02.14";
        private string examDate;

        private CancellationTokenSource source;

        private BackgroundWorker bw;

        private string expFolder;
        private int currentregion;
        private string fileName;
        private Action<ExportInterviewDto> AddFileToView;
        private Action CloseProgressBar;
        private List<Governmentinfo> Governmentinfos;
        private Action<int> Progressbar;
        private Action<string> FileLabel;
        List<(int School, string Class)> SchoolClassInfo;
        //ConcurrentDictionary<string, int> filesList;
        BarcodeGenerator barcodeGenerator;
        Guid SessionId;
        LicenseType License;
        ChooseSchoolsOrClass SchoolsOrClass;

        private int total;
        private readonly ICommonRepository CommonRepository;
        private readonly Container DIContainer;

        public ExportFinalInterviewService(Container container, ICommonRepository commonRepository)
        {
            this.DIContainer = container;
            this.CommonRepository = commonRepository;
        }
        public void Init(
            string exportFolder,
            Action<ExportInterviewDto> addFileToView,
            string examDate,
            Action<int> progressbar,
            Action<string> fileLabel,
            Action closeProgressBar,
            List<(int School, string Class)> schoolClassInfo,
            List<Governmentinfo> governmentinfos,
            ChooseSchoolsOrClass schoolsOrClass,
            CancellationTokenSource source
            )
        {
            this.expFolder = exportFolder;
            this.AddFileToView = addFileToView;
            this.examDate = examDate;
            this.Progressbar = progressbar;
            this.FileLabel = fileLabel;
            this.source = source;
            this.CloseProgressBar = closeProgressBar;
            this.Governmentinfos = governmentinfos;
            this.SchoolsOrClass = schoolsOrClass;
            this.SchoolClassInfo = schoolClassInfo;
            barcodeGenerator = DIContainer.GetInstance<BarcodeGenerator>();

            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }

        protected void DoWork(object sender, DoWorkEventArgs e)
        {
            List<Governmentinfo> governmentschools = (List<Governmentinfo>)e.Argument;
            bw.ReportProgress(0);
            string status = string.Empty;
            MainProcess(governmentschools, e, out status);
            bw.ReportProgress(100);
        }

        protected void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int percentage = e.ProgressPercentage;
            // показываем процент выполнения
            Progressbar(percentage);
        }

        protected void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // закрываем сессию
            if (CommonRepository.TryCloseFinalInterviewSession(SessionId) == false)
            {
                CommonRepository.GetLogger().Error("Невозможно закрыть сессию!");
            }
            if (e.Cancelled)
            {
                FormsHelper.ShowStyledMessageBox("Внимание!", "Операция выгрузки была прервана!");
                CommonRepository.GetLogger().Warn("Операция выгрузки была прервана!");
                // закрываем прогрессбар
                CloseProgressBar();
            }
            else if (e.Error != null)
            {
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Выгрузка произошла с ошибками!");
                CloseProgressBar();
            }
            else
            {
                CloseProgressBar();
            }
        }

        private void MainProcess(List<Governmentinfo> governmentschools, DoWorkEventArgs e, out string status)
        {
            try
            {
                status = string.Empty;
                total = GetTotalBarCount(governmentschools);
                int i = 1;
                foreach (var gs in governmentschools)
                {
                    // рассчитываем подкаталог прибавляя АТЕ
                    string govDir = Path.Combine(
                        this.expFolder,
                        MakeSafePathName(string.IsNullOrWhiteSpace(gs.GovernmentName) ? NullReplacer("ОИВ") : gs.GovernmentName, ' ')
                        );
                    Directory.CreateDirectory(govDir);
                    foreach (var sc in gs.Schools)
                    {
                        // рассчитываем подкаталог прибавляя Школу
                        string schDir = Path.Combine(
                            govDir,
                            MakeSafePathName(string.IsNullOrWhiteSpace(sc.SchoolName) ? NullReplacer("Школа") : sc.SchoolName, ' '));
                        Directory.CreateDirectory(schDir);
                        Guid newPackageId = Guid.NewGuid();
                        // рассчитываем подкаталог прибавляя Дату экзамена
                        string examDir = Path.Combine(
                            schDir,
                            MakeSafePathName(string.IsNullOrWhiteSpace(this.examDate) ? NullReplacer("дата экзамена") : this.examDate, ' '));
                        Directory.CreateDirectory(examDir);
                        int participantsCount = 0;
                        // если в настройках выставлено учитывать разбиение по классам
                        if (this.SchoolsOrClass == ChooseSchoolsOrClass.Classes)
                        {
                            var pclassesParicipants = sc.Participants
                                .GroupBy(c => c.PClass)
                                .ToDictionary(pp => pp.Key, pp => pp.ToList());
                            // создаём классы в подкаталоге школы, проходя по классам
                            foreach (var pclassPart in pclassesParicipants)
                            {
                                participantsCount = pclassPart.Value.Count();
                                string parentDir = Path.Combine(
                                    examDir,
                                    MakeSafePathName(string.IsNullOrWhiteSpace(pclassPart.Key) ? NullReplacer("Класс") : pclassPart.Key, ' '));
                                Directory.CreateDirectory(parentDir);
                                this.fileName = ConstructFileName(
                                    gs.GovernmentCode,
                                    sc.SchoolCode,
                                    MakeSafePathName(string.IsNullOrWhiteSpace(pclassPart.Key) ? NullReplacer("Класс") : pclassPart.Key, ' '));
                                // проверим на уже существующий packageID
                                Guid guid;
                                if (CheckPackageAlreadyExported(sc.SchoolCode, out guid))
                                {
                                    // если найдено, то берём найденное, иначе пользуемся новым значением
                                    newPackageId = guid;
                                }
                                CreateXMLFile(pclassPart.Value, gs.GovernmentCode, sc.SchoolCode, newPackageId, parentDir, out status);
                                // отлавливаем отмену
                                if (bw.CancellationPending || source.IsCancellationRequested) { e.Cancel = true; return; }
                                // если файл создан успешно, только тогда делаем запись в sht_Packages
                                if (string.IsNullOrEmpty(status))
                                {
                                    string cstatus = string.Empty;
                                    if (InsertNewPackageRow(gs.GovernmentCode, sc.SchoolCode, participantsCount, newPackageId, out cstatus) == false)
                                    {
                                        status += cstatus;
                                        File.Delete(this.fileName);
                                        return;
                                    }
                                }
                                newPackageId = Guid.NewGuid();
                                int percent = (i * 100) / total;
                                bw.ReportProgress(percent);
                                i += 1;
                            }
                        }
                        // если не учитывать по классам, только по школам
                        else
                        {
                            participantsCount = sc.Participants.Count();
                            this.fileName = ConstructFileName(gs.GovernmentCode, sc.SchoolCode, null);
                            // проверим на уже существующий packageID
                            Guid guid;
                            if (CheckPackageAlreadyExported(sc.SchoolCode, out guid))
                            {
                                // если найдено, то берём найденное, иначе пользуемся новым значением
                                newPackageId = guid;
                            }
                            CreateXMLFile(sc.Participants, gs.GovernmentCode, sc.SchoolCode, newPackageId, examDir, out status);
                            // отлавливаем отмену
                            if (bw.CancellationPending || source.IsCancellationRequested) { e.Cancel = true; return; }
                            // если файл создан успешно, только тогда делаем запись в sht_Packages
                            if (string.IsNullOrEmpty(status))
                            {
                                string cstatus = string.Empty;
                                if (InsertNewPackageRow(gs.GovernmentCode, sc.SchoolCode, participantsCount, newPackageId, out cstatus) == false)
                                {
                                    status += cstatus;
                                    File.Delete(this.fileName);
                                    return;
                                }
                            }
                            int percent = (i * 100) / total;
                            bw.ReportProgress(percent);
                            i += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При выполнении создания структуры каталогов произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
            }
        }

        private int GetTotalBarCount(List<Governmentinfo> governmentschools)
        {
            int total = 0;
            if (this.SchoolsOrClass == ChooseSchoolsOrClass.Classes)
            {
                total = governmentschools.SelectMany(sc => sc.Schools)
                    .SelectMany(p => p.Participants, (s, o) => new { s.SchoolCode, o.PClass })
                    .GroupBy(c => new { c.SchoolCode, c.PClass }).Count();
            }
            else
            {
                total = governmentschools.SelectMany(sc => sc.Schools).Count();
            }
            return total;
        }

        /// <summary>
        /// Точка входа
        /// </summary>
        public void ExportAllExistingData()
        {
            if (!CommonRepository.CheckAccessToFolder(this.expFolder))
            {
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Нет доступа к указанному каталогу!");
                CloseProgressBar();
                return;
            }
            string error = "";
            this.currentregion = CommonRepository.GetCurrentRegion(out error);
            if (error.Length != 0)
            {
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Невозможно получить код региона!");
                CloseProgressBar();
                return;
            }
            // пробуем получить лицензии из БД
            //List<MFtcLicInfo> licenses;
            //if (CommonRepository.TryGetFinalInterviewLicense(this.currentregion, out licenses) == true && licenses != null && licenses.Count > 0)
            //{
            //    // берём наивысшую?
            //    this.License = licenses.Where(x => x.TestType == TestTypeNS.SS9).OrderByDescending(x => x.LicenseType).Select(y => y.LicenseType).FirstOrDefault();
            //}
            //else
            //{
            //    // пробуем получить из файла
            //    // ищем первый файл с раcширением .ftclc выше по каталогу
            //    var licFiles = Directory.GetFiles(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "*.ftclc");
            //    foreach (var licFile in licFiles)
            //    {
            //        error = "";
            //        MFtcLicInfo licinfo = new MFtcLicReader().ReadFile(licFile, this.currentregion, out error);
            //        if (string.IsNullOrEmpty(error))
            //        {
            //            licenses.Add(licinfo);
            //        }
            //    }
            //    if (licFiles == null || licFiles.Length == 0)
            //    {
            //        // если совсем ничего нету
            //        FormsHelper.ShowStyledMessageBox("Ошибка лицензии!", "Лицензия не активирована!");
            //        CloseProgressBar();
            //        return;
            //    }
            //    // берём наивысшую?
            //    this.License = licenses.Where(x => x.TestType == TestTypeNS.SS9).OrderByDescending(x => x.LicenseType).Select(y => y.LicenseType).FirstOrDefault();
            //    if (this.License == 0)
            //    {
            //        // если совсем ничего нету
            //        FormsHelper.ShowStyledMessageBox("Ошибка лицензии!", "Лицензия не активирована!");
            //        CloseProgressBar();
            //        return;
            //    }
            //}
            // создаём сессию
            Guid sessionid;
            if (CommonRepository.TryCreateFinalInterviewSession(out sessionid) == false)
            {
                CommonRepository.GetLogger().Error("Невозможно создать сессию!");
                FormsHelper.ShowStyledMessageBox("Ошибка!", "Невозможно создать сессию!");
                CloseProgressBar();
                return;
            }
            this.SessionId = sessionid;
            // получаем данные распределения участников
            //List<Governmentinfo> governmentschools = GetData();
            if (this.Governmentinfos.Count() == 0 || this.SchoolClassInfo.Count == 0)
            {
                FormsHelper.ShowStyledMessageBox("Предупреждение!", "Данных об участниках нет, отсутствует распределение на экзамен, либо они уже были выгружены.");
                CloseProgressBar();
                return;
            }
            this.Governmentinfos = CleanHierarchy(this.Governmentinfos);
            error = "";
            bw.DoWork += DoWork;
            bw.ProgressChanged += ProgressChanged;
            bw.RunWorkerCompleted += RunWorkerCompleted;
            bw.RunWorkerAsync(Governmentinfos);
        }

        private string NullReplacer(string place)
        {
            return place + " " + Guid.NewGuid().ToString().Substring(0, 8);
        }

        public string MakeSafePathName(string pathname, char replaceChar)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                pathname = pathname.Replace(c, replaceChar);
            }
            pathname = pathname.Replace('"', replaceChar);
            if (pathname.Equals(' '))
            {
                pathname = pathname.Replace(' ', '-');
            }
            return pathname;
        }

        private bool InsertNewPackageRow(int governmentCode, int schoolCode, int participantCount, Guid newID, out string status)
        {
            status = "";
            bool result = true;
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    db.sht_Packages.SchemaName("loader").Insert(() => new sht_Package
                    {
                        PackageID = newID,
                        REGION = this.currentregion,
                        RegionCode = this.currentregion,
                        FileName = this.fileName,
                        DepartmentCode = this.fileName.Substring(0, 4),
                        TestTypeCode = 9,
                        SubjectCode = 20,
                        ExamDate = examDate,
                        StationCode = schoolCode,
                        AuditoriumCode = "-1",
                        Condition = 10,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        SheetsCount = participantCount,
                        IsExported = false,
                        ProjectBatchID = -1
                    });
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                result = false;
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
            return result;
        }

        private bool InsertNewRegBlankRow(int schoolCode, Guid packageId, Guid participantId, Guid sheetId, string barcode)
        {
            string status = "";
            bool result = true;
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    db.sht_Sheets_R.SchemaName("loader").Insert(() => new sht_Sheets_R
                    {
                        SheetID = sheetId,
                        REGION = this.currentregion,
                        RegionCode = this.currentregion,
                        FileName = this.fileName,
                        DepartmentCode = this.fileName.Substring(0, 4),
                        TestTypeCode = 9,
                        SubjectCode = 20,
                        // TODO: брать из имени пакета!
                        ExamDate = examDate,
                        StationCode = schoolCode,
                        SchoolCode = schoolCode,
                        AuditoriumCode = "-1",
                        Condition = 1,
                        ProjectBatchID = -1,
                        PackageFK = packageId,
                        HasSignature = false,
                        Barcode = barcode,
                        Reserve01 = barcode.Substring(5, 7),
                        ParticipantID = participantId,
                        ImageNumber = 0,
                        VariantCode = 0,
                        Sex = 0
                    });
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                result = false;
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
            return result;
        }

        private bool CheckPackageAlreadyExported(int schoolCode, out Guid guid)
        {
            bool result = true;
            DataConnection dc = null;
            guid = Guid.Empty;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    IEnumerable<Guid> query = db.sht_Packages.SchemaName("loader")
                        .Where(a => a.REGION == this.currentregion &&
                        a.FileName == this.fileName &&
                        a.StationCode == schoolCode
                        ).Select(p => p.PackageID);
                    if (query.Count() == 0)
                    {
                        result = false;
                    }
                    else
                    if (query.Count() > 1)
                    {
                        CommonRepository.GetLogger().Error("Дублирование записей в loader.sht_Packages для ППЭ:{0}", schoolCode);
                    }
                    guid = query.FirstOrDefault();
                    if (guid != null && !guid.Equals(Guid.Empty))
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonRepository.GetLogger().Error(string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString()));
                result = false;
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
            return result;
        }

        private void CreateXMLFile(List<Participantinfo> participantInfo, int governmentCode, int schoolCode, Guid packageID, string parentDir, out string status)
        {
            status = string.Empty;
            HwPackage package = new HwPackage
            {
                Owner = "Federal Test Center",
                // Состояние выгрузки-загрузки
                //Registered = 10,        // зарегистрирован
                //Imported = 11,        // импортирован/выгружен повторно
                //SavedNoCheck = 12,        // Сохранён без проверки
                //SavedWithErrors = 13,        // Сохранён с ошибками после проверки
                //CheckedAndSaved = 19,        // Сохранён после проверки (без ошибок)
                //Uploaded = 20,           // загружены бланки (без ошибок)
                Condition = 10,
                // Установка версии
                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            };
            // проверяем на повтор
            //if (CheckAlreadyExported(schoolCode) == true)
            //{
            //    package.Condition = 11;
            //}
            // сортируем список учащихся
            List<Participantinfo> sortedList = participantInfo.OrderBy(p => p.Surname).ToList();
            int i = 0;
            int pageNum = 1;
            do
            {
                FtcPage page = new FtcPage();
                page.PageTemplate = "1";
                page.PageNumber = pageNum;
                // выборка участников по определённому количеству на страницу
                var group = sortedList.Skip(i).Take(Globals.PARTICIPANTS_IN_PAGE).ToList();
                //FillCommonBlocks(areaCode, schoolCode, ref page);
                FtcBlock block = new FtcBlock();
                block.BlockName = "Регион";
                block.Value = this.currentregion.ToString("00");
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Код_ППЭ";
                block.Value = schoolCode.ToString("000000");
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Код_предмета";
                // TODO: hardcode
                block.Value = "20";
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Код_МСУ";
                block.Value = governmentCode.ToString();
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Название_предмета";
                block.Value = "Итоговое собеседование по русскому языку";
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Дата";
                block.Value = string.Join("-", examDate.Split(new[] { "." }, StringSplitOptions.None).Select(st => st.Substring(st.Length - 2, 2)).Reverse());
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Номер_изображения";
                block.Value = page.PageNumber.ToString();
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Код_1";
                block.Value = "";
                page.blocks.Add(block);
                block = new FtcBlock();
                block.BlockName = "Хеш_документа";
                block.Value = "";
                page.blocks.Add(block);
                //
                int pindex = 1;

                // добить до числа на страницу
                while (group.Count < Globals.PARTICIPANTS_IN_PAGE)
                {
                    group.Add(new Participantinfo(Guid.Empty, "", "", "", "", "", "", FileStatus.Exported, Guid.Empty, "", "", Guid.Empty, "", 0));
                }
                foreach (var pinfo in group)
                {
                    // постфиксный индекс штото01
                    string pindexPostfix = pindex.ToString("00");
                    // блоки
                    block = new FtcBlock("Фамилия" + pindexPostfix, pinfo.Surname);
                    page.blocks.Add(block);
                    block = new FtcBlock("Имя" + pindexPostfix, pinfo.Name);
                    page.blocks.Add(block);
                    block = new FtcBlock("Отчество" + pindexPostfix, pinfo.Secondname);
                    page.blocks.Add(block);
                    block = new FtcBlock("ФИО_участника" + pindexPostfix, GetParticipantName(pinfo.Surname, pinfo.Name, pinfo.Secondname));
                    page.blocks.Add(block);
                    block = new FtcBlock("Класс" + pindexPostfix, pinfo.PClass);
                    page.blocks.Add(block);
                    block = new FtcBlock("Аудитория" + pindexPostfix, "");
                    page.blocks.Add(block);
                    block = new FtcBlock("Документ_серия" + pindexPostfix, pinfo.DocSeries);
                    page.blocks.Add(block);
                    block = new FtcBlock("Документ_номер" + pindexPostfix, pinfo.DocNumber);
                    page.blocks.Add(block);
                    block = new FtcBlock("Номер_варианта" + pindexPostfix, "");
                    page.blocks.Add(block);
                    // блоки оценок
                    for (int columnIndex = 1; columnIndex < (ParticipantData.CRITERIES_COUNT + 1); columnIndex++)
                    {
                        string columnPostfix = columnIndex.ToString("00");
                        // английская C
                        block = new FtcBlock("С_" + pindexPostfix + columnPostfix, "");
                        page.blocks.Add(block);
                    }
                    // блоки промежуточных итогов
                    for (int columnIndex = 1; columnIndex < (ParticipantData.SEMIANSWERS_COUNT + 1); columnIndex++)
                    {
                        string columnPostfix = columnIndex.ToString("00");
                        // английская D
                        block = new FtcBlock("D_" + pindexPostfix + columnPostfix, "");
                        page.blocks.Add(block);
                    }
                    //
                    block = new FtcBlock("Итоговый_Балл" + pindexPostfix, "");
                    page.blocks.Add(block);
                    block = new FtcBlock("Зачёт" + pindexPostfix, "");
                    page.blocks.Add(block);
                    block = new FtcBlock("ФИО Эксперта" + pindexPostfix, "");
                    page.blocks.Add(block);
                    block = new FtcBlock("Не_закончил" + pindexPostfix, "");
                    page.blocks.Add(block);
                    // Код_в_базе - это будет ParticipantID
                    string pid = pinfo.ParticipantId.Equals(Guid.Empty) ? "" : pinfo.ParticipantId.ToString();
                    block = new FtcBlock("Код_в_базе" + pindexPostfix, pid);
                    page.blocks.Add(block);

                    string barCode = null;
                    string kimCode = null;
                    Guid sheetId = Guid.Empty;
                    // проверяем, если экспортируется в первый раз
                    if (!pinfo.ParticipantId.Equals(Guid.Empty) && pinfo.Condition == FileStatus.Exported)
                    {
                        // сгенерировали баркод и кимкод
                        barCode = this.barcodeGenerator.GetNextBarcode();
                        kimCode = barCode.Substring(5, 7);
                        // сгенерировали SheetID
                        sheetId = Guid.NewGuid();
                        // добавляем пакет со сгенерированным PackageId
                        page.CRC = packageID.ToString();
                        // добавляем бланк регистрации
                        InsertNewRegBlankRow(schoolCode, packageID, pinfo.ParticipantId, sheetId, barCode);
                    }
                    // проверяем, если экспортируется повторно
                    else if (!pinfo.ParticipantId.Equals(Guid.Empty) && pinfo.Condition == FileStatus.ConditionReexport)
                    {
                        // выставляем пакет
                        package.Condition = System.Convert.ToInt32(pinfo.Condition);
                        // берём идентификатор найденного бланка уже заведённого
                        sheetId = pinfo.SheetId;
                        // уже найденные штрихкод и ким
                        barCode = pinfo.Barcode;
                        kimCode = pinfo.Kimcode;
                        // добавляем пакет с найденным PackageId
                        page.CRC = pinfo.PackageId.ToString();
                        // выставляем статус для несоздания пакета
                        status += string.Format("Повторная выгрузка(пакет уже создан): {0}; ", pinfo.PackageId.ToString());
                    }
                    // генерация SheetID, это будет Локальный_ID
                    string lid = pinfo.ParticipantId.Equals(Guid.Empty) ? "" : sheetId.ToString();
                    block = new FtcBlock("Локальный_ID" + pindexPostfix, lid);
                    page.blocks.Add(block);
                    // штрихкод Barcode
                    block = new FtcBlock("Штрих_код" + pindexPostfix, pinfo.ParticipantId.Equals(Guid.Empty) ? "" : barCode);
                    page.blocks.Add(block);
                    // ким
                    block = new FtcBlock("Номер_КИМ_" + pindexPostfix, pinfo.ParticipantId.Equals(Guid.Empty) ? "" : kimCode);
                    page.blocks.Add(block);
                    // код участника
                    block = new FtcBlock("Код_участника" + pindexPostfix, pinfo.ParticipantCode ?? "");
                    page.blocks.Add(block);
                    // признак овз
                    block = new FtcBlock("РезервI" + pindexPostfix, pinfo.Property == 1 ? "22" : "");
                    page.blocks.Add(block);

                    pindex += 1;
                }

                package.pages.Add(page);

                // увеличиваем на количество участников на страницу
                i += Globals.PARTICIPANTS_IN_PAGE;
                // инкрементируем страницу
                pageNum += 1;
            } while (i < sortedList.Count);
            // подсчёт CRC: MD5(все значения Block + condition + version)
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(package.Condition).Append(package.Version);
            foreach (var page in package.Pages)
            {
                foreach (var block in page.Blocks)
                {
                    stringBuilder.Append(block.Value);
                }
            }
            package.CRC = FinalInterview.CreateMD5(stringBuilder.ToString());
            //package.CRC = FinalInterview.CreateCRC32(stringBuilder.ToString());
            // заполнение хидера
            // версия библиотеки
            package.Header.LibraryVersion = MFtcUtils.Digest.Dictionaries.LibVersions[ProjectType.BSE];
            // тип проекта?
            package.Header.ProjectType = ProjectType.BSE;
            // файл
            package.Header.FileName = this.fileName;
            // регион
            package.Header.RegionCode = this.currentregion;
            // ППЭ = Школа
            package.Header.StationCode = schoolCode;
            // ИС
            package.Header.SubjectCode = 20;
            // КОД АТЕ
            package.Header.AuditoriumCode = governmentCode.ToString();
            // дата экзамена
            package.Header.ExamDate = DateTime.Parse(examDate);
            // Лицензия
            package.Header.LicenseType = this.License;
            // статус
            package.Header.PackageCondition = PackageCondition.Registered;
            // версия ПО
            package.Header.Version = Assembly.GetExecutingAssembly().GetName().Version;
            // версии продукта
            if (Properties.Settings.Default.Base == null || Properties.Settings.Default.Version == null)
            {
                CommonRepository.GetLogger().Fatal("неисправный файл конфигурации, нет версий ПО");
            }
            if (package.Header.SupportedVersions == null)
            {
                package.Header.SupportedVersions = new List<Version>();
            }
            // базовая (т.е. для ручного ввода)
            package.Header.SupportedVersions.Add(Version.Parse(Properties.Settings.Default.Base));
            // текущая
            package.Header.SupportedVersions.Add(Version.Parse(Properties.Settings.Default.Version));
            try
            {
                using (Stream file = File.Create(Path.Combine(parentDir, this.fileName + ".b2p")))
                {
                    Packager<HwPackage>.Write(package, file);
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При выполнении сериализации B2P файла произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Fatal(status);
            }
            //filesList.TryAdd(this.fileName + ".b2p", sortedList.Count);
            // добавляем файл в грид
            AddFileToView(
                new ExportInterviewDto()
                {
                    FileName = this.fileName,
                    GovernmentName = governmentCode.ToString(),
                    SchoolName = schoolCode.ToString(),
                    FileCRC = package.CRC,
                    FileSize = (int)new FileInfo(Path.Combine(parentDir, this.fileName + ".b2p")).Length
                }
                );
            // добавляем в прогрессбар
            FileLabel(this.fileName);
            Task.Delay(2000).Wait();
        }

        private string ConstructFileName(int governmentCode, int schoolCode, string pclass)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.currentregion.ToString("00"))
                // представительство(центр обработки)
                .Append("00")
                // код предмета
                .Append("20")
                // вид тестирования
                .Append("9")
                // этап проведения
                .Append("0")
                .Append("-")
                // код типа бланка
                .Append("06")
                .Append("-")
                .Append(schoolCode.ToString("000000"))
                .Append("-")
                .Append(governmentCode.ToString("000"))
                .Append(pclass != null ? "(" + pclass + ")" : "")
                .Append("-")
                .Append(examDate.Replace(".", ""));
            // убрал Guid к чертям собачим...
            //.Append("-")
            //.Append(newID.ToString().Replace("-", " "));
            return sb.ToString();
        }

        private string GetParticipantName(string surname, string name, string secondname)
        {
            string secondNameStr = string.IsNullOrWhiteSpace(secondname) ? "" : secondname.Substring(0, 1);
            string nameStr = string.IsNullOrWhiteSpace(name) ? "" : name.Substring(0, 1);
            return surname + " " + nameStr + secondNameStr;
        }

        public class QueryDto
        {
            public int GovernmentCode { get; set; }
            public string GovernmentName { get; set; }
            public Guid SchoolID { get; set; }
            public int SchoolCode { get; set; }
            public string ShortName { get; set; }
            public Guid ParticipantID { get; set; }
            public string pClass { get; set; }
            public string Surname { get; set; }
            public string Name { get; set; }
            public string SecondName { get; set; }
            public string DocumentSeries { get; set; }
            public string DocumentNumber { get; set; }
            public string ParticipantCode { get; set; }
            public int Property { get; set; }
        }

        public class SheetsRDTO
        {
            public Guid ParticipantID { get; set; }
            public Guid SheetID { get; set; }
            public string Barcode { get; set; }
            public string Kimcode { get; set; }
            public Guid PackageId { get; set; }
        }

        private Governmentinfo FindGovernment(List<Governmentinfo> gi, int GovernmentCode)
        {
            foreach (var item in gi)
            {
                if (item.GovernmentCode.Equals(GovernmentCode))
                    return item;
            }
            return null;
        }

        private static void CheckExistingFreeDiskSpace(string exportFolder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод для урезания общего датасета выборки по выбранному
        /// </summary>
        /// <param name="governmentinfos"></param>
        /// <returns></returns>
        private List<Governmentinfo> CleanHierarchy(List<Governmentinfo> governmentinfos)
        {
            var outinfos = new List<Governmentinfo>();
            foreach (var gv in governmentinfos)
            {
                var govinfo = new Governmentinfo();
                govinfo.GovernmentCode = gv.GovernmentCode;
                govinfo.GovernmentName = gv.GovernmentName;
                foreach (var sc in gv.Schools)
                {
                    if (this.SchoolClassInfo.Any(x=>x.School == sc.SchoolCode))
                    {
                        var partspnts = new List<Participantinfo>();
                        foreach (var par in sc.Participants)
                        {
                            if (this.SchoolClassInfo.Any(x => x.Class == par.PClass && x.School == sc.SchoolCode) 
                                                         || this.SchoolsOrClass == ChooseSchoolsOrClass.Schools)
                            {
                                var partnt = new Participantinfo(par.ParticipantId, par.PClass, par.Surname, par.Name, par.Secondname, par.DocSeries,
                                    par.DocNumber, par.Condition, par.SheetId, par.Barcode, par.Kimcode, par.PackageId, par.ParticipantCode, par.Property);
                                partspnts.Add(partnt);
                            }
                        }
                        if (partspnts.Count > 0)
                        {
                            Schoolinfo school = new Schoolinfo(sc.SchoolID, sc.SchoolCode, sc.SchoolName, sc.ShortName, partspnts);
                            govinfo.Schools.Add(school);
                        }
                    }
                }
                if (govinfo.Schools.Count > 0)
                {
                    outinfos.Add(govinfo);
                }
            }
            return outinfos;
        }
    }
}
