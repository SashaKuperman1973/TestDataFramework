namespace TestDataFramework.Populator
{
    internal interface IPopulator
    {
        void Populate();

        void Add<T>() where T : new();
    }
}