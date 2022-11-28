using System;
using System.Configuration;

namespace GiaImport2
{
    public class FormSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        public String TempDirectoryText
        {
            get { return (String)(this["TempDirectoryText"]); }
            set { this["TempDirectoryText"] = value; }
        }
        [UserScopedSetting()]
        public String LastPathText
        {
            get { return (String)(this["LastPathText"]); }
            set { this["LastPathText"] = value; }
        }
        [UserScopedSetting()]
        [DefaultSettingValue("True")]
        public bool PuraSurEraro
        {
            get { return (bool)(this["PuraSurEraro"]); }
            set { this["PuraSurEraro"] = value; }
        }
    }
}
