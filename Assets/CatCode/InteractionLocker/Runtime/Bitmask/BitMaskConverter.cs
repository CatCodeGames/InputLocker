namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Конвертер битовых масок.
    /// Преобразует входную битовую маску в выходную по заданным правилам сопоставления.
    /// Для ускорения повторных вызовов кэширует последний результат.
    /// </summary>
    public sealed class BitMaskConverter
    {
        private const int BitsCount = 32;
        private readonly int[] _bitToValue = new int[BitsCount];

        private int _lastInputMask;
        private int _lastOutputMask;

        private bool _isCached;

        public void Clear()
        {
            for (int i = 0; i < BitsCount; i++)
                _bitToValue[i] = 0;
        }

        public void AddMapping(int inputMask, int outputMask)
        {
            for (int bit = 0; bit < BitsCount; bit++)
                if (HasBit(inputMask, bit))
                    _bitToValue[bit] = outputMask;
        }

        public int Convert(int inputMask)
        {
            if (_isCached && _lastInputMask == inputMask)
                return _lastOutputMask;

            var outputMask = ApplyMapping(inputMask);

            _lastInputMask = inputMask;
            _lastOutputMask = outputMask;
            _isCached = true;
            
            return outputMask;
        }

        private int ApplyMapping(int inputMask)
        {
            int outMask = 0;

            for (int bit = 0; bit < BitsCount; bit++)
                if (HasBit(inputMask, bit))
                    outMask |= _bitToValue[bit];

            return outMask;
        }

        private bool HasBit(int mask, int bit)
            => (mask & (1 << bit)) != 0;

        public void ResetCache()
        {
            _isCached = false;
            _lastOutputMask = 0;
            _lastInputMask = 0;
        }
    }
}