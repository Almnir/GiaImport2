using System;
using System.Collections.Generic;

namespace GiaImport2.Models
{
    public class Governmentinfo
    {
        public Governmentinfo()
        {
            Schools = new List<Schoolinfo>();
        }

        public int GovernmentCode { get; set; }
        public string GovernmentName { get; set; }
        public List<Schoolinfo> Schools { get; set; }

        public Schoolinfo FindSchoolByID(Guid id)
        {
            foreach (var item in Schools)
            {
                if (item.SchoolID.Equals(id))
                    return item;
            }
            return null;
        }
    }
}
