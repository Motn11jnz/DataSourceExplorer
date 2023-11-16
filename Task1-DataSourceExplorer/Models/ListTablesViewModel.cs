namespace Task1_DataSourceExplorer.Models
{
    public class ListTablesViewModel
    {
        public List<string> TableNames { get; set; }
        public string SelectedTableName { get; set; }

        public TableColumnViewModel TableColumnViewModel { get; set; }
    }
}
