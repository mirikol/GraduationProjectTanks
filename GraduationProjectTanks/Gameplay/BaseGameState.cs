using GraduationProjectTanks.Systems.Renderer;

namespace GraduationProjectTanks.Gameplay
{
    public abstract class BaseGameState
    {
        public abstract void Update(float deltaTime);

        public abstract void Reset();

        public abstract void Draw(ConsoleRenderer renderer);

        public abstract bool IsDone();
    }
}