namespace GiaImport2.Services
{
    public interface ICommonRepository
    {
        MFtcSfl.Scheme GetCurrentScheme();
        void SetupConnectionString(string serverText, string databaseText, string loginText, string passwordText);
        string GetConnectionString();
    }
}
