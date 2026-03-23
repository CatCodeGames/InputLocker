
namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Обёртка над целочисленной битовой маской.
    /// 
    /// Позволяет работать с маской как с типом: проверять биты, объединять и исключать маски
    /// </summary>
    public struct InteractionLayerMask
    {
        private int _mask;
        public int Mask
        {
            readonly get => _mask;
            set => _mask = value;
        }

        public InteractionLayerMask(int value)
        {
            _mask = value;
        }

        public readonly bool HasBit(int bit)
            => (_mask & (1 << bit)) != 0;

        public readonly bool Contain(InteractionLayerMask other)
        {
            var otherMask = other._mask;
            return (_mask & otherMask) == otherMask;
        }

        public static InteractionLayerMask operator &(InteractionLayerMask a, InteractionLayerMask b)
            => new(a._mask & b._mask);

        public static InteractionLayerMask operator |(InteractionLayerMask a, InteractionLayerMask b)
            => new(a._mask | b._mask);

        public static implicit operator int(InteractionLayerMask m)
            => m._mask;

        public static implicit operator InteractionLayerMask(int v)
            => new(v);
    }
}