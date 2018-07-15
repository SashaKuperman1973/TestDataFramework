namespace ExplicitlySettingProperties
{
    public class Subject
    {
        public Deep AProperty { get; set; }
    }

    public class Deep
    {
        public string Text { get; set; }
    }
}
