using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay
{
    internal class TanksGameLogic : BaseGameLogic
    {
        private TanksGameplayState _gameplayState;
        private ConsoleInput _input;
        private TankEntity _playerTank;

        public TanksGameLogic(TanksGameplayState gameplayState, ConsoleInput input)
        {
            _gameplayState = gameplayState;
            _input = input;
            _input.Subscribe(this);

            var characteristics = TankCharacteristics.CreatePlayerCharacteristics();
            _playerTank = new TankEntity(1, 1, true, characteristics, _gameplayState.EntityManager, _gameplayState.GetMap());
            _gameplayState.EntityManager.AddEntity(_playerTank);
        }

        public override void OnArrowUp()
        {
            _playerTank.Move(Direction.Up);
        }

        public override void OnArrowDown()
        {
            _playerTank.Move(Direction.Down);
        }

        public override void OnArrowLeft()
        {
            _playerTank.Move(Direction.Left);
        }

        public override void OnArrowRight()
        {
            _playerTank.Move(Direction.Right);
        }

        public override void OnArrowShoot()
        {
            _playerTank.Shoot();
        }
    }
}