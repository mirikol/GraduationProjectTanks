using GraduationProjectTanks.Systems;
using GraduationProjectTanks.Gameplay;
using GraduationProjectTanks.Systems.Renderer;

namespace GraduationProjectTanks
{
    internal class Program
    {
        private const int ConsoleWidth = 120;
        private const int ConsoleHeight = 40;
        private const int MapWidth = 15;
        private const int MapHeight = 15;
        private const int RandomSeed = 1;

        private const int FrameDelayMs = 100;

        static void Main()
        {            
            var renderer = new ConsoleRenderer();
            renderer.SetConsoleSize(ConsoleWidth, ConsoleHeight);

            var input = new ConsoleInput();            
            var gameplayState = new TanksGameplayState(MapWidth, MapHeight, RandomSeed, renderer);
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