namespace POS.UI.Views;

partial class ProductsView
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlToolbar;
    private Label lblSearch;
    private TextBox txtSearch;
    private Button btnAdd;
    private Button btnEdit;
    private Button btnDelete;
    private Button btnRefresh;
    private DataGridView dgvProducts;
    private Panel pnlStatus;
    private Label lblStatus;

    private void InitializeComponent()
    {
        pnlToolbar = new Panel();
        lblSearch = new Label();
        txtSearch = new TextBox();
        btnAdd = new Button();
        btnEdit = new Button();
        btnDelete = new Button();
        btnRefresh = new Button();
        dgvProducts = new DataGridView();
        pnlStatus = new Panel();
        lblStatus = new Label();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
        pnlStatus.SuspendLayout();
        SuspendLayout();

        pnlToolbar.BackColor = Color.White;
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnDelete);
        pnlToolbar.Controls.Add(btnEdit);
        pnlToolbar.Controls.Add(btnAdd);
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
        lblSearch.Size = new Size(161, 15);
        lblSearch.TabIndex = 0;
        lblSearch.Text = "Search by name or barcode";

        txtSearch.Font = new Font("Segoe UI", 10F);
        txtSearch.Location = new Point(0, 24);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Type to search...";
        txtSearch.Size = new Size(280, 25);
        txtSearch.TabIndex = 1;

        btnAdd.BackColor = Color.FromArgb(30, 64, 124);
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnAdd.ForeColor = Color.White;
        btnAdd.Location = new Point(300, 22);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(90, 30);
        btnAdd.TabIndex = 2;
        btnAdd.Text = "Add";
        btnAdd.UseVisualStyleBackColor = false;

        btnEdit.Font = new Font("Segoe UI", 9F);
        btnEdit.Location = new Point(396, 22);
        btnEdit.Name = "btnEdit";
        btnEdit.Size = new Size(90, 30);
        btnEdit.TabIndex = 3;
        btnEdit.Text = "Edit";
        btnEdit.UseVisualStyleBackColor = true;

        btnDelete.Font = new Font("Segoe UI", 9F);
        btnDelete.Location = new Point(492, 22);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(90, 30);
        btnDelete.TabIndex = 4;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;

        btnRefresh.Font = new Font("Segoe UI", 9F);
        btnRefresh.Location = new Point(588, 22);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(90, 30);
        btnRefresh.TabIndex = 5;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;

        dgvProducts.AllowUserToAddRows = false;
        dgvProducts.AllowUserToDeleteRows = false;
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvProducts.BackgroundColor = Color.White;
        dgvProducts.BorderStyle = BorderStyle.FixedSingle;
        dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvProducts.Dock = DockStyle.Fill;
        dgvProducts.Location = new Point(16, 88);
        dgvProducts.MultiSelect = false;
        dgvProducts.Name = "dgvProducts";
        dgvProducts.ReadOnly = true;
        dgvProducts.RowHeadersVisible = false;
        dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvProducts.Size = new Size(728, 368);
        dgvProducts.TabIndex = 1;

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
        lblStatus.Size = new Size(88, 15);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "0 product(s) found";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);
        Controls.Add(dgvProducts);
        Controls.Add(pnlStatus);
        Controls.Add(pnlToolbar);
        Name = "ProductsView";
        Padding = new Padding(16);
        Size = new Size(760, 500);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
        pnlStatus.ResumeLayout(false);
        pnlStatus.PerformLayout();
        ResumeLayout(false);
    }
}
