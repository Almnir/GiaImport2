using GiaImport2.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace GiaImport2.Models
{
    public class CacheData
    {
        public IQueryable<sht_Package> sht_Packages { get; set; }
        public IQueryable<sht_Sheets_R> sht_Sheets_Rs { get; set; }
        public IQueryable<sht_Sheets_AB> sht_Sheets_ABs { get; set; }
        public IQueryable<sht_Sheets_C> sht_Sheets_Cs { get; set; }
        public IQueryable<sht_Sheets_D> sht_Sheets_Ds { get; set; }
        public IQueryable<sht_Marks_AB> sht_Marks_ABs { get; set; }
        public IQueryable<sht_Marks_C> sht_Marks_Cs { get; set; }
        public IQueryable<sht_Marks_D> sht_Marks_Ds { get; set; }
        public IQueryable<sht_FinalMarks_C> sht_FinalMarks_Cs { get; set; }
        public IQueryable<sht_FinalMarks_D> sht_FinalMarks_Ds { get; set; }

    }
}
