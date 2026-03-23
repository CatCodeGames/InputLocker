using CatCode.InteractionLocking;
using UnityEngine;

public sealed class InteractionMaskDisabler : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _targetComponent;
    [Space]
    [SerializeField] private InteractionLocker _locker;
    [SerializeField] private InteractionLayerAsset _layer;
    private void OnEnable()
    {
        _locker.LockDataChanged += OnLockDataChanged;
        OnLockDataChanged(_locker.LockData);
    }

    private void OnDisable()
    {
        if (_locker != null)
            _locker.LockDataChanged -= OnLockDataChanged;
    }

    private void OnLockDataChanged(InteractionLockData lockData)
    {
        var inputBlocked = (lockData.InteractionMask & _layer.InteractionMask) == _layer.InteractionMask;
        _locker.enabled = !inputBlocked;
    }
}