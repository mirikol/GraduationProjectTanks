using GraduationProjectTanks.Systems;
using GraduationProjectTanks.Gameplay;
using GraduationProjectTanks.Systems.Renderer;

namespace GraduationProjectTanks
{
    internal class Program
    {
        private const int ConsoleWidth = 120;
        private const int ConsoleHeight = 40;
        private const int FrameDelayMs = 100;
        private const int GameOverDelayMultiplier = 30;

        static void Main()
        {
            MapConfiguration mapConfiguration = MapConfiguration.CreateDefault();

            var renderer = new ConsoleRenderer();
            renderer.SetConsoleSize(ConsoleWidth, ConsoleHeight);

            var input = new ConsoleInput();            
            var gameplayState = new TanksGameplayState(mapConfiguration, renderer);
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
                    Thread.Sleep(FrameDelayMs * GameOverDelayMultiplier);
                    break;
                }

                Thread.Sleep(FrameDelayMs);
            }
        }
    }
}