using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSQL.DataAccess;

namespace TestSQL.Models
{
    public class ToDoItem : BusinessEntityBase
    {
		public string Name { get; set; }
		public DateTime Created { get; set; }
		public bool Done { get; set; }

		public ToDoItem ()
		{
			Name = string.Empty;
			Created = DateTime.UtcNow;
			Done = false;
		}

		[SQLite.Ignore]
		public string CreatedDisplay
		{
			get { return Created.ToLocalTime().ToString("f"); }
		}

    }
}
