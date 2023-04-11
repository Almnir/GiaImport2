using DevExpress.XtraBars.Ribbon;
using GiaImport2.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static GiaImport2.Services.ImportXMLFilesService;

namespace GiaImport2.Services
{
    public  interface IImportXMLFilesService
    {
        bool TryCheckFilesNames(string zipFileName);
        void ClearFiles();
        bool UnpackFiles(string zipfilename, RibbonControl ribbonControl, Action<ImportXMLFilesDto> addFileToView);
        void ValidateFiles(ImportGridPanel importGridPanel, RibbonControl ribbonControl, List<ImportXMLFilesDto> checkedFiles, Action<string> showValidationErrors, Action<(List<TableInfo> tableInfos, ConcurrentDictionary<string, string> dependencyErrors)> showResultWindow);
        List<ParentTable> GetDependencyMap();
        ConcurrentDictionary<string, string> SearchDependencies(Action<string> addFileToView, string directoryPath, List<string> selectedTables, List<ParentTable> dependentTables, CancellationToken ct);
        IEnumerable<string> FindDuplicateKeys(Dictionary<string, List<string>> keyValueDict);
        Dictionary<string, List<string>> GetGuidsFromXML(string filename, string tablename, List<string> fkeys);
        void ImportFiles(
            ImportGridPanel importGridPanel,
            RibbonControl ribbonControl,
            BulkManager bulkManager,
            List<ImportXMLFilesDto> checkedFiles,
            Action showDeleteImport,
            Action<DeserializeException> deserializeErrorCallback,
            Action<BulkException> bulkErrorCallback,
            Action<Exception> exceptionCallback,
            Action<Dictionary<string, long>> showResultWindow,
            Action cancelCallback
            );
        ConcurrentDictionary<string, Tuple<string, long, TimeSpan>> RunImport(
            ConcurrentDictionary<string, FileInfo> actualCheckedFiles,
            BulkManager bm,
            Action<string> addFileToView,
            Action closeProgressBar,
            IProgress<int> progressLine,
            IProgress<int> progressTotal,
            Action changeProgressLine,
            Action showDeleteImport,
            Action<DeserializeException> deserializeErrorCallback,
            Action<BulkException> bulkErrorCallback,
            Action cancelCallback,
            Action<Exception> exceptionCallback,
            CancellationToken token);
    }
}
