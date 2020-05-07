using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using WebApplication3.Models;
using System.Diagnostics;

namespace WebApplication3.Baza
{
    public class ConnectionDB
    {
        public static string ConnectionString = "SERVER=localhost" + ";" + "DATABASE=new_schema" + ";" + "UID=root" + ";" + "PASSWORD=password";
        // using default user and password instead
        public ConnectionDB(string connectionString)
        {
            ConnectionDB.ConnectionString = connectionString;
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        public List<Book> GetAllBooks()
        {
            List<Book> list = new List<Book>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from books where BooksID < 1000", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Book()
                        {
                            BooksID = Convert.ToInt32(reader["BooksID"]),
                            Title = reader["Title"].ToString(),
                            Genre = reader["Genre"].ToString(),
                            Author = reader["Author"].ToString(),
                            Edition = Convert.ToInt32(reader["Edition"]),
                            Available = Convert.ToInt32(reader["Available"]),
                        });
                    }
                }
            }
            return list;
        }
        public List<Administration> GetAllAdmins()
        {
            List<Administration> list = new List<Administration>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from administrator where AdminID < 1000", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Administration()
                        {
                            AdminID = Convert.ToInt32(reader["AdminID"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNum = Convert.ToInt32(reader["PhoneNum"]),
                        });
                    }
                }
            }
            return list;
        }
        public List<Reader> GetAllReaders()
        {
            List<Reader> list = new List<Reader>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from reader where ReaderID < 1000", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Reader()
                        {
                            ReaderID = Convert.ToInt32(reader["ReaderID"]),
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            PhoneNum = Convert.ToInt32(reader["PhoneNum"]),
                        });
                    }
                }
            }
            return list;
        }
        public List<RentalList> GetAllRentals()
        {
            List<RentalList> list = new List<RentalList>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from rental_list where RentalID < 1000", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RentalList()
                        {
                            RentalID = Convert.ToInt32(reader["RentalID"]),
                            Date = reader["Date"].ToString(),
                            ReturnDate = reader["ReturnDate"].ToString(),
                            ReaderID = Convert.ToInt32(reader["ReaderID"]),
                            BookID = Convert.ToInt32(reader["BookID"])
                        });
                    }
                }
            }
            return list;
        }

        // will be used only inside this class
        public static string calculateMD5sum(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

        public int checkUserID(userModel model)
        {   
                // TODO:
                // → store data about users that are logged in (cookies/global variable?)
                // → navigation bar options ("log in" if not logged in, "welcome,User" and log out if not)
                // → ability to remove (deactivate) user account
                // → log in as admin (separate function or extend this one)

            int userID = -1;   
            string username = model.username;
            string password = model.password;
            string passwordMD5 = calculateMD5sum(password);
            string passwordHash = "";      

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sqlCommand1 = "select LoginID from hashlogininfo where login =" + "\"" + username + "\"";
                string sqlCommand2 = "select password from hashlogininfo where login =" + "\"" + username + "\"";
                MySqlCommand cmd1 = new MySqlCommand(sqlCommand1, conn);
                using (var reader1 = cmd1.ExecuteReader())
                {
                    while (reader1.Read()) userID = Convert.ToInt32(reader1["LoginID"]);
                }
                if(userID == -1) 
                {
                    // User not found
                    // something that makes sense
                    return -1;
                }
                else
                {
                    MySqlCommand cmd2 = new MySqlCommand(sqlCommand2, conn);
                    using (var reader2 = cmd2.ExecuteReader())
                    {
                        while (reader2.Read()) passwordHash = reader2["password"].ToString();
                    }
                    if(passwordHash != passwordMD5)
                    {
                        // Wrong password
                        return -2;
                    }
                    else return userID;
                }
            }
            // control will never reach this point
        }

        public int createUser(signupForm form)
        {
                // TODO: 
                // → Repeat password field and check if they are the same 
                // → Captcha?? (api)
                // → Catch MySQL exceptions (ex. empty field)
                // → What about admins?? createAdmin()...
                // → Redirect user somewhere based on success/error while creating account

            int rowsCountReader = 0, rowsCountHash = 0, newUserID = 0;
            string name = form.name;
            string email = form.email;
            string phone = form.phone;
            string username = form.username;
            string password = form.password;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                String checkUsername = "select LoginID from hashlogininfo where login =" + "\"" + username + "\"";
                MySqlCommand checkUsernameCommand = new MySqlCommand(checkUsername, conn);
                using (var reader1 = checkUsernameCommand.ExecuteReader())
                {
                    while (reader1.Read()) newUserID = Convert.ToInt32(reader1["LoginID"]);
                }
                if(newUserID != 0)
                {
                    // Error - user with this name already exist
                    return -1;
                } 

                String cmd1 = "SELECT COUNT(*) FROM reader";
                String cmd2 = "SELECT COUNT(*) FROM hashlogininfo";
                MySqlCommand cmd1check = new MySqlCommand(cmd1, conn);
                using (var reader1 = cmd1check.ExecuteReader())
                {
                    while (reader1.Read()) rowsCountReader = Convert.ToInt32(reader1["COUNT(*)"]);
                }
                MySqlCommand cmd2check = new MySqlCommand(cmd2, conn);
                using (var reader2 = cmd2check.ExecuteReader())
                {
                    while (reader2.Read()) rowsCountHash = Convert.ToInt32(reader2["COUNT(*)"]);
                }
                if(rowsCountHash != rowsCountReader)
                {
                    // Database User ID conflict between Readers and hashLogin
                    return -2;
                }
                else newUserID = rowsCountReader+1;

                // construct mysql commands
                String cmd3str = "INSERT INTO `new_schema`.`reader` (`ReaderID`, `Name`, `Email`, `PhoneNum`) VALUES (";
                cmd3str += "\"" + newUserID.ToString() + "\",\"" + name + "\",\"" + email + "\",\"" + phone + "\")";
                String cmd4str = "INSERT INTO `new_schema`.`hashlogininfo` (`LoginID`, `password`, `login`) VALUES (";
                cmd4str += "\"" + newUserID.ToString() + "\",\"" + calculateMD5sum(password) + "\",\"" + username + "\")";
                MySqlCommand cmd3 = new MySqlCommand(cmd3str, conn);
                MySqlCommand cmd4 = new MySqlCommand(cmd4str, conn);

                cmd3.ExecuteNonQuery();
                cmd4.ExecuteNonQuery();
            }
            return newUserID;   // id of newly created user
        }
        public void EditBook(int BooksID, string Title, string Author, int Edition, string Genre, int Available)
        {

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand("UPDATE new_schema.books SET Title = @Title, Author = @Author, Edition = @Edition, Genre = @Genre, Available = @Available WHERE BooksID = @BooksID", conn))
                {
                    cmd.Parameters.AddWithValue("@BooksID", BooksID);
                    cmd.Parameters.AddWithValue("@Title", Title);
                    cmd.Parameters.AddWithValue("@Author", Author);
                    cmd.Parameters.AddWithValue("@Edition", Edition);
                    cmd.Parameters.AddWithValue("@Genre", Genre);
                    cmd.Parameters.AddWithValue("@Available", Available);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public void DeleteBook(int BooksID)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM new_schema.books WHERE BooksID = @BooksID", conn))
                {
                    cmd.Parameters.AddWithValue("@BooksID", BooksID);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
        public void CreateBook(int BooksID, string Title, string Author, int Edition, string Genre, int Available)
        {

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO new_schema.books (BooksID, Title, Author, Edition, Genre, Available)  VALUES (@BooksID, @Title, @Author, @Edition, @Genre, @Available)", conn))
                {
                    cmd.Parameters.AddWithValue("@BooksID", BooksID);
                    cmd.Parameters.AddWithValue("@Title", Title);
                    cmd.Parameters.AddWithValue("@Author", Author);
                    cmd.Parameters.AddWithValue("@Edition", Edition);
                    cmd.Parameters.AddWithValue("@Genre", Genre);
                    cmd.Parameters.AddWithValue("@Available", Available);

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}