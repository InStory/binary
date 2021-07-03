using System;
using InStory.binary.stream;

namespace InStory.binary.pool
{
    public interface ILoadOnGet : IDisposable
    {
        public void Load();

    }
}