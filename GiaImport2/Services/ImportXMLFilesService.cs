using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using GiaImport2.Models;
using NLog.Fluent;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Container = SimpleInjector.Container;

namespace GiaImport2.Services
{
    public class ImportXMLFilesService : IImportXMLFilesService
    {
        private readonly Container Container;
        private readonly ICommonRepository CommonRepository;

        public ImportXMLFilesService(Container container, ICommonRepository commonRepository)
        {
            this.Container = container;
            this.CommonRepository = commonRepository;
        }

        public bool CheckFilesNames(string zipFileName)
        {
            bool result = true;
            using (var zip = ZipFile.OpenRead(zipFileName))
            {
                foreach (var ze in zip.Entries)
                {
                    if (!Globals.TABLES_NAMES.Contains(Path.GetFileNameWithoutExtension(ze.Name)))
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        public void ClearFiles()
        {
            string[] files = Directory.GetFiles(Globals.frmSettings.TempDirectoryText == null ?
                Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir"));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
        public bool UnpackFiles(string zipfilename, RibbonControl ribbonControl, Action<ImportXMLFilesDto> addFileToView)
        {
            bool error = false;
            CancellationTokenSource source = new CancellationTokenSource();
            ProgressBarSingleBar pbw = new ProgressBarSingleBar();
            pbw.SetCancellationToken(source);
            pbw.SetTitle("Распаковка файлов выполняется...");
            ProgressBarControl pbarTotal = pbw.GetProgressBarTotal();
            pbw.Refresh();
            LabelControl plabel = pbw.GetLabel();
            FileInfo fi = new FileInfo(zipfilename);
            plabel.Text = fi.Name;
            int entriesCount = 0;
            using (var zip = ZipFile.OpenRead(zipfilename))
            {
                entriesCount = zip.Entries.Count;
            }
            pbarTotal.Properties.Maximum = entriesCount - 1;
            IProgress<int> progress = new Progress<int>(value =>
            {
                ProgressBarControl pb = pbw.GetProgressBarTotal();
                if (!pb.IsDisposed)
                {
                    pb.Invoke((MethodInvoker)(() =>
                    {
                        pb.EditValue = value;
                    }));
                }
            });
            pbw.FormClosed += (a, e) => { source.Cancel(); };
            // отключаем кнопки на риббоне
            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, false);
            pbw.Show();
            try
            {
                Task task = Task.Run(() => RunUnpacker(addFileToView, zipfilename, progress,
                    () =>
                    {
                        if (pbw != null && pbw.IsDisposed != true)
                        {
                            pbw.Invoke((MethodInvoker)(() =>
                            {
                                pbw.Close();
                            }));
                        }
                        // включаем кнопки на риббоне
                        ribbonControl.Invoke((MethodInvoker)(() =>
                        {
                            FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                        }));
                    },
                    source.Token), source.Token);
                task.ContinueWith(taskc =>
                {
                    if (taskc.Exception == null && !source.IsCancellationRequested)
                    {
                        EndUnpacker(ribbonControl,
                            () =>
                            {
                                if (pbw != null && pbw.IsDisposed != true)
                                {
                                    pbw.Invoke((MethodInvoker)(() =>
                                    {
                                        pbw.Close();
                                    }));
                                }
                                // включаем кнопки на риббоне
                                ribbonControl.Invoke((MethodInvoker)(() =>
                                {
                                    FormsHelper.ToggleRibbonButtonsAll(ribbonControl, true);
                                }));
                            }
                        );
                    }
                });
            }
            catch (TaskCanceledException)
            {
                error = true;
                FormsHelper.ShowStyledMessageBox("Внимание!", "Операция прервана!");
            }
            catch (Exception ex)
            {
                error = true;
                FormsHelper.ShowStyledMessageBox("Ошибка!", ex.ToString());
                CommonRepository.GetLogger().Error(ex.ToString());
            }
            return error;
        }

        private void EndUnpacker(RibbonControl ribbonControl, Action closeProgressBar)
        {
            closeProgressBar();
        }

        private void RunUnpacker(Action<ImportXMLFilesDto> addFileToView, string zipfilename, IProgress<int> progress, Action closeProgressBar, CancellationToken ct)
        {
            try
            {
                int counter = 0;
                using (ZipArchive zip = ZipFile.OpenRead(zipfilename))
                {
                    foreach (var e in zip.Entries)
                    {
                        ct.ThrowIfCancellationRequested();
                        string filenewpath = Path.Combine(Globals.frmSettings.TempDirectoryText == null ?
                            Path.Combine(Path.GetTempPath(), "Tempdir") : Path.Combine(Globals.frmSettings.TempDirectoryText, "Tempdir"), e.FullName);
                        e.ExtractToFile(filenewpath, true);
                        FileInfo fi = new FileInfo(filenewpath);
                        var ixf = new ImportXMLFilesDto();
                        ixf.Name = fi.Name;
                        ixf.CreationTime = fi.CreationTime;
                        ixf.Length = fi.Length;
                        addFileToView(ixf);
                        progress.Report(counter);
                        counter++;
                        Thread.Sleep(500);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                closeProgressBar();
            }
            catch (Exception ex)
            {
                CommonRepository.GetLogger().Error(ex.ToString());
                closeProgressBar();
            }
        }
    }
}
