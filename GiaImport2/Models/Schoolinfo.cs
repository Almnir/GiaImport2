using System;
using System.Collections.Generic;

namespace GiaImport2.Models
{
    public class Schoolinfo
    {
        public Schoolinfo(Guid schoolID, int schoolCode, string schoolName, string shortName, List<Participantinfo> participant)
        {
            SchoolID = schoolID;
            SchoolCode = schoolCode;
            SchoolName = schoolName;
            Participants = participant;
            ShortName = shortName;
        }

        public Guid SchoolID { get; set; }
        public int SchoolCode { get; set; }
        public string SchoolName { get; set; }
        public string ShortName { get; set; }
        public List<Participantinfo> Participants { get; set; }
    }
}
