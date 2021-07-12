using NaturalPersonAPI.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class UpdatePersonResponse : BaseResponse
    {
        public NaturalPersonDto UpatedPerson { get; set; }

    }
}
