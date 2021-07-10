﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using NaturalPersonAPI.Helper;
using NaturalPersonAPI.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NaruralPersonController : ControllerBase
    {
        private readonly INaturalPersonService _naturalPersonService;
        private readonly IFileProcessingService _fileProcessingService;

        public NaruralPersonController(INaturalPersonService naturalPersonService, IFileProcessingService fileProcessingService)
        {
            _naturalPersonService = naturalPersonService;
            _fileProcessingService = fileProcessingService;
        }


        [HttpGet("/searchPeople")]
        public IActionResult GetAllPeople()
        {
            return Ok(_naturalPersonService.SearchPeople());
        }

        [HttpPatch("/updatePerson")]
        public async Task<IActionResult> UpdatePerson(long id, [FromBody] JsonPatchDocument<NaturalPerson> person)
        {
            var p = await _naturalPersonService.GetPersonByIdAsync(id);
            if (p == null)
            {
                return NotFound();
            }

            person.ApplyTo(p, ModelState);
            await _naturalPersonService.SaveChangesAsync();

            return Ok(p);
        }

        [HttpPost("/naturalPerson")]
        public async Task<IActionResult> Create(CreateNaturalPersonRequest request)
        {
            if (!await _naturalPersonService.CityExistsAsync(request.CityId))
            {
                return BadRequest("City do not exist");
            }

            var person = new NaturalPerson
            {
                BirthDate = request.BirthDate,
                CityId = request.CityId,
                FirstName = request.FirstName,
                Gender = request.Gender.ToString(),
                LastName = request.LastName,
                PersonalNumber = request.PersonalNumber,
                PhoneNumbers = request.PhoneNumbers.Select(x => new PhoneNumber { Phone = x.Phone, Type = x.PhoneNumberType.ToString() }).ToList()
            };

            var result = await _naturalPersonService.CreatePersonAsync(person);
            if (result == null)
            {
                return BadRequest();
            }

            //var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            //var locationUri = baseUrl + "api/naturalpeople/" + result;

            return Ok(result);
        }


        [HttpPost("/uploadOrEditPersonImage")]
        public async Task<IActionResult> UploadOrdEditPersonPhoto(long id, IFormFile photo)
        {
            var p = await _naturalPersonService.GetPersonByIdAsync(id);
            if (p == null)
            {
                return NotFound();
            }

            if (!photo.IsImage())
            {
                return BadRequest("upload valid photo.");
            }

            string relativePhotoPath = await _fileProcessingService.UploadFileAsync(photo);
            if (relativePhotoPath == string.Empty)
            {
                return BadRequest();
            }

            string photoPath = Request.Scheme + "://" + Request.Host + relativePhotoPath;

            await _naturalPersonService.SetPhotoToPersonAsync(id, photoPath);

            return Ok(photoPath);
        }


        [HttpPost("/addRelatedPerson")]
        public async Task<IActionResult> AddRelatedPerson(long parentPersonId, RelationType relationType, CreateNaturalPersonRequest person)
        {
            var p = new NaturalPerson
            {
                BirthDate = person.BirthDate,
                CityId = person.CityId,
                FirstName = person.FirstName,
                Gender = person.Gender.ToString(),
                LastName = person.LastName,
                PersonalNumber = person.PersonalNumber,
                PhoneNumbers = person.PhoneNumbers.Select(x => new PhoneNumber { Phone = x.Phone, Type = x.PhoneNumberType.ToString() }).ToList()
            };

            var result = await _naturalPersonService.AddRelatedPersonAsync(parentPersonId, relationType, p);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }


        [HttpDelete("/deleteRelatedPerson")]
        public async Task<IActionResult> DeleteRelatedPerson(DeleteRelatedPersonRequest request)
        {
            await _naturalPersonService.DeleteRelatedPerson(request.ParentPersonId, request.RelatedPersonId);

            return Ok();
        }
    }
}
