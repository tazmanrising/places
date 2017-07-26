using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public class RepositoryContext : IRepositoryContext
    {
        private readonly string _connectionString;

        public RepositoryContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
