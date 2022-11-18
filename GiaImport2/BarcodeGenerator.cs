using GiaImport2.DataModels;
using GiaImport2.Services;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiaImport2
{
    public class BarcodeGenerator
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private List<int> KimcodeList { get; set; }

        public ICommonRepository CommonRepository { get; }

        public BarcodeGenerator(ICommonRepository commonRepository)
        {
            KimcodeList = new List<int>();
            CommonRepository = commonRepository;
        }

        /// <summary>
        /// Извлечь последнее максимальное из штрихкодов или вернуть базовое значение кимкода
        /// </summary>
        /// <returns></returns>
        private int GetGoodBarcodeFromDB()
        {
            string status = "";
            int result = 9000000;
            DataConnection dc = null;
            try
            {
                using (dc = SqlServerTools.CreateDataConnection(CommonRepository.GetConnection()))
                using (var db = new GIA_DB(dc.DataProvider, CommonRepository.GetConnection()))
                {
                    string maxBarcode = "";
                    var query = db.sht_Sheets_R.SchemaName("loader")
                        .Where(a =>
                        a.Barcode != null && a.Barcode.StartsWith(Globals.BARCODE_STARTS + Globals.TEST_TYPE))
                        .Select(b => b.Barcode).Max(v=>v);
                    // если нет ничего в лоадере, то смотрим в дбо
                    if (query == null)
                    {
                        query = db.sht_Sheets_R.SchemaName("dbo")
                            .Where(a =>
                            a.Barcode != null && a.Barcode.StartsWith(Globals.BARCODE_STARTS + Globals.TEST_TYPE))
                            .Select(b => b.Barcode).Max(v => v);
                        // если нет и в дбо, то берём начальное значение по дефолту
                        if (query == null)
                        {
                            result = 9000000;
                            return result;
                        }
                    }
                    maxBarcode = query.Substring(5, 7);
                    result = Convert.ToInt32(maxBarcode) + 1;
                }
            }
            catch (Exception ex)
            {
                status = string.Format("При выполнении запроса к базе данных произошла ошибка: {0}", ex.ToString());
                log.Error(status);
                result = 9000000;
            }
            finally
            {
                if (dc != null)
                {
                    dc.Close();
                    dc = null;
                }
            }
            return result;
        }

        /// <summary>
        /// Посчитать контрольную сумму EAN-13
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int ChecksumEAN13(String data)
        {
            // Test string for correct length
            if (data.Length != 12 && data.Length != 13)
                return -1;

            // Test string for being numeric
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < 0x30 || data[i] > 0x39)
                    return -1;
            }

            int sum = 0;

            for (int i = 11; i >= 0; i--)
            {
                int digit = data[i] - 0x30;
                if ((i & 0x01) == 1)
                    sum += digit;
                else
                    sum += digit * 3;
            }
            int mod = sum % 10;
            return mod == 0 ? 0 : 10 - mod;
        }

        /// <summary>
        /// Выдать на основе кимкода правильный штрихкод
        /// </summary>
        /// <param name="goodcode"></param>
        /// <returns></returns>
        private string GenerateBarcode(int goodcode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Globals.BARCODE_STARTS).Append(goodcode.ToString());
            int checksum = ChecksumEAN13(sb.ToString());
            sb.Append(checksum.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Сгенерировать следующий валидный штрихкод
        /// </summary>
        /// <returns></returns>
        public string GetNextBarcode()
        {
            string result = "";
            if (this.KimcodeList.Any())
            {
                int gcode = KimcodeList.Last() + 1;
                KimcodeList.Add(gcode);
                result = GenerateBarcode(gcode);
            }
            else
            {
                int goodCode = GetGoodBarcodeFromDB();
                this.KimcodeList.Add(goodCode);
                result = GenerateBarcode(goodCode);
            }
            return result;
        }
    }
}
