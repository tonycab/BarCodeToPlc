using System;
using System.Collections.Generic;
using System.Data;
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
                        Console.WriteLine("Connecting database:");
                        Console.WriteLine("=========================================\n");

                        connection.Open();

                    Random r = new Random();

                    int name = r.Next();
                    DateTime d = DateTime.Now;


                    //Ecriture dans la base de donnees
                    String sql = "insert into GRAVING_TABLE values (@SERIAL_NUMBER,@DATE);";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.Add("@SERIAL_NUMBER", SqlDbType.Int).Value = name;
                        command.Parameters.Add("@DATE", SqlDbType.DateTime).Value = d;

                        command.ExecuteNonQuery();

                    }

              

                    //Lecture dans la base de donnees
                    String sqlread = "Select TOP(10) * from GRAVING_TABLE ORDER BY DATE DESC";

                    using (SqlCommand command = new SqlCommand(sqlread, connection))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine($" {"SERIAL_NUMBER".PadRight(16)}| {"DATE".PadRight(25)}");

                            while (reader.Read())
                            {
                                DateTime t = reader.GetDateTime(1);

                                Console.WriteLine($" {reader.GetValue(0).ToString().PadRight(16)}| {reader.GetDateTime(1).ToString().PadRight(25)}");
                            }
                        }

                    }
   

                    ////Suppression dans la base de donnees
                    //String sqlDelete = "delete from GRAVING_TABLE WHERE SERIAL_NUMBER=@SERIAL_NUMBER";

                    //using (SqlCommand command = new SqlCommand(sqlDelete, connection))
                    //{
                    //    command.Parameters.AddWithValue("@SERIAL_NUMBER", name);
                    //    command.ExecuteNonQuery();
 

                    //}


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

