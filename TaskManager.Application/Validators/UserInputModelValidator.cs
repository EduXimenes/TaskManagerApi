using FluentValidation;
using TaskManager.Domain.InputModels;

namespace TaskManager.Application.Validators
{
    public class UserInputModelValidator : AbstractValidator<CreateUserInputModel>
    {
        public UserInputModelValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado é inválido.");
        }
    }
}
