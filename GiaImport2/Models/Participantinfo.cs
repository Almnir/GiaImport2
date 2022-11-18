using GiaImport2.Enumerations;
using System;

namespace GiaImport2.Models
{
    public class Participantinfo
    {
        public Participantinfo(Guid partId, string pClass, string surname, string name, 
            string secondname, string docSeries, string docNumber, FileStatus condition, Guid sheetId, string barcode, string kimcode, Guid packageId,
            string participantCode, int property)
        {
            ParticipantId = partId;
            PClass = pClass;
            Surname = surname;
            Name = name;
            Secondname = secondname;
            DocNumber = docNumber;
            DocSeries = docSeries;
            Condition = condition;
            SheetId = sheetId;
            Barcode = barcode;
            Kimcode = kimcode;
            PackageId = packageId;
            ParticipantCode = participantCode;
            Property = property;
        }
        public Guid ParticipantId { get; set; }
        public string PClass { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Secondname { get; set; }
        public string DocSeries { get; set; }
        public string DocNumber { get; set; }
        public FileStatus Condition { get; set; }
        public Guid SheetId { get; set; }
        public string Barcode { get; set; }
        public string Kimcode { get; set; }
        public Guid PackageId { get; set; }
        public string ParticipantCode { get; set; }
        public int Property { get; set; }
    }
}
