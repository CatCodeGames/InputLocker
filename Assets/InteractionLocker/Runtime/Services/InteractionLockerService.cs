using System;
using UnityEngine;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Центральный сервис системы блокировок взаимодействия.
    /// 
    /// Управляет битовой маской активных блокировок, конвертирует её
    /// в Unity LayerMask и уведомляет подписчиков об изменениях.
    /// </summary>
    public sealed class InteractionLockerService : IInteractionLockerService
    {
        private bool _disposed = false;
        private InteractionLockData _lockData;

        private readonly BitMaskConverter _maskConverter;
        private readonly BitmaskController _controller;

        public event Action<InteractionLockData> LockDataChanged;

        public LayerMask LockedLayerMask
            => _maskConverter.Convert(_controller.Mask);

        public int LockedInteractionMask
            => _controller.Mask;

        public InteractionLockData LockData
        {
            get => _lockData;
            set
            {
                if (value.InteractionMask == _lockData.InteractionMask &&
                    value.LayerMask == _lockData.LayerMask)
                    return;
                _lockData = value;
                LockDataChanged?.Invoke(_lockData);
            }
        }

        public static InteractionLockerService Create(InteractionLayersAsset assets)
        {
            var service = new InteractionLockerService(assets);
            service.Activate();
            return service;
        }

        private InteractionLockerService(InteractionLayersAsset assets)
        {
            var bitmaskLockerCore = new BitmaskArrayCounter();

            _maskConverter = new();
            foreach (var layer in assets.Layers)
                _maskConverter.AddMapping(layer.InteractionMask, layer.LayerMask);

            _controller = new(bitmaskLockerCore);
        }

        public void Activate()
        {
            _controller.MaskChanged += OnInteractionMaskChanged;
            OnInteractionMaskChanged(_controller.Mask);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _controller.Clear();
            _controller.MaskChanged -= OnInteractionMaskChanged;

            _disposed = true;
        }

        public InteractionLockHandle Lock(InteractionLayerAsset asset)
            => Lock(asset.InteractionMask);

        public InteractionLockHandle Lock(InteractionLayerAsset[] assets)
        {
            int mask = 0;
            for (int i = 0; i < assets.Length; i++)
                mask |= assets[i].InteractionMask;
            return Lock(mask);
        }

        public InteractionLockHandle Lock(InteractionLayerMask interactionMask)
        {
            var handle = _controller.Add(interactionMask);
            return new InteractionLockHandle(_controller, handle);
        }

        public InteractionLockHandle Lock(InteractionLayerMask[] interactionMasks)
        {
            int mask = 0;
            for (int i = 0; i < interactionMasks.Length; i++)
                mask |= interactionMasks[i].Mask;
            var handle = _controller.Add(mask);
            return new InteractionLockHandle(_controller, handle);
        }

        public InteractionLockSwitch GetLockSwitch(InteractionLayerAsset asset)        
            => GetLockSwitch(asset.InteractionMask);        

        public InteractionLockSwitch GetLockSwitch(InteractionLayerMask interactionMask)
        {
            var lockSwitch = new InteractionLockSwitch(_controller, interactionMask);
            return lockSwitch;
        }

        public void Clear()
            => _controller.Clear();

        private InteractionLockHandle Lock(int mask)
        {
            var handle = _controller.Add(mask);
            return new InteractionLockHandle(_controller, handle);
        }

        private void OnInteractionMaskChanged(int interactionMask)
        {
            var layerMask = _maskConverter.Convert(interactionMask);
            LockData = new(interactionMask, layerMask);
        }
    }
}