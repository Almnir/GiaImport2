using GiaImport2.Enumerations;
using MFtcFinalInterview;
using System.Collections.Concurrent;
using System.Linq;

namespace GiaImport2.Models
{
    public class FilesInfo
    {
        public class FilesInfoContainer
        {
            public string Filename { get; set; }
            public FileStatus Status { get; set; }
            public string StatusMessage { get; set; }
        }

        private ConcurrentBag<FilesInfoContainer> infoList;

        public ConcurrentBag<FilesInfoContainer> GetList() => infoList;

        public FilesInfo()
        {
            infoList = new ConcurrentBag<FilesInfoContainer>();
        }

        public FilesInfoContainer Add(string file, FileStatus status, string message)
        {
            FilesInfoContainer result = null;
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
                    result = new FilesInfoContainer()
                    {
                        Filename = file,
                        Status = status,
                        StatusMessage = message
                    }
                );
            }
            return result;
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
