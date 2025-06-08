using FluentValidation;
using TaskManager.Domain.InputModels;

namespace TaskManager.Application.Validators.Comment
{
    public class CreateCommentInputModelValidator : AbstractValidator<CreateCommentInputModel>
    {
        public CreateCommentInputModelValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("O conteúdo do comentário é obrigatório.")
                .MaximumLength(500).WithMessage("O comentário deve ter no máximo 500 caracteres.");

            RuleFor(x => x.TaskItemId)
                .NotEmpty().WithMessage("A tarefa é obrigatória.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("O autor do comentário é obrigatório.");
        }
    }
}