using System.Collections.Generic;

namespace TestDataFramework.AttributeDecorator
{
    public class TypeDictionaryEqualityComparer : IEqualityComparer<Table>
    {
        public delegate bool EqualsCriteriaDelegate(Table fromSet, Table input);
        private EqualsCriteriaDelegate equalsCriteria;

        public void SetEqualsCriteria(TypeDictionaryEqualityComparer.EqualsCriteriaDelegate equalsCriteria)
        {
            this.equalsCriteria = equalsCriteria;
        }

        public bool Equals(Table fromSet, Table input)
        {
            bool result = this.equalsCriteria(fromSet, input) && fromSet.BasicFieldsEqual(input);
            return result;
        }

        public int GetHashCode(Table obj)
        {
            return obj.GetHashCode();
        }
    }
}
