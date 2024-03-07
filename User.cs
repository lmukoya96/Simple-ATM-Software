using MySql.Data.MySqlClient;

namespace Simple_ATM_Software
{
	class User
	{
		public string CreateUser(string FullName, string CardNumber, string Pin)
		{
			try
            {               
				DB_Operations dbConnection = DB_Operations.DB_Connection();
				using (MySqlConnection connection = new MySqlConnection(dbConnection.connectionString))
				{
					connection.Open();

					// First, set the current database to 'simple_atm_software'
					string useDatabaseQuery = "USE simple_atm_software;";
					using (MySqlCommand useDbCmd = new MySqlCommand(useDatabaseQuery, connection))
					{
						useDbCmd.ExecuteNonQuery();
					}

					// Define the SQL query to insert product data into the 'products' table
					string insertQuery = "INSERT INTO Users(FullName, CardNumber, Pin) VALUES(@FullName, @CardNumber, @Pin);";

					using (MySqlCommand cmd = new MySqlCommand(insertQuery, connection))
					{
						cmd.Parameters.AddWithValue("@FullName", FullName);
						cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
						cmd.Parameters.AddWithValue("@Pin", Pin);

						int rowsAffected = cmd.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							Account user = new Account();
							string AccountCreated = user.CreateAccount(CardNumber);

							if(AccountCreated.Contains("successfully"))
							{
								return "User created in the database successfully.";
							}
							else
							{
								return AccountCreated;
							}		
						}
						else
						{
							return "User creation failed.";
						}
					}
				}
                
            }
            catch (MySqlException ex)
            {
                return "Error adding user to the database: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Unhandled error: " + ex.Message;
            }
		}

		public string Login(string CardNumber, string Pin)
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

					// SQL query to select user by card number
					string selectUserQuery = "SELECT * FROM Users WHERE CardNumber = @CardNumber;";
					using (MySqlCommand selectUserCmd = new MySqlCommand(selectUserQuery, connection))
					{
						selectUserCmd.Parameters.AddWithValue("@CardNumber", CardNumber);

						// Execute the query
						using (MySqlDataReader usersReader = selectUserCmd.ExecuteReader())
						{
							// Check if any rows are affected
							if (usersReader.HasRows)
							{
								// Close the DataReader after you're done reading data
    							usersReader.Close();

								// Rows are affected, continue with user validation.
								string selectUserNameQuery = "SELECT FullName FROM Users WHERE CardNumber = @CardNumber AND Pin = @Pin;";
								using (MySqlCommand selectUserNameCmd = new MySqlCommand(selectUserNameQuery, connection))
								{
									selectUserNameCmd.Parameters.AddWithValue("@CardNumber", CardNumber);
									selectUserNameCmd.Parameters.AddWithValue("@Pin", Pin);

									using (MySqlDataReader usersNameReader = selectUserNameCmd.ExecuteReader())
									{
										if (usersNameReader.HasRows)
										{
											if (usersNameReader.Read())
											{
												// Retrieve the FullName from the query result and store it in AcquiredFullName
												object fullNameObj = usersNameReader["FullName"];
												
												#nullable disable
												string AcquiredFullName = fullNameObj != DBNull.Value ? fullNameObj.ToString() : string.Empty;
												#nullable restore
												
												return "Welcome " + AcquiredFullName;
											}

											// Close the DataReader after you're done reading data
											usersNameReader.Close();
										}
										else
										{
											// Close the DataReader after you're done reading data
    										usersNameReader.Close();
											
											// No rows affected, pin is invalid.
											return "Invalid pin.";
										}
									}
								}
							}
							else
							{
								usersReader.Close();

								// No rows affected, card number is not available
								return "Card number is not available.";
							}
						}
					}
				}
			}
			catch (MySqlException ex)
			{
				return "Error fetching data from the database: " + ex.Message;
			}
			catch (Exception ex)
			{
				return "Unhandled error: " + ex.Message;
			}

			return "Unhandled condition in Login method";
		}

		public string ChangePIN(string CardNumber, string OldPIN, string NewPIN1, string NewPIN2)
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
					string selectQuery = "SELECT Pin FROM Users WHERE CardNumber = @CardNumber;";

					using (MySqlCommand cmd = new MySqlCommand(selectQuery, connection))
					{
						cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
						
						using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								string Pin = reader.GetString(0);

								if(OldPIN == Pin && NewPIN1 == NewPIN2 && NewPIN1 != OldPIN && NewPIN2 != OldPIN)
								{
									reader.Close();

									string updateQuery = "UPDATE Users SET Pin = @NewPin WHERE CardNumber = @CardNumber;";

									using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection))
									{
										updateCmd.Parameters.AddWithValue("@NewPin", NewPIN2);
										updateCmd.Parameters.AddWithValue("@CardNumber", CardNumber);
										
										int rowsAffected = updateCmd.ExecuteNonQuery();

										if (rowsAffected > 0)
										{
											return "PIN changed successfully.";
										}
										else
										{
											return "Failed to change PIN";
										}
									}
									
								}
								else if(OldPIN != Pin)
								{
									return "Invalid old password.";
								}
								else if(NewPIN1 != NewPIN2)
								{ 
									return "New  PINs do not match.";
								}
								else if(NewPIN1 == OldPIN || NewPIN2 == OldPIN)
								{
									return "New PIN cannot be the same as the old PIN.";
								}								
							}
							else
							{
								return "No data in the database.";
							}
						}
					}
				}
				
            }
			catch (MySqlException ex)
			{
				return "Error fetching data from the database: " + ex.Message;
			}
			catch (Exception ex)
			{
				return "Unhandled error: " + ex.Message;
			}

			return "Unhandled error in PIN change process.";
        }
    }
}