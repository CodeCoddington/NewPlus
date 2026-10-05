using System.Runtime.InteropServices;
using System.Text;




namespace NewPlus.Core
{
    public static class IconSynchronizer
    {
        #region Configuration
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, string dwItem1, string dwItem2);

        private const uint SHCNE_UPDATEITEM = 0x00002000;
        private const uint SHCNF_PATHW = 0x0005;
        #endregion



        #region PublicMethods
        public static async Task ExecuteSweepAsync(Form callerForm, string[] targetDirs, string iconPath, bool silent = false, ProgressBar progressBar = null, Button controlButton = null)
        {
            if (!silent && progressBar == null) { throw new ArgumentException("UI element for progressBar must be provided when not running silently."); }

            IProgress<int> progress = null;
            if (!silent)
            {
                progressBar.Visible = true;
                progressBar.Value = 0;
                progress = new Progress<int>(percent => progressBar.Value = percent);

                if (controlButton != null && controlButton is Button) controlButton.Enabled = false;
            }

            int processedFolders = await Task.Run(() => { return SweepDirectories(targetDirs, iconPath, silent, progress); });

            if (!silent)
            {
                progressBar.Visible = false;
                if (controlButton != null && controlButton is Button) controlButton.Enabled = true;

                MessageBox.Show(callerForm, $"Icon sweep completed successfully. Applied to {processedFolders} folders.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static int SweepDirectories(string[] targetDirs, string iconPath, bool silent, IProgress<int> progress)
        {
            // HashSet automatically drops duplicate paths, and OrdinalIgnoreCase handles Windows pathing safely
            HashSet<string> allFoldersToProcess = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string dir in targetDirs)
            {
                if (!Directory.Exists(dir)) continue;

                // Add the root directory itself
                allFoldersToProcess.Add(dir);

                // Add all subdirectories
                string[] subFolders = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
                foreach (string subFolder in subFolders)
                {
                    allFoldersToProcess.Add(subFolder);
                }
            }

            int totalFolderCount = allFoldersToProcess.Count;
            if (totalFolderCount > 0)
            {
                int lastPercentReported = 0;
                int currentIndex = 0;

                foreach (string folderPath in allFoldersToProcess)
                {
                    IconSynchronizer.ApplyIconToFolder(folderPath, iconPath);
                    currentIndex++;

                    int percentComplete = (int)((float)currentIndex / totalFolderCount * 100);

                    if (percentComplete > lastPercentReported && !silent)
                    {
                        progress.Report(percentComplete);
                        lastPercentReported = percentComplete;
                    }
                }
            }

            return totalFolderCount;
        }

        public static void ApplyIconToFolder(string folderPath, string iconFileName)
        {
            if (string.IsNullOrWhiteSpace(iconFileName))
            {
                return;
            }

            string absoluteIconPath = Path.Combine(ConfigurationManager.IcoPath, iconFileName);

            if (!File.Exists(absoluteIconPath))
            {
                return;
            }

            string iniPath = Path.Combine(folderPath, "desktop.ini");

            try
            {
                if (File.Exists(iniPath))
                {
                    ClearSystemAttributes(iniPath);
                }

                string iniContent = $"[.ShellClassInfo]\r\nIconResource={absoluteIconPath},0\r\n";
                File.WriteAllText(iniPath, iniContent, Encoding.Unicode);

                File.SetAttributes(iniPath, FileAttributes.Hidden | FileAttributes.System);

                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                dirInfo.Attributes |= FileAttributes.ReadOnly;

                SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW, folderPath, null);
            }
            catch (UnauthorizedAccessException)
            {
                // Suppress access exceptions to prevent crashes during recursive sweeps of protected directories
            }
        }
        #endregion



        #region HelperMethod
        private static void ClearSystemAttributes(string filePath)
        {
            FileAttributes attributes = File.GetAttributes(filePath);
            attributes &= ~FileAttributes.Hidden;
            attributes &= ~FileAttributes.System;
            attributes &= ~FileAttributes.ReadOnly;
            File.SetAttributes(filePath, attributes);
        }
        #endregion
    }
}