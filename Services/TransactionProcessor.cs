﻿using Microsoft.Extensions.Configuration;
using BudgetTracker.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Transactions;

namespace BudgetTracker.Services
{
    public class TransactionProcessor
    {
        // Transaction processor is a class dedicated to communicate with the database.
        private readonly string? _connectionString;
        private SqlConnection _connection;
        
        public TransactionProcessor()
        {
            _connectionString = GetConnectionString();
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException("Connection string is null or empty");
            }
            _connection = new SqlConnection(_connectionString);
        }
        private async Task OpenConnection()
        {
            if(_connection.State == System.Data.ConnectionState.Closed)
            {
                await _connection.OpenAsync();
            }
        }
        private void CloseConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }
        public async Task sendTransaction(Models.Transaction transaction)
        {
            // This method will send transactions  to mssql database
            string query = $"INSERT INTO Transactions (Amount, UserId, TransactionDate, CategoryId, BankId) " +
                           $"VALUES ('{transaction.Amount}', '{transaction.UserId}', '{transaction.Date}', '{transaction.CategoryId}', '{transaction.BankId}')";
            try
            {
                await OpenConnection();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    await command.ExecuteNonQueryAsync();
                    Console.Clear();
                    Console.WriteLine("Added transaction!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while adding transaction to the database : " + ex);
            }
            finally
            {
                CloseConnection();
            }
        }
        public async Task<int> GetCategoryIdOrCreate(string name)
        {
            int returnId = 0;
            try
            {
                await OpenConnection();
                await using(SqlCommand command = new SqlCommand("SELECT * FROM Categories WHERE CategoryName = @Name", _connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            returnId = reader.GetInt32(0);
                        }
                        else
                        {
                            // Create category
                            CloseConnection();
                            await OpenConnection();
                            string insertQuery = "INSERT INTO Categories (CategoryName) VALUES (@Name); SELECT SCOPE_IDENTITY();";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, _connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Name", name);
                                returnId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync()); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while retrieving Categories : " + ex);
            }
            finally
            {
                CloseConnection();
            } 
            return returnId;
        }
        public async Task<List<Category>> GetCategories()
        {
            List<Category> retCategories = new List<Category>();
            string query = "SELECT * FROM Categories";
            try
            {
                await OpenConnection();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            retCategories.Add(new Category(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while retrieving Categories : " + ex);
            }
            finally
            {
                CloseConnection();
            }
            return retCategories;
        }
        public async Task<List<Models.Transaction>> GetTransactions()
        {
            // This method will get transactions from mssql database
            List<Models.Transaction> retTransactions = new List<Models.Transaction>();
            string query = "SELECT * FROM Transactions";
            try
            {
                await OpenConnection();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // This line is probably wrong
                            retTransactions.Add(new Models.Transaction(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDateTime(3), reader.GetInt32(4),reader.GetInt32(5)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while retrieving Transactions : " + ex);
            }
            finally
            {
                CloseConnection();
            }
            return retTransactions;
        }
        public async Task<List<Bank>> GetUserBank()
        {
            // This function will eventually take a user object as a parameter and return that user's banks
            List<Bank> retBanks = new List<Bank>();
            string query = "SELECT * FROM Banks";
            try
            {
                await OpenConnection();
                using (SqlCommand command = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            retBanks.Add(new Bank(reader.GetInt32(0), reader.GetString(2), reader.GetString(1)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while retrieving Transactions : " + ex);
            }
            finally
            {
                CloseConnection();
            }
            return retBanks;
        }
        private static string? GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build(); 
             
            return configuration.GetConnectionString("DefaultConnection");
        }
    }
}
