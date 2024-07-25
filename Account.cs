using System;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Security;

namespace Simple_ATM_Software
{
    class Account
    {
        public string CreateAccount(string CardNumber)
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

                    // Define the SQL query to add new card number. 
                    string updateQuery = "INSERT INTO Accounts(CardNumber) VALUES(@CardNumber);";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return "Account created successfully.";
                        }
                        else
                        {
                            return "Account creation failed.";
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return "Error updating account in the database: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public string Deposit(string CardNumber, double Amount)
        {
            try
            {
                if(Amount > 0)
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

						// Define the SQL query to delete a Purchase Order
						string updateQuery = "UPDATE Accounts SET Transactions = Transactions + 1, Amount = Amount + @Amount WHERE CardNumber = @CardNumber;";

						using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
						{
							cmd.Parameters.AddWithValue("@Amount", Amount);
							cmd.Parameters.AddWithValue("@CardNumber", CardNumber);

							int rowsAffected = cmd.ExecuteNonQuery();

							if (rowsAffected > 0)
							{
								Transactions transaction = new Transactions();
                                transaction.RecordTransaction(CardNumber, "Deposit", Amount);

                                Account account = new Account();
                                string newBalance = account.CheckBalance(CardNumber);

                                return "Deposit successful. Your balance is " + newBalance;
							}
							else
							{
								return "Deposit failed.";
							}
						}
					}
                }
                else
                {
                    return "Invalid amount.";
                }
            }
            catch (MySqlException ex)
            {
                return "Error updating account in the database: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public string Withdraw(string CardNumber, double Amount)
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

					// Define the SQL query to delete a Purchase Order
					string selectQuery = "SELECT Withdrawals, Amount FROM Accounts WHERE CardNumber = @CardNumber;";

					using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
					{
						cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
						
						using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								int Withdrawals = reader.GetInt32(0);
                                double Balance = reader.GetDouble(1);

                                if(Amount > 0 && Amount <= 10000 && Withdrawals < 10 && Amount <= Balance)
                                {
                                    reader.Close();

                                    string updateQuery = "UPDATE Accounts SET Transactions = Transactions + 1, Withdrawals = Withdrawals + 1, Amount = Amount - @Amount;";
                                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
                                    {
                                        updateCmd.Parameters.AddWithValue("@Amount", Amount);
                                        updateCmd.Parameters.AddWithValue("@CardNumber", CardNumber);

                                        int rowsAffected = updateCmd.ExecuteNonQuery();

                                        if (rowsAffected > 0)
                                        {
                                            Transactions transaction = new Transactions();
                                            transaction.RecordTransaction(CardNumber, "Withdrawal", Amount);

                                            Account account = new Account();
			                                string newBalance = account.CheckBalance(CardNumber);

                                            return "Withdrawal successful. Your balance is " + newBalance;
                                        }
                                        else
                                        {
                                            return "Withdrawal failed.";
                                        }
                                    }								
                                }
                                else if(Amount > 10000)
                                {
                                    reader.Close();

                                    return "You cannot make a withdrawal of more than 10,000 in one transaction.";
                                }
                                else if(Withdrawals >= 10)
                                {
                                    reader.Close();

                                    return "You have reached the withdrawal limit of 10";
                                }
                                else if(Amount > Balance)
                                {
                                    reader.Close();

                                    return "You cannot withdraw more than you have in your account. Your Balance is " + Balance.ToString("N2");
                                }
                                else
                                {
                                    reader.Close();

                                    return "Invalid input.";
                                }
							}
                            else
                            {
                                return "No data in the database";
                            }
						}
					}
				}
            }
            catch (MySqlException ex)
            {
                return "Error updating account in the database: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public string CheckBalance(string CardNumber)
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

                    // Define the SQL query to delete a Purchase Order
                    string updateQuery = "SELECT Amount FROM Accounts WHERE CardNumber = @CardNumber;";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								double Balance = reader.GetDouble(0);

                                return Balance.ToString("N2");
                            }
                            else
                            {
                                return "No data in the database";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return "Error fetching balance from the database: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public List<(DateTime, string, double)> GetTransactions(string CardNumber)
		{
			List<(DateTime, string, double)> transactionsList = new List<(DateTime, string, double)>();

            try
            {
                DB_Operations dbConnection = DB_Operations.DB_Connection();
				using(MySqlConnection connection = new MySqlConnection(dbConnection.connectionString))
				{
					connection.Open();

					// Set the current database to 'simple_atm_software'
					string useDatabaseQuery = "USE simple_atm_software;";
					using (MySqlCommand useDbCmd = new MySqlCommand(useDatabaseQuery, connection))
					{
						useDbCmd.ExecuteNonQuery();

						string selectQuery = "SELECT TransTime, TransType, Amount FROM Transactions WHERE CardNumber = @CardNumber ORDER BY TransTime DESC LIMIT 5;";

						using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
						{
							cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
							
							using (MySqlDataReader reader = cmd.ExecuteReader())
							{
								while (reader.Read())
								{
									DateTime TransTime = reader.GetDateTime("TransTime");
									string TransType = reader.GetString("TransType");
									double Amount = reader.GetDouble("Amount");

									transactionsList.Add((TransTime, TransType, Amount));
								}
							}
						}
					}
				}
            }
            catch (MySqlException ex)
            {
                string errorMessage = "Error fetching data from the database: " + ex.Message;
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error: " + ex.Message;
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }


            return transactionsList;
        }
	}
}