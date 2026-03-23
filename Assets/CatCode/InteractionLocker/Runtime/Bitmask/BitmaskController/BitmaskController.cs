using System;
using UnityEngine.Pool;

namespace CatCode.InteractionLocking
{
    public sealed class BitmaskController : IBitmaskController
    {
        private readonly IObjectPool<BitmaskHandle> _pool;
        private readonly IBitmaskCounter _counter;
        private int _epoch;


        public int Mask => _counter.Mask;

        public event Action<int> MaskChanged
        {
            add => _counter.MaskChanged += value;
            remove => _counter.MaskChanged -= value;
        }

        public event Action Cleared
        {
            add => _counter.Cleared += value;
            remove => _counter.Cleared -= value;
        }

        public BitmaskController(IBitmaskCounter locker)
        {
            _counter = locker;
            _pool = new ObjectPool<BitmaskHandle>(createFunc: () => new());
        }

        public BitmaskHandle Add(int mask)
        {
            var handle = _pool.Get();
            handle.BitMask = mask;
            handle.Epoch = _epoch;
            _counter.Add(mask);
            return handle;
        }

        public void Release(BitmaskHandle handle)
        {
            if (handle == null)
                return;
            if (handle.Epoch != _epoch)
                handle.Generation = 0;
            else
            {
                handle.Generation++;
                _counter.Remove(handle.BitMask);
            }
            _pool.Release(handle);
        }

        public bool IsHandleValid(BitmaskHandle handle)
            => handle != null && handle.Epoch == _epoch;

        public void Clear()
        {
            _epoch++;
            _counter.Clear();
        }
    }
}