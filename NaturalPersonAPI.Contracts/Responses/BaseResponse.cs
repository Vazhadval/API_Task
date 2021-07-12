using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
