using NLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace GiaImport2.Services
{
    class Verifier
    {
        public ConcurrentDictionary<string, string> errorDict;
        private int errCounter;

        public static string GetPath(string filename)
        {
            string curdir = Directory.GetCurrentDirectory();
            return curdir + @"\XSD\" + filename;
        }

        public bool errorState { get; set; }

        public string errorString { get; set; }

        public Verifier()
        {
            this.errorState = false;
            this.errorString = string.Empty;
            this.errorDict = new ConcurrentDictionary<string, string>();
            this.errCounter = 0;
        }

        public void VerifySingleFile(string xsdFileName, string xmlFileName, CancellationToken ct)
        {
            string tableName = Path.GetFileNameWithoutExtension(xmlFileName);
            try
            {
                errCounter = 0;
                XmlReaderSettings readerSettings = new XmlReaderSettings();
                readerSettings.Async = false;
                // TODO: хардкод, вынести в константы
                readerSettings.Schemas.Add("http://www.rustest.ru/giadbset", xsdFileName);
                readerSettings.ValidationType = ValidationType.Schema;
                readerSettings.ValidationEventHandler += (sender, e) => ValidationEventHandler(sender, e, tableName);

                using (XmlReader xml = XmlReader.Create(xmlFileName, readerSettings))
                {
                    while (xml.Read())
                    {
                        ct.ThrowIfCancellationRequested();
                        //progress.Report(progressCounter);
                        //progressCounter++;
                    }
                }
            }
            catch (Exception ex)
            {
                // если отменено юзером
                if (ex is OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                string status = string.Format("При проверке таблицы произошла ошибка {0}.", ex.ToString());
                if (!errorDict.ContainsKey(tableName))
                {
                    this.errorDict.TryAdd(tableName, status);
                }
                throw new VerifyException(errorDict);
            }
        }

        public void ValidationEventHandler(object sender, ValidationEventArgs e, string tableName)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                // TODO: что-то сделать с предупреждениями
                //log.Warn("WARNING: ");
                //log.Warn(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                this.errorState = true;
                if (this.errCounter < 20)
                {
                    if (this.errorDict.ContainsKey(tableName))
                    {
                        string errstr = string.Empty;
                        this.errorDict.TryGetValue(tableName, out errstr);
                        errstr += Environment.NewLine;
                        // если уже встречалась такая ошибка, не повторяем
                        if (!errstr.Contains(e.Message))
                        {
                            errstr += e.Message;
                            errCounter += 1;
                            this.errorDict.AddOrUpdate(tableName, errstr, (key, old) => errstr);
                            //this.errorDict[tableName] = errstr;
                        }
                    }
                    else
                    {
                        this.errorDict.TryAdd(tableName, e.Message);
                    }
                }
                else
                {
                    this.errorDict.AddOrUpdate(tableName, "Слишком много ошибок", (key, old) => e.Message + " Слишком много подобных ошибок");
                }
            }
        }

    }
}
