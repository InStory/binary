using System;

namespace InStory.binary.pool
{
    public abstract class PooledObject<T> : NamedObject, IDisposable where T : NamedObject, new()
    {
        private static readonly LoadObjectPool<T> Pool = new();

        public static T Get()
        {
            return Pool.Get();
        }

        public static T Get(string name)
        {
            var obj = Get();
            obj.Name = name;
            return obj;
        }
        
        public static int PooledObjects => Pool.PooledObjects;
        public static int AllObjects => Pool.AllObjects;

        public virtual void Dispose()
        {
            Name = string.Empty;
            Pool.Return(this as T);
        }
    }
}