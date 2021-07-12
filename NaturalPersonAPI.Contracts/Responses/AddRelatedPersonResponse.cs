using NaturalPersonAPI.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class AddRelatedPersonResponse : BaseResponse
    {
        public NaturalPersonDto CreatedPerson { get; set; }
    }
}
