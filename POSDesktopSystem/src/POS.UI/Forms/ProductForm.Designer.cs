namespace POS.UI.Forms;

partial class ProductForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlHeader;
    private Label lblFormTitle;
    private Panel pnlBody;
    private Label lblName;
    private TextBox txtName;
    private Label lblBarcode;
    private TextBox txtBarcode;
    private Label lblPrice;
    private NumericUpDown numPrice;
    private Label lblStock;
    private NumericUpDown numStock;
    private Label lblErrorMessage;
    private Panel pnlFooter;
    private Button btnSave;
    private Button btnCancel;

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblFormTitle = new Label();
        pnlBody = new Panel();
        lblName = new Label();
        txtName = new TextBox();
        lblBarcode = new Label();
        txtBarcode = new TextBox();
        lblPrice = new Label();
        numPrice = new NumericUpDown();
        lblStock = new Label();
        numStock = new NumericUpDown();
        lblErrorMessage = new Label();
        pnlFooter = new Panel();
        btnSave = new Button();
        btnCancel = new Button();
        pnlHeader.SuspendLayout();
        pnlBody.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numStock).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        pnlHeader.BackColor = Color.FromArgb(30, 64, 124);
        pnlHeader.Controls.Add(lblFormTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(464, 56);
        pnlHeader.TabIndex = 0;

        lblFormTitle.AutoSize = true;
        lblFormTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.White;
        lblFormTitle.Location = new Point(20, 16);
        lblFormTitle.Name = "lblFormTitle";
        lblFormTitle.Size = new Size(114, 25);
        lblFormTitle.TabIndex = 0;
        lblFormTitle.Text = "Add Product";

        pnlBody.Controls.Add(lblErrorMessage);
        pnlBody.Controls.Add(numStock);
        pnlBody.Controls.Add(lblStock);
        pnlBody.Controls.Add(numPrice);
        pnlBody.Controls.Add(lblPrice);
        pnlBody.Controls.Add(txtBarcode);
        pnlBody.Controls.Add(lblBarcode);
        pnlBody.Controls.Add(txtName);
        pnlBody.Controls.Add(lblName);
        pnlBody.Dock = DockStyle.Fill;
        pnlBody.Location = new Point(0, 56);
        pnlBody.Name = "pnlBody";
        pnlBody.Padding = new Padding(24, 20, 24, 12);
        pnlBody.Size = new Size(464, 254);
        pnlBody.TabIndex = 1;

        lblName.AutoSize = true;
        lblName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblName.Location = new Point(27, 23);
        lblName.Name = "lblName";
        lblName.Size = new Size(42, 15);
        lblName.TabIndex = 0;
        lblName.Text = "Name";

        txtName.Font = new Font("Segoe UI", 10F);
        txtName.Location = new Point(27, 43);
        txtName.Name = "txtName";
        txtName.Size = new Size(410, 25);
        txtName.TabIndex = 1;

        lblBarcode.AutoSize = true;
        lblBarcode.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBarcode.Location = new Point(27, 78);
        lblBarcode.Name = "lblBarcode";
        lblBarcode.Size = new Size(54, 15);
        lblBarcode.TabIndex = 2;
        lblBarcode.Text = "Barcode";

        txtBarcode.Font = new Font("Segoe UI", 10F);
        txtBarcode.Location = new Point(27, 98);
        txtBarcode.Name = "txtBarcode";
        txtBarcode.Size = new Size(410, 25);
        txtBarcode.TabIndex = 2;

        lblPrice.AutoSize = true;
        lblPrice.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPrice.Location = new Point(27, 133);
        lblPrice.Name = "lblPrice";
        lblPrice.Size = new Size(35, 15);
        lblPrice.TabIndex = 3;
        lblPrice.Text = "Price";

        numPrice.DecimalPlaces = 2;
        numPrice.Font = new Font("Segoe UI", 10F);
        numPrice.Location = new Point(27, 153);
        numPrice.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numPrice.Name = "numPrice";
        numPrice.Size = new Size(190, 25);
        numPrice.TabIndex = 3;

        lblStock.AutoSize = true;
        lblStock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStock.Location = new Point(247, 133);
        lblStock.Name = "lblStock";
        lblStock.Size = new Size(38, 15);
        lblStock.TabIndex = 4;
        lblStock.Text = "Stock";

        numStock.Font = new Font("Segoe UI", 10F);
        numStock.Location = new Point(247, 153);
        numStock.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numStock.Name = "numStock";
        numStock.Size = new Size(190, 25);
        numStock.TabIndex = 4;

        lblErrorMessage.Font = new Font("Segoe UI", 9F);
        lblErrorMessage.ForeColor = Color.FromArgb(198, 40, 40);
        lblErrorMessage.Location = new Point(27, 190);
        lblErrorMessage.Name = "lblErrorMessage";
        lblErrorMessage.Size = new Size(410, 40);
        lblErrorMessage.TabIndex = 5;
        lblErrorMessage.Visible = false;

        pnlFooter.Controls.Add(btnCancel);
        pnlFooter.Controls.Add(btnSave);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 310);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Padding = new Padding(24, 10, 24, 16);
        pnlFooter.Size = new Size(464, 64);
        pnlFooter.TabIndex = 2;

        btnSave.BackColor = Color.FromArgb(30, 64, 124);
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSave.ForeColor = Color.White;
        btnSave.Location = new Point(227, 10);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 0;
        btnSave.Text = "Save";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Click += btnSave_Click;

        btnCancel.Font = new Font("Segoe UI", 10F);
        btnCancel.Location = new Point(337, 10);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;

        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        CancelButton = btnCancel;
        ClientSize = new Size(464, 374);
        Controls.Add(pnlBody);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProductForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add Product";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlBody.ResumeLayout(false);
        pnlBody.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
        ((System.ComponentModel.ISupportInitialize)numStock).EndInit();
        pnlFooter.ResumeLayout(false);
        ResumeLayout(false);
    }
}
