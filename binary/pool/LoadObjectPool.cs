using System;
using System.Collections.Concurrent;
using System.Threading;
using InStory.binary.other;

namespace InStory.binary.pool
{
    public class LoadObjectPool<T> where T : new()
    {
        private readonly ConcurrentBag<T> _objects;
        #if DEBUG
        private readonly ConcurrentHashSet<T> _usingObjects;
        #endif
        
        public int PooledObjects => _objects.Count;
        public int AllObjects => _allObjects;
        public int UsingObjects => AllObjects - PooledObjects; // We cant use _usingObjects.Count() because in Release mode this structure will not be used

        private int _allObjects = 0;

        public LoadObjectPool()
        {
            _objects = new ConcurrentBag<T>();
#if DEBUG
            _usingObjects = new ConcurrentHashSet<T>();
#endif
        }

        public T Get()
        {
            if(!_objects.TryTake(out var item))
            {
                item = new T();
                Interlocked.Increment(ref _allObjects);
            }
            
#if DEBUG
            _usingObjects.Add(item);
#endif            
            if(item is ILoadOnGet i){
                i.Load();
            }

            return item;
        }

        public void Return(T item)
        {
            _objects.Add(item);
            
#if DEBUG
            if (!_usingObjects.Contains(item))
            {
                throw new Exception("");
            }
            
            _usingObjects.Remove(item);
#endif
        } 
    }
}