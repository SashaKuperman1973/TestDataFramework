# TestDataFramework
This is a unit testing tool. It randomizes values in your classes automatically so you don't need to do data entry.
It is currently in beta.

It includes the ability to write out objects to a database to populate it for tests which involve data access. 
Using an uncommitted transaction scope makes it possible for the data to be temporary only. 
Foreign/primary key relationships can be specified and are respected. Currently only SQL Server is supported.
New implememntations of the persistence layer can be created to support additional database systems or any other persistence medium.

The entry point is the IPopulator factory. For in-memory object population the usage of the factory is:

    using (var factory = new PopulatorFactory())
    {
      IPopulator populator = this.factory.CreateMemoryPopulator();
      
      ...{populate/test}
    }

For the database population it's:

    using (var factory = new PopulatorFactory())
    {
      IPopulator testDataPopulator = factory.CreateSqlClientPopulator(
        @"Data Source=.\SqlExpress;Initial Catalog=TestDataFramework;Integrated Security=SSPI;");
      
      ...{populate/test}
    }

NOTE: root namespace is TestDataFramework. The PopulatorFactory is in namespace TestDataFramework.Factories.

Of course replace all values with what you need. Currently you need to specify the default catalogue. 
For the next iteration I plan to associate specification of the catalogue with the individual class/type being populated, 
which would override this value. For now, if you need multiple databases in play at the same time you should nest the
factory.CreateSqlClientPopulator using statements.

Right now only properties, not field members are populated. This tool is mostly designed to be used to populate DTOs for your tests
when the in-memory option is used. For database population you would create a class that mirrors the database table schema, if 
you don't already have such a class. Values written to the database are also populated into the related classes, like with the 
in-memory populator.

While in-memory testing can be done right on your DTO, primay/foreign key relationship and other items such as maximum string length
are specified with attributes on properties.

NOTE: For properties to be picked up, they must be public with a getter and a setter.

Here is an example of how to specify the objects to be generated. The usage is identical for both populators.

        using (var factory = new PopulatorFactory())
        {
          IPopulator testDataPopulator = factory.Create...
          
          // a list 2 record references
          IList<RecordReference<ManualKeyPrimaryTable>> primaries = populator.Add<ManualKeyPrimaryTable>(2);
          
          // a single record reference
          RecordReference<ManualKeyPrimaryTable> aPrimary = populator.Add<ManualKeyPrimaryTable>();
          
          // a list of 2 foreign key record references:
          IList<RecordReference<ManualKeyForeignTable>> foreignSet1 = populator.Add<ManualKeyForeignTable>(2);
          
          // specify which primary key records are associated with the foreign key records. 
          // (Usage of primary/foreign key relationship property attributes is enforced)
          foreignSet1.ToList().ForEach(f => f.AddPrimaryRecordReference(primaries[0]));
        
          // another primary/foreign key record relationship created
          IList<RecordReference<ManualKeyForeignTable>> foreignSet2 = populator.Add<ManualKeyForeignTable>(2);
          foreignSet2.ToList().ForEach(f => f.AddPrimaryRecordReference(primaries[1]));
          
          // an example of specifying explicit values to be used on properties instead of random ones.
          // these values will override any automatic agreement between foreign and primary key values 
          // and it is possible to introduce foreign key constraint vialotions this way, so be careful!
          // Of course this can be avoided by not giving explicit values to foreign key properties.
          
          primaries[0].Set(o => o.ADecimal, 112233.445566m).Set(o => o.AString, "AAXX").Set(o => o.Key1, "HummHummHumm");
          foreignSet2[1].Set(o => o.ALong, 11111L).Set(o => o.AShort, (short) 1234);
          
          // Here's the call that runs the engine
          
          using (var transactionScope =
              new TransactionScope(TransactionScopeOption.Required,
                  new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted}))
          {
              populator.Bind();
        
              // Typical use case: don't commit the transaction. So no transactionScope.Complete() statement.
              
              {Call your DB data consuming test semantics within the transaction scope if you don't want it to be committed}
          }
        }

NOTE: Access the generated object through the RecordObject property of the RecordReference<T>.

The transaction scope semantics can be completely omitted for the in-memory populator. 
All classes involved will be populated after the call to IPopulator.Bind.

Either way, you can completely swap one populator for the other and the client code can stay the same.
Properties need to be decorated with foreign/primary key attributes, if there are any such constrainsts
in the target database. This is so that random values don't break referential integrity.
Note that such attributes will maintain the referential integrity rules with the in-memory populator,
if they are specified and RecordReference.AddPrimaryRecordReference statements are made.
  
Here is the ManualKeyPrimaryTable class used above:

    public class ManualKeyPrimaryTable
    {
        [PrimaryKey]
        [StringLength(20)]
        public string Key1 { get; set; }

        [PrimaryKey]
        public int Key2 { get; set; }

        public string AString { get; set; }

        [Precision(2)]
        public decimal ADecimal { get; set; }

        [Precision(3)]
        public float AFloat { get; set; }
    }

And here is its foreign key record class:

    public class ManualKeyForeignTable
    {
        [PrimaryKey]
        public Guid UserId { get; set; }

        [StringLength(20)]
        [PrimaryKey]
        [ForeignKey(typeof(ManualKeyPrimaryTable), "Key1")]
        public string ForeignKey1 { get; set; }

        [ForeignKey(typeof(ManualKeyPrimaryTable), "Key2")]
        public int ForeignKey2 { get; set; }

        public short AShort { get; set; }

        public long ALong { get; set; }
    }

Note above, foreign keys can be primary keys too.

See this example of a class with an auto-increment primary key:

    public class TertiaryManualKeyForeignTable
    {
        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Pk { get; set; }

        [ForeignKey(typeof(ManualKeyForeignTable), "UserId")]
        public Guid FkManualKeyForeignTable { get; set; }

        [ForeignKey(typeof(ManualKeyForeignTable), "ForeignKey1")]
        [StringLength(20)]
        public string FkStringForeignKey { get; set; }

        public int AnInt { get; set; }
    }

NOTE: Don't put a composite primary key in a class with an auto-increment key.

Note that string length is a maximum length for primary keys, and a full length for normal properties.

Currently the primitive types are supported (including nullables), plus strings, DateTimes and Guids. 
Also, an attempt will be made to populate complex types, if they have a parameterless constructor. 
This is in line with the idea of populating DTOs.

In addition, there are a couple of known types that the system will attempt to populate:

KeyValuePair,
IDictionary, Dictionary,
IEnumerable, List, IList

Obviously, this doesn't make sense for database population and won't work with it since the related handlers are not written.

Here are usages of the other attributes:

    public class SubjectClass
    {
        public const int StringLength = 10;
        public const int Precision = 4;
        public const long Max = 7;

        public int Getter { get { throw new NotImplementedException();} }
        public int Setter { set { throw new NotImplementedException();} }

        [PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
        public int Key { get; set; }

        public int Integer { get; set; }
        public uint UnsignedInteger { get; set; }
        public uint? UnsignedNullableInteger { get; set; }

        [Max(SubjectClass.Max)]
        public int IntegerWithMax { get; set; }

        public int? NullableInteger { get; set; }

        public long LongInteger { get; set; }
        public ulong UnsignedLongInteger { get; set; }
        public ulong? UnsignedNullableLong { get; set; }

        [Max(SubjectClass.Max)]
        public long LongIntegerWithMax { get; set; }

        public long? NullableLong { get; set; }

        public short ShortInteger { get; set; }
        public ushort UnsignedShortInteger { get; set; }
        public ushort? UnsignedNullableShort { get; set; }

        [Max(SubjectClass.Max)]
        public short ShortIntegerWithMax { get; set; }

        public short? NullableShort { get; set; }

        public string Text { get; set; }

        [StringLength(SubjectClass.StringLength)]
        public string TextWithLength { get; set; }

        public char Character { get; set; }

        public decimal Decimal { get; set; }

        [Precision(SubjectClass.Precision)]
        public decimal DecimalWithPrecision { get; set; }

        public bool Boolean { get; set; }

        public DateTime DateTime { get; set; }

        [PastOrFuture(PastOrFuture.Future)]
        public DateTime DateTimeWithTense { get; set; }

        public byte Byte { get; set; }

        public double Double { get; set; }

        [Precision(SubjectClass.Precision)]
        public double DoubleWithPrecision { get; set; }

        public float Float { get; set; }

        [Email]
        public string AnEmailAddress { get; set; }

        [Email]
        public int NotValidForEmail { get; set; }

        public SecondClass SecondObject { get; set; }

        public int[] SimpleArray { get; set; }

        public int[,,] MultiDimensionalArray { get; set; }

        public int[][][] JaggedArray { get; set; }

        public int[,,][][,] MultiDimensionalJaggedArray { get; set; }

        public int[][,,][] JaggedMultiDimensionalArray { get; set; }
    }

My next step is to have a SQL script that will generate a C# type from a database table. 
I saw an implementation of this on stackoverflow that I can enhance.

I am also considering making all attributes assignable programmatically to their properties 
in addition to being able to decorate properties directly.

This document will be updated as I determine areas that are omitted or need clarification.

Cheers,

Sasha Kuperman
February 9, 2016
