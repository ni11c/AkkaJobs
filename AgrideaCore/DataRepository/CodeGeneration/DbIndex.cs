using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;

namespace Agridea.DataRepository
{

    public abstract class DbIndex
    {
        protected DbIndex()
        {
            ColumnNames = new List<string>();
        }
        public string TableName { get; set; }
        public IList<string> ColumnNames { get; set; }
        public IndexKeyType IndexKeyType { get; set; }

        public string Columns {get { return string.Join("-", ColumnNames.OrderBy(x => x)); }}

        


    }

    public class DbIndexComparer : IEqualityComparer<DbIndex>
    {
        public bool Equals(DbIndex x, DbIndex y)
        {
            //Check whether the compared objects reference the same data. 
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.TableName == y.TableName && x.Columns == y.Columns;
        }

        public int GetHashCode(DbIndex obj)
        {
            return obj.TableName.GetHashCode() ^ obj.Columns.GetHashCode();
        }
    }
    public class UniqueIndex : DbIndex
    {
        public UniqueIndex()
        {
            IndexKeyType = IndexKeyType.DriUniqueKey;
        }


        public override string ToString()
        {
            return string.Format("{0}_UC", TableName);
        }
    }

    public class NonUniqueIndex:DbIndex
    {
        public NonUniqueIndex()
        {
            IndexKeyType = IndexKeyType.None;
        }
        public override string ToString()
        {
            return string.Format("{0}_{1}_NU", TableName, Columns);
        }
    }
}
