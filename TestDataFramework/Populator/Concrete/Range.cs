using System.CodeDom.Compiler;

namespace TestDataFramework.Populator.Concrete
{
    public class Range
    {
        private int? endPositionField;

        private int? lengthField;

        private int? startPositionField;

        public static Range StartAndLength(int startPosition, int length)
        {
            var result = new Range
            {
                startPositionField = startPosition,
                endPositionField = startPosition + length - 1,
                lengthField = length
            };

            return result;
        }

        public static Range StartAndEndPositions(int startPosition, int endPosition)
        {
            var result = new Range
            {
                startPositionField = startPosition,
                endPositionField = endPosition,
                lengthField = endPosition + 1 - startPosition
            };

            return result;
        }
            
        internal Range()
        {
        }

        public int StartPosition
        {
            get => this.startPositionField.Value;

            set
            {
                this.startPositionField = value;

                if (this.lengthField.HasValue)
                    this.endPositionField = this.startPositionField.Value + this.lengthField.Value - 1;
                else if (this.endPositionField.HasValue)
                    this.lengthField = this.endPositionField.Value + 1 - this.startPositionField.Value;
            }
        }

        public int EndPosition
        {
            get => this.endPositionField.Value;

            set
            {
                this.endPositionField = value;

                if (this.startPositionField.HasValue)
                    this.lengthField = this.endPositionField.Value + 1 - this.startPositionField.Value;
                else if (this.lengthField.HasValue)
                    this.startPositionField = this.endPositionField.Value + 1 - this.lengthField.Value;
            }
        }

        public int Length
        {
            get => this.lengthField.Value;

            set
            {
                this.lengthField = value;

                if (this.startPositionField.HasValue)
                    this.endPositionField = this.startPositionField.Value + this.lengthField.Value - 1;
                else if (this.endPositionField.HasValue)
                    this.startPositionField = this.endPositionField.Value - this.lengthField.Value + 1;
            }
        }

        public static implicit operator Range(int i)
        {
            var result = new Range {StartPosition = i, EndPosition = i};
            return result;
        }
    }
}