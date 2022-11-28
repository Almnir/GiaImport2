using DevExpress.XtraBars.Ribbon;
using GiaImport2.Models;
using System;
using System.IO;

namespace GiaImport2.Services
{
    public  interface IImportXMLFilesService
    {
        bool CheckFilesNames(string zipFileName);
        void ClearFiles();
        bool UnpackFiles(string zipfilename, RibbonControl ribbonControl, Action<ImportXMLFilesDto> addFileToView);
    }
}
