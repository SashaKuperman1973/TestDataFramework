using System;
using TestDataFramework;

namespace IntegrationTests.TestModels.Generated.Declarative
{
  [Table("TestDataFramework", "dbo", "ForeignToAutoPrimaryTable")]
  public class ForeignToAutoPrimaryTable
  {
	[ForeignKey("dbo", "TertiaryManualKeyForeignTable", "Pk")]
	[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
    public int ForignKey { get; set; }
	
  }
}