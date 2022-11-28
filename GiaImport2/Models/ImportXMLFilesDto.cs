using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GiaImport2.Models
{
    public class ImportXMLFilesDto
    {
        //[DisplayName(" ")]
        //[Editable(true)]
        //public bool Check { get; set; }
        [DisplayName("Имя")]
        [Editable(false)]
        public string Name { get; set; }
        [DisplayName("Размер")]
        [Editable(false)]
        public long Length { get; set; }
        [DisplayName("Дата создания")]
        [Editable(false)]
        public DateTime CreationTime { get; set; }
    }
}
