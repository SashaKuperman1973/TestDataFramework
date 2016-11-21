This is a unit testing tool. It randomizes values in your classes automatically so you don't need to do data entry.
It is currently in beta.

NEW FEATURES in this release:

- POCO classes.
- GUI code generation tool

This tool includes the ability to write out objects to a database for tests which involve data access. 

Foreign/primary key relationships can be specified and are respected. Currently only SQL Server is supported, 
plus the option to generate in-memory objects only. 

Using an uncommitted transaction scope makes it possible for data written to a database to be temporary only. 
New implementations of the persistence layer can be created to support additional database systems or any 
other persistence medium.

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

NOTE: Root namespace is TestDataFramework. The PopulatorFactory is in namespace TestDataFramework.Factories.

Replace all values with what you need. Note that you need to specify the default catalogue in the connection 
string if there are any classes involved in an Sql Server population session that don't have a TableAttribute with a catalogue given.

Right now only properties, not field members are populated. This tool is designed to be used to populate DTOs for your tests
when the in-memory option is used and/or to run tests that include the persistence layer when the Sql Server option is used.

For database population you would create a class that mirrors the database table schema, if you don't already have such a class.
You can use the included GUI tool to generate classes from Sql Server tables. See the New Features section below. 
Values written to the database are also populated into the related classes, as with the in-memory populator.

While in-memory testing can be done right on your DTO, primary/foreign key relationships and other items such as maximum string length
are specified with attributes on properties.

NOTE: For properties to be picked up, they must be public with a public getter and setter.

Here is an example of how to specify the objects to be generated. The usage is identical for both populators.

        using (var factory = new PopulatorFactory())
        {
          IPopulator testDataPopulator = factory.Create...
          
          // a list of 2 record references
          
		  IList<RecordReference<ManualKeyPrimaryTable>> primaries = populator.Add<ManualKeyPrimaryTable>(2);
          
          // a single record reference
          
		  RecordReference<ManualKeyPrimaryTable> aPrimary = populator.Add<ManualKeyPrimaryTable>();
          
          // a list of 2 foreign key record references:
          
		  IList<RecordReference<ManualKeyForeignTable>> foreignSet1 = populator.Add<ManualKeyForeignTable>(2);
          
          // specify which primary key records are associated with the foreign key records. 
          // (Usage of primary/foreign key relationship property attributes is enforced by default)
		  
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
Be aware when you use the Sql Server populator in a context without a transaction scope since that
can cause DB write commits.

IMPORTANT:
Primary/foreign relationships must be made with calls to RecordReference<SubjectClass>.AddPrimaryRecordReference method, if there are any such constrainsts
in the target database. This is so the randomization algorithm can generate corresponding values and not break referential integrity.

In order for calls to AddPrimaryRecordReference to succeed, properties need to be decorated with related foreign/primary key attributes.
All properties in a class marked with a PrimaryKeyAttribute must have a corresponding ForeignKeyAttribute property in the SubjectClass (generic argument above).
Otherwise the call will throw.

Note that such attributes will maintain the referential integrity rules with the in-memory populator 
as well as the Sql Server populator, if they are specified and RecordReference.AddPrimaryRecordReference 
statements are made.

UPDATE: In this version of the tool, you can have POCO classes that are decorated programatically.
The new features are described in the last section.
  
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

Also table/column names that have spaces in them (eg: [First Name]) have not 
been tested and shouold probably be avoided.

Note that string length is a maximum length for primary keys, and a full length for normal properties.

Currently the primitive types are supported (including nullables), plus strings, DateTimes and Guids. 
Also, an attempt will be made to populate complex types, if they have a parameterless constructor. 
This is in line with the idea of populating DTOs.

In addition, there are a couple of known types that the system will attempt to populate:

KeyValuePair,
IDictionary, Dictionary,
IEnumerable, List, IList

Obviously, this doesn't make sense for database population and won't work with it since the related handlers are not written.

Here are usages of the other attributes, along with some example scenarios for properties:

    public class SubjectClass
    {
        public const int StringLength = 10;
        public const int Precision = 4;
        public const long Max = 7;

        public int Getter { get { throw new NotImplementedException();} }
        public int Setter { set { throw new NotImplementedException();} }

        [PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
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


NEW FEATURES in this release:

1. POCO classes.

2. GUI generation tool:

	Generates C# types from an SQL Server database, 
	with your option of declarative or programmatic auto-generation of attributes.

Your entities can now be POCOs using programmatic assignment of "virtual" attributes to your classes and properties.

Attributes that are assigned programmatically are treated by this framework as if they were actually decorated on classes and properties.
You can also mix and match programmatic attributes and declarative attributes (though I don't know why you would want to, this is a consequence of the design).

I am also bundling a GUI tool that I derived from an SQL script I found online to generate code from database tables.
I'm putting the executable in the nuget package folder for this project.

Here is a comparison of the Declarative mode output and the POCO mode output of the GUI tool.
The POCO mode output also serves as an example of how to manually code programmatic attribute assignments:

DECLARATIVE

ManualKeyPrimaryTable.cs:

	using System;
	using TestDataFramework;

	namespace Ding
	{
	  [Table("TestDataFramework", "dbo", "ManualKeyPrimaryTable")]
	  public class ManualKeyPrimaryTable
	  {
		[StringLength(20)]
		[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
		public string Key1 { get; set; }
		
		[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
		public int Key2 { get; set; }
		
		[StringLength(50)]
		public string AString { get; set; }
		
		[Precision(2)]
		public decimal ADecimal { get; set; }
		
		[Precision(3)]
		public decimal AFloat { get; set; }
		
	  }
	}

ManualKeyForeignTable.cs

	using System;
	using TestDataFramework;

	namespace Dong
	{
	  [Table("TestDataFramework", "dbo", "ManualKeyForeignTable")]
	  public class ManualKeyForeignTable
	  {
		[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
		public Guid UserId { get; set; }
		
		[StringLength(20)]
		[ForeignKey("dbo", "ManualKeyPrimaryTable", "Key1")]
		[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
		public string ForeignKey1 { get; set; }
		
		[ForeignKey("dbo", "ManualKeyPrimaryTable", "Key2")]
		public int ForeignKey2 { get; set; }
		
		public short AShort { get; set; }
		
		public long ALong { get; set; }
		
	  }
	}
	
PROGRAMMATIC (POCO)

ManualKeyPrimaryTable.cs

	using System;
	using TestDataFramework;

	namespace Ding
	{
	  public class ManualKeyPrimaryTable
	  {
		public string Key1 { get; set; }
		
		public int Key2 { get; set; }
		
		public string AString { get; set; }
		
		public decimal ADecimal { get; set; }
		
		public decimal AFloat { get; set; }
		
	  }
	}

ManualKeyForeignTable.cs

	using System;
	using TestDataFramework;

	namespace Dong
	{
	  public class ManualKeyForeignTable
	  {
		public Guid UserId { get; set; }
		
		public string ForeignKey1 { get; set; }
		
		public int ForeignKey2 { get; set; }
		
		public short AShort { get; set; }
		
		public long ALong { get; set; }
		
	  }
	}

Decorator.cs
	
	using System;
	using TestDataFramework;
	using TestDataFramework.Populator.Interfaces;
	using Ding;
	using Dong;

	namespace TestModel
	{
		public class Decorator
		{
			public static void Decorate(IPopulator populator)
			{

				populator.DecorateType<ManualKeyPrimaryTable>()
				.AddAttributeToType(new TableAttribute("TestDataFramework", "dbo", "ManualKeyPrimaryTable"))
					.AddAttributeToMember(m => m.Key1, new StringLengthAttribute(20))
					.AddAttributeToMember(m => m.Key1, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Manual))
					.AddAttributeToMember(m => m.Key2, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Manual))
					.AddAttributeToMember(m => m.AString, new StringLengthAttribute(50))
					.AddAttributeToMember(m => m.ADecimal, new PrecisionAttribute(2))
					.AddAttributeToMember(m => m.AFloat, new PrecisionAttribute(3));

				populator.DecorateType<ManualKeyForeignTable>()
				.AddAttributeToType(new TableAttribute("TestDataFramework", "dbo", "ManualKeyForeignTable"))
					.AddAttributeToMember(m => m.UserId, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Manual))
					.AddAttributeToMember(m => m.ForeignKey1, new StringLengthAttribute(20))
					.AddAttributeToMember(m => m.ForeignKey1, new ForeignKeyAttribute("dbo", "ManualKeyPrimaryTable", "Key1"))
					.AddAttributeToMember(m => m.ForeignKey1, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Manual))
					.AddAttributeToMember(m => m.ForeignKey2, new ForeignKeyAttribute("dbo", "ManualKeyPrimaryTable", "Key2"));
			}
		}
	}
	
It doesn't matter what the name of the Decorator file/class/method is. The important part is to call IPopulator.DecorateType<T> and fluently define the attributes you want.

Note that the arguments to the TableAttribute constructor are catalogue (database name), schema and table name. There are overloads with fewer parameters.
Also note that adding a TableAttribute is optional. If omitted the table name will be the name of the class and a default schema may be in effect. 
The default schema is "dbo" for the Sql Server populator. It can be changed.

To be clear, the programmatic population code of which the above is an example is generated by the GUI tool when choosing the POCO option, along with the target classes as well.

Since the generator only has database schema information with which to decorate the target classes, generated Foreign Key attributes won't specify Primary Key classes via strong typing.
That means that the primary/foreign class matching routine does its best with what it has. 

For example, a class with a TableAttribute takes precedence over a class with the same name but no TableAttribute, when searching for a matching primary key class.
Here the idea is that something explicitly decorated is considered to be more likely intended to be used with this software.

Note that it is possible to specify primary key classes in a strongly typed manner with the typeof operator when manually using the ForeignKeyAttribute with the appropriate constructor overload.
	
See the source code for the details of the matching alogrithm.
	
Side note: The GUI tool was written very quickly and doesn't have good style, it was just made as a bonus to support the project. 
I would rather not pushish the code, but people might want to extend it for themselves, so I am putting the source on GitHub. 
Please don't judge the GUI tool code too harshly, my intent was for this framework to be the actual product.

Minor breaking change from last version: all attribute initialization is given via the constructor. Direct write access to the properties has been removed.

So,

	[PrimaryKey(KeyType = PrimaryKeyAttribute.KeyTypeEnum.Auto)]
	public int Key { get; set; }

won't work (note the assignment operator). You need to use one of the constructor overloads:

	[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
	public int Key { get; set; }
	

Lastly, I don't want this tool to be in beta forever; if you try it but find bugs and are still interested, please drop me a line with the details of the error.


Cheers,

Sasha Kuperman
alexander.kuperman@gmail.com
May 13, 2016
