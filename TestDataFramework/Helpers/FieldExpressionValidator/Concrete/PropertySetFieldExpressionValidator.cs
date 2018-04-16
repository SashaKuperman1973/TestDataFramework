using TestDataFramework.Exceptions;

namespace TestDataFramework.Helpers.FieldExpressionValidator.Concrete
{
    public class PropertySetFieldExpressionValidator : FieldExpressionValidatorBase
    {
        protected override string ErrorMessage => Messages.PropertySetExpressionMustBePropertyAccess;
    }
}
