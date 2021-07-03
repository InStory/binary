using System;

namespace InStory.binary.pool
{
    public abstract class PooledObject<T> : IDisposable where T : class, new()
    {
        private static readonly LoadObjectPool<T> Pool = new();

        public static T Get()
        {
            return Pool.Get();
        }

        public virtual void Dispose()
        {
            Pool.Return(this as T);
        }
    }
}