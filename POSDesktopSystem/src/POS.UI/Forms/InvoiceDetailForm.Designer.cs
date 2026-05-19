namespace POS.UI.Forms;

partial class InvoiceDetailForm
{
    private System.ComponentModel.IContainer components = null;
    private Panel pnlHeader;
    private Label lblInvoiceTitle;
    private Panel pnlSummary;
    private Label lblDate;
    private Label lblDateValue;
    private Label lblCreatedAt;
    private Label lblCreatedAtValue;
    private Label lblCashier;
    private Label lblCashierValue;
    private Label lblPayment;
    private Label lblPaymentValue;
    private Label lblSubtotal;
    private Label lblSubtotalValue;
    private Label lblDiscount;
    private Label lblDiscountValue;
    private Label lblTax;
    private Label lblTaxValue;
    private Label lblFinal;
    private Label lblFinalValue;
    private DataGridView dgvItems;
    private Panel pnlFooter;
    private Label lblReceiptFormat;
    private ComboBox cmbReceiptFormat;
    private Button btnPrintPreview;
    private Button btnPrint;
    private Button btnExportPdf;
    private Button btnClose;

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
        lblInvoiceTitle = new Label();
        pnlSummary = new Panel();
        lblDate = new Label();
        lblDateValue = new Label();
        lblCreatedAt = new Label();
        lblCreatedAtValue = new Label();
        lblCashier = new Label();
        lblCashierValue = new Label();
        lblPayment = new Label();
        lblPaymentValue = new Label();
        lblSubtotal = new Label();
        lblSubtotalValue = new Label();
        lblDiscount = new Label();
        lblDiscountValue = new Label();
        lblTax = new Label();
        lblTaxValue = new Label();
        lblFinal = new Label();
        lblFinalValue = new Label();
        dgvItems = new DataGridView();
        pnlFooter = new Panel();
        lblReceiptFormat = new Label();
        cmbReceiptFormat = new ComboBox();
        btnPrintPreview = new Button();
        btnPrint = new Button();
        btnExportPdf = new Button();
        btnClose = new Button();
        pnlHeader.SuspendLayout();
        pnlSummary.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        pnlHeader.BackColor = Color.FromArgb(30, 64, 124);
        pnlHeader.Controls.Add(lblInvoiceTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(684, 56);
        pnlHeader.TabIndex = 0;

        lblInvoiceTitle.AutoSize = true;
        lblInvoiceTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblInvoiceTitle.ForeColor = Color.White;
        lblInvoiceTitle.Location = new Point(20, 16);
        lblInvoiceTitle.Name = "lblInvoiceTitle";
        lblInvoiceTitle.Size = new Size(108, 25);
        lblInvoiceTitle.TabIndex = 0;
        lblInvoiceTitle.Text = "Invoice #001";

        pnlSummary.BackColor = Color.White;
        pnlSummary.Controls.Add(lblFinalValue);
        pnlSummary.Controls.Add(lblFinal);
        pnlSummary.Controls.Add(lblTaxValue);
        pnlSummary.Controls.Add(lblTax);
        pnlSummary.Controls.Add(lblDiscountValue);
        pnlSummary.Controls.Add(lblDiscount);
        pnlSummary.Controls.Add(lblSubtotalValue);
        pnlSummary.Controls.Add(lblSubtotal);
        pnlSummary.Controls.Add(lblPaymentValue);
        pnlSummary.Controls.Add(lblPayment);
        pnlSummary.Controls.Add(lblCashierValue);
        pnlSummary.Controls.Add(lblCashier);
        pnlSummary.Controls.Add(lblCreatedAtValue);
        pnlSummary.Controls.Add(lblCreatedAt);
        pnlSummary.Controls.Add(lblDateValue);
        pnlSummary.Controls.Add(lblDate);
        pnlSummary.Dock = DockStyle.Top;
        pnlSummary.Location = new Point(0, 56);
        pnlSummary.Name = "pnlSummary";
        pnlSummary.Padding = new Padding(20, 16, 20, 12);
        pnlSummary.Size = new Size(684, 150);
        pnlSummary.TabIndex = 1;

        lblDate.AutoSize = true;
        lblDate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDate.Location = new Point(23, 19);
        lblDate.Name = "lblDate";
        lblDate.Size = new Size(34, 15);
        lblDate.TabIndex = 0;
        lblDate.Text = "Date";

        lblDateValue.AutoSize = true;
        lblDateValue.Location = new Point(120, 19);
        lblDateValue.Name = "lblDateValue";
        lblDateValue.Size = new Size(31, 15);
        lblDateValue.TabIndex = 1;
        lblDateValue.Text = "Date";

        lblCreatedAt.AutoSize = true;
        lblCreatedAt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCreatedAt.Location = new Point(350, 19);
        lblCreatedAt.Name = "lblCreatedAt";
        lblCreatedAt.Size = new Size(66, 15);
        lblCreatedAt.TabIndex = 2;
        lblCreatedAt.Text = "Created At";

        lblCreatedAtValue.AutoSize = true;
        lblCreatedAtValue.Location = new Point(450, 19);
        lblCreatedAtValue.Name = "lblCreatedAtValue";
        lblCreatedAtValue.Size = new Size(66, 15);
        lblCreatedAtValue.TabIndex = 3;
        lblCreatedAtValue.Text = "Created At";

        lblCashier.AutoSize = true;
        lblCashier.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCashier.Location = new Point(23, 46);
        lblCashier.Name = "lblCashier";
        lblCashier.Size = new Size(49, 15);
        lblCashier.TabIndex = 4;
        lblCashier.Text = "Cashier";

        lblCashierValue.AutoSize = true;
        lblCashierValue.Location = new Point(120, 46);
        lblCashierValue.Name = "lblCashierValue";
        lblCashierValue.Size = new Size(49, 15);
        lblCashierValue.TabIndex = 5;
        lblCashierValue.Text = "Cashier";

        lblPayment.AutoSize = true;
        lblPayment.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPayment.Location = new Point(350, 46);
        lblPayment.Name = "lblPayment";
        lblPayment.Size = new Size(58, 15);
        lblPayment.TabIndex = 6;
        lblPayment.Text = "Payment";

        lblPaymentValue.AutoSize = true;
        lblPaymentValue.Location = new Point(450, 46);
        lblPaymentValue.Name = "lblPaymentValue";
        lblPaymentValue.Size = new Size(34, 15);
        lblPaymentValue.TabIndex = 7;
        lblPaymentValue.Text = "Cash";

        lblSubtotal.AutoSize = true;
        lblSubtotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSubtotal.Location = new Point(23, 78);
        lblSubtotal.Name = "lblSubtotal";
        lblSubtotal.Size = new Size(52, 15);
        lblSubtotal.TabIndex = 8;
        lblSubtotal.Text = "Subtotal";

        lblSubtotalValue.AutoSize = true;
        lblSubtotalValue.Location = new Point(120, 78);
        lblSubtotalValue.Name = "lblSubtotalValue";
        lblSubtotalValue.Size = new Size(34, 15);
        lblSubtotalValue.TabIndex = 9;
        lblSubtotalValue.Text = "$0.00";

        lblDiscount.AutoSize = true;
        lblDiscount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDiscount.Location = new Point(200, 78);
        lblDiscount.Name = "lblDiscount";
        lblDiscount.Size = new Size(58, 15);
        lblDiscount.TabIndex = 10;
        lblDiscount.Text = "Discount";

        lblDiscountValue.AutoSize = true;
        lblDiscountValue.Location = new Point(270, 78);
        lblDiscountValue.Name = "lblDiscountValue";
        lblDiscountValue.Size = new Size(34, 15);
        lblDiscountValue.TabIndex = 11;
        lblDiscountValue.Text = "$0.00";

        lblTax.AutoSize = true;
        lblTax.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTax.Location = new Point(350, 78);
        lblTax.Name = "lblTax";
        lblTax.Size = new Size(27, 15);
        lblTax.TabIndex = 12;
        lblTax.Text = "Tax";

        lblTaxValue.AutoSize = true;
        lblTaxValue.Location = new Point(450, 78);
        lblTaxValue.Name = "lblTaxValue";
        lblTaxValue.Size = new Size(34, 15);
        lblTaxValue.TabIndex = 13;
        lblTaxValue.Text = "$0.00";

        lblFinal.AutoSize = true;
        lblFinal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblFinal.Location = new Point(23, 110);
        lblFinal.Name = "lblFinal";
        lblFinal.Size = new Size(73, 19);
        lblFinal.TabIndex = 14;
        lblFinal.Text = "Final Total";

        lblFinalValue.AutoSize = true;
        lblFinalValue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblFinalValue.ForeColor = Color.FromArgb(30, 64, 124);
        lblFinalValue.Location = new Point(120, 107);
        lblFinalValue.Name = "lblFinalValue";
        lblFinalValue.Size = new Size(50, 21);
        lblFinalValue.TabIndex = 15;
        lblFinalValue.Text = "$0.00";

        dgvItems.AllowUserToAddRows = false;
        dgvItems.AllowUserToDeleteRows = false;
        dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvItems.BackgroundColor = Color.White;
        dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvItems.Dock = DockStyle.Fill;
        dgvItems.Location = new Point(0, 206);
        dgvItems.Name = "dgvItems";
        dgvItems.ReadOnly = true;
        dgvItems.RowHeadersVisible = false;
        dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvItems.Size = new Size(684, 254);
        dgvItems.TabIndex = 2;
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "Product", FillWeight = 120 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Barcode", HeaderText = "Barcode", FillWeight = 80 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Qty", FillWeight = 40 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Unit Price", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }, FillWeight = 70 });
        dgvItems.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SubTotal", HeaderText = "Subtotal", DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }, FillWeight = 70 });

        pnlFooter.Controls.Add(btnClose);
        pnlFooter.Controls.Add(btnExportPdf);
        pnlFooter.Controls.Add(btnPrint);
        pnlFooter.Controls.Add(btnPrintPreview);
        pnlFooter.Controls.Add(cmbReceiptFormat);
        pnlFooter.Controls.Add(lblReceiptFormat);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 460);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Padding = new Padding(20, 10, 20, 12);
        pnlFooter.Size = new Size(684, 52);
        pnlFooter.TabIndex = 3;

        lblReceiptFormat.AutoSize = true;
        lblReceiptFormat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblReceiptFormat.Location = new Point(23, 18);
        lblReceiptFormat.Name = "lblReceiptFormat";
        lblReceiptFormat.Size = new Size(47, 15);
        lblReceiptFormat.TabIndex = 0;
        lblReceiptFormat.Text = "Format";

        cmbReceiptFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbReceiptFormat.Font = new Font("Segoe UI", 9F);
        cmbReceiptFormat.Location = new Point(76, 14);
        cmbReceiptFormat.Name = "cmbReceiptFormat";
        cmbReceiptFormat.Size = new Size(130, 23);
        cmbReceiptFormat.TabIndex = 1;

        btnPrintPreview.BackColor = Color.FromArgb(30, 64, 124);
        btnPrintPreview.FlatAppearance.BorderSize = 0;
        btnPrintPreview.FlatStyle = FlatStyle.Flat;
        btnPrintPreview.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnPrintPreview.ForeColor = Color.White;
        btnPrintPreview.Location = new Point(230, 10);
        btnPrintPreview.Name = "btnPrintPreview";
        btnPrintPreview.Size = new Size(110, 32);
        btnPrintPreview.TabIndex = 2;
        btnPrintPreview.Text = "Print Preview";
        btnPrintPreview.UseVisualStyleBackColor = false;
        btnPrintPreview.Click += btnPrintPreview_Click;

        btnPrint.Font = new Font("Segoe UI", 9F);
        btnPrint.Location = new Point(346, 10);
        btnPrint.Name = "btnPrint";
        btnPrint.Size = new Size(80, 32);
        btnPrint.TabIndex = 3;
        btnPrint.Text = "Print";
        btnPrint.UseVisualStyleBackColor = true;
        btnPrint.Click += btnPrint_Click;

        btnExportPdf.BackColor = Color.FromArgb(46, 125, 50);
        btnExportPdf.FlatAppearance.BorderSize = 0;
        btnExportPdf.FlatStyle = FlatStyle.Flat;
        btnExportPdf.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportPdf.ForeColor = Color.White;
        btnExportPdf.Location = new Point(432, 10);
        btnExportPdf.Name = "btnExportPdf";
        btnExportPdf.Size = new Size(90, 32);
        btnExportPdf.TabIndex = 4;
        btnExportPdf.Text = "Export PDF";
        btnExportPdf.UseVisualStyleBackColor = false;
        btnExportPdf.Click += btnExportPdf_Click;

        btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClose.Location = new Point(564, 10);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(100, 32);
        btnClose.TabIndex = 4;
        btnClose.Text = "Close";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += btnClose_Click;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);
        ClientSize = new Size(684, 512);
        Controls.Add(dgvItems);
        Controls.Add(pnlFooter);
        Controls.Add(pnlSummary);
        Controls.Add(pnlHeader);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "InvoiceDetailForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Invoice Details";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlSummary.ResumeLayout(false);
        pnlSummary.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
        pnlFooter.ResumeLayout(false);
        ResumeLayout(false);
    }
}
