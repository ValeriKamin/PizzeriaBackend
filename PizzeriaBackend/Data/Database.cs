using MySql.Data.MySqlClient;


    public class Database
    {
        private readonly string connectionString;

        public Database()
        {
            connectionString = "server=localhost;user=root;password=123456789;database=pizzahome;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
