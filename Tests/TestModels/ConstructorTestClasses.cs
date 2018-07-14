/*
    Copyright 2016, 2017, 2018 Alexander Kuperman

    This file is part of TestDataFramework.

    TestDataFramework is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    TestDataFramework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with TestDataFramework.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Tests.TestModels
{
    // Normal classes for testing constructor

    public class ConstructorTestSubjectClass
    {
    }

    public class TwoParameterConstructor
    {
        public ConstructorTestSubjectClass SubjectReference;

        public TwoParameterConstructor(ConstructorTestSubjectClass subject,
            OneParameterConstructor oneParameterConstructor)
        {
            this.Subject = subject;
            this.SubjectReference = subject;
            this.OneParameterConstructor = oneParameterConstructor;
        }

        public ConstructorTestSubjectClass Subject { get; }

        public OneParameterConstructor OneParameterConstructor { get; }
    }

    public class OneParameterConstructor
    {
        public DefaultConstructor DefaultConstructorReference;

        public OneParameterConstructor(DefaultConstructor defaultConstructor)
        {
            this.DefaultConstructor = defaultConstructor;
            this.DefaultConstructorReference = defaultConstructor;
        }

        public DefaultConstructor DefaultConstructor { get; set; }
    }

    public class DefaultConstructor
    {
    }

    // Classes with an uninstantiatable dependency

    public class Uninstantiatable
    {
        private Uninstantiatable()
        {
        }
    }

    public class WithUninstantiatableDependency
    {
        public WithUninstantiatableDependency(DefaultConstructor defaultConstructor, Uninstantiatable uninstantiatable)
        {
        }
    }
}