using Ardalis.SmartEnum;

namespace GiaImport2.Enumerations
{
    public class ChooseSchoolsOrClass : SmartEnum<ChooseSchoolsOrClass>
    {
        public static readonly ChooseSchoolsOrClass Schools = new ChooseSchoolsOrClass("Школы", 0);
        public static readonly ChooseSchoolsOrClass Classes = new ChooseSchoolsOrClass("Классы",1);

        public ChooseSchoolsOrClass(string name, int value) : base(name, value)
        {
        }
    }
}
