using GraduationProjectTanks.Systems;
using GraduationProjectTanks.Gameplay;
using GraduationProjectTanks.Systems.Renderer;

namespace GraduationProjectTanks
{
    internal class Program
    {
        private const int FrameDelayMs = 100;

        static void Main()
        {            
            var renderer = new ConsoleRenderer();
            var input = new ConsoleInput();            
            var gameplayState = new TanksGameplayState(15, 15, 4219, renderer);
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
                {
                    Thread.Sleep(FrameDelayMs * 30);
                    break;
                }

                Thread.Sleep(FrameDelayMs);
            }
        }
    }
}