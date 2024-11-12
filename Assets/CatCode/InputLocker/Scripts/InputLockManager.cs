using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatCode
{
    public enum InputLayer
    {
        None = 0,
        Game = 1 << 0,
        UI = 1 << 1,
        All = Game | UI
    }


    [ObjectCreationConfig(
        ObjectCreationMode.FindOnScene | ObjectCreationMode.LoadFromResources,
        instanceName: nameof(InputLockManager),
        resourceName: "Singletons/" + nameof(InputLockManager))]
    public sealed class InputLockManager : MonoSingleton<InputLockManager>
    {
        private int _prevLayerMask;
        private InputLayer _prevInputMask;

        private readonly LockableMask _layerMask = new();
        private readonly LockableMask _inputMask = new();

        [SerializeField] private LayerMask _gameLayers;
        [SerializeField] private LayerMask _uiLayers;

        public InputLayer InputMask => (InputLayer)_inputMask.Mask;
        public int LayerMask => _layerMask.Mask;

        public IReadOnlyDictionary<object, int> InputLockers => _inputMask.Lockers;
        public IReadOnlyDictionary<object, int> LayerLockers => _layerMask.Lockers;


        public event Action<int> LayerMaskChanged;
        public event Action<InputLayer> InputMaskChanged;


        public void LockLayer(object locker, int layerMask)
        {
            _layerMask.Add(locker, layerMask);
            TryNotifyLayer();
        }

        public void UnlockLayer(object locker, int layerMask)
        {
            _layerMask.Remove(locker, layerMask);
            TryNotifyLayer();
        }

        public void UnlockLayer(object locker)
        {
            _layerMask.Remove(locker);
            TryNotifyLayer();
        }

        public int ApplyMask(int layerMask)
            => layerMask & ~_layerMask.Mask;

        public void LockInput(object locker, InputLayer inputMask)
        {
            _inputMask.Add(locker, (int)inputMask);
            LockLayer(locker, InputToLayer(inputMask));
            TryNotifyInput();
        }

        public void UnlockInput(object locker, InputLayer inputMask)
        {
            _inputMask.Remove(locker, (int)inputMask);
            UnlockLayer(locker, InputToLayer(inputMask));
            TryNotifyInput();
        }

        public void UnlockInput(object locker)
        {
            _inputMask.Remove(locker);
            UnlockLayer(locker);
            TryNotifyInput();
        }


        private void TryNotifyLayer()
        {
            var newMask = _layerMask.Mask;
            if (_prevLayerMask == newMask)
                return;
            _prevLayerMask = newMask;
            LayerMaskChanged?.Invoke(newMask);
        }

        private void TryNotifyInput()
        {
            var newMask = (InputLayer)_inputMask.Mask;
            if (_prevInputMask == newMask)
                return;
            _prevInputMask = newMask;
            InputMaskChanged?.Invoke(newMask);
        }


        private int InputToLayer(InputLayer layer)
        {
            int layerMask = 0;
            if (InputLayerUtils.HasFlag(layer, InputLayer.Game))
                layerMask |= _gameLayers;
            if (InputLayerUtils.HasFlag(layer, InputLayer.UI))
                layerMask |= _uiLayers;
            return layerMask;
        }
    }

    //public sealed class LayerLockManager
    //{
    //    private int _prevLayerMask;
    //    private readonly LockableMask _layerMask = new();

    //    public int LayerMask => _layerMask.Mask;
    //    public IReadOnlyDictionary<object, int> Lockers => _layerMask.Lockers;

    //    public event Action<int> LayerMaskChanged;

    //    public void LockLayer(object locker, int layerMask)
    //    {
    //        _layerMask.Add(locker, layerMask);
    //        TryNotifyLayer();
    //    }

    //    public void UnlockLayer(object locker, int layerMask)
    //    {
    //        _layerMask.Remove(locker, layerMask);
    //        TryNotifyLayer();
    //    }

    //    public void UnlockLayer(object locker)
    //    {
    //        _layerMask.Remove(locker);
    //        TryNotifyLayer();
    //    }

    //    private void TryNotifyLayer()
    //    {
    //        var newMask = _layerMask.Mask;
    //        if (_prevLayerMask == newMask)
    //            return;
    //        _prevLayerMask = newMask;
    //        LayerMaskChanged?.Invoke(newMask);
    //    }
    //}


    //public sealed class InputLockManager : MonoSingleton<InputLockManager>
    //{
    //    private InputLayer _prevInputMask;
    //    private readonly LockableMask _inputMask = new();

    //    [SerializeField] private LayerMask _gameLayers;
    //    [SerializeField] private LayerMask _uiLayers;

    //    public InputLayer InputMask => (InputLayer)_inputMask.Mask;
    //    public IReadOnlyDictionary<object, int> Lockers => _inputMask.Lockers;

    //    public event Action<InputLayer> InputMaskChanged;

    //    public void LockInput(object locker, InputLayer inputMask)
    //    {
    //        _inputMask.Add(locker, (int)inputMask);
    //        TryNotifyInput();
    //    }

    //    public void UnLockInput(object locker, InputLayer inputMask)
    //    {
    //        _inputMask.Remove(locker, (int)inputMask);
    //        TryNotifyInput();
    //    }

    //    public void UnlockInput(object locker)
    //    {
    //        _inputMask.Remove(locker);
    //        TryNotifyInput();
    //    }

    //    private void TryNotifyInput()
    //    {
    //        var newMask = (InputLayer)_inputMask.Mask;
    //        if (_prevInputMask == newMask)
    //            return;
    //        _prevInputMask = newMask;
    //        InputMaskChanged?.Invoke(newMask);
    //    }
    //}
}