using UnityEngine;
using UnityEngine.EventSystems;


namespace CatCode
{
    [RequireComponent(typeof(PhysicsRaycaster))]
    public sealed class PhysicsRaycasterLocker : MonoBehaviour
    {
        [SerializeField] private PhysicsRaycaster _raycaster;
        [SerializeField, InspectorReadOnly] private LayerMask _layerMask;

        private void Awake()
        {
            _layerMask = _raycaster.eventMask;
        }

        private void OnEnable()
        {
            var inputLockManager = InputLockManager.Instance;
            inputLockManager.LayerMaskChanged += OnLayerMaskChanged;
            OnLayerMaskChanged(inputLockManager.LayerMask);
        }

        private void OnDisable()
        {
            if (InputLockManager.TryGetInstance(out var inputLockManager))
                inputLockManager.LayerMaskChanged -= OnLayerMaskChanged;
        }

        private void OnLayerMaskChanged(int layerMask)
        {
            _raycaster.eventMask = _layerMask & ~layerMask;
        }


#if UNITY_EDITOR
        private void Reset()
        {
            _raycaster = GetComponent<PhysicsRaycaster>();
        }
#endif
    }
}