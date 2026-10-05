namespace NewPlus.Forms
{
    public class Form_GenericInput : Form
    {
        #region Configuration
        public string InputText { get; private set; } = string.Empty;

        private readonly string _language;
        private readonly string _extension;
        private TextBox _txtInput;
        private Button _btnOk;

        private int _currentY = 0;
        private enum YGap
        {
            Padding = 20,
            InterFieldSpacing = 25,
            ExtraFieldSpacing = 50,
            ConfirmButtonSpacing = 75
        }
        #endregion



        #region Constructor
        public Form_GenericInput(string formText, string labelText, string textboxDefaultText = "", string language = "", string extension = "")
        {
            if (string.IsNullOrEmpty(language) && string.IsNullOrEmpty(extension))
            {
                _language = "Text";
                _extension = ".txt";
            }
            else
            {
                _language = language;
                _extension = extension;
            }

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font ("Segoe UI", 12F, FontStyle.Regular);
            this.Size = new Size(645, 180);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Text = formText;

            this.Shown += Form_BlankFile_Input_FormShown;
            this.FormClosing += Form_BlankFile_Input_FormClosing;

            IncrementY(YGap.Padding);

            Label lblPrompt = new Label
            {
                Text = labelText,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 12F, FontStyle.Italic),
                Location = new Point(20, _currentY),
                AutoSize = true
            };

            IncrementY(YGap.InterFieldSpacing);

            _txtInput = CreateTextBox(new Point(20, _currentY), this.Size.Width - 60);
            _txtInput.Text = textboxDefaultText;

            IncrementY(YGap.ExtraFieldSpacing);

            int btnW = 120;
            _btnOk = CreateStyledButton("OK", new Point(_txtInput.Right - btnW, _currentY));
            _btnOk.Click += BtnOk_Click;

            this.Controls.Add(lblPrompt);
            this.Controls.Add(_txtInput);
            this.Controls.Add(_btnOk);

            this.AcceptButton = _btnOk;
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
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
        }
        #endregion



        #region EventHandlers
        private void Form_BlankFile_Input_FormShown(object? sender, EventArgs e)
        {
            _txtInput.Focus();

            int txtLength = _txtInput.Text.Length;
            if (txtLength > _extension.Length + 1 && _txtInput.Text.EndsWith(_extension)) // +1 for the dot before the extension
            {
                _txtInput.Select(0, txtLength - (_extension.Length + 1));
            }
            else
            {
                _txtInput.SelectAll();
            }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            InputText = _txtInput.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Form_BlankFile_Input_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void IncrementY(YGap gap) => _currentY = _currentY + (int)gap;
        #endregion
    }
}
