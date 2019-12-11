using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication5.Controllers
{
    public class Database
    {
        public static DataTable excuteQuery(String sql)
        {
            SqlCommand command;
            string connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=smartmenu;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection cnn;
            DataTable dt = new DataTable();
            try
            {
                cnn = new SqlConnection(connetionString);
                if (cnn != null && cnn.State != ConnectionState.Closed)
                {
                    cnn.Close();
                }

                if (cnn != null && cnn.State == ConnectionState.Closed)
                {
                    cnn.Open();
                }

                command = new SqlCommand(sql, cnn);
                SqlDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
                DataTable dtSchema = dr.GetSchemaTable();
                dt = new DataTable();
                // You can also use an ArrayList instead of List<> 
                List<DataColumn> listCols = new List<DataColumn>();
                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string columnName = System.Convert.ToString(drow["ColumnName"]);
                        DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]));
                        column.Unique = (bool)drow["IsUnique"];
                        column.AllowDBNull = (bool)drow["AllowDBNull"];
                        column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                        listCols.Add(column);
                        dt.Columns.Add(column);
                    }

                }

                // Read rows from DataReader and populate the DataTable 

                while (dr.Read())
                {
                    DataRow dataRow = dt.NewRow();
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        dataRow[((DataColumn)listCols[i])] = dr[i];
                    }

                    dt.Rows.Add(dataRow);
                }
                if (cnn != null && cnn.State != ConnectionState.Closed)
                {
                    cnn.Close();
                }
            }

            catch (SqlException ex)
            {
                // handle error 
            }

            catch (Exception ex)
            {
                // handle error 
            }

            finally
            {
            }
            return dt;
        }
        public static int executeScalar(String sql)
        {
            SqlCommand command;
            string connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=smartmenu;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection cnn;
            cnn = new SqlConnection(connetionString);
            if (cnn != null && cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }
            command = new SqlCommand(sql, cnn);
            int id = Convert.ToInt32(command.ExecuteScalar());
            if (cnn != null && cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            return id;
        }
    }
}
