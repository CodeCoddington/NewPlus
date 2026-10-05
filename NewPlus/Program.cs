using NewPlus.Core;



namespace NewPlus
{
    internal static class Program
    {
        #region MainMethod
        [STAThread]
        static void Main(string[] args)
        {
            ////// Remove before publish
            ////System.Diagnostics.Debugger.Launch();

            ApplicationConfiguration.Initialize();
            ConfigurationManager.InitializeEnvironment();
            string checkResult = InitialErrorChecks.PathChecks();

            if (!string.IsNullOrEmpty(checkResult))
            {
                MessageBox.Show(checkResult + "\r\n\r\nExiting Application...", "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Application.Exit();
                return;
            }

            // If no arguments are passed, open the configuration dashboard
            if (args.Length == 0)
            {
                Application.Run(new Forms.Form_NewPlus());
            }
            // If an argument is passed, it should be the target directory from the context menu
            else
            {
                string targetDirectory = args[0];

                // Clean the path in case Windows passes it with surrounding quotes
                targetDirectory = targetDirectory.Trim('"');

                if (Directory.Exists(targetDirectory))
                {
                    Application.Run(new Forms.Form_TemplatePicker(targetDirectory));
                }
                else
                {
                    MessageBox.Show("The specified target directory does not exist.", "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}