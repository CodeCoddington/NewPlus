using NewPlus.Core;
using NewPlus.Models;



namespace NewPlus.Forms
{
    public class Form_TemplatePicker : Form
    {
        #region Configuration
        private readonly string _targetDirectory;
        private bool _isDeploying = false;
        #endregion



        #region Constructor
        public Form_TemplatePicker(string targetDirectory)
        {
            _targetDirectory = targetDirectory;

            // Make the host form completely invisible but give it geometry
            // so Windows Desktop Window Manager can grant it foreground focus
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.Size = new Size(10, 10);
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Cursor.Position;

            this.Load += Form_TemplatePicker_Load;
        }

        private void Form_TemplatePicker_Load(object sender, EventArgs e)
        {
            var templates = ConfigurationManager.LoadTemplates();

            if (templates.Count == 0)
            {
                MessageBox.Show("No templates configured.", "New+", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.BackColor = Color.FromArgb(45, 45, 48);
            menu.ForeColor = Color.White;
            menu.Font = new Font("Segoe UI", 10F);
            menu.ShowImageMargin = false;

            var groupedTemplates = templates.GroupBy(t => t.Category).OrderBy(g => g.Key);

            foreach (var group in groupedTemplates)
            {
                ToolStripMenuItem categoryItem = new ToolStripMenuItem(group.Key);
                categoryItem.DropDown.BackColor = Color.FromArgb(45, 45, 48);
                categoryItem.DropDown.ForeColor = Color.White;
                categoryItem.DropDown.Font = new Font("Segoe UI", 10F);

                if (categoryItem.DropDown is ToolStripDropDownMenu dropDownMenu)
                {
                    dropDownMenu.ShowImageMargin = false;
                    dropDownMenu.ShowCheckMargin = false;
                }

                foreach (var template in group.OrderBy(t => t.TemplateFolderName))
                {
                    ToolStripMenuItem templateItem = new ToolStripMenuItem(template.TemplateFolderName);
                    templateItem.Tag = template;
                    templateItem.Click += BtnTemplate_Click;
                    categoryItem.DropDownItems.Add(templateItem);
                }

                menu.Items.Add(categoryItem);
            }

            menu.Closed += Menu_Closed;

            // Steal foreground focus so the inline input box won't spawn behind Explorer
            this.Activate();
            this.Focus();

            menu.Show(Cursor.Position);
        }
        #endregion



        #region EventHandlers
        private void BtnTemplate_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is TemplateProfile profile)
            {
                // Lock the application open
                _isDeploying = true;

                // Queue the deployment to run AFTER the current UI thread message loop finishes.
                this.BeginInvoke(new Action(() => ExecuteDeployment(profile)));
            }
        }

        private async void Menu_Closed(object sender, ToolStripDropDownClosedEventArgs ev)
        {
            // The Magic Bridge: Yield the UI thread for 50 milliseconds. This gives the WinForms message pump enough time to fire the Click event and flip the _isDeploying boolean before we decide whether to kill the app.
            await Task.Delay(50);

            if (!_isDeploying) this.Close();
        }

        private void ExecuteDeployment(TemplateProfile profile)
        {
            try
            {
                bool fileRenameExtensionIsValid = !string.IsNullOrWhiteSpace(profile.FileRenameExtension);
                if (fileRenameExtensionIsValid)
                {
                    // Get language from extension and prompt user for project name
                    List<ExtensionProfile> extensionProfiles = ConfigurationManager.LoadExtensions();
                    string language = extensionProfiles.Select(p => p).FirstOrDefault(p => p.Extension.Equals(profile.FileRenameExtension, StringComparison.OrdinalIgnoreCase))?.Language ?? "";
                    string promptMessage = string.IsNullOrWhiteSpace(language) ? "Name your new project:" : $"Name your new project ({language}):";

                    using Form_InlineInput inputForm = new Form_InlineInput(language, promptMessage);

                    if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(inputForm.InputText))
                    {
                        TemplateDeployer.DeployTemplate(profile, _targetDirectory, forcedProjectName: inputForm.InputText);
                    }
                    // If DialogResult is not OK (e.g. Cancel or Escape), we do nothing and let the process abort.
                }
                else
                {
                    // If there is no fileRenameExtension, perform the standard deployment
                    TemplateDeployer.DeployTemplate(profile, _targetDirectory); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deployment failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Teardown the application thread safely once all file I/O finishes
                _isDeploying = false;
                this.Close();
            }
        }
        #endregion
    }
}