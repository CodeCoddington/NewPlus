using Microsoft.Win32;



namespace NewPlus.Core
{
    public static class RegistryManager
    {
        #region Configuration
        private const string MenuName = "&New+";
        private const string KeyName = "NewPlus";
        #endregion



        #region PublicMethods
        public static void RegisterContextMenu()
        {
            string exePath = Application.ExecutablePath;

            using (RegistryKey bgKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\Directory\Background\shell\{KeyName}"))
            {
                bgKey.SetValue("MUIVerb", MenuName);
                bgKey.SetValue("Icon", exePath);

                using (RegistryKey commandKey = bgKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", $"\"{exePath}\" \"%V\"");
                }
            }

            using (RegistryKey folderKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\Directory\shell\{KeyName}"))
            {
                folderKey.SetValue("MUIVerb", MenuName);
                folderKey.SetValue("Icon", exePath);

                using (RegistryKey commandKey = folderKey.CreateSubKey("command"))
                {
                    commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }
        }

        public static void UnregisterContextMenu()
        {
            Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\Directory\Background\shell\{KeyName}", false);
            Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\Directory\shell\{KeyName}", false);
        }

        public static bool IsRegistered()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($@"Software\Classes\Directory\Background\shell\{KeyName}"))
            {
                return key != null;
            }
        }
        #endregion
    }
}