using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System.Configuration;

namespace SimpleSqlBackend.Controllers
{
    public class ItemsController : ApiController
    {
		private string connStr = "";

		private void GetSecret()
		{
			// Check if we are in Azure running under a managed service identity (if so the MSI_ENDPOINT environmental variable will be present)
			if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("MSI_ENDPOINT")))
			{
				// Get the connection string from the web.config (or app settings)
				connStr = ConfigurationManager.AppSettings["DbConnectionString"];
			}
			else
			{
				// Running in Azure - use the token provider to get a token to access keyvault with
				var azureServiceTokenProvider = new AzureServiceTokenProvider();
				var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

				// Get the URL of the secret from web.config
				string secretUri = ConfigurationManager.AppSettings["DbConnStrKeyVaultSecretUri"];

				// Now get the connection string from keyvault
				var secret = keyVaultClient.GetSecretAsync(secretUri).Result;
				connStr = secret.Value;
				// TODO: handle errors if the web app cannot connect to the keyvault or get the secret. 
				// Also, this should be cached and refeshed if there are connection errors (e.g. the key is cycled) 
			}
		}

		// GET: api/Items
		public IEnumerable<TodoItem> Get()
        {
			GetSecret();
			List<TodoItem> items = new List<TodoItem>();
			using (var connection = new SqlConnection(connStr))
			{
				var command = new SqlCommand("select Id, ItemName, ItemCreateDate from TodoItems", connection);
				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						TodoItem test = new TodoItem
						{
							id = (int)reader["Id"],
							ItemName = (string)reader["ItemName"],
							ItemCreateDate = (DateTime)reader["ItemCreateDate"]
						};
						items.Add(test);
					}
				}
			}
			return items;
        }

        // POST: api/Items
        public TodoItem Post(TodoItem newItem)
        {
			GetSecret();
			newItem.ItemCreateDate = DateTime.Now;
			using (var connection = new SqlConnection(connStr))
			{
				string query = "INSERT INTO TodoItems (ItemName, ItemCreateDate) VALUES (@ItemName, @ItemCreateDate);SELECT SCOPE_IDENTITY();";
				SqlCommand myCommand = new SqlCommand(query, connection);
				myCommand.Parameters.AddWithValue("@ItemName", newItem.ItemName);
				myCommand.Parameters.AddWithValue("@ItemCreateDate", newItem.ItemCreateDate);
				connection.Open();
				int newId = Convert.ToInt32(myCommand.ExecuteScalar());
				newItem.id = newId;
				return newItem;
			}
		}

       
        // DELETE: api/Items/5
        public void Delete(int id)
        {
			GetSecret();
			using (var connection = new SqlConnection(connStr))
			{
				string query = "delete from TodoItems where id = @ItemId";
				SqlCommand myCommand = new SqlCommand(query, connection);
				myCommand.Parameters.AddWithValue("@ItemId", id);
				connection.Open();
				myCommand.ExecuteNonQuery();
			}
		}
	}
}
