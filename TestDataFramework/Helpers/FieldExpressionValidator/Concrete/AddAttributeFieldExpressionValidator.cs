using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers.FieldExpressionValidator.Concrete
{
    public class AddAttributeFieldExpressionValidator : FieldExpressionValidatorBase
    {
        protected override string ErrorMessage => Messages.AddAttributeExpressionMustBePropertyAccess;
    }
}
