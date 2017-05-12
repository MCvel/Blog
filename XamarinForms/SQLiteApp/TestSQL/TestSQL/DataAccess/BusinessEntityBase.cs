using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TestSQL.DataAccess
{
    public class BusinessEntityBase : IBusinessEntity
    {
        public BusinessEntityBase()
        {

        }

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get; set;
        }
    }
}
