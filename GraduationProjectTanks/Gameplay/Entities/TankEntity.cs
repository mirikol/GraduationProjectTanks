using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay.Entities
{
    public class TankEntity : BaseEntity, IMoveable
    {
        public Direction Direction { get; private set; }
        public bool IsPlayer { get; }
        public TankCharacteristics Characteristics { get; }
        public Vector2 Position => _position;
        public float MoveSpeed => 1.0f / Characteristics.MoveDelay;
        public bool IsMoving => _moveCooldown <= 0f;

        private float _moveCooldown = 0f;
        private float _shootCooldown = 0f;
        private Vector2 _position;
        private EntityController _entityController;
        private Map _map;
        private EnemyAI? _enemyAI;

        public override int X => _position.X;
        public override int Y => _position.Y;
        public override bool IsSolid => true;
        public override bool CanTakeDamage => true;

        public TankEntity(int x, int y, bool isPlayer, TankCharacteristics characteristics, EntityController entityController, Map map, Random? random = null)
            : base(x, y, characteristics.MaxHealth)
        {
            _position = new Vector2(x, y);
            IsPlayer = isPlayer;
            Characteristics = characteristics;
            _entityController = entityController;
            _map = map;
            Direction = Direction.Up;

            if (!isPlayer && random != null)
            {
                _enemyAI = new EnemyAI(this, map, entityController, random);
            }
        }

        public void Move(Direction direction)
        {
            if (_moveCooldown > 0f) return;

            Direction = direction;
            Vector2 newPosition = CalculateNextPosition(direction);

            if (CanMoveTo(newPosition))
            {
                _position = newPosition;
                _moveCooldown = Characteristics.MoveDelay;
            }
        }

        public bool CanMoveTo(Vector2 position)
        {
            return _map.IsCellPassable((int)position.X, (int)position.Y) && !CheckTankCollision(position);
        }

        public bool CanMoveInDirection(Direction direction)
        {
            Vector2 newPosition = CalculateNextPosition(direction);
            return CanMoveTo(newPosition);
        }

        private Vector2 CalculateNextPosition(Direction direction)
        {
            return direction switch
            {
                Direction.Up => _position + new Vector2(0, -1),
                Direction.Down => _position + new Vector2(0, 1),
                Direction.Left => _position + new Vector2(-1, 0),
                Direction.Right => _position + new Vector2(1, 0),
                _ => _position
            };
        }

        public void Shoot()
        {
            if (_shootCooldown > 0) return;

            Vector2 projectilePos = _position;
            var projectile = new ProjectileEntity(
                projectilePos.X,
                projectilePos.Y,
                Direction,
                Characteristics.ProjectileSpeed,
                Characteristics.Damage,
                _entityController,
                _map,
                this
            );

            _entityController.AddEntity(projectile);
            _shootCooldown = Characteristics.ShootDelay;
        }


        private bool CheckTankCollision(Vector2 position)
        {
            var tanks = _entityController.GetEntitiesOfType<TankEntity>();

            foreach (var tank in tanks)
            {
                if (tank != this && tank.IsAlive && Math.Abs(tank.X - position.X) < 1 && Math.Abs(tank.Y - position.Y) < 1)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Update(float deltaTime)
        {
            if (_moveCooldown > 0) _moveCooldown -= deltaTime;
            if (_shootCooldown > 0) _shootCooldown -= deltaTime;
            if (!IsPlayer && _enemyAI != null)
            {
                _enemyAI.Update(deltaTime);
            }
        }

        public override void Draw()
        {            
        }

        public override void OnCollision(IEntity other)
        {
            if (other is ProjectileEntity projectile && projectile.Shooter != this)
            {
                TakeDamage(projectile.Damage);
            }
        }
    }
}