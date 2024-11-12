using UnityEngine;

namespace CatCode
{
    public sealed class InputLocker : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _raycaster;
        [SerializeField] private InputLayer _inputLayer;

        private void OnEnable()
        {
            var inputLockManager = InputLockManager.Instance;
            inputLockManager.InputMaskChanged += OnInputMaskChanged;
            OnInputMaskChanged(inputLockManager.InputMask);
        }

        private void OnDisable()
        {
            if (InputLockManager.TryGetInstance(out var inputLockManager))
                inputLockManager.InputMaskChanged -= OnInputMaskChanged;
        }

        private void OnInputMaskChanged(InputLayer inputLayer)
        {
            var inputBlocked = InputLayerUtils.HasFlag(inputLayer, _inputLayer);
            _raycaster.enabled = !inputBlocked;
        }
    }
}