using System.Collections.Generic;

namespace TestDataFramework.AttributeDecorator
{
    public class TypeDictionaryEqualityComparer : IEqualityComparer<Table>
    {
        public delegate bool EqualsCriteriaDelegate(Table fromSet, Table input);

        private EqualsCriteriaDelegate equalsCriteria;

        public bool Equals(Table fromSet, Table input)
        {
            var result = this.equalsCriteria(fromSet, input) && fromSet.BasicFieldsEqual(input);
            return result;
        }

        public int GetHashCode(Table obj)
        {
            return obj.GetHashCode();
        }

        public void SetEqualsCriteria(EqualsCriteriaDelegate equalsCriteria)
        {
            this.equalsCriteria = equalsCriteria;
        }
    }
}