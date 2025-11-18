using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay.Entities
{
    public class ProjectileEntity : BaseEntity
    {
        public Direction Direction { get; }
        public float Speed { get; }
        public int Damage { get; }
        public TankEntity? Shooter { get; }

        private Vector2 _position;
        private float _moveTimer = 0f;
        private EntityManager _entityManager;
        private Map _map;

        public override int X => (int)_position.X;
        public override int Y => (int)_position.Y;
        public override bool IsSolid => false;
        public override bool CanTakeDamage => false;

        public ProjectileEntity(int x, int y, Direction direction, float speed, int damage, EntityManager entityManager, Map map, TankEntity? shooter = null)
            : base(x, y, 1)
        {
            _position = new Vector2(x, y);
            Direction = direction;
            Speed = speed;
            Damage = damage;
            _entityManager = entityManager;
            _map = map;
            Shooter = shooter;
        }

        public override void Update(float deltaTime)
        {
            _moveTimer += deltaTime;

            if (_moveTimer >= 1.0f / Speed)
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

            if (!_map.IsCellPassbleForProjectile((int)newPosition.X, (int)newPosition.Y))
            {
                _map.DamageWall((int)newPosition.X, (int)newPosition.Y);
                Health = 0;
                return;
            }

            _position = newPosition;
        }

        private void CheckCollision()
        {
            var tanks = _entityManager.GetEntitiesOfType<TankEntity>();

            foreach (var tank in tanks)
            {
                if (tank.IsAlive && tank != Shooter && Math.Abs(tank.X - X) < 0.5f && Math.Abs(tank.Y - Y) < 0.5f)
                {
                    tank.TakeDamage(Damage);
                    Health = 0;
                    return;
                }
            }

            var projectiles = _entityManager.GetEntitiesOfType<ProjectileEntity>();

            foreach (var projectile in projectiles)
            {
                if (projectile != this && projectile.IsAlive && Math.Abs(projectile.X - X) < 0.5f && Math.Abs(projectile.Y - Y) < 0.5f)
                {
                    continue;
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