namespace Tests.TestModels
{
    // Normal classes for testing constructor

    public class ConstructorTestSubjectClass
    {
    }

    public class TwoParameterConstructor
    {
        public TwoParameterConstructor(ConstructorTestSubjectClass subject, OneParameterConstructor oneParameterConstructor)
        {
            this.Subject = subject;
            this.SubjectReference = subject;
            this.OneParameterConstructor = oneParameterConstructor;
        }

        public ConstructorTestSubjectClass Subject { get; }
        public ConstructorTestSubjectClass SubjectReference;

        public OneParameterConstructor OneParameterConstructor { get; }
    }

    public class OneParameterConstructor
    {
        public OneParameterConstructor(DefaultConstructor defaultConstructor)
        {
            this.DefaultConstructor = defaultConstructor;
            this.DefaultConstructorReference = defaultConstructor;
        }

        public DefaultConstructor DefaultConstructor { get; set; }

        public DefaultConstructor DefaultConstructorReference;
    }

    public class DefaultConstructor
    {        
    }

    // Classes with an uninstantiatable dependency

    public class Uninstantiatable
    {
        private Uninstantiatable() { }
    }

    public class WithUninstantiatableDependency
    {
        public WithUninstantiatableDependency(DefaultConstructor defaultConstructor, Uninstantiatable uninstantiatable)
        {
        }
    }
}
