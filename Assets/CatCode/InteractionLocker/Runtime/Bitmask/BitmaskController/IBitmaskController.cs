using System;

namespace CatCode.InteractionLocking
{
    public interface IBitmaskController
    {
        int Mask { get; }

        event Action<int> MaskChanged;
        event Action Cleared;

        BitmaskHandle Add(int mask);
        void Release(BitmaskHandle handle);
        bool IsHandleValid(BitmaskHandle handle);

        void Clear();
    }
}