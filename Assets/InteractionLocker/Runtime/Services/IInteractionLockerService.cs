using System;

namespace CatCode.InteractionLocking
{
    public interface IInteractionLockerService : IDisposable
    {
        event Action<InteractionLockData> LockDataChanged;
        InteractionLockData LockData { get; }
        
        InteractionLockHandle Lock(InteractionLayerAsset asset);
        InteractionLockHandle Lock(InteractionLayerAsset[] assets);
        InteractionLockHandle Lock(InteractionLayerMask interactionMask);
        InteractionLockHandle Lock(InteractionLayerMask[] interactionMasks);

        InteractionLockSwitch GetLockSwitch(InteractionLayerAsset asset);
        InteractionLockSwitch GetLockSwitch(InteractionLayerMask interactionMask);

        void Clear();
    }
}