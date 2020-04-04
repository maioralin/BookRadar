using Books.OtherClasses;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static string InstallationId
        {
            get
            {
                return AppSettings.GetValueOrDefault("InstallationId", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("InstallationId", value);
            }
        }

        public static string Channel
        {
            get
            {
                return AppSettings.GetValueOrDefault("Channel", string.Empty);
            }
            set
            {
                AppSettings.AddOrUpdateValue("Channel", value);
            }
        }

    }
}
