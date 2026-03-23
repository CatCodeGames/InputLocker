using UnityEngine;

namespace CatCode.InteractionLocking
{
    public static class LayerMaskUtils
    {
        public static LayerMask Exclude(LayerMask original, LayerMask locked)
        {
            return original & ~locked;
        }
    }
}