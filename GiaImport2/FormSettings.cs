using System;
using System.Configuration;

namespace GiaImport2
{
    public class FormSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        public String ServerText
        {
            get { return (String)this["ServerText"]; }
            set { this["ServerText"] = value; }
        }

        [UserScopedSetting()]
        public String DatabaseText
        {
            get { return (String)(this["DatabaseText"]); }
            set { this["DatabaseText"] = value; }
        }
        [UserScopedSetting()]
        public String LoginText
        {
            get { return (String)(this["LoginText"]); }
            set { this["LoginText"] = value; }
        }
        [UserScopedSetting()]
        public String PasswordText
        {
            get { return (String)(this["PasswordText"]); }
            set { this["PasswordText"] = value; }
        }
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
        [DefaultSettingValue("False")]
        public bool DoClasses
        {
            get { return (bool)(this["DoClasses"]); }
            set { this["DoClasses"] = value; }
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
