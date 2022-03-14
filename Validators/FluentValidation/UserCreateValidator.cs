using System;
using FluentValidation;
using forfarmingcase.Entities;
using forfarmingcase.ViewModels;

namespace forfarmingcase.Validators.FluentValidation
{
  public class UserCreateValidator: AbstractValidator<UserCreateVM>
  {
    public UserCreateValidator()
    {
      RuleFor(m => m.FirstName).NotEmpty().WithMessage("FirstName can not be empty");
      RuleFor(m => m.LastName).NotEmpty().WithMessage("LastName can not be empty");
      RuleFor(m => m.BirthDate).NotEmpty().WithMessage("BirthDate can not be empty");
      RuleFor(m => m.Email).NotEmpty().WithMessage("Email can not be empty").EmailAddress().WithMessage("A valid email address is required.");
    }
  }
}
