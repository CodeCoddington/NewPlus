using NewPlus.Core;
using NewPlus.Models;
using NewPlus.StateMemory;



namespace NewPlus.Forms
{
    public class Form_TemplateEditor : Form
    {
        #region Configuration
        // --- Controls ---
        // Template profile(s)
        public TemplateProfile CreatedProfile { get; private set; }
        private readonly TemplateProfile _editingProfile;

        // TextBoxes
        private TextBox _txtFolderName;
        private TextBox _txtIconPath;

        // ComboBox
        private ComboBox _cmbCategory;
        private ComboBox _cmbFileRenameExtension;

        // Buttons
        private Button _btnBrowseIcon;
        private Button _btnSave;
        private Button _btnCancel;


        // --- FormBuilder Y coord ---
        private int _currentY = 0;
        private enum YGap
        {
            Padding = 20,
            InterFieldSpacing = 25,
            ExtraFieldSpacing = 50,
            ConfirmButtonSpacing = 75
        }


        // --- Fields ---
        private readonly List<string> _existingCategories;
        private readonly List<string> _existingFolderNames;
        private readonly List<ExtensionProfile> _extensionProfiles;
        private ExtensionProfile _selectedExtensionProfile;
        #endregion



        #region Constructor
        public Form_TemplateEditor(List<string> existingCategories, List<string> existingFolderNames, List<ExtensionProfile> extensionProfiles, TemplateProfile profileToEdit = null)
        {
            _existingCategories = existingCategories;
            _existingCategories.Sort(StringComparer.OrdinalIgnoreCase);

            _existingFolderNames = existingFolderNames;
            _existingFolderNames.Sort(StringComparer.OrdinalIgnoreCase);

            _extensionProfiles = extensionProfiles;

            _editingProfile = profileToEdit;

            InitializeSleekUI();

            if (_editingProfile != null)
            {
                this.Text = "Edit Template";
                _btnSave.Text = "Update Template";

                _cmbCategory.Text = _editingProfile.Category;
                _txtFolderName.Text = _editingProfile.TemplateFolderName;

                if (!string.IsNullOrWhiteSpace(_editingProfile.IconFileName))
                {
                    string fullIconPath = Path.Combine(ConfigurationManager.IcoPath, _editingProfile.IconFileName);
                    if (File.Exists(fullIconPath))
                    {
                        _txtIconPath.Text = fullIconPath;
                    }
                }
            }
        }

        private void InitializeSleekUI()
        {
            this.Text = "Template Editor";
            this.Size = new Size(450, 435);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            IncrementY(YGap.Padding);

            Label lblCategory = CreateLabel("Category:", new Point(20, _currentY));

            IncrementY(YGap.InterFieldSpacing);
            _cmbCategory = CreateComboBox(_existingCategories, new Point(20, _currentY));

            IncrementY(YGap.ExtraFieldSpacing);

            Label lblFolder = CreateLabel("Target Folder Name:", new Point(20, _currentY));
            IncrementY(YGap.InterFieldSpacing);
            _txtFolderName = CreateTextBox(new Point(20, _currentY), 390);

            IncrementY(YGap.ExtraFieldSpacing);

            Label lblIcon = CreateLabel("Icon File (.ico):", new Point(20, _currentY));

            IncrementY(YGap.InterFieldSpacing);
            _txtIconPath = CreateTextBox(new Point(20, _currentY), 280);
            _txtIconPath.ReadOnly = true;

            int buttonWidth = 100;
            _btnBrowseIcon = CreateStyledButton("Browse...", new Point(_txtFolderName.Right - buttonWidth, _currentY), buttonWidth);
            _btnBrowseIcon.Click += BtnBrowseIcon_Click;

            IncrementY(YGap.ExtraFieldSpacing);

            Label lblExtension = CreateLabel("File Rename Extension:", new Point(20, _currentY));
            IncrementY(YGap.InterFieldSpacing);

            List<string> validExtensions = _extensionProfiles.Select(e => $"{e.Extension} | {e.Language}").ToList();
            _cmbFileRenameExtension = CreateComboBox(validExtensions, new Point(20, _currentY), ComboBoxStyle.DropDownList, startingIndex: 0, width: 390);
            _cmbFileRenameExtension.SelectedIndexChanged += (s, e) => { if (_cmbFileRenameExtension.SelectedIndex >= 0) _selectedExtensionProfile = _extensionProfiles[_cmbFileRenameExtension.SelectedIndex]; };
                
            IncrementY(YGap.ConfirmButtonSpacing);

            _btnSave = CreateStyledButton("Save Template", new Point(180, _currentY), 120);
            _btnSave.BackColor = Color.FromArgb(0, 120, 215);
            _btnSave.Click += BtnSave_Click;

            _btnCancel = CreateStyledButton("Cancel", new Point(310, _currentY), 100);
            _btnCancel.BackColor = Color.FromArgb(60, 60, 60);
            _btnCancel.Click += (_, _) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] {
                lblCategory,
                _cmbCategory,
                lblFolder,
                _txtFolderName,
                lblIcon,
                _txtIconPath,
                lblExtension,
                _cmbFileRenameExtension,
                _btnBrowseIcon,
                _btnSave,
                _btnCancel
            });
        }

        private Label CreateLabel(string text, Point location)
        {
            return new Label { Text = text, Location = location, AutoSize = true, ForeColor = Color.LightGray };
        }

        private ComboBox CreateComboBox(List<string> items, Point location, ComboBoxStyle dropDownStyle = ComboBoxStyle.DropDown, int startingIndex = -1, int width = 300)
        {
            ComboBox comboBox = new ComboBox
            {
                BackColor = Color.FromArgb(45, 45, 48),
                DropDownStyle = dropDownStyle,
                ForeColor = Color.White,
                Location = location,
                SelectedIndex = -1,
                Size = new Size(width, 30)
            };
            comboBox.Items.AddRange(items.ToArray());
            return comboBox;
        }

        private TextBox CreateTextBox(Point location, int width)
        {
            return new TextBox
            {
                BackColor = Color.FromArgb(45, 45, 48),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.White,
                Location = location,
                Width = width
            };
        }

        private Button CreateStyledButton(string text, Point location, int width)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 30),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
        }
        #endregion



        #region EventHandlers
        private void BtnBrowseIcon_Click(object sender, EventArgs e)
        {
            string lastTemplateIcoFolder = StateMemory.FolderMemory.Default.lastTemplateIcoFolder;
            if (string.IsNullOrEmpty(lastTemplateIcoFolder) || !Directory.Exists(lastTemplateIcoFolder)) lastTemplateIcoFolder = ConfigurationManager.IcoPath;

            using OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select Template Icon",
                Filter = "Icon Files|*.ico",
                InitialDirectory = lastTemplateIcoFolder
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _txtIconPath.Text = ofd.FileName;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            // Normalize and check folder name for valid characters, and ensure it's not empty
            var normalizedFolderNameResult = InitialErrorChecks.NormalizePathName(_txtFolderName.Text);
            List<char> illegalCharsStripped = normalizedFolderNameResult.IllegalCharsStripped;
            string normalizedFolderName = normalizedFolderNameResult.NormalizedName;

            if (illegalCharsStripped.Count > 0)
            {
                string illegalChars = string.Join(", ", illegalCharsStripped);
                MessageBox.Show($"Folder Name contains illegal characters: {illegalChars}. They have been removed.", "Folder Name Correction", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (string.IsNullOrWhiteSpace(normalizedFolderName))
            {
                MessageBox.Show("Folder Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Folder name uniqueness check (case-insensitive) - ignore if it's the exact same folder we are currently editing
            bool isSameFolder = _editingProfile != null && _editingProfile.TemplateFolderName.Equals(normalizedFolderName, StringComparison.OrdinalIgnoreCase);

            if (!isSameFolder && _existingFolderNames.Contains(normalizedFolderName, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("A template with the same folder name already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get .ico file (if provided) and copy it to the IcoPath
            string finalIconName = string.Empty;
            string destinationIconPath = string.Empty;

            if (!string.IsNullOrWhiteSpace(_txtIconPath.Text) && File.Exists(_txtIconPath.Text))
            {
                finalIconName = Path.GetFileName(_txtIconPath.Text);
                destinationIconPath = Path.Combine(ConfigurationManager.IcoPath, finalIconName);

                if (!_txtIconPath.Text.Equals(destinationIconPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(_txtIconPath.Text, destinationIconPath, overwrite: true);
                }
            }

            // Handle physical directory creation or renaming
            string newSkeletonPath = Path.Combine(ConfigurationManager.TemplatesPath, normalizedFolderName);

            try
            {
                if (_editingProfile != null && !isSameFolder)
                {
                    string oldSkeletonPath = Path.Combine(ConfigurationManager.TemplatesPath, _editingProfile.TemplateFolderName);
                    if (Directory.Exists(oldSkeletonPath))
                    {
                        Directory.Move(oldSkeletonPath, newSkeletonPath);
                    }
                }
                else if (!Directory.Exists(newSkeletonPath))
                {
                    Directory.CreateDirectory(newSkeletonPath);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to rename the template folder. Ensure the folder or its files are not open in another program.", "File Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string category = string.IsNullOrEmpty(_cmbCategory.Text.Trim()) ? "Uncategorized" : _cmbCategory.Text.Trim();

            // Update existing or create new
            if (_editingProfile != null)
            {
                _editingProfile.Category = category;
                _editingProfile.TemplateFolderName = normalizedFolderName;
                _editingProfile.IconFileName = finalIconName;
                _editingProfile.FileRenameExtension = _selectedExtensionProfile?.Extension ?? string.Empty;
                CreatedProfile = _editingProfile;
            }
            else
            {
                CreatedProfile = new TemplateProfile
                {
                    Category = category,
                    TemplateFolderName = normalizedFolderName,
                    IconFileName = finalIconName,
                    FileRenameExtension = _selectedExtensionProfile?.Extension ?? string.Empty
                };
            }

            // Apply the icon to the skeleton folder itself and subfolders
            try
            {
                await IconSynchronizer.ExecuteSweepAsync(this, new[] { newSkeletonPath }, destinationIconPath, silent: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to apply template icon: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            this.DialogResult = DialogResult.OK;

            // Update state memory for the last used icon folder
            FolderMemory.Default.lastTemplateIcoFolder = Path.GetDirectoryName(_txtIconPath.Text);
            FolderMemory.Default.Save();
            this.Close();
        }
        #endregion



        #region HelperMethods
        private void IncrementY(YGap gap) => _currentY = _currentY + (int)gap;
        #endregion
    }
}