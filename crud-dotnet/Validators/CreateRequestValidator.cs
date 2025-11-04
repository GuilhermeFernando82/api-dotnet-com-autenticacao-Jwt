using FluentValidation;

namespace crud_dotnet.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("O pedido deve conter pelo menos um item.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.Name)
                    .NotEmpty().WithMessage("O nome do produto é obrigatório.");

                items.RuleFor(i => i.Category)
                    .NotEmpty().WithMessage("A categoria é obrigatória.");

                items.RuleFor(i => i.Price)
                    .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
            });
        }
    }
}
