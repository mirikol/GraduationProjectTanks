using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay
{
    public interface IMoveable
    {
        public Vector2 Position { get; }
        public Direction Direction { get; }
        public bool IsMoving { get; }
        public float MoveSpeed { get; }

        public void Move(Direction direction);
        public bool CanMoveTo(Vector2 position);
        public bool CanMoveInDirection(Direction direction);
    }
}