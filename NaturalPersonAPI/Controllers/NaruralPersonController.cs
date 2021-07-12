using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NaturalPersonAPI.Contracts.Dtos;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Contracts.Responses;
using NaturalPersonAPI.Domain;
using NaturalPersonAPI.Domain.Enums;
using NaturalPersonAPI.Helper;
using NaturalPersonAPI.Repository;
using Newtonsoft.Json;
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
        private readonly IMapper _mapper;

        public NaruralPersonController(INaturalPersonService naturalPersonService, IFileProcessingService fileProcessingService, IMapper mapper)
        {
            _naturalPersonService = naturalPersonService;
            _fileProcessingService = fileProcessingService;
            _mapper = mapper;
        }


        [HttpPost("/createPerson")]
        public async Task<IActionResult> Create(CreateNaturalPersonRequest request)
        {
            if (!await _naturalPersonService.CityExistsAsync(request.CityId))
            {
                return BadRequest(new CreateNaturalPersonResponse
                {
                    Success = false,
                    Error = "City with this id does not exist"
                });
            }

            var person = new NaturalPerson
            {
                BirthDate = request.BirthDate,
                CityId = request.CityId,
                FirstName = request.FirstName,
                Gender = request.Gender,
                LastName = request.LastName,
                PersonalNumber = request.PersonalNumber,
                PhoneNumbers = request.PhoneNumbers.Select(x => new PhoneNumber { Phone = x.Phone, Type = x.PhoneNumberType.ToString() }).ToList()
            };

            var result = await _naturalPersonService.CreatePersonAsync(person);
            if (result == null)
            {
                return BadRequest(new CreateNaturalPersonResponse
                {
                    Success = false,
                    Error = "Incorrect fieds"
                });
            }

            //var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            //var locationUri = baseUrl + "api/naturalpeople/" + result;

            return Ok(new CreateNaturalPersonResponse
            {
                Success = true,
                CreatedPerson = result
            });
        }

        [HttpPatch("/updatePerson")]
        public async Task<IActionResult> UpdatePerson(long id, [FromBody] JsonPatchDocument<NaturalPerson> person)
        {
            var p = await _naturalPersonService.GetPersonByIdAsync(id, false);
            if (p == null)
            {
                return NotFound();
            }

            var a = _mapper.Map<NaturalPersonDto>(p);

            person.ApplyTo(p, ModelState);
            await _naturalPersonService.SaveChangesAsync();

            return Ok(p);
        }

        [HttpPost("/uploadOrEditPersonImage")]
        public async Task<IActionResult> UploadOrdEditPersonPhoto(long id, IFormFile photo)
        {
            var p = await _naturalPersonService.GetPersonByIdAsync(id, false);
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
            var result = await _naturalPersonService.DeleteRelatedPerson(request.ParentPersonId, request.RelatedPersonId);
            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpGet("/getPersonById")]
        public async Task<IActionResult> GetPersonById(long personId)
        {
            return Ok(await _naturalPersonService.GetPersonByIdAsync(personId, includeRelatedPeople: true));
        }

        [HttpGet("/searchPeople")]
        public IActionResult SearchPeople([FromQuery] SearchPeopleRequest request)
        {

            var people = _naturalPersonService.SearchPeople(request);

            return Ok(new SearchPeopleResponse
            {
                People = people,
                CurrentPage = people.CurrentPage,
                TotalPages = people.TotalPages,
                PageSize = people.PageSize,
                HasNext = people.HasNext,
                HasPrevious = people.HasPrevious,
                TotalCount = people.TotalCount
            });
        }

        [HttpGet("/getPersonReport")]
        public async Task<IActionResult> GetPersonReport(long personId)
        {
            return Ok(await _naturalPersonService.GetPersonReport(personId));
        }
    }
}
