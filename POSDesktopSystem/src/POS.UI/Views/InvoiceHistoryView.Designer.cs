namespace POS.UI.Views;

partial class InvoiceHistoryView
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlToolbar;
    private Label lblSearch;
    private TextBox txtSearch;
    private Button btnViewDetails;
    private Button btnRefresh;
    private DataGridView dgvInvoices;
    private Panel pnlStatus;
    private Label lblStatus;

    private void InitializeComponent()
    {
        pnlToolbar = new Panel();
        lblSearch = new Label();
        txtSearch = new TextBox();
        btnViewDetails = new Button();
        btnRefresh = new Button();
        dgvInvoices = new DataGridView();
        pnlStatus = new Panel();
        lblStatus = new Label();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvInvoices).BeginInit();
        pnlStatus.SuspendLayout();
        SuspendLayout();

        pnlToolbar.BackColor = Color.White;
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnViewDetails);
        pnlToolbar.Controls.Add(txtSearch);
        pnlToolbar.Controls.Add(lblSearch);
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Location = new Point(16, 16);
        pnlToolbar.Name = "pnlToolbar";
        pnlToolbar.Padding = new Padding(0, 0, 0, 12);
        pnlToolbar.Size = new Size(728, 72);
        pnlToolbar.TabIndex = 0;

        lblSearch.AutoSize = true;
        lblSearch.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSearch.Location = new Point(0, 4);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(205, 15);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Search by invoice # or cashier name";

        txtSearch.Font = new Font("Segoe UI", 10F);
        txtSearch.Location = new Point(0, 24);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Type to search...";
        txtSearch.Size = new Size(280, 25);
        txtSearch.TabIndex = 1;

        btnViewDetails.BackColor = Color.FromArgb(30, 64, 124);
        btnViewDetails.FlatAppearance.BorderSize = 0;
        btnViewDetails.FlatStyle = FlatStyle.Flat;
        btnViewDetails.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnViewDetails.ForeColor = Color.White;
        btnViewDetails.Location = new Point(300, 22);
        btnViewDetails.Name = "btnViewDetails";
        btnViewDetails.Size = new Size(110, 30);
        btnViewDetails.TabIndex = 2;
        btnViewDetails.Text = "View Details";
        btnViewDetails.UseVisualStyleBackColor = false;

        btnRefresh.Font = new Font("Segoe UI", 9F);
        btnRefresh.Location = new Point(416, 22);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(90, 30);
        btnRefresh.TabIndex = 3;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;

        dgvInvoices.AllowUserToAddRows = false;
        dgvInvoices.AllowUserToDeleteRows = false;
        dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvInvoices.BackgroundColor = Color.White;
        dgvInvoices.BorderStyle = BorderStyle.FixedSingle;
        dgvInvoices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvInvoices.Dock = DockStyle.Fill;
        dgvInvoices.Location = new Point(16, 88);
        dgvInvoices.MultiSelect = false;
        dgvInvoices.Name = "dgvInvoices";
        dgvInvoices.ReadOnly = true;
        dgvInvoices.RowHeadersVisible = false;
        dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvInvoices.Size = new Size(728, 368);
        dgvInvoices.TabIndex = 1;

        pnlStatus.Controls.Add(lblStatus);
        pnlStatus.Dock = DockStyle.Bottom;
        pnlStatus.Location = new Point(16, 456);
        pnlStatus.Name = "pnlStatus";
        pnlStatus.Padding = new Padding(0, 8, 0, 0);
        pnlStatus.Size = new Size(728, 28);
        pnlStatus.TabIndex = 2;

        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9F);
        lblStatus.ForeColor = Color.FromArgb(90, 90, 90);
        lblStatus.Location = new Point(0, 8);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(98, 15);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "0 invoice(s) found";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);
        Controls.Add(dgvInvoices);
        Controls.Add(pnlStatus);
        Controls.Add(pnlToolbar);
        Name = "InvoiceHistoryView";
        Padding = new Padding(16);
        Size = new Size(760, 500);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvInvoices).EndInit();
        pnlStatus.ResumeLayout(false);
        pnlStatus.PerformLayout();
        ResumeLayout(false);
    }
}
