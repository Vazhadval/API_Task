using NaturalPersonAPI.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class GetPersonReportResponse
    {
        public Dictionary<string, int> Relations { get; set; }
    }
}
