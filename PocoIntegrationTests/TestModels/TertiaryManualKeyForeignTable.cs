using System;

namespace PocoIntegrationTests.TestModels
{
  public class TertiaryManualKeyForeignTable
  {
    public int Pk { get; set; }
	
    public Guid FkManualKeyForeignTable { get; set; }
	
    public string FkStringForeignKey { get; set; }
	
    public int AnInt { get; set; }
	
  }
}