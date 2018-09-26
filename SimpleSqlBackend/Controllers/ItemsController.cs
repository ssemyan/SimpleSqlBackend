using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace SimpleSqlBackend.Controllers
{
    public class ItemsController : ApiController
    {
		const string connStr = "Server=localhost;Initial Catalog=TodoItems;Integrated Security=SSPI;Persist Security Info=True;";

        // GET: api/Items
        public IEnumerable<TodoItem> Get()
        {
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
			//return new TodoItem[] { new TodoItem { ItemName = "foo", ItemCreateDate = DateTime.Now }, new TodoItem { ItemName = "bar", ItemCreateDate = DateTime.Now } };
        }

        // POST: api/Items
        public TodoItem Post(TodoItem newItem)
        {
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
