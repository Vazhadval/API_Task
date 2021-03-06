using NaturalPersonAPI.Contracts.Dtos;
using NaturalPersonAPI.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class CreateNaturalPersonResponse : BaseResponse
    {
        public NaturalPersonDto CreatedPerson { get; set; }
    }
}
