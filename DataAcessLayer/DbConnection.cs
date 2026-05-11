using Npgsql;


namespace DataAccessLayer
{
    public class DbConnection
    {
        private string connectionString =
            "Host=localhost;Port=5432;Database=notification;Username=postgres;Password=12345";

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}