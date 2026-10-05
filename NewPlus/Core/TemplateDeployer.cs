using NewPlus.Models;



namespace NewPlus.Core
{
    public static class TemplateDeployer
    {
        #region PublicMethod
        public static string DeployTemplate(TemplateProfile profile, string targetPath, string defaultFolderName = "New Folder", string forcedProjectName = "")
        {
            string sourceDir = Path.Combine(ConfigurationManager.TemplatesPath, profile.TemplateFolderName);

            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"Template source directory not found: {sourceDir}");
            }

            bool isIntercepted = !string.IsNullOrWhiteSpace(forcedProjectName);
            string finalFolderName = isIntercepted ? forcedProjectName : defaultFolderName;

            string destinationDir = GetUniqueDirectoryPath(targetPath, finalFolderName);
            string newFolderName = Path.GetFileName(destinationDir);

            CopyDirectory(sourceDir, destinationDir);

            if (isIntercepted && !string.IsNullOrWhiteSpace(profile.FileRenameExtension))
            {
                RenameInteriorFile(destinationDir, profile.FileRenameExtension, newFolderName);
            }

            IconSynchronizer.ApplyIconToFolder(destinationDir, profile.IconFileName);

            Thread.Sleep(150);

            // If intercepted, just select it (false). If standard, drop into rename mode (true).
            TriggerExplorerSelection(targetPath, newFolderName, !isIntercepted);

            return destinationDir;
        }
        #endregion



        #region ExplorerInteraction
        private static void RenameInteriorFile(string targetDir, string extension, string newName)
        {
            if (!extension.StartsWith(".")) extension = "." + extension;

            string[] matchingFiles = Directory.GetFiles(targetDir, $"*{extension}", SearchOption.AllDirectories);

            if (matchingFiles.Length > 0)
            {
                string targetFile = matchingFiles[0];
                string fileDir = Path.GetDirectoryName(targetFile);
                string newFilePath = Path.Combine(fileDir, newName + extension);

                if (!targetFile.Equals(newFilePath, StringComparison.OrdinalIgnoreCase) && !File.Exists(newFilePath))
                {
                    File.Move(targetFile, newFilePath);
                }
            }
        }

        private static void TriggerExplorerSelection(string parentDirectory, string folderName, bool enterRenameMode)
        {
            try
            {
                Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
                if (shellAppType == null) return;

                dynamic shell = Activator.CreateInstance(shellAppType);

                foreach (dynamic window in shell.Windows())
                {
                    try
                    {
                        string fullName = window.FullName;
                        if (fullName != null && fullName.EndsWith("explorer.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            string windowPath = window.Document.Folder.Self.Path;
                            string normalizedTarget = parentDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            string normalizedWindow = windowPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                            if (string.Equals(normalizedWindow, normalizedTarget, StringComparison.OrdinalIgnoreCase))
                            {
                                dynamic item = window.Document.Folder.ParseName(folderName);
                                if (item != null)
                                {
                                    // 31 = Select + Edit + DeselectOthers + EnsureVisible + Focused
                                    // 29 = Select + DeselectOthers + EnsureVisible + Focused
                                    int flags = enterRenameMode ? 31 : 29;
                                    window.Document.SelectItem(item, flags);
                                    break;
                                }
                            }
                        }
                    }
                    catch { } // Ignore exceptions for specific windows (e.g., protected processes or IE frames)
                }
            }
            catch { } // Suppress outer COM exceptions so a failed rename doesn't crash the deployment
        }
        #endregion



        #region HelperMethods
        private static string GetUniqueDirectoryPath(string basePath, string desiredName)
        {
            string fullPath = Path.Combine(basePath, desiredName);
            int count = 1;

            while (Directory.Exists(fullPath))
            {
                fullPath = Path.Combine(basePath, $"{desiredName} ({count})");
                count++;
            }

            return fullPath;
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }
        #endregion
    }
}