using System;
using TestDataFramework;

namespace IntegrationTests.TestModels
{
  public class TertiaryManualKeyForeignTable
  {
	[PrimaryKey(PrimaryKeyAttribute.KeyTypeEnum.Auto)]
    public int Pk { get; set; }
	
	[ForeignKey("dbo", "ManualKeyForeignTable", "UserId")]
    public Guid FkManualKeyForeignTable { get; set; }
	
	[StringLength(20)]
	[ForeignKey("dbo", "ManualKeyForeignTable", "ForeignKey1")]
    public string FkStringForeignKey { get; set; }
	
    public int AnInt { get; set; }
	
  }
}