using UnityEngine;

namespace CatCode
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CanvasGroupInputLocker : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
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
            _canvasGroup.blocksRaycasts = !inputBlocked;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
#endif
    }
}