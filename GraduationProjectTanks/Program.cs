using GraduationProjectTanks.Shared;
using GraduationProjectTanks.Tanks;

namespace GraduationProjectTanks
{
    internal class Program
    {
        private const int FrameDelayMs = 100;
        static void Main()
        {            
            var renderer = new ConsoleRenderer();
            var input = new ConsoleInput();            
            var gameplayState = new TanksGameplayState(20, 15, 12345, renderer);
            var gameLogic = new TanksGameLogic(gameplayState, input);

            var lastTime = DateTime.Now;

            while (true)
            {
                var currentTime = DateTime.Now;
                var deltaTime = (float)(currentTime - lastTime).TotalSeconds;
                lastTime = currentTime;

                input.Update();
                gameplayState.Update(deltaTime);
                gameplayState.Draw(renderer);
                renderer.Render();

                if (gameplayState.IsDone())
                    break;

                Thread.Sleep(FrameDelayMs);
            }
        }
    }
}