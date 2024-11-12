using System;
using System.Collections.Generic;

namespace CatCode
{
    public sealed class LockableMask
    {
        private int _mask;
        private readonly Dictionary<object, int> _lockers = new();

        public int Mask => _mask;

        public IReadOnlyDictionary<object, int> Lockers => _lockers;

        public void Add(object locker, int mask)
        {
            if (_lockers.TryGetValue(locker, out int currentMask))
            {
                currentMask |= mask;
                _lockers[locker] = currentMask;
            }
            else
            {
                _lockers.Add(locker, mask);
            }
            CalculateMask();
        }

        public void Remove(object locker, int mask)
        {
            if (!_lockers.TryGetValue(locker, out int currentMask))
                return;
            currentMask &= ~mask;
            if (currentMask == 0)
                _lockers.Remove(locker);
            else
                _lockers[locker] = currentMask;
            CalculateMask();
        }

        public void Remove(object locker)
        {
            if (!_lockers.Remove(locker))
                return;
            CalculateMask();
        }


        public int AddToMask(in int mask)
            => mask | _mask;

        public int ExcludeFromMask(in int mask)
            => mask & ~_mask;

        private void CalculateMask()
        {
            int mask = 0;
            foreach (var lockerValue in _lockers.Values)
                mask |= lockerValue;

            if (_mask == mask)
                return;

            _mask = mask;
        }
    }
}