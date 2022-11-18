using MFtcFinalInterview;
using System.Collections.Concurrent;
using System.Linq;

namespace GiaImport2
{
    /// <summary>
    /// Вспомогательный класс для хранения статуса обработки файлов
    /// </summary>
    public class FilesInfo
    {
        public class FilesInfoContainer
        {
            public string Filename { get; set; }
            public FinalInterview.FileStatus Status { get; set; }
            public string StatusMessage { get; set; }
        }

        private ConcurrentBag<FilesInfoContainer> infoList;

        public ConcurrentBag<FilesInfoContainer> GetList() => infoList;

        public FilesInfo()
        {
            infoList = new ConcurrentBag<FilesInfoContainer>();
        }

        public void Add(string file, FinalInterview.FileStatus status, string message)
        {
            var found = this.infoList.Where(a => a.Filename == file);
            if (found.Count() > 0)
            {
                FilesInfoContainer infoRow = found.Cast<FilesInfoContainer>().First();
                infoRow.StatusMessage += "; ";
                infoRow.StatusMessage += message;
            }
            else
            {
                this.infoList.Add(
                    new FilesInfoContainer()
                    {
                        Filename = file,
                        Status = status,
                        StatusMessage = message
                    }
                );
            }
        }

        public bool CheckForOK()
        {
            bool result = true;
            foreach (FilesInfoContainer item in this.infoList)
            {
                if (!item.Status.Equals(FinalInterview.FileStatus.ConditionSavedNoErrors))
                {
                    result = false;
                }
            }
            return result;
        }

        public void Clean()
        {
            this.infoList = new ConcurrentBag<FilesInfoContainer>();
        }
    }
}
