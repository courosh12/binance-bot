namespace Binance.Bot
{
    public class RollingStack<T>
    {
        private T[] _stack;
        private int _size;
        
        public RollingStack()
        {
            _size = 60;
            _stack = new T[_size];
        }

        public void Push(T item)
        {
            if (_stack[0] == null)
                _stack[0] = item;

            for (var i = (_size-1); i > 0; i--)
            {
                _stack[i] = _stack[i - 1];
            }

            _stack[0] = item;
        }

        public T LookUp(int steps)
        {
            var value = _stack[steps - 1];
            if (value == null)
                return default(T);
            else
                return _stack[steps-1];
        }
    }
}