using System.Reflection;

namespace Asteroid.Utility
{
    public struct DestroyableInstance
    {
        public object Instance { get; }
        public MethodInfo DestroyMethod { get; }
        
        public DestroyableInstance(object instance, MethodInfo destroyMethod)
        {
            Instance = instance;
            DestroyMethod = destroyMethod;
        }
    }
}