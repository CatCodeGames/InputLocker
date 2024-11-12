using UnityEngine;
using UnityEngine.UI;
using CatCode;

public sealed class Tester : MonoBehaviour
{
    private readonly object _gameInputLocker = new();
    private readonly object _uiInputLocker = new();

    [SerializeField] private Button _lockGameButton;
    [SerializeField] private Button _unlockGameButton;
    [SerializeField] private Button _lockUIButton;
    [SerializeField] private Button _unlockUIButton;

    private void Awake()
    {
        _lockGameButton.onClick.AddListener(() => InputLockManager.Instance.LockInput(_gameInputLocker, InputLayer.Game));
        _unlockGameButton.onClick.AddListener(() => InputLockManager.Instance.UnlockInput(_gameInputLocker, InputLayer.Game));
        _lockUIButton.onClick.AddListener(() => InputLockManager.Instance.LockInput(_uiInputLocker, InputLayer.UI));
        _unlockUIButton.onClick.AddListener(() => InputLockManager.Instance.UnlockInput(_uiInputLocker, InputLayer.UI));
    }
}
