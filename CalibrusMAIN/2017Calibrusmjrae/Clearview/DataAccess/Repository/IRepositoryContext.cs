using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.Repository
{
    public interface IRepositoryContext
    {
        string GetConnectionString();
    }
}
