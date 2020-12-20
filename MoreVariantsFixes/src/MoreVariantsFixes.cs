using Vintagestory.API.Common;

namespace MoreVariantsFixes
{
    public class MoreVariantsFixes : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterEntityBehaviorClass("fixtypedcontainer", typeof(FixTypedContainer));
            api.RegisterBlockEntityBehaviorClass("BEFixTypedChest", typeof(BEFixTypedChest));
        }
    }
}