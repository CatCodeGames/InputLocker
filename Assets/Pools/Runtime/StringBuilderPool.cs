using System.Text;
using UnityEngine.Pool;

namespace CatCode.Pools
{
    public static class StringBuilderPool
    {
        private static ObjectPool<StringBuilder> _pool = new(
            createFunc: () => new StringBuilder(),
            actionOnGet: instance => instance.Clear());

        public static PooledObject<StringBuilder> Get(out StringBuilder stringBuilder)
            => _pool.Get(out stringBuilder);
    }
}