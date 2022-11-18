namespace GiaImport2.Models
{
    public class SchoolClasses
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public int SchoolCode { get; set; }
        public string ShortName { get; set; }
        public string PClassName { get; set; }

        public SchoolClasses(int ID, int parentID, int schoolCode, string shortName, string pClassName)
        {
            this.ID = ID;
            ParentID = parentID;
            SchoolCode = schoolCode;
            ShortName = shortName;
            PClassName = pClassName;
        }
        public SchoolClasses(int schoolCode, string shortName, string pClassName)
        {
            this.ID = 0;
            this.ParentID = 0;
            SchoolCode = schoolCode;
            ShortName = shortName;
            PClassName = pClassName;
        }
        public SchoolClasses(int schoolCode, string shortName)
        {
            this.ID = 0;
            this.ParentID = 0;
            SchoolCode = schoolCode;
            ShortName = shortName;
            PClassName = null;
        }
    }
}
