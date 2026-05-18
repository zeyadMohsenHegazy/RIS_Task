namespace POS.UI.Forms;

partial class LoginForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlHeader;
    private Label lblTitle;
    private Label lblSubtitle;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPassword;
    private TextBox txtPassword;
    private Label lblErrorMessage;
    private Label lblInitStatus;
    private Button btnLogin;
    private Button btnCancel;
    private Panel pnlForm;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        lblSubtitle = new Label();
        pnlForm = new Panel();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        lblErrorMessage = new Label();
        lblInitStatus = new Label();
        btnLogin = new Button();
        btnCancel = new Button();
        pnlHeader.SuspendLayout();
        pnlForm.SuspendLayout();
        SuspendLayout();

        pnlHeader.BackColor = Color.FromArgb(30, 64, 124);
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(420, 100);
        pnlHeader.TabIndex = 0;

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(24, 22);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(245, 32);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "POS Desktop System";

        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 10F);
        lblSubtitle.ForeColor = Color.FromArgb(220, 230, 245);
        lblSubtitle.Location = new Point(27, 60);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(165, 19);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Sign in to your account";

        pnlForm.Controls.Add(lblInitStatus);
        pnlForm.Controls.Add(lblUsername);
        pnlForm.Controls.Add(txtUsername);
        pnlForm.Controls.Add(lblPassword);
        pnlForm.Controls.Add(txtPassword);
        pnlForm.Controls.Add(lblErrorMessage);
        pnlForm.Controls.Add(btnLogin);
        pnlForm.Controls.Add(btnCancel);
        pnlForm.Dock = DockStyle.Fill;
        pnlForm.Location = new Point(0, 100);
        pnlForm.Name = "pnlForm";
        pnlForm.Padding = new Padding(32, 24, 32, 24);
        pnlForm.Size = new Size(420, 280);
        pnlForm.TabIndex = 1;

        lblInitStatus.AutoSize = true;
        lblInitStatus.Font = new Font("Segoe UI", 9F);
        lblInitStatus.ForeColor = Color.FromArgb(46, 125, 50);
        lblInitStatus.Location = new Point(35, 8);
        lblInitStatus.Name = "lblInitStatus";
        lblInitStatus.Size = new Size(0, 15);
        lblInitStatus.TabIndex = 6;
        lblInitStatus.Visible = false;

        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUsername.Location = new Point(35, 28);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(63, 15);
        lblUsername.TabIndex = 0;
        lblUsername.Text = "Username";

        txtUsername.Font = new Font("Segoe UI", 10F);
        txtUsername.Location = new Point(35, 48);
        txtUsername.Name = "txtUsername";
        txtUsername.PlaceholderText = "Enter username";
        txtUsername.Size = new Size(350, 25);
        txtUsername.TabIndex = 1;

        lblPassword.AutoSize = true;
        lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPassword.Location = new Point(35, 88);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(60, 15);
        lblPassword.TabIndex = 2;
        lblPassword.Text = "Password";

        txtPassword.Font = new Font("Segoe UI", 10F);
        txtPassword.Location = new Point(35, 108);
        txtPassword.Name = "txtPassword";
        txtPassword.PasswordChar = '●';
        txtPassword.PlaceholderText = "Enter password";
        txtPassword.Size = new Size(350, 25);
        txtPassword.TabIndex = 2;
        txtPassword.KeyDown += txtPassword_KeyDown;

        lblErrorMessage.Font = new Font("Segoe UI", 9F);
        lblErrorMessage.ForeColor = Color.FromArgb(198, 40, 40);
        lblErrorMessage.Location = new Point(35, 142);
        lblErrorMessage.Name = "lblErrorMessage";
        lblErrorMessage.Size = new Size(350, 34);
        lblErrorMessage.TabIndex = 3;
        lblErrorMessage.TextAlign = ContentAlignment.MiddleLeft;
        lblErrorMessage.Visible = false;

        btnLogin.BackColor = Color.FromArgb(30, 64, 124);
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(35, 186);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(170, 38);
        btnLogin.TabIndex = 4;
        btnLogin.Text = "Login";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += btnLogin_Click;

        btnCancel.Font = new Font("Segoe UI", 10F);
        btnCancel.Location = new Point(215, 186);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(170, 38);
        btnCancel.TabIndex = 5;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;

        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(420, 380);
        Controls.Add(pnlForm);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Login - POS Desktop System";
        Shown += LoginForm_Shown;
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlForm.ResumeLayout(false);
        pnlForm.PerformLayout();
        ResumeLayout(false);
    }
}
