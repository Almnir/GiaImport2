using AdysTech.CredentialManager;
using FtcLicensing;
using GiaImport2.DataModels;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using MFtcSfl;
using MFtcUtils.Digest.Enumerators;
using NLog;
using NLog.Fluent;
using RT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GiaImport2.Services
{
    class CommonRepository : ICommonRepository
    {
        public StringBuilder OutLog = new StringBuilder();

        /// <summary>
        /// Структура атрибутов для дополнительных данных в реквизитах
        /// </summary>
        [Serializable]
        public struct SettingsAttributes
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public Scheme GetCurrentScheme()
        {
            Scheme scheme = new Scheme();
            return scheme;
        }
        public string _connectionString { get; set; }

        public void SetupConnectionString(string serverText, string databaseText, string loginText, string passwordText)
        {
            _connectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Application Name=GiaImport v.{4}", serverText, databaseText, loginText, passwordText, Properties.Settings.Default.Version);
        }

        public string GetConnection()
        {
            var cred = GetCredentials();
            if (cred.Item1 == null) return null;
            return string.Format("Server={0};Database={1};User Id={2};Password={3};Application Name=GiaImport v.{4}", cred.ServerName, cred.Database, cred.UserName, cred.Password, Properties.Settings.Default.Version);
        }
        public (string UserName, string Password, string ServerName, string Database) GetCredentials()
        {
            // чтение реквизитов для входа в GiaImport
            var cred = CredentialManager.GetICredential(Globals.TARGET_CREDENTIAL);
            if (cred == null) return new Tuple<string, string, string, string>("", "", "", "").ToValueTuple();
            var username = cred.UserName;
            SettingsAttributes server = new SettingsAttributes();
            SettingsAttributes database = new SettingsAttributes();
            if (cred.Attributes != null && cred.Attributes.Count != 0)
            {
                server = (SettingsAttributes)cred.Attributes["ServerAttribute"];
                database = (SettingsAttributes)cred.Attributes["DatabaseAttribute"];
            }
            return new Tuple<string, string, string, string>(username, cred.CredentialBlob, server.value, database.value).ToValueTuple();
        }

        public bool CheckConnection()
        {
            bool result = false;
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(GetConnection()))
                {
                    var query = "select 1";
                    var command = new SqlCommand(query, connection);
                    connection.Open();
                    command.ExecuteScalar();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
            }
            return result;
        }

        public bool CheckAccessToFolder(string exportFolder)
        {
            CurrentUserSecurity cus = new CurrentUserSecurity();
            DirectoryInfo di = new DirectoryInfo(exportFolder);
            if (cus.HasAccess(di, FileSystemRights.WriteData & FileSystemRights.CreateDirectories))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetCurrentRegion(out string status)
        {
            int result = 0;
            status = "";
            try
            {
                using (var dc = SqlServerTools.CreateDataConnection(GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, GetConnection()))
                {
                    var query = db.GetTable<rbd_CurrentRegion>().SchemaName("dbo").Select(a => a.REGION);
                    result = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При попытке получения кода региона произошла ошибка: {0}", ex.ToString());
                GetLogger().Error(status);
            }
            return result;
        }

        public bool TryGetFinalInterviewLicense(int currentregion, out List<MFtcLicInfo> licenses)
        {
            bool result = false;
            SqlConnection connection = null;
            licenses = null;
            try
            {
                using (connection = new SqlConnection(GetConnection()))
                {
                    string error = "";
                    licenses = new MFtcLicReader().GetLicsFromBD(connection, currentregion, int.Parse(Globals.TEST_TYPE), out error);
                    if (string.IsNullOrEmpty(error) == true && licenses != null && licenses.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                GetLogger().Error(ex.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
            }
            return result;
        }

        public bool TryCreateFinalInterviewSession(out Guid sessionid)
        {
            bool result = false;
            SqlConnection connection = null;
            sessionid = Guid.Empty;
            try
            {
                using (connection = new SqlConnection(GetConnection()))
                {
                    sessionid = RtManager.CreateSimpleSession(connection, 0, (int)WorkstationType.FinalInterview);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                GetLogger().Error(ex.ToString());
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
            }
            return result;
        }
        public bool TryCloseFinalInterviewSession(Guid sessionid)
        {
            bool result = false;
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(GetConnection()))
                {
                    RtManager.CloseSession(connection, sessionid);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                GetLogger().Error(ex.ToString());
                result = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }
            }
            return result;
        }
        public Logger GetLogger()
        {
            return LogManager.GetLogger("GiaImport");
        }
        public void RunStoredSynchronize()
        {
            int errorCount = 0;
            this.OutLog = new StringBuilder();
            try
            {
                using (var conn = new SqlConnection(GetConnection()))
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
                            GetLogger().Error("Ошибки слияния: " + errorCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При выполнении слияния была обнаружена ошибка: {0}", ex.ToString());
                GetLogger().Error(status);
                GetLogger().Error(this.OutLog.ToString());
                throw new Exception(status);
            }
        }

        public void RunStoredComplection(string packageMask)
        {
            this.OutLog.Clear();
            try
            {
                using (var conn = new SqlConnection(GetConnection()))
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
                GetLogger().Error(status);
                GetLogger().Error(this.OutLog.ToString());
                throw new Exception(status);
            }
        }

        public void RunStoredConvertation(string packageMask)
        {
            this.OutLog.Clear();
            try
            {
                using (var conn = new SqlConnection(GetConnection()))
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
                GetLogger().Error(status);
                GetLogger().Error(this.OutLog.ToString());
                throw new Exception(status);
            }
        }

        public void RunDeleteSynchronize(int tableGroup)
        {
            this.OutLog.Clear();
            try
            {
                using (var conn = new SqlConnection(GetConnection()))
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
                GetLogger().Error(status);
                GetLogger().Error(this.OutLog.ToString());
                throw new Exception(status);
            }
        }
        private void WriteProcedureLog(object sndr, SqlInfoMessageEventArgs evt)
        {
            if (evt.Errors.Count > 0)
            {
                foreach (SqlError err in evt.Errors)
                {
                    if (err.Number > 0)
                    {
                        this.OutLog.Append(err.Message).Append(Environment.NewLine);
                        GetLogger().Info(err.Message);
                    }
                }
            }
            return;
        }
        public bool IsDataTableExists(string schemaName, string tableName)
        {
            bool result = false;
            string commandText = string.Format(@"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = @TABLE_NAME AND TABLE_SCHEMA = @TABLE_SCHEMA;");
            using (var connection = new SqlConnection(GetConnection()))
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.Add(new SqlParameter("TABLE_SCHEMA", schemaName));
                command.Parameters.Add(new SqlParameter("TABLE_NAME", tableName));

                connection.Open();
                result = (int)command.ExecuteScalar() > 0;
            }
            return result;
        }
        /// <summary>
        /// Проверка на существование сразу трёх хранимок, необходимых для работы софта
        /// </summary>
        /// <returns></returns>
        public bool CheckIfStoredExist()
        {
            int result = 0;
            try
            {
                using (var dc = SqlServerTools.CreateDataConnection(GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, GetConnection()))
                {
                    var query = db.Query<int>(@"COUNT(ROUTINE_SCHEMA)
                                    FROM INFORMATION_SCHEMA.ROUTINES
                                    WHERE ROUTINE_NAME IN ('Synchronize', 'CleanupTables', 'Statistics')
                                    and ROUTINE_SCHEMA = 'loader'");
                    result = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string status = string.Format("При попытке получения данных о количестве хранимых процедур произошла ошибка: {0}", ex.ToString());
                GetLogger().Error(status);
            }
            return result == 3;
        }
        public void DeleteLoaderTables()
        {
            var sb = new StringBuilder();
            foreach (var item in Globals.TABLES_NAMES_FOR_DELETE)
            {
                sb.AppendLine($"TRUNCATE TABLE loader.{item};");
            }
            using (var dc = SqlServerTools.CreateDataConnection(GetConnection()))
            using (var db = new GIA_DB(dc.DataProvider, GetConnection()))
            {
                try
                {
                    db.BeginTransaction();
                    db.Execute(sb.ToString());
                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollbackTransaction();
                    string status = string.Format("При попытке удаления данных временных таблиц произошла ошибка: {0}", ex.ToString());
                    GetLogger().Error(status);
                }
            }
        }
    }
}
