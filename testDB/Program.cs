using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace testDB
{
    internal class Program
    {

            static void Main(string[] args)
            {
                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                    builder.DataSource = "PORT-AUTO02\\SQLEXPRESS";
                    builder.UserID = "alecabellec";
                    builder.Password = "P@ssword56";
                    builder.InitialCatalog = "LINAMAR_DB";
                
                builder.TrustServerCertificate = true;


                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        Console.WriteLine("\nQuery data example:");
                        Console.WriteLine("=========================================\n");

                        connection.Open();

                        String sql = "SELECT name, collation_name FROM sys.databases";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                                }
                            }
                        }
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                Console.WriteLine("\nDone. Press enter.");
                Console.ReadLine();
            }
        }
    }

