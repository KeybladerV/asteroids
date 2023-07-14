using System;
using System.Collections.Generic;
using System.Linq;

namespace Asteroid.Utility
{
    public sealed class SuperStupidDI : IInjectionBinder
    {
        
        private Dictionary<Type, IBinding> _dependencies = new();
        
        private HashSet<DestroyableInstance> _destroyableInstances = new();

        private Type _currentBind;

        public IBinding Bind<T, T1>() where T1 : T, new()
        {
            IBinding binding;
            if (!_dependencies.ContainsKey(typeof(T)))
            {
                binding = new Binding(typeof(T), typeof(T1));
                _dependencies.Add(typeof(T), binding);
            }
            else
            {
                binding = new Binding(typeof(T), typeof(T1));
                _dependencies[typeof(T)] = binding;
            }

            return binding;
        }
        
        public void Unbind<T>()
        {
            if(_dependencies.ContainsKey(typeof(T)))
                _dependencies.Remove(typeof(T));
        }

        public T GetOrCreate<T>()
        {
            return (T) GetOrCreate(typeof(T));
        }

        public object GetOrCreate(Type type)
        {
            if (_dependencies.TryGetValue(type, out var binding) && binding.Result != null)
                return binding.Result;

            if (!_dependencies.ContainsKey(type))
                throw new Exception($"Dependency for {type} not found");

            var instance = Activator.CreateInstance(binding.Value);
            binding.Result = instance;
            
            InjectDependencies(instance);
            OnPostCreate(instance);
            TryAddOnDestroy(instance);

            return instance;
        }

        private void TryAddOnDestroy(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var methodInfo in methods)
            {
                if(methodInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(OnDestroyAttribute)) == null)
                    continue;
                
                _destroyableInstances.Add(new DestroyableInstance(instance, methodInfo));
                return;
            }
        }

        public void InitStartBindings()
        {
            foreach (var binding in _dependencies.Values)
            {
                if(!binding.OnStart)
                    continue;
                
                GetOrCreate(binding.Key);
            }
        }
        
        private void InjectDependencies(object instance)
        {
            var properties = instance.GetType().GetProperties();
            foreach (var propertyInfo in properties)
            {
                if(propertyInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(InjectAttribute)) == null)
                    continue;
                if (!_dependencies.ContainsKey(propertyInfo.PropertyType))
                    continue;

                var dependency = GetOrCreate(propertyInfo.PropertyType);
                propertyInfo.SetValue(instance, dependency);
            }
        }
        
        private void OnPostCreate(object instance)
        {
            var methods = instance.GetType().GetMethods();
            foreach (var methodInfo in methods)
            {
                if(methodInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(PostCreateAttribute)) == null)
                    continue;
                
                methodInfo.Invoke(instance, null);
            }
        }

        public void DestroyInstances()
        {
            foreach (var destroyableInstance in _destroyableInstances)
            {
                destroyableInstance.DestroyMethod.Invoke(destroyableInstance.Instance, null);
            }
        }
    }
}