using System;

namespace Asteroid.Utility
{
    public interface IBinding
    {
        Type Key { get; }
        Type Value { get; }
        object Result { get; set; }
        bool OnStart { get; }
        void CreateOnStart();
    }
}