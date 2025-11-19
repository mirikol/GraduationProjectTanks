using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay
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
            GetPlayerTank()?.Move(Direction.Up);
        }

        public override void OnArrowDown()
        {
            GetPlayerTank()?.Move(Direction.Down);
        }

        public override void OnArrowLeft()
        {
            GetPlayerTank()?.Move(Direction.Left);
        }

        public override void OnArrowRight()
        {
            GetPlayerTank()?.Move(Direction.Right);
        }

        public override void OnArrowShoot()
        {
            GetPlayerTank()?.Shoot();
        }

        private TankEntity? GetPlayerTank()
        {
            return _gameplayState.EntityController.GetEntitiesOfType<TankEntity>()
                .FirstOrDefault(t => t.IsPlayer && t.IsAlive);

        }
    }
}