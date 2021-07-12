using FluentValidation;
using Microsoft.Extensions.Localization;
using NaturalPersonAPI.Contracts.Requests;
using NaturalPersonAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NaturalPersonAPI.Validators
{
    public class UpdateNaturalPersonValidator : AbstractValidator<UpdateNaturalpersonRequest>
    {
        private readonly IStringLocalizer<NaruralPersonController> _localizer;

        public UpdateNaturalPersonValidator(IStringLocalizer<NaruralPersonController> localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.Id)
               .Must(x => x > 0)
               .WithMessage("Id must be more than 0");

            RuleFor(x => x.FirstName)
              .MinimumLength(2)
              .MaximumLength(50)
              .When(x => !string.IsNullOrEmpty(x.FirstName))
              .WithMessage(_localizer["FirstNameLengthMsg"]);

            RuleFor(x => x.FirstName)
               .Matches(@"^[ა-ჰ]+$").When(x => !string.IsNullOrEmpty(x.FirstName) && Regex.IsMatch(x.FirstName, @"[ა-ჰ]"))
               .WithMessage(_localizer["OnlyLatinOrGeorgian"])
               .Matches(@"^[a-zA-Z]+$").When(x => !string.IsNullOrEmpty(x.FirstName) && Regex.IsMatch(x.FirstName, @"[a-zA-Z]"))
               .WithMessage(_localizer["OnlyLatinOrGeorgian"]);

            RuleFor(x => x.LastName)
              .MinimumLength(2)
              .MaximumLength(50)
              .When(x => !string.IsNullOrEmpty(x.LastName))
              .WithMessage(_localizer["LastNameLengthMsg"]);


            RuleFor(x => x.LastName)
               .Matches(@"^[ა-ჰ]+$").When(x => !string.IsNullOrEmpty(x.LastName) && Regex.IsMatch(x.LastName, @"[ა-ჰ]"))
               .WithMessage(_localizer["OnlyLatinOrGeorgian"])
               .Matches(@"^[a-zA-Z]+$").When(x => !string.IsNullOrEmpty(x.LastName) && Regex.IsMatch(x.LastName, @"[a-zA-Z]"))
               .WithMessage(_localizer["OnlyLatinOrGeorgian"]);


            RuleFor(x => x.Gender)
                .Must(x => x.ToUpper() == "MALE" || x.ToUpper() == "FEMALE").When(x => x.Gender != null)
                .WithMessage(_localizer["MaleOrFemale"]);

            RuleFor(x => x.PersonalNumber)
               .Matches(@"\d{11}").When(x => !string.IsNullOrEmpty(x.PersonalNumber))
               .WithMessage(_localizer["PersonalNumber11Digits"]);


            RuleFor(x => x.BirthDate)
               .LessThan(new DateTime(DateTime.Now.Year - 18, DateTime.Now.Month, DateTime.Now.Day)).When(x => x.BirthDate != default(DateTime))
               .WithMessage(_localizer["AdultCheck"]);


            RuleForEach(x => x.PhoneNumbers)
                .Must(x => x.PhoneNumberType.ToUpper() == "HOME" || x.PhoneNumberType.ToUpper() == "OFFICE").When(x => x.PhoneNumbers != null)
                .WithMessage(_localizer["PhoneType"])
                .Must(x => x.Phone.Length >= 4 && x.Phone.Length <= 50).When(x => x.PhoneNumbers != null)
                .WithMessage(_localizer["PhoneNumberCheck"]);

        }
    }
}
