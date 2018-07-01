namespace TestDataFramework.Populator.Interfaces
{
    public interface IMakeable<out TParent>
    {
        TParent Make();

        TParent BindAndMake();
    }
}