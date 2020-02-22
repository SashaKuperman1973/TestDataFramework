# TestDataFramework
This is a unit testing tool. It randomizes values in your classes automatically so you don't need to do data entry.

In addition to the source code here, TestDataFramework is available as a nuget package for use in .NET applications.

Detailed instructions are on the wiki: https://github.com/SashaKuperman1973/TestDataFramework/wiki

This tool includes the ability to write out objects to a database for tests which involve data access.

Foreign/primary key relationships can be specified and are respected. Currently only SQL Server is supported, plus the option to generate in-memory objects only.

Using an uncommitted transaction scope makes it possible for data written to a database to be temporary only. New implementations of the persistence layer can be created to support additional database systems or any other persistence medium.

The most basic usage is:

    IPopulator populator = this.factory.CreateMemoryPopulator();

    OperableList<SubjectClass> subjectReferences = populator.Add<SubjectClass>(10);

    populator.Bind();

    IEnumerable<SubjectClass> populatedCollection = 
        subjectReferences.Select(reference => reference.RecordObject).ToList();

Then do what you want with your populated collection. Only properties with a public getter and setter will be populated, and only supported types.

The argument to populator.Add<T>() is the number of elements that you want of that type in your collection.
Call Add as many times as you want before calling populator.Bind().

To just create a single object:

    RecordReference<SubjectClass> reference = 
        populator.Add<SubjectClass>()    // No numeric argument to Add method

    populator.Bind();

    SubjectClass subject = reference.RecordObject;
