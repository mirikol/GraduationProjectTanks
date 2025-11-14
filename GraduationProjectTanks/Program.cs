using GraduationProjectTanks.Shared;
using GraduationProjectTanks.Tanks;

namespace GraduationProjectTanks
{
    internal class Program
    {
        static void Main()
        {
            var colors = new ConsoleColor[]
            {
                ConsoleColor.Black,
                ConsoleColor.Red,
                ConsoleColor.DarkRed,
                ConsoleColor.Blue,
                ConsoleColor.White,
                ConsoleColor.Green,
                ConsoleColor.Yellow
            };

            var renderer = new ConsoleRenderer(colors);
            var input = new ConsoleInput();
            
            var gameplayState = new TanksGameplayState(20, 15, 12345);
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

                Thread.Sleep(100);
            }
        }
    }
}