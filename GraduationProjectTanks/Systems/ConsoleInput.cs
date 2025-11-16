namespace GraduationProjectTanks.Systems
{
    internal class ConsoleInput
    {
        public interface IArrowListener
        {
            abstract void OnArrowUp();
            abstract void OnArrowDown();
            abstract void OnArrowLeft();
            abstract void OnArrowRight();
            abstract void OnArrowShoot();
        }

        private readonly HashSet<IArrowListener> _arrowListeners = [];

        public void Subscribe(IArrowListener listener)
        {
            if (listener != null)
            {
                _arrowListeners.Add(listener);
            }
        }

        public void Unsubscribe(IArrowListener listener)
        {
            _arrowListeners.Remove(listener);
        }

        public void Update()
        {
            if (!Console.KeyAvailable)
                return;

            var key = Console.ReadKey(intercept: true).Key;

            foreach (var listener in _arrowListeners.ToArray())
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow or ConsoleKey.W:
                        listener.OnArrowUp();
                        break;
                    case ConsoleKey.DownArrow or ConsoleKey.S:
                        listener.OnArrowDown();
                        break;
                    case ConsoleKey.LeftArrow or ConsoleKey.A:
                        listener.OnArrowLeft();
                        break;
                    case ConsoleKey.RightArrow or ConsoleKey.D:
                        listener.OnArrowRight();
                        break;
                    case ConsoleKey.Enter or ConsoleKey.Spacebar:
                        listener.OnArrowShoot();
                        break;
                }
            }
        }
    }
}