using System;
using TestDataFramework;

namespace IntegrationTests.TestModels.Generated.POCO
{
  public class TertiaryManualKeyForeignTable
  {
    public int Pk { get; set; }
	
    public Guid FkManualKeyForeignTable { get; set; }
	
    public string FkStringForeignKey { get; set; }
	
    public int AnInt { get; set; }
	
  }
}