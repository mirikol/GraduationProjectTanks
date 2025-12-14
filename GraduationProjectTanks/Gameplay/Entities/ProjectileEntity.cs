using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay.Entities
{
    public class ProjectileEntity : BaseEntity
    {
        private const float MoveThreshold = 1.0f;
        private const float CollisionThreshold = 0.5f;
        private const int InitialHealth = 1;
        public Direction Direction { get; }
        public float Speed { get; }
        public int Damage { get; }
        public TankEntity? Shooter { get; }

        private Vector2 _position;
        private float _moveTimer = 0f;
        private EntityController _entityController;
        private Map _map;

        public override int X => (int)_position.X;
        public override int Y => (int)_position.Y;
        public override bool IsSolid => false;
        public override bool CanTakeDamage => false;

        public ProjectileEntity(int x, int y, Direction direction, float speed, int damage, EntityController entityController, Map map, TankEntity? shooter = null)
            : base(x, y, InitialHealth)
        {
            _position = new Vector2(x, y);
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _entityController = entityController;
            _map = map;
            Shooter = shooter;
        }

        public override void Update(float deltaTime)
        {
            _moveTimer += deltaTime;

            if (_moveTimer >= MoveThreshold / Speed)
            {
                Move();
                _moveTimer = 0f;
            }

            CheckCollision();
        }

        private void Move()
        {
            Vector2 newPosition = Direction switch
            {
                Direction.Up => _position + new Vector2(0, -1),
                Direction.Down => _position + new Vector2(0, 1),
                Direction.Left => _position + new Vector2(-1, 0),
                Direction.Right => _position + new Vector2(1, 0),
                _ => _position
            };

            if (newPosition.X < 0 || newPosition.X >= _map.Width || newPosition.Y < 0 || newPosition.Y >= _map.Height)
            {
                Health = 0;
                return;
            }

            if (!_map.IsCellPassableForProjectile((int)newPosition.X, (int)newPosition.Y))
            {
                _map.DamageWall((int)newPosition.X, (int)newPosition.Y);
                Health = 0;
                return;
            }

            _position = newPosition;
        }

        private void CheckCollision()
        {
            var tanks = _entityController.GetEntitiesOfType<TankEntity>();

            foreach (var tank in tanks)
            {
                if (tank.IsAlive && tank != Shooter && Math.Abs(tank.X - X) < CollisionThreshold && Math.Abs(tank.Y - Y) < CollisionThreshold)
                {
                    tank.TakeDamage(Damage);
                    Health = 0;
                    return;
                }
            }
        }

        public override void Draw()
        {
        }

        public override void OnCollision(IEntity other)
        {
        }
    }
}