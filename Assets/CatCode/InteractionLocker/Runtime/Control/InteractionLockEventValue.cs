using System;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Управляет состоянием блокировки по битовой маске через контроллер.
    /// Позволяет включать и выключать блокировку, отслеживать текущее состояние и подписываться на изменения.
    /// </summary>
    public sealed class InteractionLockSwitch : IDisposable
    {
        private readonly IBitmaskController _controller;
        private readonly int _mask;

        private bool _disposed;
        private BitmaskHandle _handle;
        private Action<bool> _changed;
        private bool _subscribed;

        public bool IsDisposed => _disposed;

        public InteractionLockSwitch(IBitmaskController controller, int mask)
        {
            _controller = controller;
            _mask = mask;
        }

        public bool IsLocked
        {
            get => !_disposed && _handle != null && _controller.IsHandleValid(_handle);
            set
            {
                if (value)
                    Lock();
                else
                    Unlock();
            }
        }

        public event Action<bool> IsLockChanged
        {
            add => Subscribe(value);            
            remove => Unsubscribe(value);
        }


        public void Lock()
        {
            if (_disposed)
                return;
            ReleaseHandle();
            _handle = _controller.Add(_mask);
            Notify(true);
        }

        public void Unlock()
        {
            if (_disposed)
                return;
            ReleaseHandle();
            Notify(false);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            UnsubscribeFromController();
            ReleaseHandle();
        }


        private void ReleaseHandle()
        {
            if (_handle != null)
                _controller.Release(_handle);
            _handle = null;
        }


        private void Subscribe(Action<bool> action)
        {
            if (_disposed)
                return;

            _changed += action;

            if (!_subscribed && _changed != null)
            {
                _subscribed = true;
                _controller.Cleared += OnControllerCleared;
            }
        }

        private void Unsubscribe(Action<bool> action)
        {
            if (_disposed)
                return;

            _changed -= action;

            if (_subscribed && _changed == null)
            {
                _subscribed = false;
                _controller.Cleared -= OnControllerCleared;
            }
        }

        private void Notify(bool state)
        {
            if (_subscribed)
                _changed?.Invoke(state);
        }


        private void UnsubscribeFromController()
        {
            if (_subscribed)
                _controller.Cleared -= OnControllerCleared;

            _subscribed = false;
        }

        private void OnControllerCleared()
            => Notify(false);
    }
}