using TestDataFramework;

namespace IntegrationTests.TestModels.Generated.Declarative
{
  public class ForeignToAutoPrimaryTable
  {
	[ForeignKey("dbo", "TertiaryManualKeyForeignTable", "Pk")]
	[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Manual)]
    public int ForignKey { get; set; }
	
  }
}