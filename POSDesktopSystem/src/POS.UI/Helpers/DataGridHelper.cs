namespace POS.UI.Helpers;

public static class DataGridHelper
{
    public static void ConfigureReadOnlyGrid(DataGridView grid)
    {
        grid.AutoGenerateColumns = false;
        grid.BackgroundColor = UiTheme.Card;
        grid.BorderStyle = BorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.PrimaryDark;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = UiTheme.SectionFont;
        grid.ColumnHeadersHeight = 36;
        grid.DefaultCellStyle.Font = UiTheme.BodyFont;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
    }

    public static DataGridViewTextBoxColumn AddTextColumn(
        DataGridView grid,
        string dataProperty,
        string header,
        float fillWeight = 100,
        string? format = null)
    {
        var column = new DataGridViewTextBoxColumn
        {
            DataPropertyName = dataProperty,
            HeaderText = header,
            FillWeight = fillWeight,
            ReadOnly = true
        };

        if (!string.IsNullOrWhiteSpace(format))
        {
            column.DefaultCellStyle = new DataGridViewCellStyle { Format = format };
        }

        grid.Columns.Add(column);
        return column;
    }
}
