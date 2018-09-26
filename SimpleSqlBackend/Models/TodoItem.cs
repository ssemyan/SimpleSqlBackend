using System;

namespace SimpleSqlBackend
{
    public class TodoItem
    {
		public int id { get; set; }
		public string ItemName { get; set; }
		public DateTime? ItemCreateDate { get; set; }
	}
}
