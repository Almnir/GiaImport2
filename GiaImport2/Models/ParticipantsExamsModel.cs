using System;

namespace GiaImport2.Models
{
    public class ParticipantsExamsModel
    {
        public int GovernmentCode { get; set; }
        public string GovernmentName { get; set; }
        public Guid SchoolID { get; set; }
        public int SchoolCode { get; set; }
        public Guid ParticipantID { get; set; }
        public string pClass { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string DocumentSeries { get; set; }
        public string DocumentNumber { get; set; }
        public string ParticipantCode { get; set; }
        public int Property { get; set; }
    }
}
