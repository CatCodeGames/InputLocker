using UnityEngine;
using UnityEngine.UI;
using CatCode.InteractionLocking;

public sealed class InteractionLockerTester : MonoBehaviour
{
    private InteractionLockHandle _gameLock = default;
    private InteractionLockSwitch _uiLockSwitch;

    [SerializeField] private Button _lockGameButton;
    [SerializeField] private Button _unlockGameButton;
    [SerializeField] private Button _lockUIButton;
    [SerializeField] private Button _unlockUIButton;
    [Space]
    [SerializeField] private InteractionLocker _locker;
    [Header("Lock Layers")]
    [SerializeField] private InteractionLayerAsset _gameLayer;
    [SerializeField] private InteractionLayerAsset _uiLayer;

    private void Awake()
    {
        _uiLockSwitch = _locker.GetLockSwitch(_uiLayer);

        _lockGameButton.onClick.AddListener(() =>
        {
            _gameLock.Dispose();
            _gameLock = _locker.Lock(_gameLayer);
        });
        _unlockGameButton.onClick.AddListener(() => _gameLock.Dispose());

        _lockUIButton.onClick.AddListener(() => _uiLockSwitch.Lock());
        _unlockUIButton.onClick.AddListener(() => _uiLockSwitch.Unlock());
    }

    private void OnDestroy()
    {
        _gameLock.Dispose();
        _uiLockSwitch.Dispose();
    }
}
