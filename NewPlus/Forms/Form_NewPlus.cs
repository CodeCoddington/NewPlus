using NewPlus.Core;
using NewPlus.Models;
using System.ComponentModel;
using System.Diagnostics;



namespace NewPlus.Forms
{
    public class Form_NewPlus : Form
    {
        #region Configuration
        // --- Controls ---
        // BindingList
        public BindingList<TemplateProfile> Templates;
        public BindingList<ExtensionProfile> Extensions;

        // DataGridView
        private DataGridView _dgvTemplates;

        // ContextMenuStrip
        private ContextMenuStrip _dgvContextMenu;

        // Buttons
        private Button _btnGoTo_TemplateDirectory;
        private Button _btnAdd;
        private Button _btnDelete;
        private Button _btnSweepIcons;
        private Button _btnSweepExtensions;
        private Button _btnToggleContextMenu;

        // Picturebox
        private PictureBox _pbIconPreview;

        // ComboBox
        private ComboBox _cmbCategoryFilter;

        // ProgressBar
        private ProgressBar _pbSweep;

        // --- State Bools ---
        private static bool _loading { get; set; } = true;
        private static bool _contextMenu_IsRegistered { get; set; } = false;

        // Colors
        private Color _colorBlue = Color.FromArgb(0, 120, 215);
        private Color _colorGreen = Color.FromArgb(40, 167, 69);
        #endregion



        #region Constructor
        public Form_NewPlus()
        {
            InitializeComponent();
            InitializeSleekUI();
            LoadData();
            LoadComboBoxCategories();
            _loading = false;

            this.Shown += Form_NewPlus_Shown;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ClientSize = new Size(284, 261);
            Name = "Form_NewPlus";
            ResumeLayout(false);
        }

        private void InitializeSleekUI()
        {
            // Form
            this.Text = "New+ Configuration";
            this.Size = new Size(800, 500);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += Form_NewPlus_FormClosing;


            // Top Panel
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(45, 45, 48)
            };


            // DataGridView
            _dgvTemplates = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(30, 30, 30),
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(60, 60, 60),
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _dgvTemplates.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            _dgvTemplates.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _dgvTemplates.ColumnHeadersDefaultCellStyle.SelectionBackColor = _dgvTemplates.ColumnHeadersDefaultCellStyle.BackColor;
            _dgvTemplates.ColumnHeadersDefaultCellStyle.SelectionForeColor = _dgvTemplates.ColumnHeadersDefaultCellStyle.ForeColor;

            _dgvTemplates.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            _dgvTemplates.DefaultCellStyle.ForeColor = Color.White;
            _dgvTemplates.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            _dgvTemplates.ContextMenuStrip = _dgvContextMenu;

            _dgvTemplates.MouseDown += DgvTemplates_MouseDown;
            _dgvTemplates.SelectionChanged += DgvTemplates_SelectionChanged;


            // DataGridView Context Menu Strip
            _dgvContextMenu = new ContextMenuStrip
            {
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                ShowImageMargin = false
            };

            ToolStripMenuItem editMenuItem_1 = new ToolStripMenuItem("Update Template");
            ToolStripMenuItem editMenuItem_2 = new ToolStripMenuItem("Add Blank File");
            ToolStripMenuItem editMenuItem_3 = new ToolStripMenuItem("Add Existing Files");

            editMenuItem_1.Click += EditMenuItem_Click;
            editMenuItem_2.Click += EditMenuItem_Click;
            editMenuItem_3.Click += EditMenuItem_Click;

            _dgvContextMenu.Items.AddRange(new ToolStripItem[] { editMenuItem_1, editMenuItem_2, editMenuItem_3 });


            // Title Label
            Label lblTitle = new Label
            {
                Text = "Template Profiles",
                Font = new Font("Segoe UI Semibold", 14F),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true,
                Location = new Point(10, 15)
            };

            // Top Panel Buttons
            _btnGoTo_TemplateDirectory = CreateStyledButton("Open Template Directory", new Point(200, 12), 180);
            _btnGoTo_TemplateDirectory.Click += BtnGoTo_TemplateDirectory_Click;

            _btnAdd = CreateStyledButton("Add Template", new Point(530, 12));
            _btnAdd.Click += BtnAdd_Click;

            _btnDelete = CreateStyledButton("Delete Selected", new Point(655, 12));
            _btnDelete.Click += BtnDelete_Click;


            // PictureBox
            int centerPoint_BetweenButtons = (int)((_btnAdd.Left - _btnGoTo_TemplateDirectory.Right) / 2);
            _pbIconPreview = new PictureBox
            {
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle,
                BackgroundImageLayout = ImageLayout.Zoom,
                Location = new Point(_btnGoTo_TemplateDirectory.Right + centerPoint_BetweenButtons, 12),
                Size = new Size(35, 35)
            };


            // Add controls to top panel
            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(_btnGoTo_TemplateDirectory);
            topPanel.Controls.Add(_btnAdd);
            topPanel.Controls.Add(_btnDelete);
            topPanel.Controls.Add(_pbIconPreview);


            // Bottom Panel
            Panel bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(45, 45, 48)
            };


            // Bottom Panel Buttons
            _btnSweepIcons = CreateStyledButton("Run Icon Sweep", new Point(20, 20), 150);
            _btnSweepIcons.Click += BtnSweep_Click;

            _btnSweepExtensions = CreateStyledButton("Run Ext Sweep", new Point(_btnSweepIcons.Right + 25, 20), 150);
            _btnSweepExtensions.Click += BtnSweepExtensions_Click;

            _contextMenu_IsRegistered = RegistryManager.IsRegistered();

            _btnToggleContextMenu = CreateStyledButton("Install Context Menu", new Point(this.Width - 200, 20), 160);
            _btnToggleContextMenu.BackColor = _contextMenu_IsRegistered ? _colorGreen : _colorBlue;
            _btnToggleContextMenu.Text = _contextMenu_IsRegistered ? "Remove Context Menu" : "Install Context Menu";
            _btnToggleContextMenu.Click += BtnToggleContextMenu_Click;

            
            // ComboBox
            _cmbCategoryFilter = CreateComboBox(new Point(_btnToggleContextMenu.Left - 225, (int)((20 + 35) / 2)));
            _cmbCategoryFilter.SelectedIndexChanged += CmbCategoryFilter_SelectedIndexChanged;


            // ProgressBar
            _pbSweep = new ProgressBar
            {
                Location = new Point(190, 25),
                Width = 570,
                Height = 25,
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };


            // Add controls to bottom panel
            bottomPanel.Controls.Add(_btnSweepIcons);
            bottomPanel.Controls.Add(_btnSweepExtensions);
            bottomPanel.Controls.Add(_btnToggleContextMenu);
            bottomPanel.Controls.Add(_cmbCategoryFilter);
            bottomPanel.Controls.Add(_pbSweep);


            // Add controls to the form
            this.Controls.Add(_dgvTemplates);
            this.Controls.Add(topPanel);
            this.Controls.Add(bottomPanel);
        }

        private Button CreateStyledButton(string text, Point location, int width = 120)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
        }

        private ComboBox CreateComboBox(Point location)
        {
            ComboBox comboBox = new ComboBox
            {
                BackColor = Color.FromArgb(45, 45, 48),
                DropDownStyle = ComboBoxStyle.DropDownList,
                ForeColor = Color.White,
                Location = location,
                SelectedIndex = -1,
                Size = new Size(200, 30)
            };
            return comboBox;
        }

        private void LoadData()
        {
            var templates = ConfigurationManager.LoadTemplates();
            Templates = new BindingList<TemplateProfile>(templates);

            var extensions = ConfigurationManager.LoadExtensions();
            Extensions = new BindingList<ExtensionProfile>(extensions);

            _dgvTemplates.DataSource = Templates;

            _dgvTemplates.Columns["Id"].Visible = false;
            _dgvTemplates.Columns["TemplateFolderName"].HeaderText = "Target Folder Name";
            _dgvTemplates.Columns["IconFileName"].HeaderText = "Icon File (.ico)";
        }

        private void LoadComboBoxCategories()
        {
            _cmbCategoryFilter.Items.Clear();
            _cmbCategoryFilter.Items.Add("All Categories");
            var categories = Templates.Select(t => t.Category).Distinct().ToList();
            categories.Sort();
            foreach (var category in categories)
            {
                _cmbCategoryFilter.Items.Add(category);
            }
            _cmbCategoryFilter.SelectedIndex = 0;
        }
        #endregion



        #region EventHandlers
        private void Form_NewPlus_Shown(object sender, EventArgs e)
        {
            // Clear all DataGridView Selections
            _dgvTemplates.ClearSelection();
            _dgvTemplates.CurrentCell = null;
        }

        private void BtnGoTo_TemplateDirectory_Click(object sender, EventArgs e)
        {
            string templateDir = Directory.GetParent(ConfigurationManager.TemplatesPath).FullName;
            if (!Directory.Exists(templateDir))
            {
                Directory.CreateDirectory(templateDir);
            }
            Process.Start("explorer.exe", templateDir);
        }

        private void DgvTemplates_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = _dgvTemplates.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0)
                {
                    // Select the row the user right-clicked on
                    _dgvTemplates.ClearSelection();
                    _dgvTemplates.Rows[hitTest.RowIndex].Selected = true;

                    _dgvContextMenu.Show(_dgvTemplates, e.Location);
                }
                else
                {
                    // Cancel the menu popup if they right-clicked empty grid space
                    _dgvContextMenu.Hide();
                }
            }
        }

        private void DgvTemplates_SelectionChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            if (_dgvTemplates.SelectedRows.Count < 1)
            {
                _pbIconPreview.Image = null;
                return;
            }

            var selected = (TemplateProfile)_dgvTemplates.SelectedRows[0].DataBoundItem;
            string iconPath = string.IsNullOrEmpty(selected.IconFileName) ? Path.Combine(ConfigurationManager.IcoPath, @"_Default\Default.ico") : Path.Combine(ConfigurationManager.IcoPath, selected.IconFileName);
            _pbIconPreview.Image = new Icon(iconPath).ToBitmap();
        }

        private void EditMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            string action = menuItem.Text;

            if (_dgvTemplates.SelectedRows.Count > 0)
            {
                var selected = (TemplateProfile)_dgvTemplates.SelectedRows[0].DataBoundItem;

                var categories = Templates.Select(t => t.Category).Distinct().ToList();
                var folderNames = Templates.Select(t => t.TemplateFolderName).ToList();
                var extensionProfiles = ConfigurationManager.LoadExtensions();

                switch (action)
                {
                    case "Update Template":
                        UpdateTemplate(selected, categories, folderNames, extensionProfiles);
                        break;
                    case "Add Blank File":
                        Add_BlankFile(selected);
                        break;
                    case "Add Existing Files":
                        Add_ExistingFiles(selected);
                        break;
                }
            }
        }

        private void UpdateTemplate(TemplateProfile selected, List<string> categories, List<string> folderNames, List<ExtensionProfile> extensionProfiles)
        {
            using (Form_TemplateEditor editor = new Form_TemplateEditor(categories, folderNames, extensionProfiles, selected))
            {
                if (editor.ShowDialog(this) == DialogResult.OK)
                {
                    // Refresh the grid and filters to reflect the edited data
                    Templates.ResetBindings();
                    LoadComboBoxCategories();
                    ConfigurationManager.SaveTemplates(Templates.ToList());
                }
            }
        }

        private void Add_BlankFile(TemplateProfile selected)
        {
            string msgBoxMsg = string.Empty;
            string msgBoxTitle = "Error";
            MessageBoxButtons msgBoxButtons = MessageBoxButtons.OK;
            MessageBoxIcon msgBoxIcon = MessageBoxIcon.Error;

            string extension = selected.FileRenameExtension;
            string language = string.IsNullOrEmpty(extension) ? string.Empty : Extensions.Where(e => e.Extension == extension).Select(e => e.Language).First();

            using (Form_GenericInput inputForm = new Form_GenericInput(
                formText: "New+ - Create Blank File",
                labelText: $"Enter name of blank file to create including extension (e.g., 'New{language.Replace(" ", "")}File.{extension}'):",
                textboxDefaultText: $"New{language.Replace(" ", "")}File.{extension}",
                language,
                extension
            ))
            {
                if (inputForm.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(inputForm.InputText.Trim()))
                {
                    return;
                }
                else
                {
                    string blankFileName = inputForm.InputText.Trim();
                    string[] parse_blankFileName = blankFileName.Split('.');
                    if (parse_blankFileName.Length < 2 && string.IsNullOrWhiteSpace(parse_blankFileName.Last()))
                    {
                        msgBoxMsg = "The blank file name must include an extension (e.g., 'NewPythonFile.py').";
                        DisplayMessageBox(msgBoxMsg, msgBoxTitle, msgBoxButtons, msgBoxIcon);
                        return;
                    }

                    string templateFolderPath = Path.Combine(ConfigurationManager.TemplatesPath, selected.TemplateFolderName);
                    Debug.WriteLine(templateFolderPath);
                    if (!Directory.Exists(templateFolderPath))
                    {
                        msgBoxMsg = $"The template folder '{selected.TemplateFolderName}' does not exist.";
                        DisplayMessageBox(msgBoxMsg, msgBoxTitle, msgBoxButtons, msgBoxIcon);
                        return;
                    }

                    int templateInternalDirectoryCount = Directory.GetDirectories(templateFolderPath, "*", SearchOption.TopDirectoryOnly).Length;
                    string newFilePath = AddFile(blankFileName, selected, templateInternalDirectoryCount);

                    if (string.IsNullOrEmpty(newFilePath))
                    {
                        msgBoxMsg = $"Failed to create blank file '{blankFileName}' in template '{selected.TemplateFolderName}'.";
                        DisplayMessageBox(msgBoxMsg, msgBoxTitle, msgBoxButtons, msgBoxIcon);
                        return;
                    }
                    else
                    {
                        if (File.Exists(newFilePath))
                        {
                            string ogFileDirectory = Path.GetDirectoryName(newFilePath);
                            string ogFileName = Path.GetFileNameWithoutExtension(newFilePath);
                            string ogFileExtension = Path.GetExtension(newFilePath);

                            string tryPath = newFilePath;
                            int counter = 0;
                            
                            while (File.Exists(tryPath) && counter < 9999)
                            {
                                counter++;
                                tryPath = Path.Combine(ogFileDirectory, ogFileName + "_" + counter.ToString($"D{Math.Max(2, counter.ToString().Length)}") + ogFileExtension);
                            }

                            if (counter >= 9999 && File.Exists(tryPath))
                            {
                                msgBoxMsg = $"Failed to create blank file '{blankFileName}' in template '{selected.TemplateFolderName}'. Maximum duplicate limit reached.";
                                DisplayMessageBox(msgBoxMsg, msgBoxTitle, msgBoxButtons, msgBoxIcon);
                                return;
                            }

                            File.Create(tryPath).Dispose();
                            msgBoxMsg = $"Blank file '{Path.GetFileName(tryPath)}' created in template '{selected.TemplateFolderName}'.\r\n\r\nSweeping Template Directory to update extensions...";
                            msgBoxTitle = "Success";
                            msgBoxIcon = MessageBoxIcon.Information;
                            DisplayMessageBox(msgBoxMsg, msgBoxTitle, msgBoxButtons, msgBoxIcon);
                            SweepExtensions();
                            return;
                        }
                        else
                        {
                            File.Create(newFilePath).Dispose();
                            msgBoxMsg = $"Blank file '{blankFileName}' created in template '{selected.TemplateFolderName}'.\r\n\r\nSweeping Template Directory to update extensions...";
                            msgBoxTitle = "Success";
                            msgBoxIcon = MessageBoxIcon.Information;
                            DisplayMessageBox(msgBoxMsg, msgBoxTitle, msgBoxButtons, msgBoxIcon);
                            SweepExtensions();
                            return;
                        }
                    }
                }
            }
        }


        private void Add_ExistingFiles(TemplateProfile selected)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Existing Files to Add";
                ofd.Multiselect = true;
                if (StateMemory.FolderMemory.Default.lastFileAddFolder != null && Directory.Exists(StateMemory.FolderMemory.Default.lastFileAddFolder))
                {
                    ofd.InitialDirectory = StateMemory.FolderMemory.Default.lastFileAddFolder;
                }
                else
                {
                    ofd.InitialDirectory = ConfigurationManager.StandardUserDocumentsPath;
                }

                if (ofd.ShowDialog() != DialogResult.OK) return;

                string newFilePath = string.Empty;
                string templateFolderPath = Path.Combine(ConfigurationManager.TemplatesPath, selected.TemplateFolderName);
                foreach (string filePath in ofd.FileNames)
                {
                    string fileName = Path.GetFileName(filePath);
                    newFilePath = AddFile(fileName, selected, Directory.GetDirectories(templateFolderPath, "*", SearchOption.TopDirectoryOnly).Length);
                    if (!string.IsNullOrWhiteSpace(newFilePath) && Directory.Exists(Directory.GetParent(newFilePath).FullName))
                    {
                        File.Copy(filePath, newFilePath, overwrite: true);
                    }
                }

                if (!string.IsNullOrEmpty(newFilePath))
                {
                    StateMemory.FolderMemory.Default.lastFileAddFolder = Path.GetDirectoryName(newFilePath);
                    StateMemory.FolderMemory.Default.Save();
                }

                MessageBox.Show($"Added {ofd.FileNames.Length} files to template '{selected.TemplateFolderName}'.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static string AddFile(string fileName, TemplateProfile selected, int templateInternalDirectoryCount)
        {
            string newFilePath = string.Empty;
            if (templateInternalDirectoryCount == 0)
            {
                newFilePath = Path.Combine(ConfigurationManager.TemplatesPath, selected.TemplateFolderName, fileName);
            }
            else
            {
                // Start user at top level and prompt to select the subdirectory where they want to create the blank file
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.SelectedPath = Path.Combine(ConfigurationManager.TemplatesPath, selected.TemplateFolderName);
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        newFilePath = Path.Combine(fbd.SelectedPath, fileName);
                    }
                }
            }

            return newFilePath;
        }

        private void DisplayMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            MessageBox.Show(this, message, title, buttons, icon);
        }
        

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var categories = Templates.Select(t => t.Category).Distinct().ToList();
            var folderNames = Templates.Select(t => t.TemplateFolderName).ToList();
            var extensionProfiles = ConfigurationManager.LoadExtensions();

            using Form_TemplateEditor editor = new Form_TemplateEditor(categories, folderNames, extensionProfiles);

            if (editor.ShowDialog(this) == DialogResult.OK)
            {
                Templates.Add(editor.CreatedProfile);
                ConfigurationManager.SaveTemplates(Templates.ToList());
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dgvTemplates.SelectedRows.Count > 0)
            {
                var selected = (TemplateProfile)_dgvTemplates.SelectedRows[0].DataBoundItem;
                Templates.Remove(selected);
            }
        }

        private void CmbCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return; 

            if (_cmbCategoryFilter.SelectedIndex == 0)
            {
                _dgvTemplates.DataSource = Templates;
            }
            else
            {
                string selectedCategory = _cmbCategoryFilter.SelectedItem.ToString();
                var filteredTemplates = new BindingList<TemplateProfile>(Templates.Where(t => t.Category == selectedCategory).ToList());
                _dgvTemplates.DataSource = filteredTemplates;
            }

            _dgvTemplates.ClearSelection();
            _dgvTemplates.CurrentCell = null;
        }

        private async void BtnSweep_Click(object sender, EventArgs e)
        {
            string lastSweepIcoFolder = StateMemory.FolderMemory.Default.lastSweepIcoFolder;
            if (string.IsNullOrEmpty(lastSweepIcoFolder) || !Directory.Exists(lastSweepIcoFolder)) lastSweepIcoFolder = ConfigurationManager.IcoPath;

            using OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select Icon for Sweep",
                Filter = "Icon Files|*.ico",
                InitialDirectory = lastSweepIcoFolder
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            string selectedIcon = ofd.FileName;

            string lastSweepParentDir = StateMemory.FolderMemory.Default.lastSweepParentDirectory;
            if (string.IsNullOrEmpty(lastSweepParentDir) || !Directory.Exists(lastSweepParentDir)) lastSweepParentDir = ConfigurationManager.StandardUserDocumentsPath;

            using FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                Description = "Select target directory for recursive icon sweep",
                UseDescriptionForTitle = true,
                AutoUpgradeEnabled = true,
                InitialDirectory = lastSweepParentDir,
                Multiselect = true
            };

            if (fbd.ShowDialog() != DialogResult.OK) return;

            string[] targetDirs = fbd.SelectedPaths;

            if (targetDirs.Length == 0)
            {
                MessageBox.Show("No valid directories selected for icon sweep.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update state memory only after successful error check
            StateMemory.FolderMemory.Default.lastSweepIcoFolder = Path.GetDirectoryName(selectedIcon);
            StateMemory.FolderMemory.Default.lastSweepParentDirectory = Directory.GetParent(targetDirs[0]).FullName;
            StateMemory.FolderMemory.Default.Save();

            // Execute sweep asynchronously
            try
            {
                await IconSynchronizer.ExecuteSweepAsync(this, targetDirs, selectedIcon, silent: false, _pbSweep, _btnSweepIcons);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during the icon sweep: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSweepExtensions_Click(object sender, EventArgs e)
        {
            SweepExtensions();
        }

        private void SweepExtensions()
        {
            var extensions = ConfigurationManager.LoadExtensions();
            Extensions = new BindingList<ExtensionProfile>(extensions);
            List<string> fileExtensions = extensions.Select(e => e.Extension).ToList();

            string[] templateFiles = Directory.GetFiles(ConfigurationManager.TemplatesPath, "*.*", SearchOption.AllDirectories);
            List<string> templateExtensions = templateFiles.Select(f => Path.GetExtension(f)).Distinct().ToList();

            List<string> orphanedAppData_FileExtensions = templateExtensions.Except(fileExtensions).ToList();

            if (orphanedAppData_FileExtensions.Count > 0)
            {
                for (int e = 0; e < orphanedAppData_FileExtensions.Count; e++)
                {
                    string orphanedExtension = orphanedAppData_FileExtensions[e];
                    int attemptCount = 0;
                    bool userEntry_IsValid = false;
                    string userInput = string.Empty;

                    while (attemptCount < 3 && !userEntry_IsValid)
                    {
                        using (Form_GenericInput inputForm = new Form_GenericInput("Enter Extension Info", $"Enter the language name and / or short description for the file extension '{orphanedExtension}'"))
                        {
                            if (inputForm.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(inputForm.InputText.Trim()))
                            {
                                attemptCount++;
                                continue;
                            }
                            else
                            {
                                userInput = inputForm.InputText.Trim();
                                userEntry_IsValid = true;
                            }
                        }
                    }

                    if (attemptCount >= 3 && !userEntry_IsValid)
                    {
                        MessageBox.Show("Addding an 'Undefined' Language entry for the orphaned file extension due to repeated invalid input.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        int count = 0;
                        bool uniqueEntryFound = false;
                        string generatedInput = string.Empty;
                        while (count < 9999)
                        {
                            count++;
                            string newLanguageEntry = "Undefined_" + (count.ToString($"D{Math.Max(2, count.ToString().Length)}"));
                            uniqueEntryFound = !extensions.Any(e => e.Language == newLanguageEntry);
                        }

                        // Basically impossible corner case, if this happens, there is something seriously wrong.
                        if (!uniqueEntryFound)
                        {
                            MessageBox.Show("[CRITICAL FAILURE]: Failed to create a unique 'Undefined' Language entry for the orphaned file extension. Please review your config files.\r\n\r\nExiting the Application...", "CRITICAL ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            Application.Exit();
                            return;
                        }
                        else
                        {
                            generatedInput = "Undefined_" + count.ToString($"D{Math.Max(2, count.ToString().Length)}");
                        }

                        ExtensionProfile newExtension = new ExtensionProfile
                        {
                            Extension = orphanedExtension,
                            Language = userEntry_IsValid ? userInput : generatedInput
                        };

                        Extensions.Add(newExtension);
                    }
                }
            }
            else
            {
                MessageBox.Show("No orphaned file extensions found. All template files have corresponding registered extensions.", "Sweep Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnToggleContextMenu_Click(object sender, EventArgs e)
        {
            if (_contextMenu_IsRegistered)
            {
                RegistryManager.UnregisterContextMenu();
                _contextMenu_IsRegistered = false;
                _btnToggleContextMenu.Text = "Install Context Menu";
                _btnToggleContextMenu.BackColor = _colorBlue;
            }
            else
            {
                RegistryManager.RegisterContextMenu();
                _contextMenu_IsRegistered = true;
                _btnToggleContextMenu.Text = "Remove Context Menu";
                _btnToggleContextMenu.BackColor = _colorGreen;
            }
        }
        private void Form_NewPlus_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfigurationManager.SaveTemplates(Templates.ToList());
        }
        #endregion
    }
}