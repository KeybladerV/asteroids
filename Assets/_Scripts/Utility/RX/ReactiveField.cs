namespace Utility
{
    public class ReactiveField<T>
    {
        private T _value;
        
        public delegate void OnValueChanged(T value);
        
        public event OnValueChanged OnValueChangedEvent;

        public void SetValue(T value)
        {
            _value = value;
            
            OnValueChangedEvent?.Invoke(value);
        }

        public T GetValue() => _value;

        public void Reset()
        {
            OnValueChangedEvent = null;
            _value = default;
        }
    }
}