using System.Data;
using System.Data.SqlClient;
using Task1_DataSourceExplorer.Models;

namespace Task1_DataSourceExplorer.Repository
{
    public class DataExplorerRepo
    {

        internal (string,string) SqlConnectionRepo(ConnectionViewModel con)
        {

            string conString = $"Server={con.ServerName};Database={con.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false";
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(conString))
                {
                    sqlConnection.Open();
                    return (string.Empty,conString);
                }

            }
            catch (Exception ex)
            {

                return (ex.Message,string.Empty);
            }

        }

        internal List<string> getSqlTablesRepo(string con)
        {

            string query = "SELECT name FROM sys.tables WHERE lob_data_space_id = 0";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(con))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        List<string> tableNames = new List<string>();

                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string tableName = reader["name"].ToString();
                                tableNames.Add(tableName);
                            }
                        }

                        return tableNames;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        internal async Task<List<string>> getSqlColumnsRepo(string con, string tableName)
        {

            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(con))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        List<string> columnNames = new List<string>();

                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["COLUMN_NAME"].ToString();
                                columnNames.Add(columnName);
                            }
                        }

                        return columnNames;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                return null;
            }
        }

        public async Task<DataTable> ExecuteQuery(string connectionString, string query)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
