using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiaImport2.Models
{
    public class SheetsModel
    {
        public Guid ParticipantID { get; set; }
        public Guid SheetID { get; set; }
        public string Barcode { get; set; }
        public string Kimcode { get; set; }
        public Guid PackageId { get; set; }
    }
}
