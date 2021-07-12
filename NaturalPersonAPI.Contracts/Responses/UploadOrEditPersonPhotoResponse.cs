using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalPersonAPI.Contracts.Responses
{
    public class UploadOrEditPersonPhotoResponse : BaseResponse
    {
        public string PhotoPath { get; set; }
    }
}
