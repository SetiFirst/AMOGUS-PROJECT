using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BCrypt;

namespace WpfApp2
{
    public class DBConnect
    {
        private static readonly string conStr = "Data Source=C:/Users/Alex/Desktop/ASP.NET apps/WpfApp2/WpfApp2/db/auth.db";

        static private SQLiteConnection Con(string conStr)
        {
            return new SQLiteConnection(conStr);
        }

        static private bool checkUser(SQLiteDataReader db)
        {
            if(db.HasRows)
            {
                return true;
            }

            return false;
        }

        static private List<List<string>> GetData(SQLiteDataReader db)
        {
            List<List<string>> arr = new List<List<string>> ();
            int i = 0;

            if (db.HasRows)
            {
                while (db.Read())
                {

                    arr.Add(new List<string>());
                    for (int j = 0; j < db.FieldCount; j++)
                    {
                        Console.WriteLine("j");
                        arr[i].Add(db.GetValue(j).ToString());
                    }
                    i++;
                }
            } else
            {
                MessageBox.Show("Этого пользователя нет в сети!");
            }
            Console.WriteLine(arr);
            return arr;
        }

        public static void InsertData(string items, string db, string columns)
        {
            SQLiteConnection connection = Con(conStr);

            try
            {

                connection.Open();

                string insert = $"INSERT INTO {db}({columns}) VALUES({items})";

                SQLiteCommand cmd = new SQLiteCommand(insert, connection);
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Connection close");
                }
            }
        }

        static public void DeleteData(string items, string bd, string values)
        {
            SQLiteConnection connection = Con(conStr);

            try
            {
                connection.Open();

                string delete = $"DELETE {items} FROM {bd} WHERE {values}";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Connection close");
                }
            }
        }

        static public List<List<string>> GetData1(string items, string db)
        {

            SQLiteConnection connection = Con(conStr);

            try
            {
                connection.Open();

                string get = $"SELECT {items} FROM {db}";

                SQLiteCommand cmd = new SQLiteCommand();
                cmd.CommandText = get;
                cmd.Connection = connection;
                SQLiteDataReader data = cmd.ExecuteReader();
                if (checkUser(data))
                {
                    MessageBox.Show("Пользователь с таким логином уже зарегистрирован!");
                    return null;
                }
                List<List<string>> datas = GetData(data);
                Console.WriteLine(datas + "is");

                return datas;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {


                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Connection close");

                }

            }

            Console.WriteLine("The programm is finally");

            return new List<List<string>>();
        }

        public static void Register(string firstName, string lastName, string birthday, string login, string password, string phone, Window a)
        {
            string db = "auth";
            string columns = "firstname, lastname, birthday, userLogin, userPassword, number";
            string hPassword = BCrypt.Net.BCrypt.HashPassword(password);
            string values = $"'{firstName}', '{lastName}', '{birthday}', '{login}', '{hPassword}', '{phone}'";

            string insert = $"INSERT INTO {db} ({columns}) VALUES ({values})";

            SQLiteConnection connection = Con(conStr);

            try
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(insert, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Вы успешно зарегистрировались!");

                changeWindow(a);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Пользователь с таким логином или номером телефона уже разегистрирован!");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        static private void changeWindow(Window a)
        {
            Window1 window1 = new Window1();

            window1.Show();
            a.Close();
        }

        public static void auth(string login, string pass)
        {
            string db = "auth";
            string columns = "userLogin, userPassword";

            // Получаем пару логин-пароль из базы данных
            string get = $"SELECT {columns} FROM {db} WHERE userLogin='{login}'";
            SQLiteConnection connection = Con(conStr);
            List<List<string>> data = new List<List<string>>();

            try
            {
                connection.Open();
                SQLiteCommand cmd = new SQLiteCommand(get, connection);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data.Add(new List<string>
                {
                    reader.GetString(0), // userLogin
                    reader.GetString(1)  // userPassword
                });
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином не найден!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            
            // Проверяем пароль
            if (BCrypt.Net.BCrypt.Verify(pass, data[0][1]))
            {
                MessageBox.Show("Вы успешно авторизовались!");
            }
            else
            {
                MessageBox.Show("Логин или пароль не совпадают!");
            }
        }
    }
}
