using System;
using System.Collections.Generic;
using UnityEngine;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Счётчик битовой маски на основе словаря.
    /// 
    /// Хранит, сколько раз каждый бит был включён.    
    /// Бит считается установленным, если его счётчик больше нуля.
    /// </summary>
    public sealed class BitmaskDictionaryCounter : IBitmaskCounter
    {
        private readonly Dictionary<int, int> _masks = new();

        private int _mask;

        public event Action<int> MaskChanged;
        public event Action Cleared;

        public int Mask
        {
            get => _mask; set
            {
                if (_mask == value)
                    return; _mask = value;
                MaskChanged?.Invoke(_mask);
            }
        }

        public void Add(int mask)
        {
            if (_masks.ContainsKey(mask))
                _masks[mask]++;
            else
            {
                _masks.Add(mask, 1);
                RecalculateMask();
            }
        }

        public void Remove(int mask)
        {
            if (!_masks.ContainsKey(mask))
                return;
            var value = _masks[mask];
            if (value == 0)
                return;

            value = Mathf.Max(value - 1, 0);

            if (value != 0)
                _masks[mask] = value;
            else
            {
                _masks.Remove(mask);
                RecalculateMask();
            }
        }

        public void Clear()
        {
            _masks.Clear();

            Mask = 0;
            Cleared?.Invoke();
        }

        private void RecalculateMask()
        {
            int resultMask = 0;
            foreach (var key in _masks)
                resultMask |= resultMask;
            Mask = resultMask;
        }
    }
}