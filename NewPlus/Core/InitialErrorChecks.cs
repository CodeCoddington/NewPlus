using NewPlus.Models;

namespace NewPlus.Core
{
    public static class InitialErrorChecks
    {
        #region Configuration
        private static readonly string _standardUserDocumentsPath_A = $@"C:\Users\{Environment.UserName}\Documents";
        private static readonly string _standardUserDocumentsPath_B = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        #endregion



        #region PathChecks
        public static string PathChecks()
        {
            string errorMessage = string.Empty;

            if (!Directory.Exists(ConfigurationManager.StandardUserDocumentsPath))
            {
                return $"Could not find either of the standard user documents paths:\r\n\r\n{_standardUserDocumentsPath_A}\r\n{_standardUserDocumentsPath_B}";
            }

            string[] appData_Directories = Directory.GetDirectories(ConfigurationManager.TemplatesPath, "*", SearchOption.TopDirectoryOnly);
            int appData_DirectoryCount = appData_Directories.Length;

            List<TemplateProfile> jsonConfig_Templates = ConfigurationManager.LoadTemplates();
            int jsonConfig_TemplateCount = jsonConfig_Templates.Count;

            if (appData_DirectoryCount != jsonConfig_TemplateCount)
            {
                return $"The number of template directories ({appData_DirectoryCount}) does not match the number of template profiles ({jsonConfig_TemplateCount}).";
            }


            List<string> jsonConfig_FolderNames = jsonConfig_Templates.Select(f => f.TemplateFolderName).ToList();
            List<string> orphanedJsonConfig_FolderNames = jsonConfig_FolderNames.Except(appData_Directories.Select(d => new DirectoryInfo(d).Name)).ToList();
            List<string> orphanedAppData_DirectoryNames = appData_Directories.Select(d => new DirectoryInfo(d).Name).Except(jsonConfig_FolderNames).ToList();

            if (orphanedJsonConfig_FolderNames.Count > 0 || orphanedAppData_DirectoryNames.Count > 0)
            {
                if (orphanedJsonConfig_FolderNames.Count > 0)
                {
                    errorMessage += $"Orphaned template folders (in config but not in directories): {string.Join("\r\n", orphanedJsonConfig_FolderNames)}\r\n\r\n";
                }

                if (orphanedAppData_DirectoryNames.Count > 0)
                {
                    errorMessage += $"Orphaned template directories (in directories but not in config): {string.Join("\r\n", orphanedAppData_DirectoryNames)}";
                }
                return errorMessage;
            }

            string[] appData_IcoFiles = Directory.GetFiles(ConfigurationManager.IcoPath, "*.ico", SearchOption.TopDirectoryOnly);
            int appData_IcoFileCount = appData_IcoFiles.Length;

            List<string> jsonConfig_IconNames = jsonConfig_Templates.Select(f => f.IconFileName).Where(f => !string.IsNullOrEmpty(f)).ToList();
            int jsonConfig_IcoCount = jsonConfig_IconNames.Count;

            if (appData_IcoFileCount != jsonConfig_IcoCount)
            {
                return $"The number of .ico files ({appData_IcoFileCount}) does not match the number of template profiles ({jsonConfig_IcoCount}).";
            }
            
            List<string> orphaned_jsonConfig_IcoNames = jsonConfig_IconNames.Except(appData_IcoFiles.Select(f => Path.GetFileName(f))).Where(f => !string.IsNullOrEmpty(f)).ToList();
            List<string> orphaned_appData_IcoFiles = appData_IcoFiles.Select(f => Path.GetFileName(f)).Except(jsonConfig_IconNames).ToList();

            if (orphaned_jsonConfig_IcoNames.Count > 0 || orphaned_appData_IcoFiles.Count > 0)
            {
                if (orphaned_jsonConfig_IcoNames.Count > 0)
                {
                    errorMessage += $"Orphaned .ico files (in config but not in directories): {string.Join("\r\n", orphaned_jsonConfig_IcoNames)}\r\n\r\n";
                }

                if (orphaned_appData_IcoFiles.Count > 0)
                {
                    errorMessage += $"Orphaned .ico files (in directories but not in config): {string.Join("\r\n", orphaned_appData_IcoFiles)}";
                }
                return errorMessage;
            }

            return string.Empty;
        }
        #endregion



        #region FileName_NormalizationHelper
        public static (List<char> IllegalCharsStripped, string NormalizedName) NormalizePathName(string fileOrFolderName)
        {
            List<char> illegalCharsStripped = new List<char>();
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (fileOrFolderName.Contains(c))
                {
                    illegalCharsStripped.Add(c);
                    fileOrFolderName = fileOrFolderName.Replace(c.ToString(), string.Empty);
                }
            }
            return (illegalCharsStripped, fileOrFolderName.Trim());
        }
        #endregion
    }
}
