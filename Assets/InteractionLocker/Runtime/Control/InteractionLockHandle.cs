using System;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Дескриптор активной блокировки: хранит поколение выданного контроллером хэндла
    /// и позволяет безопасно освободить блокировку через Dispose.
    /// </summary>
    public readonly struct InteractionLockHandle : IDisposable
    {
        private readonly IBitmaskController _controller;
        private readonly BitmaskHandle _handle;
        private readonly int _generation;

        public readonly bool IsLocked
        {
            get
            {
                if (_handle == null)
                    return false;
                return _generation == _handle.Generation && _controller.IsHandleValid(_handle);
            }
        }
        public readonly bool IsDisposed
        {
            get
            {
                if (_handle == null)
                    return true;
                return _generation != _handle.Generation;
            }
        }

        public InteractionLockHandle(IBitmaskController locker, BitmaskHandle handle)
        {
            _controller = locker;
            _handle = handle;
            _generation = _handle.Generation;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            _controller.Release(_handle);
        }
    }
}