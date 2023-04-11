using Ardalis.SmartEnum;

namespace GiaImport2.Enumerations
{
    public class FileStatus : SmartEnum<FileStatus>
    {
        public static readonly FileStatus ConditionExported = new FileStatus("Создано(экспортировано)", 10);
        public static readonly FileStatus ConditionReexport = new FileStatus("Повторно экспортировано", 11);
        public static readonly FileStatus ConditionSavedNotChecked = new FileStatus("Сохранено без проверки", 12);
        public static readonly FileStatus ConditionSavedWithErrors = new FileStatus("Сохранено с ошибками", 13);
        public static readonly FileStatus ConditionSavedNoErrors = new FileStatus("Сохранено без ошибок", 19);
        public static readonly FileStatus Success = new FileStatus("Импортировано", 20);
        public static readonly FileStatus WrongName = new FileStatus("Некорректное имя файла", 1);
        public static readonly FileStatus NotValidXML = new FileStatus("Невалидный XML", 2);
        public static readonly FileStatus WrongVersion = new FileStatus("Несоответствующая версия файла", 5);
        public static readonly FileStatus NotFinished = new FileStatus("Незаполненный файл", 3);
        
        private FileStatus(string name, int value) : base(name, value)
        {
        }
    }
}
