using FtcLicensing;
using NLog;
using System;
using System.Collections.Generic;

namespace GiaImport2.Services
{
    public interface ICommonRepository
    {
        MFtcSfl.Scheme GetCurrentScheme();
        void SetupConnectionString(string serverText, string databaseText, string loginText, string passwordText);
        string GetConnection();
        bool CheckConnection();
        (string UserName, string Password, string ServerName, string Database) GetCredentials();
        bool CheckAccessToFolder(string path);
        Logger GetLogger();
        int GetCurrentRegion(out string status);
        bool TryGetFinalInterviewLicense(int currentregion, out List<MFtcLicInfo> licenses);
        bool TryCreateFinalInterviewSession(out Guid sessionid);
        bool TryCloseFinalInterviewSession(Guid sessionid);
        void RunStoredSynchronize();
        void RunStoredComplection(string packageMask);
        void RunStoredConvertation(string packageMask);
        void RunDeleteSynchronize(int tableGroup);
        bool IsDataTableExists(string schemaName, string tableName);
        bool CheckIfStoredExist();
    }
}
