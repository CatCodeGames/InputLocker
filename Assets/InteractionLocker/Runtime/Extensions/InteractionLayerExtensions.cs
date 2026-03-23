using System.Collections.Generic;

namespace CatCode.InteractionLocking
{
    public static class InteractionLayerExtensions
    {
        public static InteractionLayerMask ToMask(this IEnumerable<InteractionLayerAsset> layers)
        {
            int mask = 0;
            foreach (var layer in layers)
                mask |= 1 << layer.BitIndex;
            return mask;
        }
    }
}