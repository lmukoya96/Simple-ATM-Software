using System;
using MySql.Data.MySqlClient;
using System.Diagnostics;


namespace Simple_ATM_Software
{
    public class DB_Operations
    {
        private string server;
        private string username;
        private string password;
        public string connectionString;
        
        public DB_Operations(string server, string username, string password)
        {
            this.server = server;
            this.username = username;
            this.password = password;
            // Initialize the connection string with server, username and password
            connectionString = $"Server={server};User={username};Password={password};";
        }

        public static DB_Operations DB_Connection()
        {
            const string server = "Your_Server_Address";
            const string username = "Your_Username";
            const string password = "Your_Password";
            
            return new DB_Operations(server, username, password);
        }
    }
}