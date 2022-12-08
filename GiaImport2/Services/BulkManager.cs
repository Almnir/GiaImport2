using GiaImport2.DataModels;
using GiaImport2.Enumerations;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using MFtcFinalInterview;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GiaImport2.Services
{
    class BulkManager
    {

        public ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> errorDict = new ConcurrentDictionary<string, Tuple<string, long, TimeSpan>>();

        public StringBuilder outLog = new StringBuilder();

        private readonly ICommonRepository CommonRepository;

        public BulkManager(ICommonRepository commonRepository)
        {
            this.CommonRepository = commonRepository;
        }

        public void RunStoredSynchronize()
        {
            int errorCount = 0;
            this.outLog = new StringBuilder();
            try
            {
                using (var conn = new SqlConnection(CommonRepository.GetConnection()))
                using (var command = new SqlCommand("loader.Synchronize", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@TableGroup", SqlDbType.SmallInt).Value = 0;
                    command.Parameters.Add("@SkipErrors", SqlDbType.Bit).Value = 0;
                    command.CommandTimeout = 3600;
                    SqlParameter returnParameter = command.Parameters.Add("@error_count", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;
                    conn.InfoMessage += WriteProcedureLog;
                    conn.FireInfoMessageEventOnUserErrors = true;
                    conn.Open();
                    command.ExecuteNonQuery();
                    if (returnParameter != null && returnParameter.Value != null)
                    {
                        errorCount = (int)returnParameter.Value;
                        if (errorCount != 0)
                        {
                            CommonRepository.GetLogger().Error("Ошибки слияния: " + errorCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении слияния была обнаружена ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                CommonRepository.GetLogger().Error(this.outLog.ToString());
                throw new Exception(status);
            }
        }

        public void RunStoredComplection(string packageMask)
        {
            this.outLog.Clear();
            try
            {
                using (var conn = new SqlConnection(CommonRepository.GetConnection()))
                using (var command = new SqlCommand("results.ftc_ComplectSheets", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = packageMask;
                    command.Parameters.Add("@CreateNotExisting", SqlDbType.Bit).Value = 1;
                    command.Parameters.Add("@CheckExamFinish", SqlDbType.Bit).Value = 0;
                    command.CommandTimeout = 3600;
                    conn.InfoMessage += WriteProcedureLog;
                    conn.FireInfoMessageEventOnUserErrors = true;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении комплектования была обнаружена ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                CommonRepository.GetLogger().Error(this.outLog.ToString());
                throw new Exception(status);
            }
        }

        public void RunStoredConvertation(string packageMask)
        {
            this.outLog.Clear();
            try
            {
                using (var conn = new SqlConnection(CommonRepository.GetConnection()))
                using (var command = new SqlCommand("results.ftc_ConvertSheets", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = packageMask;
                    command.CommandTimeout = 3600;
                    conn.InfoMessage += WriteProcedureLog;
                    conn.FireInfoMessageEventOnUserErrors = true;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении конвертации была обнаружена ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                CommonRepository.GetLogger().Error(this.outLog.ToString());
                throw new Exception(status);
            }
        }

        public void RunDeletedSynchronize(int tableGroup)
        {
            this.outLog.Clear();
            try
            {
                using (var conn = new SqlConnection(CommonRepository.GetConnection()))
                using (var command = new SqlCommand("loader.CleanupTables", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@TableGroup", SqlDbType.SmallInt).Value = tableGroup;
                    command.Parameters.Add("@SkipErrors", SqlDbType.Bit).Value = 0;
                    command.CommandTimeout = 3600;
                    conn.InfoMessage += WriteProcedureLog;
                    conn.FireInfoMessageEventOnUserErrors = true;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении очистки основных таблиц была обнаружена ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                CommonRepository.GetLogger().Error(this.outLog.ToString());
                throw new Exception(status);
            }
        }

        public void FinishExam(string examDate, int region)
        {
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {

                    Guid examFinishId = db.GetTable<sht_ExamFinish>().SchemaName("dbo")
                        .Where(e => e.ExamDate == examDate && e.REGION == region)
                        .Select(de => de.ExamFinishID).FirstOrDefault();
                    if (examFinishId.Equals(Guid.Empty))
                    {
                        db.GetTable<sht_ExamFinish>().SchemaName("dbo")
                            .Insert(() => new sht_ExamFinish
                            {
                                REGION = region,
                                ExamFinishID = Guid.NewGuid(),
                                ExamDate = examDate,
                                SubjectCode = 20,
                                TestTypeCode = 9,
                                FinishTime = DateTime.Now.ToString(),
                                FinishUserName = CommonRepository.GetCredentials().UserName
                            });
                    }
                    else
                    {
                        db.GetTable<sht_ExamFinish>().SchemaName("dbo")
                            .Where(ef => ef.ExamDate == examDate && ef.REGION == region)
                            .Set((n) => n.FinishTime, DateTime.Now.ToString())
                            .Update();

                    }
                    string lq = db.LastQuery;
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Fatal(status);
                throw new Exception(status);
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }

        public void UpdateHumanTestsCondition(string packageMask)
        {
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    // дейтим статус тестов в res_HumanTests в 6, типа успешно обработали
                    db.GetTable<res_HumanTest>().SchemaName("dbo")
                        .Where(t => t.FileName.Contains(packageMask) && t.ProcessCondition == 4)
                    .Set((n) => n.ProcessCondition, 6)
                    .Update();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Fatal(status);
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
        }

        public string GetPackageMask(out string status)
        {
            status = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                int region;
                using (var dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    var query = db.GetTable<rbd_CurrentRegion>().SchemaName("dbo")
                        .Select(a => new { a.REGION });
                    region = query.ElementAt(0).REGION;
                    sb.Append(region.ToString("00"))
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
                        .Append("06");
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При попытке получения кода региона произошла ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
            }
            return sb.ToString();
        }

        private void WriteProcedureLog(object sndr, SqlInfoMessageEventArgs evt)
        {
            List<int> error_codes = new List<int>();
            if (evt.Errors.Count > 0)
            {
                foreach (SqlError err in evt.Errors)
                {
                    if (err.Number > 0)
                    {
                        this.outLog.Append(err.Message).Append(Environment.NewLine);
                        CommonRepository.GetLogger().Info(err.Message);
                    }
                    else
                    {
                    }
                }
            }
            return;
        }

        public string GetOutLog()
        {
            return this.outLog.ToString();
        }

        public DataTable GetStoredStatistics()
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new SqlConnection(CommonRepository.GetConnection()))
                using (var command = new SqlCommand("loader.Statistics", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.CommandTimeout = 3600;
                    conn.Open();
                    dt.Load(command.ExecuteReader());
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении запроса статистики была обнаружена ошибка: {0}", ex.ToString());
                CommonRepository.GetLogger().Error(status);
                throw new Exception(status);
            }
            return dt;
        }

        public DataTable PrepareStatistics(Dictionary<string, long> importStatictics)
        {
            DataTable statTable = GetStoredStatistics();
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add(new DataColumn(Globals.GRID_NAME, typeof(string)));
            resultTable.Columns.Add(new DataColumn(Globals.GRID_DESCRIPTION, typeof(string)));
            resultTable.Columns.Add(new DataColumn(Globals.GRID_XML, typeof(long)));
            resultTable.Columns.Add(new DataColumn(Globals.GRID_LOADER, typeof(int)));
            resultTable.Columns.Add(new DataColumn(Globals.GRID_TOTAL, typeof(int)));
            resultTable.Columns.Add(new DataColumn(Globals.GRID_NOT_LOADED, typeof(int)));
            foreach (DataRow st in statTable.Rows)
            {
                string tname = st.Field<string>("TableName");
                // угумс
                if (tname.Equals("res_SubTests"))
                {
                    tname = "res_Subtests";
                }
                // если есть в статистике по xml подсчёту
                if (importStatictics.ContainsKey(tname))
                {
                    DataRow row = resultTable.NewRow();
                    row[Globals.GRID_NAME] = tname;
                    row[Globals.GRID_DESCRIPTION] = st.Field<string>("TableDescription");
                    row[Globals.GRID_XML] = importStatictics[tname];
                    row[Globals.GRID_LOADER] = st.Field<int>("LoaderAmount");
                    row[Globals.GRID_TOTAL] = st.Field<int>("DboAmount");
                    int difference = st.Field<int>("LoaderAmount") - st.Field<int>("DboAmount");
                    if (difference < 0)
                    {
                        difference = 0;
                    }
                    row[Globals.GRID_NOT_LOADED] = difference;
                    resultTable.Rows.Add(row);
                }
            }
            return resultTable;
        }

        public static DataTable PrepareImportXMLStatistics(FilesInfo filesInfo, FilesInfoStats filesInfoStats)
        {
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add(new DataColumn("Название файла", typeof(string)));
            resultTable.Columns.Add(new DataColumn("Статус", typeof(int)));
            resultTable.Columns.Add(new DataColumn("Описание", typeof(string)));
            DataRow row;
            foreach (var file in filesInfo.GetList())
            {
                row = resultTable.NewRow();
                row["Название файла"] = file.Filename;
                row["Статус"] = Convert.ToInt32(file.Status);
                row["Описание"] = file.StatusMessage;
                resultTable.Rows.Add(row);
            }
            row = resultTable.NewRow();
            row["Название файла"] = "ИТОГО";
            row["Статус"] = (int)FinalInterview.FileStatus.Success;
            row["Описание"] = string.Format("Файлов загружено: {0}, участников загружено: {1}.", filesInfoStats.FilesCount.ToString(), filesInfoStats.ParticipantsLoaded.ToString());
            resultTable.Rows.Add(row);
            return resultTable;
        }

        public void BulkStartNew(string tablename, string xmlfilename, SqlRowsCopiedEventHandler handler, ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> outParam)
        {
            using (var connection = new SqlConnection(CommonRepository.GetConnection()))
            {
                if (!CommonRepository.IsDataTableExists("loader", tablename))
                {
                    throw new Exception(string.Format("Таблицы {0} нет в базе данных.", tablename));
                }

                DataTable adoTable = null;
                try
                {
                    adoTable = GetAdoTable(connection, tablename);
                }
                catch (Exception exep)
                {
                    string status = string.Format("Адаптер не смог загрузить таблицу {0}, была обнаружена ошибка: {1}", tablename, exep.ToString());
                    CommonRepository.GetLogger().Error(status);
                    throw new Exception();
                }

                using (XmlReader reader = XmlReader.Create(xmlfilename))
                {
                    reader.ReadToDescendant("ns1:" + tablename);

                    do
                    {
                        try
                        {
                            GetXMLAndConvertToDataTable(tablename, adoTable, reader);
                        }
                        catch (Exception exe)
                        {
                            string status = string.Format("При выполнении чтения и преобразования данных GetXMLAndConvertToDataTable из файла таблицы {0} была обнаружена ошибка: {1}", tablename, exe.ToString());
                            CommonRepository.GetLogger().Error(status);
                            break;
                        }

                        SqlTransaction transaction = null;
                        try
                        {
                            connection.Open();
                            transaction = connection.BeginTransaction();
                            using (SqlBulkCopy bcpy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, transaction))
                            {
                                bcpy.BulkCopyTimeout = 0;
                                bcpy.DestinationTableName = "loader." + tablename;
                                bcpy.SqlRowsCopied += handler;
                                bcpy.NotifyAfter = 1;
                                bcpy.BatchSize = 5000;

                                bcpy.WriteToServer(adoTable);

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (transaction != null) transaction.Rollback();
                            // если отменено юзером
                            if (ex.InnerException is OperationCanceledException)
                            {
                                throw new OperationCanceledException();
                            }
                            string status = string.Format("При импорте таблицы произошла ошибка {0}.", ex.ToString());
                            if (!errorDict.ContainsKey(tablename))
                            {
                                // непонятно сколько записано, будет 1
                                this.errorDict.TryAdd(tablename, new Tuple<string, long, TimeSpan>(status, 1, TimeSpan.Zero));
                            }
                            throw;
                        }
                        finally
                        {
                            connection.Close();
                            adoTable.Clear();
                        }
                    } while (!reader.EOF);
                }
            }
            outParam = this.errorDict;
        }

        private void GetXMLAndConvertToDataTable(string tablename, DataTable adoTable, XmlReader reader)
        {
            IEnumerable<ExpandoObject> rbd_s = GetXMLDataAsDynamicObject(reader, tablename, 1000);

            foreach (IDictionary<string, object> row in rbd_s)
            {
                var dataRow = adoTable.NewRow();
                var fields = (IDictionary<string, object>)row.First().Value;

                foreach (var column in fields)
                {
                    var dataRowType = dataRow.Table.Columns[column.Key].DataType.ToString();
                    var expandoType = column.Value.GetType();
                    bool uselessBool = false;
                    double uselessDouble = 0;

                    //исключения
                    if (dataRowType == "System.Byte[]")
                    {
                        dataRow[column.Key] = Convert.FromBase64String(column.Value.ToString());
                        continue;
                    }
                    if (dataRowType == "System.Boolean" && !Boolean.TryParse(column.Value.ToString(), out uselessBool))
                    {
                        dataRow[column.Key] = column.Value.ToString() == "0" ? false : true;
                        continue;
                    }
                    if (dataRowType == "System.Double" && !Double.TryParse(column.Value.ToString(), out uselessDouble))
                    {
                        dataRow[column.Key] = double.Parse(column.Value.ToString(), CultureInfo.InvariantCulture);
                        continue;
                    }

                    dataRow[column.Key] = column.Value;

                }
                adoTable.Rows.Add(dataRow);
            }
        }

        public IEnumerable<ExpandoObject> GetXMLDataAsDynamicObject(XmlReader reader, string tableName, int number)
        {
            int n = 0;
            do
            {
                var s = reader.ReadOuterXml().Replace(" xmlns:ns1=\"http://www.rustest.ru/giadbset\"", "").Replace("ns1:", "");//.Replace("res_Answers", "res_Answer");
                if (s.Length == 0) break;
                XDocument xmlDoc = XDocument.Parse(s);
                string jsonText = JsonConvert.SerializeXNode(xmlDoc);
                dynamic dyn = JsonConvert.DeserializeObject<ExpandoObject>(jsonText);

                yield return dyn;

                n += 1;

            } while (reader.ReadToNextSibling("ns1:" + tableName) && n < number);
        }

        public DataTable GetAdoTable(SqlConnection connection, string tableName)
        {
            string SelectScript = "select * from loader." + tableName;
            var adapter = new SqlDataAdapter(SelectScript, connection);
            var adoTable = new DataTable();
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                adapter.FillSchema(adoTable, SchemaType.Mapped);
            return adoTable;
        }
    }
}
