using System;

namespace Asteroid.Utility
{
    public class Binding : IBinding
    {
        public Type Key { get; private set; }
        public Type Value { get; private set; }
        
        public object Result { get; set; }

        public bool OnStart { get; private set; }
        
        public Binding(Type key, Type value)
        {
            Key = key;
            Value = value;
        }
        
        public void CreateOnStart()
        {
            OnStart = true;
        }
    }
}