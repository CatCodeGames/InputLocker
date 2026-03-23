using System;

namespace CatCode.InteractionLocking
{
    /// <summary>
    /// Интерфейс счётчика битовой маски.
    /// Позволяет добавлять и удалять биты по маскам, агрегируя их в итоговую битовую маску.
    /// </summary>
    public interface IBitmaskCounter
    {
        int Mask { get; }

        event Action<int>MaskChanged;
        event Action Cleared;

        void Add(int mask);
        void Remove(int mask);
        void Clear();
    }
}