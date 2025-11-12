namespace GraduationProjectTanks.Shared
{
    internal abstract class BaseGameLogic : ConsoleInput.IArrowListener
    {
        public abstract void OnArrowDown();
        public abstract void OnArrowLeft();
        public abstract void OnArrowRight();
        public abstract void OnArrowUp();
        public abstract void OnArrowShoot();
    }
}