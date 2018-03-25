namespace TestDataFramework.Populator.Concrete
{
    public class Range
    {
        public Range()
        {
        }

        public Range(int startPosition, int endPosition)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }

        public static implicit operator Range(int i)
        {
            var result = new Range {StartPosition = i, EndPosition = i};
            return result;
        }

        private int? startPosition;
        public int StartPosition
        {
            get => this.startPosition.Value;

            set
            {
                this.startPosition = value;

                if (this.length.HasValue)
                {
                    this.endPosition = this.startPosition.Value + this.length.Value - 1;
                }
                else if (this.endPosition.HasValue)
                {
                    this.length = this.endPosition.Value + 1 - this.startPosition.Value;
                }
            }
        }

        private int? endPosition;
        public int EndPosition
        {
            get => this.endPosition.Value;

            set
            {
                this.endPosition = value;

                if (this.startPosition.HasValue)
                {
                    this.length = this.endPosition.Value + 1 - this.startPosition.Value;
                }
                else if (this.length.HasValue)
                {
                    this.startPosition = this.endPosition.Value + 1 - this.length.Value;
                }
            }
        }

        private int? length;
        public int Length
        {
            get => this.length.Value;

            set
            {
                this.length = value;

                if (this.startPosition.HasValue)
                {
                    this.endPosition = this.startPosition.Value + this.length.Value - 1;
                }
                else if (this.endPosition.HasValue)
                {
                    this.startPosition = this.endPosition.Value - this.length.Value + 1;
                }
            }
        }
    }
}
