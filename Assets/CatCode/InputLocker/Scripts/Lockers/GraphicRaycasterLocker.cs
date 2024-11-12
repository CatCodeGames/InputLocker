using UnityEngine;
using UnityEngine.UI;

namespace CatCode
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public sealed class GraphicRaycasterLocker : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _raycaster;
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

#if UNITY_EDITOR
        private void Reset()
        {
            _raycaster = GetComponent<GraphicRaycaster>();
        }
#endif
    }
}