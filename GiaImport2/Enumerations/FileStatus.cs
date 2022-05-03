using Ardalis.SmartEnum;

namespace GiaImport2.Enumerations
{
    public class FileStatus : SmartEnum<FileStatus>
    {
        public static readonly FileStatus Exported = new FileStatus("Создано", 10);
        public static readonly FileStatus Checked = new FileStatus("Проверено", 11);
        public static readonly FileStatus SavedWithNoErrors = new FileStatus("Сохранено без ошибок", 12);
        public static readonly FileStatus ConditionReexport = new FileStatus("Повторно экспортировано", 13);
        public static readonly FileStatus Imported = new FileStatus("Импортировано", 14);

        private FileStatus(string name, int value) : base(name, value)
        {
        }
    }
}
