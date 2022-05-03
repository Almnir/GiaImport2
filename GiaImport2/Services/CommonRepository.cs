using MFtcSfl;

namespace GiaImport2.Services
{
    class CommonRepository : ICommonRepository
    {
        public Scheme GetCurrentScheme()
        {
            Scheme scheme = new Scheme();
            return scheme;
        }
        public string _connectionString { get; set; }
        
        public string GetConnectionString()
        {
            return _connectionString;
        }
        public void SetupConnectionString(string serverText, string databaseText, string loginText, string passwordText)
        {
            _connectionString = string.Format("Server={0};Database={1};User Id={2};Password={3};Application Name=GiaImport v.{4}", serverText, databaseText, loginText, passwordText, Properties.Settings.Default.Version);
        }
    }
}
