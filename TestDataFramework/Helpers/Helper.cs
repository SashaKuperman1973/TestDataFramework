using System;

namespace TestDataFramework.Helpers
{
    public delegate DateTime DateTimeProvider();

    public static class Helper
    {
        public static DateTime Now => DateTime.Now;
    }
}
