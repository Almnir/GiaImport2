using System;
using System.Collections.Generic;

namespace GiaImport.Models
{
    public class Schoolinfo
    {
        public Schoolinfo(Guid schoolID, int schoolCode, string schoolName, List<Participantinfo> participant)
        {
            SchoolID = schoolID;
            SchoolCode = schoolCode;
            SchoolName = schoolName;
            Participants = participant;
        }

        public Guid SchoolID { get; set; }
        public int SchoolCode { get; set; }
        public string SchoolName { get; set; }
        public List<Participantinfo> Participants { get; set; }
    }
}
