using System;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Счётчик битовой маски на основе массива.
    /// 
    /// Хранит, сколько раз каждый бит был включён.    
    /// Бит считается установленным, если его счётчик больше нуля.
    /// </summary>
    public sealed class BitmaskArrayCounter : IBitmaskCounter
    {
        private const int BitsCount = 32;
        private readonly int[] _bits = new int[BitsCount];

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
            for (int i = 0; i < BitsCount; i++)
                if ((mask & (1 << i)) != 0)
                    _bits[i]++;
            RecalculateMask();
        }

        public void Remove(int mask)
        {
            for (int i = 0; i < BitsCount; i++)
                if ((mask & (1 << i)) != 0)
                    _bits[i]--;
            RecalculateMask();
        }

        public void Clear()
        {
            for (int i = 0; i < BitsCount; i++)
                _bits[i] = 0;
            Mask = 0;
            Cleared?.Invoke();
        }

        private void RecalculateMask()
        {
            int resultMask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (_bits[i] > 0)
                    resultMask |= (1 << i);
            }
            Mask = resultMask;
        }
    }
}