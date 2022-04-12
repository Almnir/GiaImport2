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
    }
}
