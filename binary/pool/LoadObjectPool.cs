using System;
using System.Collections.Concurrent;

namespace InStory.binary.pool
{
    public class LoadObjectPool<T> where T : new()
    {
        private readonly ConcurrentBag<T> _objects;

        public LoadObjectPool()
        {
            _objects = new ConcurrentBag<T>();
        }

        public T Get()
        {
            if(!_objects.TryTake(out var item))
            {
                item = new T();
            }
            
            if(item is ILoadOnGet i){
                i.Load();
            }

            return item;
        }

        public void Return(T item) => _objects.Add(item);
        
    }
}