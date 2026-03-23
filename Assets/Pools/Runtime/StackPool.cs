using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.Pools 
{
    public class StackPool<T>
    {
        internal static readonly ObjectPool<Stack<T>> s_Pool = new(
            createFunc: () => new Stack<T>(),
            actionOnGet: null,
            actionOnRelease: stack => stack.Clear());

        public static Stack<T> Get()
            => s_Pool.Get();

        public static PooledObject<Stack<T>> Get(out Stack<T> value)
            => s_Pool.Get(out value);

        public static void Release(Stack<T> toRelease)
            => s_Pool.Release(toRelease);
    }
}