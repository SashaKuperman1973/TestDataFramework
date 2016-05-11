using NameSpace1;
using NameSpace2;
using TestDataFramework;
using TestDataFramework.Populator.Interfaces;

namespace PocoIntegrationTests.TestModels
{
	public class Decorator
	{
		public static void Decorate(IPopulator populator)
		{

			populator.DecorateType<ForeignToAutoPrimaryTable>()
			.AddAttributeToType(new TableAttribute("TestDataFramework", "dbo", "ForeignToAutoPrimaryTable"))
				.AddAttributeToMember(m => m.ForignKey, new ForeignKeyAttribute("dbo", "TertiaryManualKeyForeignTable", "Pk"))
				.AddAttributeToMember(m => m.ForignKey, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Manual));

			populator.DecorateType<TertiaryManualKeyForeignTable>()
			.AddAttributeToType(new TableAttribute("TestDataFramework", "dbo", "TertiaryManualKeyForeignTable"))
				.AddAttributeToMember(m => m.Pk, new PrimaryKeyAttribute(PrimaryKeyAttribute.KeyTypeEnum.Auto))
				.AddAttributeToMember(m => m.FkManualKeyForeignTable, new ForeignKeyAttribute("dbo", "ManualKeyForeignTable", "UserId"))
				.AddAttributeToMember(m => m.FkStringForeignKey, new StringLengthAttribute(20))
				.AddAttributeToMember(m => m.FkStringForeignKey, new ForeignKeyAttribute("dbo", "ManualKeyForeignTable", "ForeignKey1"));
		}
	}
}
