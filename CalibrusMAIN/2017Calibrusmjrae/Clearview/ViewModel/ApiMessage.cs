using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.ViewModel
{
    public class ApiMessage<T>
    {
        public T Data { get; set; }
        public bool HasErrors { get; set; }
        public List<string> ErrorList { get; set; }
    }
}
