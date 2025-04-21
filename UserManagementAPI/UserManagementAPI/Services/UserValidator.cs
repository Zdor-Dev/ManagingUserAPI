using FluentValidation;
using UserManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace UserManagementAPI.Services
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(List<User> users)
        {
            RuleFor(user => user.Id)
                .NotEmpty().WithMessage("User ID cannot be empty.")
                .Must(id => !users.Any(u => u.Id == id)).WithMessage("A user with this ID already exists!");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Incorrect Email format.")
                .Must(email => !users.Any(u => u.Email == email)).WithMessage("A user with this Email already exists!");
        }
    }
}
