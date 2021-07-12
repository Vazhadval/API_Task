using NaturalPersonAPI.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class GetPersonByIdResponse : BaseResponse
    {
        public NaturalPerson Person { get; set; }
    }
}
