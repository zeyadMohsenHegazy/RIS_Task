namespace POS.UI.Forms;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlSidebar;
    private Label lblAppTitle;
    private Label lblWelcome;
    private Label lblRole;
    private Panel pnlCashierBadge;
    private Label lblCashierBadge;
    private Panel pnlManagerBadge;
    private Label lblManagerBadge;
    private Panel pnlNavButtons;
    private Button btnNavPos;
    private Button btnNavProducts;
    private Button btnNavInvoices;
    private Panel pnlSidebarFooter;
    private Button btnLogout;
    private Panel pnlMain;
    private Panel pnlTopBar;
    private Label lblContentTitle;
    private Label lblSystemStatus;
    private Button btnSyncNow;
    private Panel pnlContent;

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
        pnlSidebar = new Panel();
        lblAppTitle = new Label();
        lblWelcome = new Label();
        lblRole = new Label();
        pnlCashierBadge = new Panel();
        lblCashierBadge = new Label();
        pnlManagerBadge = new Panel();
        lblManagerBadge = new Label();
        pnlNavButtons = new Panel();
        btnNavPos = new Button();
        btnNavProducts = new Button();
        btnNavInvoices = new Button();
        pnlSidebarFooter = new Panel();
        btnLogout = new Button();
        pnlMain = new Panel();
        pnlTopBar = new Panel();
        lblContentTitle = new Label();
        lblSystemStatus = new Label();
        btnSyncNow = new Button();
        pnlContent = new Panel();
        pnlSidebar.SuspendLayout();
        pnlCashierBadge.SuspendLayout();
        pnlManagerBadge.SuspendLayout();
        pnlNavButtons.SuspendLayout();
        pnlSidebarFooter.SuspendLayout();
        pnlMain.SuspendLayout();
        pnlTopBar.SuspendLayout();
        SuspendLayout();

        pnlSidebar.BackColor = Color.FromArgb(24, 52, 96);
        pnlSidebar.Controls.Add(pnlSidebarFooter);
        pnlSidebar.Controls.Add(pnlNavButtons);
        pnlSidebar.Controls.Add(pnlManagerBadge);
        pnlSidebar.Controls.Add(pnlCashierBadge);
        pnlSidebar.Controls.Add(lblRole);
        pnlSidebar.Controls.Add(lblWelcome);
        pnlSidebar.Controls.Add(lblAppTitle);
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Location = new Point(0, 0);
        pnlSidebar.Name = "pnlSidebar";
        pnlSidebar.Size = new Size(230, 640);
        pnlSidebar.TabIndex = 0;

        lblAppTitle.AutoSize = true;
        lblAppTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblAppTitle.ForeColor = Color.White;
        lblAppTitle.Location = new Point(20, 24);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Size = new Size(118, 25);
        lblAppTitle.TabIndex = 0;
        lblAppTitle.Text = "POS Desktop";

        lblWelcome.AutoSize = true;
        lblWelcome.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblWelcome.ForeColor = Color.White;
        lblWelcome.Location = new Point(22, 62);
        lblWelcome.Name = "lblWelcome";
        lblWelcome.Size = new Size(73, 19);
        lblWelcome.TabIndex = 1;
        lblWelcome.Text = "Username";

        lblRole.AutoSize = true;
        lblRole.Font = new Font("Segoe UI", 9F);
        lblRole.ForeColor = Color.FromArgb(190, 205, 225);
        lblRole.Location = new Point(22, 84);
        lblRole.Name = "lblRole";
        lblRole.Size = new Size(78, 15);
        lblRole.TabIndex = 2;
        lblRole.Text = "Role: Cashier";

        pnlCashierBadge.BackColor = Color.FromArgb(46, 125, 50);
        pnlCashierBadge.Controls.Add(lblCashierBadge);
        pnlCashierBadge.Location = new Point(20, 108);
        pnlCashierBadge.Name = "pnlCashierBadge";
        pnlCashierBadge.Size = new Size(190, 30);
        pnlCashierBadge.TabIndex = 3;
        pnlCashierBadge.Visible = false;

        lblCashierBadge.Dock = DockStyle.Fill;
        lblCashierBadge.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblCashierBadge.ForeColor = Color.White;
        lblCashierBadge.Location = new Point(0, 0);
        lblCashierBadge.Name = "lblCashierBadge";
        lblCashierBadge.Size = new Size(190, 30);
        lblCashierBadge.TabIndex = 0;
        lblCashierBadge.Text = "CASHIER";
        lblCashierBadge.TextAlign = ContentAlignment.MiddleCenter;

        pnlManagerBadge.BackColor = Color.FromArgb(198, 40, 40);
        pnlManagerBadge.Controls.Add(lblManagerBadge);
        pnlManagerBadge.Location = new Point(20, 108);
        pnlManagerBadge.Name = "pnlManagerBadge";
        pnlManagerBadge.Size = new Size(190, 30);
        pnlManagerBadge.TabIndex = 4;
        pnlManagerBadge.Visible = false;

        lblManagerBadge.Dock = DockStyle.Fill;
        lblManagerBadge.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblManagerBadge.ForeColor = Color.White;
        lblManagerBadge.Location = new Point(0, 0);
        lblManagerBadge.Name = "lblManagerBadge";
        lblManagerBadge.Size = new Size(190, 30);
        lblManagerBadge.TabIndex = 0;
        lblManagerBadge.Text = "MANAGER";
        lblManagerBadge.TextAlign = ContentAlignment.MiddleCenter;

        pnlNavButtons.Controls.Add(btnNavPos);
        pnlNavButtons.Controls.Add(btnNavProducts);
        pnlNavButtons.Controls.Add(btnNavInvoices);
        pnlNavButtons.Location = new Point(0, 156);
        pnlNavButtons.Name = "pnlNavButtons";
        pnlNavButtons.Size = new Size(230, 180);
        pnlNavButtons.TabIndex = 5;

        btnNavPos.BackColor = Color.FromArgb(24, 52, 96);
        btnNavPos.Cursor = Cursors.Hand;
        btnNavPos.FlatAppearance.BorderSize = 0;
        btnNavPos.FlatStyle = FlatStyle.Flat;
        btnNavPos.Font = new Font("Segoe UI", 10F);
        btnNavPos.ForeColor = Color.White;
        btnNavPos.Location = new Point(0, 0);
        btnNavPos.Name = "btnNavPos";
        btnNavPos.Padding = new Padding(20, 0, 0, 0);
        btnNavPos.Size = new Size(230, 48);
        btnNavPos.TabIndex = 0;
        btnNavPos.Text = "  POS Screen";
        btnNavPos.TextAlign = ContentAlignment.MiddleLeft;
        btnNavPos.UseVisualStyleBackColor = false;

        btnNavProducts.BackColor = Color.FromArgb(24, 52, 96);
        btnNavProducts.Cursor = Cursors.Hand;
        btnNavProducts.FlatAppearance.BorderSize = 0;
        btnNavProducts.FlatStyle = FlatStyle.Flat;
        btnNavProducts.Font = new Font("Segoe UI", 10F);
        btnNavProducts.ForeColor = Color.White;
        btnNavProducts.Location = new Point(0, 52);
        btnNavProducts.Name = "btnNavProducts";
        btnNavProducts.Padding = new Padding(20, 0, 0, 0);
        btnNavProducts.Size = new Size(230, 48);
        btnNavProducts.TabIndex = 1;
        btnNavProducts.Text = "  Products";
        btnNavProducts.TextAlign = ContentAlignment.MiddleLeft;
        btnNavProducts.UseVisualStyleBackColor = false;

        btnNavInvoices.BackColor = Color.FromArgb(24, 52, 96);
        btnNavInvoices.Cursor = Cursors.Hand;
        btnNavInvoices.FlatAppearance.BorderSize = 0;
        btnNavInvoices.FlatStyle = FlatStyle.Flat;
        btnNavInvoices.Font = new Font("Segoe UI", 10F);
        btnNavInvoices.ForeColor = Color.White;
        btnNavInvoices.Location = new Point(0, 104);
        btnNavInvoices.Name = "btnNavInvoices";
        btnNavInvoices.Padding = new Padding(20, 0, 0, 0);
        btnNavInvoices.Size = new Size(230, 48);
        btnNavInvoices.TabIndex = 2;
        btnNavInvoices.Text = "  Invoice History";
        btnNavInvoices.TextAlign = ContentAlignment.MiddleLeft;
        btnNavInvoices.UseVisualStyleBackColor = false;

        pnlSidebarFooter.Controls.Add(btnLogout);
        pnlSidebarFooter.Dock = DockStyle.Bottom;
        pnlSidebarFooter.Location = new Point(0, 570);
        pnlSidebarFooter.Name = "pnlSidebarFooter";
        pnlSidebarFooter.Padding = new Padding(16, 12, 16, 16);
        pnlSidebarFooter.Size = new Size(230, 70);
        pnlSidebarFooter.TabIndex = 6;

        btnLogout.BackColor = Color.FromArgb(198, 40, 40);
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.Dock = DockStyle.Fill;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(16, 12);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(198, 42);
        btnLogout.TabIndex = 0;
        btnLogout.Text = "Logout";
        btnLogout.UseVisualStyleBackColor = false;
        btnLogout.Click += btnLogout_Click;

        pnlMain.Controls.Add(pnlContent);
        pnlMain.Controls.Add(pnlTopBar);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(230, 0);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(790, 640);
        pnlMain.TabIndex = 1;

        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(btnSyncNow);
        pnlTopBar.Controls.Add(lblSystemStatus);
        pnlTopBar.Controls.Add(lblContentTitle);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(790, 56);
        pnlTopBar.TabIndex = 0;

        lblContentTitle.AutoSize = true;
        lblContentTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblContentTitle.ForeColor = Color.FromArgb(30, 64, 124);
        lblContentTitle.Location = new Point(24, 16);
        lblContentTitle.Name = "lblContentTitle";
        lblContentTitle.Size = new Size(122, 25);
        lblContentTitle.TabIndex = 0;
        lblContentTitle.Text = "Point of Sale";

        lblSystemStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblSystemStatus.AutoSize = true;
        lblSystemStatus.Font = new Font("Segoe UI", 9F);
        lblSystemStatus.ForeColor = Color.FromArgb(80, 90, 100);
        lblSystemStatus.Location = new Point(420, 20);
        lblSystemStatus.Name = "lblSystemStatus";
        lblSystemStatus.Size = new Size(90, 15);
        lblSystemStatus.TabIndex = 1;
        lblSystemStatus.Text = "SQL Server";

        btnSyncNow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSyncNow.BackColor = Color.FromArgb(46, 125, 50);
        btnSyncNow.FlatAppearance.BorderSize = 0;
        btnSyncNow.FlatStyle = FlatStyle.Flat;
        btnSyncNow.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSyncNow.ForeColor = Color.White;
        btnSyncNow.Location = new Point(670, 12);
        btnSyncNow.Name = "btnSyncNow";
        btnSyncNow.Size = new Size(96, 32);
        btnSyncNow.TabIndex = 2;
        btnSyncNow.Text = "Sync Now";
        btnSyncNow.UseVisualStyleBackColor = false;
        btnSyncNow.Visible = false;

        pnlContent.BackColor = Color.FromArgb(245, 247, 250);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(0, 56);
        pnlContent.Name = "pnlContent";
        pnlContent.Padding = new Padding(16);
        pnlContent.Size = new Size(790, 584);
        pnlContent.TabIndex = 1;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);
        ClientSize = new Size(1020, 640);
        Controls.Add(pnlMain);
        Controls.Add(pnlSidebar);
        MinimumSize = new Size(1020, 640);
        Name = "DashboardForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "POS Desktop System";
        FormClosing += DashboardForm_FormClosing;
        pnlSidebar.ResumeLayout(false);
        pnlSidebar.PerformLayout();
        pnlCashierBadge.ResumeLayout(false);
        pnlManagerBadge.ResumeLayout(false);
        pnlNavButtons.ResumeLayout(false);
        pnlSidebarFooter.ResumeLayout(false);
        pnlMain.ResumeLayout(false);
        pnlTopBar.ResumeLayout(false);
        pnlTopBar.PerformLayout();
        ResumeLayout(false);
    }
}
