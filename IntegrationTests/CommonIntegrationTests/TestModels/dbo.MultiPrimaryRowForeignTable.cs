using System;
using TestDataFramework;

namespace IntegrationTests.CommonIntegrationTests.TestModels
{
  [Table("dbo", "MultiPrimaryRowForeignTable")]
  public class MultiPrimaryRowForeignTable
  {
	[ForeignKey("dbo", "ManualKeyPrimaryTable", "Tester")]
    public int TesterForeignKeyAlpha { get; set; }
	
	[StringLength(20)]
	[ForeignKey("dbo", "ManualKeyPrimaryTable", "Key1")]
    public string ForeignKey1Alpha { get; set; }
	
	[ForeignKey("dbo", "ManualKeyPrimaryTable", "Key2")]
    public int ForeignKey2Alpha { get; set; }
	
	[ForeignKey("dbo", "ManualKeyPrimaryTable", "Tester")]
    public int TesterForeignKeyBeta { get; set; }
	
	[StringLength(20)]
	[ForeignKey("dbo", "ManualKeyPrimaryTable", "Key1")]
    public string ForeignKey1Beta { get; set; }
	
	[ForeignKey("dbo", "ManualKeyPrimaryTable", "Key2")]
    public int ForeignKey2Beta { get; set; }
	
	[StringLength(20)]
    public string Data { get; set; }
	
	[ForeignKey("dbo", "TestPrimaryTable", "Key1")]
    public int ForeignToPrimaryKey1 { get; set; }
	
	[ForeignKey("dbo", "TestPrimaryTable", "Key2")]
    public int ForeignToPrimaryKey2 { get; set; }
	
  }
}