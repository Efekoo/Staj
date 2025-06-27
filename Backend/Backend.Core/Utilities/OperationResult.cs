using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Utilities
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";

        public static OperationResult SuccessResult(string message)
        {
            return new OperationResult { Success = true, Message = message };
        }

        public static OperationResult Failure(string message)
        {
            return new OperationResult { Success = false, Message = message };
        }
    }
}
