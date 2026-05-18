namespace POS.UI.Views;

partial class PosView
{
    private System.ComponentModel.IContainer components = null;
    private TableLayoutPanel tlpRoot;
    private Panel pnlLookup;
    private Label lblBarcode;
    private TextBox txtBarcode;
    private Button btnLookup;
    private Label lblProductInfo;
    private Label lblLookupStatus;
    private Label lblQuantity;
    private NumericUpDown numQuantity;
    private Button btnAddToCart;
    private Panel pnlCart;
    private Panel pnlCartHeader;
    private Label lblCartTitle;
    private Label lblCartCount;
    private DataGridView dgvCart;
    private Panel pnlCartActions;
    private Label lblCartQty;
    private NumericUpDown numCartQuantity;
    private Button btnUpdateQty;
    private Button btnRemoveItem;
    private Button btnClearCart;
    private Panel pnlTotals;
    private Label lblTotalsTitle;
    private Label lblSubtotal;
    private Label lblSubtotalValue;
    private Label lblDiscount;
    private NumericUpDown numDiscount;
    private Label lblTax;
    private NumericUpDown numTax;
    private Label lblFinalTotal;
    private Label lblFinalTotalValue;
    private Button btnPayCash;
    private Button btnPayCard;

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
        tlpRoot = new TableLayoutPanel();
        pnlLookup = new Panel();
        lblBarcode = new Label();
        txtBarcode = new TextBox();
        btnLookup = new Button();
        lblProductInfo = new Label();
        lblLookupStatus = new Label();
        lblQuantity = new Label();
        numQuantity = new NumericUpDown();
        btnAddToCart = new Button();
        pnlCart = new Panel();
        pnlCartHeader = new Panel();
        lblCartTitle = new Label();
        lblCartCount = new Label();
        dgvCart = new DataGridView();
        pnlCartActions = new Panel();
        lblCartQty = new Label();
        numCartQuantity = new NumericUpDown();
        btnUpdateQty = new Button();
        btnRemoveItem = new Button();
        btnClearCart = new Button();
        pnlTotals = new Panel();
        lblTotalsTitle = new Label();
        lblSubtotal = new Label();
        lblSubtotalValue = new Label();
        lblDiscount = new Label();
        numDiscount = new NumericUpDown();
        lblTax = new Label();
        numTax = new NumericUpDown();
        lblFinalTotal = new Label();
        lblFinalTotalValue = new Label();
        btnPayCash = new Button();
        btnPayCard = new Button();
        tlpRoot.SuspendLayout();
        pnlLookup.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
        pnlCart.SuspendLayout();
        pnlCartHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).BeginInit();
        pnlCartActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numCartQuantity).BeginInit();
        pnlTotals.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numDiscount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numTax).BeginInit();
        SuspendLayout();

        tlpRoot.ColumnCount = 2;
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
        tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
        tlpRoot.Controls.Add(pnlCart, 0, 1);
        tlpRoot.Controls.Add(pnlLookup, 0, 0);
        tlpRoot.Controls.Add(pnlTotals, 1, 0);
        tlpRoot.SetRowSpan(pnlTotals, 2);
        tlpRoot.Dock = DockStyle.Fill;
        tlpRoot.Location = new Point(0, 0);
        tlpRoot.Name = "tlpRoot";
        tlpRoot.RowCount = 2;
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
        tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpRoot.Size = new Size(760, 500);
        tlpRoot.TabIndex = 0;

        pnlLookup.BackColor = Color.White;
        tlpRoot.SetColumnSpan(pnlLookup, 1);
        pnlLookup.Controls.Add(btnAddToCart);
        pnlLookup.Controls.Add(numQuantity);
        pnlLookup.Controls.Add(lblQuantity);
        pnlLookup.Controls.Add(lblLookupStatus);
        pnlLookup.Controls.Add(lblProductInfo);
        pnlLookup.Controls.Add(btnLookup);
        pnlLookup.Controls.Add(txtBarcode);
        pnlLookup.Controls.Add(lblBarcode);
        pnlLookup.Dock = DockStyle.Fill;
        pnlLookup.Location = new Point(3, 3);
        pnlLookup.Name = "pnlLookup";
        pnlLookup.Padding = new Padding(12);
        pnlLookup.Size = new Size(510, 124);
        pnlLookup.TabIndex = 0;

        lblBarcode.AutoSize = true;
        lblBarcode.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBarcode.Location = new Point(15, 15);
        lblBarcode.Name = "lblBarcode";
        lblBarcode.Size = new Size(145, 15);
        lblBarcode.TabIndex = 0;
        lblBarcode.Text = "Barcode / Product Search";

        txtBarcode.Font = new Font("Segoe UI", 10F);
        txtBarcode.Location = new Point(15, 35);
        txtBarcode.Name = "txtBarcode";
        txtBarcode.PlaceholderText = "Scan barcode or type product name...";
        txtBarcode.Size = new Size(280, 25);
        txtBarcode.TabIndex = 1;

        btnLookup.BackColor = Color.FromArgb(30, 64, 124);
        btnLookup.FlatAppearance.BorderSize = 0;
        btnLookup.FlatStyle = FlatStyle.Flat;
        btnLookup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLookup.ForeColor = Color.White;
        btnLookup.Location = new Point(305, 33);
        btnLookup.Name = "btnLookup";
        btnLookup.Size = new Size(80, 28);
        btnLookup.TabIndex = 2;
        btnLookup.Text = "Search";
        btnLookup.UseVisualStyleBackColor = false;

        lblProductInfo.Font = new Font("Segoe UI", 9F);
        lblProductInfo.ForeColor = Color.FromArgb(60, 60, 60);
        lblProductInfo.Location = new Point(15, 66);
        lblProductInfo.Name = "lblProductInfo";
        lblProductInfo.Size = new Size(480, 18);
        lblProductInfo.TabIndex = 3;
        lblProductInfo.Text = "No product selected";

        lblLookupStatus.Font = new Font("Segoe UI", 9F);
        lblLookupStatus.ForeColor = Color.FromArgb(90, 90, 90);
        lblLookupStatus.Location = new Point(15, 86);
        lblLookupStatus.Name = "lblLookupStatus";
        lblLookupStatus.Size = new Size(480, 18);
        lblLookupStatus.TabIndex = 4;

        lblQuantity.AutoSize = true;
        lblQuantity.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblQuantity.Location = new Point(395, 15);
        lblQuantity.Name = "lblQuantity";
        lblQuantity.Size = new Size(56, 15);
        lblQuantity.TabIndex = 5;
        lblQuantity.Text = "Quantity";

        numQuantity.Font = new Font("Segoe UI", 10F);
        numQuantity.Location = new Point(395, 35);
        numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numQuantity.Name = "numQuantity";
        numQuantity.Size = new Size(70, 25);
        numQuantity.TabIndex = 6;
        numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });

        btnAddToCart.BackColor = Color.FromArgb(46, 125, 50);
        btnAddToCart.FlatAppearance.BorderSize = 0;
        btnAddToCart.FlatStyle = FlatStyle.Flat;
        btnAddToCart.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnAddToCart.ForeColor = Color.White;
        btnAddToCart.Location = new Point(395, 66);
        btnAddToCart.Name = "btnAddToCart";
        btnAddToCart.Size = new Size(100, 38);
        btnAddToCart.TabIndex = 7;
        btnAddToCart.Text = "Add to Cart";
        btnAddToCart.UseVisualStyleBackColor = false;

        pnlCart.BackColor = Color.White;
        pnlCart.Controls.Add(dgvCart);
        pnlCart.Controls.Add(pnlCartActions);
        pnlCart.Controls.Add(pnlCartHeader);
        pnlCart.Dock = DockStyle.Fill;
        pnlCart.Location = new Point(3, 133);
        pnlCart.Name = "pnlCart";
        pnlCart.Padding = new Padding(12);
        pnlCart.Size = new Size(510, 364);
        pnlCart.TabIndex = 1;

        pnlCartHeader.Controls.Add(lblCartCount);
        pnlCartHeader.Controls.Add(lblCartTitle);
        pnlCartHeader.Dock = DockStyle.Top;
        pnlCartHeader.Location = new Point(12, 12);
        pnlCartHeader.Name = "pnlCartHeader";
        pnlCartHeader.Size = new Size(486, 36);
        pnlCartHeader.TabIndex = 0;

        lblCartTitle.AutoSize = true;
        lblCartTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblCartTitle.ForeColor = Color.FromArgb(30, 64, 124);
        lblCartTitle.Location = new Point(3, 8);
        lblCartTitle.Name = "lblCartTitle";
        lblCartTitle.Size = new Size(92, 20);
        lblCartTitle.TabIndex = 0;
        lblCartTitle.Text = "Current Cart";

        lblCartCount.Dock = DockStyle.Right;
        lblCartCount.Font = new Font("Segoe UI", 9F);
        lblCartCount.ForeColor = Color.FromArgb(90, 90, 90);
        lblCartCount.Location = new Point(373, 0);
        lblCartCount.Name = "lblCartCount";
        lblCartCount.Padding = new Padding(0, 8, 3, 0);
        lblCartCount.Size = new Size(110, 36);
        lblCartCount.TabIndex = 1;
        lblCartCount.Text = "0 item(s)";
        lblCartCount.TextAlign = ContentAlignment.TopRight;

        dgvCart.AllowUserToAddRows = false;
        dgvCart.AllowUserToDeleteRows = false;
        dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCart.BackgroundColor = Color.White;
        dgvCart.BorderStyle = BorderStyle.FixedSingle;
        dgvCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvCart.Dock = DockStyle.Fill;
        dgvCart.Location = new Point(12, 48);
        dgvCart.MultiSelect = false;
        dgvCart.Name = "dgvCart";
        dgvCart.ReadOnly = true;
        dgvCart.RowHeadersVisible = false;
        dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCart.Size = new Size(486, 220);
        dgvCart.TabIndex = 1;

        pnlCartActions.Controls.Add(btnClearCart);
        pnlCartActions.Controls.Add(btnRemoveItem);
        pnlCartActions.Controls.Add(btnUpdateQty);
        pnlCartActions.Controls.Add(numCartQuantity);
        pnlCartActions.Controls.Add(lblCartQty);
        pnlCartActions.Dock = DockStyle.Bottom;
        pnlCartActions.Location = new Point(12, 268);
        pnlCartActions.Name = "pnlCartActions";
        pnlCartActions.Size = new Size(486, 84);
        pnlCartActions.TabIndex = 2;

        lblCartQty.AutoSize = true;
        lblCartQty.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCartQty.Location = new Point(3, 10);
        lblCartQty.Name = "lblCartQty";
        lblCartQty.Size = new Size(96, 15);
        lblCartQty.TabIndex = 0;
        lblCartQty.Text = "Selected Qty";

        numCartQuantity.Font = new Font("Segoe UI", 10F);
        numCartQuantity.Location = new Point(3, 30);
        numCartQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numCartQuantity.Name = "numCartQuantity";
        numCartQuantity.Size = new Size(70, 25);
        numCartQuantity.TabIndex = 1;
        numCartQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });

        btnUpdateQty.Font = new Font("Segoe UI", 9F);
        btnUpdateQty.Location = new Point(79, 28);
        btnUpdateQty.Name = "btnUpdateQty";
        btnUpdateQty.Size = new Size(100, 28);
        btnUpdateQty.TabIndex = 2;
        btnUpdateQty.Text = "Update Qty";
        btnUpdateQty.UseVisualStyleBackColor = true;

        btnRemoveItem.Font = new Font("Segoe UI", 9F);
        btnRemoveItem.Location = new Point(185, 28);
        btnRemoveItem.Name = "btnRemoveItem";
        btnRemoveItem.Size = new Size(100, 28);
        btnRemoveItem.TabIndex = 3;
        btnRemoveItem.Text = "Remove";
        btnRemoveItem.UseVisualStyleBackColor = true;

        btnClearCart.Font = new Font("Segoe UI", 9F);
        btnClearCart.Location = new Point(291, 28);
        btnClearCart.Name = "btnClearCart";
        btnClearCart.Size = new Size(100, 28);
        btnClearCart.TabIndex = 4;
        btnClearCart.Text = "Clear Cart";
        btnClearCart.UseVisualStyleBackColor = true;

        pnlTotals.BackColor = Color.White;
        pnlTotals.Controls.Add(btnPayCard);
        pnlTotals.Controls.Add(btnPayCash);
        pnlTotals.Controls.Add(lblFinalTotalValue);
        pnlTotals.Controls.Add(lblFinalTotal);
        pnlTotals.Controls.Add(numTax);
        pnlTotals.Controls.Add(lblTax);
        pnlTotals.Controls.Add(numDiscount);
        pnlTotals.Controls.Add(lblDiscount);
        pnlTotals.Controls.Add(lblSubtotalValue);
        pnlTotals.Controls.Add(lblSubtotal);
        pnlTotals.Controls.Add(lblTotalsTitle);
        pnlTotals.Dock = DockStyle.Fill;
        pnlTotals.Location = new Point(519, 3);
        pnlTotals.Name = "pnlTotals";
        pnlTotals.Padding = new Padding(16);
        pnlTotals.Size = new Size(238, 494);
        pnlTotals.TabIndex = 2;

        lblTotalsTitle.AutoSize = true;
        lblTotalsTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTotalsTitle.ForeColor = Color.FromArgb(30, 64, 124);
        lblTotalsTitle.Location = new Point(19, 19);
        lblTotalsTitle.Name = "lblTotalsTitle";
        lblTotalsTitle.Size = new Size(108, 21);
        lblTotalsTitle.TabIndex = 0;
        lblTotalsTitle.Text = "Order Summary";

        lblSubtotal.AutoSize = true;
        lblSubtotal.Font = new Font("Segoe UI", 9F);
        lblSubtotal.Location = new Point(19, 58);
        lblSubtotal.Name = "lblSubtotal";
        lblSubtotal.Size = new Size(52, 15);
        lblSubtotal.TabIndex = 1;
        lblSubtotal.Text = "Subtotal";

        lblSubtotalValue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSubtotalValue.Location = new Point(19, 76);
        lblSubtotalValue.Name = "lblSubtotalValue";
        lblSubtotalValue.Size = new Size(200, 20);
        lblSubtotalValue.TabIndex = 2;
        lblSubtotalValue.Text = "$0.00";
        lblSubtotalValue.TextAlign = ContentAlignment.MiddleRight;

        lblDiscount.AutoSize = true;
        lblDiscount.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDiscount.Location = new Point(19, 110);
        lblDiscount.Name = "lblDiscount";
        lblDiscount.Size = new Size(58, 15);
        lblDiscount.TabIndex = 3;
        lblDiscount.Text = "Discount";

        numDiscount.DecimalPlaces = 2;
        numDiscount.Font = new Font("Segoe UI", 10F);
        numDiscount.Location = new Point(19, 128);
        numDiscount.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numDiscount.Name = "numDiscount";
        numDiscount.Size = new Size(200, 25);
        numDiscount.TabIndex = 4;

        lblTax.AutoSize = true;
        lblTax.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTax.Location = new Point(19, 162);
        lblTax.Name = "lblTax";
        lblTax.Size = new Size(27, 15);
        lblTax.TabIndex = 5;
        lblTax.Text = "Tax";

        numTax.DecimalPlaces = 2;
        numTax.Font = new Font("Segoe UI", 10F);
        numTax.Location = new Point(19, 180);
        numTax.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numTax.Name = "numTax";
        numTax.Size = new Size(200, 25);
        numTax.TabIndex = 6;

        lblFinalTotal.AutoSize = true;
        lblFinalTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblFinalTotal.Location = new Point(19, 220);
        lblFinalTotal.Name = "lblFinalTotal";
        lblFinalTotal.Size = new Size(73, 19);
        lblFinalTotal.TabIndex = 7;
        lblFinalTotal.Text = "Final Total";

        lblFinalTotalValue.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblFinalTotalValue.ForeColor = Color.FromArgb(30, 64, 124);
        lblFinalTotalValue.Location = new Point(19, 242);
        lblFinalTotalValue.Name = "lblFinalTotalValue";
        lblFinalTotalValue.Size = new Size(200, 36);
        lblFinalTotalValue.TabIndex = 8;
        lblFinalTotalValue.Text = "$0.00";
        lblFinalTotalValue.TextAlign = ContentAlignment.MiddleRight;

        btnPayCash.BackColor = Color.FromArgb(46, 125, 50);
        btnPayCash.FlatAppearance.BorderSize = 0;
        btnPayCash.FlatStyle = FlatStyle.Flat;
        btnPayCash.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnPayCash.ForeColor = Color.White;
        btnPayCash.Location = new Point(19, 300);
        btnPayCash.Name = "btnPayCash";
        btnPayCash.Size = new Size(200, 42);
        btnPayCash.TabIndex = 9;
        btnPayCash.Text = "Pay Cash";
        btnPayCash.UseVisualStyleBackColor = false;

        btnPayCard.BackColor = Color.FromArgb(30, 64, 124);
        btnPayCard.FlatAppearance.BorderSize = 0;
        btnPayCard.FlatStyle = FlatStyle.Flat;
        btnPayCard.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnPayCard.ForeColor = Color.White;
        btnPayCard.Location = new Point(19, 350);
        btnPayCard.Name = "btnPayCard";
        btnPayCard.Size = new Size(200, 42);
        btnPayCard.TabIndex = 10;
        btnPayCard.Text = "Pay Card";
        btnPayCard.UseVisualStyleBackColor = false;

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);
        Controls.Add(tlpRoot);
        Name = "PosView";
        Size = new Size(760, 500);
        tlpRoot.ResumeLayout(false);
        pnlLookup.ResumeLayout(false);
        pnlLookup.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
        pnlCart.ResumeLayout(false);
        pnlCartHeader.ResumeLayout(false);
        pnlCartHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCart).EndInit();
        pnlCartActions.ResumeLayout(false);
        pnlCartActions.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numCartQuantity).EndInit();
        pnlTotals.ResumeLayout(false);
        pnlTotals.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numDiscount).EndInit();
        ((System.ComponentModel.ISupportInitialize)numTax).EndInit();
        ResumeLayout(false);
    }
}
