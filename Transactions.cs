using MySql.Data.MySqlClient;

namespace Simple_ATM_Software
{
    class Transactions
    {
        public string RecordTransaction(string CardNumber, string TransType, double Amount)
        {
            try
            {
                DB_Operations dbConnection = DB_Operations.DB_Connection();
                using (MySqlConnection connection = new MySqlConnection(dbConnection.connectionString))
                {
                    connection.Open();

                    // Set the current database to 'simple_atm_software'
                    string useDatabaseQuery = "USE simple_atm_software;";
                    using (MySqlCommand useDbCmd = new MySqlCommand(useDatabaseQuery, connection))
                    {
                        useDbCmd.ExecuteNonQuery();
                    }

                    // Define the SQL query to record a transaction.
                    string updateQuery = "INSERT INTO Transactions(CardNumber, TransType, Amount, TransTime) VALUES(@CardNumber, @TransType, @Amount, @TransTime);";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@TransTime", DateTime.Now);
                        

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return "Transaction recorded successfully.";
                        }
                        else
                        {
                            return "Transaction recording failed.";
                        }
                    }
                }
            }
            catch (MySqlException ex)
			{
				return "Error recording transaction in the database: " + ex.Message;
			}
			catch (Exception ex)
			{
				return "Unhandled error: " + ex.Message;
			}
        }
    }
}