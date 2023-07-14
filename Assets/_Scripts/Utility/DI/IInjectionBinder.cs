using System;

namespace Asteroid.Utility
{
    public interface IInjectionBinder
    {
        IBinding Bind<T, T1>() where T1 : T, new();
        T GetOrCreate<T>();
        object GetOrCreate(Type type);
        void Unbind<T>();
        void InitStartBindings();
        void DestroyInstances();
    }
}