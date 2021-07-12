using Domain.Domain.Enums;
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
    public class CreateNaturalPersonValidator : AbstractValidator<CreateNaturalPersonRequest>
    {
        private readonly IStringLocalizer<NaruralPersonController> _localizer;

        public CreateNaturalPersonValidator(IStringLocalizer<NaruralPersonController> localizer)
        {
            _localizer = localizer;


            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(50)
                .WithMessage(_localizer["LastNameLengthMsg"]);




            RuleFor(x => x.FirstName)
                .Matches(@"^[ა-ჰ]+$").When(x => Regex.IsMatch(x.FirstName, @"[ა-ჰ]"))
                .WithMessage(_localizer["OnlyLatinOrGeorgian"])
                .Matches(@"^[a-zA-Z]+$").When(x => Regex.IsMatch(x.FirstName, @"[a-zA-Z]"))
                .WithMessage(_localizer["OnlyLatinOrGeorgian"]);

            RuleFor(x => x.LastName)
               .NotEmpty()
               .MinimumLength(2)
               .MaximumLength(50)
               .WithMessage(_localizer["FirstNameLengthMsg"]);





            RuleFor(x => x.LastName)
                .Matches(@"^[ა-ჰ]+$").When(x => Regex.IsMatch(x.LastName, @"[ა-ჰ]"))
                .WithMessage(_localizer["OnlyLatinOrGeorgian"])
                .Matches(@"^[a-zA-Z]+$").When(x => Regex.IsMatch(x.LastName, @"[a-zA-Z]"))
                .WithMessage(_localizer["OnlyLatinOrGeorgian"]);


            RuleFor(x => x.Gender)
                .Must(x => x.ToUpper() == "MALE" || x.ToUpper() == "FEMALE")
                .WithMessage(_localizer["MaleOrFemale"]);


            RuleFor(x => x.PersonalNumber)
                .Matches(@"\d{11}")
                .WithMessage(_localizer["PersonalNumber11Digits"]);

            RuleFor(x => x.BirthDate)
                .LessThan(new DateTime(DateTime.Now.Year - 18, DateTime.Now.Month, DateTime.Now.Day))
                .WithMessage(_localizer["AdultCheck"]);


            RuleForEach(x => x.PhoneNumbers)
                .Must(x => x.PhoneNumberType.ToUpper() == "HOME" || x.PhoneNumberType.ToUpper() == "OFFICE")
                .WithMessage(_localizer["PhoneType"])
                .Must(x => x.Phone.Length >= 4 && x.Phone.Length <= 50)
                .WithMessage(_localizer["PhoneNumberCheck"]);

            RuleFor(x => x.PhoneNumbers)
                .NotNull()
                .WithMessage(_localizer["AtLeastOnePhone"]);
        }
    }
}
