using GraduationProjectTanks.Shared;

namespace GraduationProjectTanks.Tanks
{
    internal class TanksGameLogic : BaseGameLogic
    {
        private TanksGameplayState _gameplayState;
        private ConsoleInput _input;

        public TanksGameLogic(TanksGameplayState gameplayState, ConsoleInput input)
        {
            _gameplayState = gameplayState;
            _input = input;
            _input.Subscribe(this);
        }

        public override void OnArrowUp()
        {
            Console.WriteLine("Move Up");
        }

        public override void OnArrowDown()
        {
            Console.WriteLine("Move Down");
        }

        public override void OnArrowLeft()
        {
            Console.WriteLine("Move Left");
        }

        public override void OnArrowRight()
        {
            Console.WriteLine("Move Right");
        }

        public override void OnArrowShoot()
        {
            Console.WriteLine("Shoot!");
        }
    }
}