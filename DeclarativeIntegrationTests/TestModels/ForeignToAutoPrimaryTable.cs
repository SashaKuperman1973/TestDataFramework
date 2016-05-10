using TestDataFramework;

namespace DeclarativeIntegrationTests.TestModels
{
  [Table("TestDataFramework", "dbo", "ForeignToAutoPrimaryTable")]
  public class ForeignToAutoPrimaryTable
  {
	[ForeignKey("dbo", "TertiaryManualKeyForeignTable", "Pk")]
	[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
    public int ForignKey { get; set; }
	
  }
}