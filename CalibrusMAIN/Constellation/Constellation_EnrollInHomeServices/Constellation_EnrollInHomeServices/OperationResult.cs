using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Constellation_EnrollInHomeServices
{
    public class OperationResult
    {
        public List<ErrorMessage> ErrorBusinessMessageList { get; set; }
        public List<ErrorMessage> ErrorMessageList { get; set; }
        public bool HasBusinessError { get; set; }
        public bool HasError { get; set; }
        public bool IsCredentialValid { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ErrorMessage
    {
        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }
        public string ErrorType { get; set; }
        public bool IsBusinessError { get; set; }
        public bool IsSystemError { get; set; }
    }


}
