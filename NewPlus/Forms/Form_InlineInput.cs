using NewPlus.Core;

namespace NewPlus.Forms
{
    public class Form_InlineInput : Form
    {
        #region Configuration
        public string InputText { get; private set; } = string.Empty;
        private readonly string _language;
        private TextBox _txtInput;
        #endregion



        #region Constructor
        public Form_InlineInput(string language, string promptMessage)
        {
            _language = language;

            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Cursor.Position;
            this.Size = new Size(250, 48);
            this.ShowInTaskbar = false;
            this.TopMost = true;

            Label lblPrompt = new Label
            {
                Text = promptMessage,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                Location = new Point(5, 2),
                AutoSize = true
            };

            _txtInput = new TextBox
            {
                Location = new Point(5, 22),
                Size = new Size(240, 20),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };

            _txtInput.Text = string.IsNullOrEmpty(_language) ? "MyProject" : ($"My {_language} Project").Replace(" ", "");

            _txtInput.KeyDown += TxtInput_KeyDown;

            // Hook the named event handler
            this.Deactivate += Form_InlineInput_Deactivate;

            this.Controls.Add(lblPrompt);
            this.Controls.Add(_txtInput);
        }
        #endregion



        #region Overrides
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _txtInput.Focus();
            _txtInput.SelectAll();
        }
        #endregion



        #region EventHandlers
        private void Form_InlineInput_Deactivate(object? sender, EventArgs e)
        {
            // If the user clicks away, cancel the deployment
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                InputText = InitialErrorChecks.NormalizePathName(_txtInput.Text.Trim()).NormalizedName;

                // Unsubscribe to prevent Deactivate from overriding our OK result
                this.Deactivate -= Form_InlineInput_Deactivate;

                this.DialogResult = DialogResult.OK;
                e.Handled = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;

                // Unsubscribe to prevent Deactivate from firing redundantly
                this.Deactivate -= Form_InlineInput_Deactivate;

                this.DialogResult = DialogResult.Cancel;
                e.Handled = true;
                this.Close();
            }
        }
        #endregion
    }
}