using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay
{
    public class EnemyAI
    {
        private TankEntity _tank;
        private Map _map;
        private EntityManager _entityManager;
        private Random _random;

        private Vector2 _targetPosition;
        private float _decisionCooldown;
        private float _decisionInterval = 2.0f;
        private float _playerDetectionRange = 5.0f;

        public EnemyAI(TankEntity tank, Map map, EntityManager entityManager, Random random)
        {
            _tank = tank;
            _map = map;
            _entityManager = entityManager;
            _random = random;
            _targetPosition = new Vector2(tank.X, tank.Y);
            _decisionCooldown = _decisionInterval;
        }

        public void Update(float deltaTime)
        {
            _decisionCooldown -= deltaTime;
            if (_decisionCooldown <= 0)
            {
                MakeMovementDecision();
                _decisionCooldown = _decisionInterval;
            }

            MoveTowardsTarget();
            TryShootAtPlayer();
        }

        private void MakeMovementDecision()
        {
            int attempts = 0;
            Vector2 newTarget;

            do
            {
                newTarget = new Vector2(_random.Next(1, _map.Width - 1), _random.Next(1, _map.Height - 1));
                attempts++;
            } while (!IsPositionReachable(newTarget) && attempts < 10);

            _targetPosition = newTarget;
        }

        private bool IsPositionReachable(Vector2 position)
        {
            return _map.IsCellPassable(position.X, position.Y);
        }

        private void MoveTowardsTarget()
        {
            if (Math.Abs(_tank.X - _targetPosition.X) < 1 && Math.Abs(_tank.Y - _targetPosition.Y) < 1)
                return;

            int dx = _targetPosition.X - _tank.X;
            int dy = _targetPosition.Y - _tank.Y;

            Direction preferredDirection;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                preferredDirection = dx > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                preferredDirection = dy > 0 ? Direction.Down : Direction.Up;
            }

            if (CanMoveInDirection(preferredDirection))
            {
                _tank.Move(preferredDirection);
                return;
            }

            var directions = new List<Direction>()
            {
                Direction.Up, Direction.Down, Direction.Left, Direction.Right
            };

            directions.Remove(preferredDirection);
            Shuffle(directions);

            foreach (var direction in directions)
            {
                if (CanMoveInDirection(direction))
                {
                    _tank.Move(direction);
                    return;
                }
            }
        }

        private bool CanMoveInDirection(Direction direction)
        {
            Vector2 newPosition = direction switch
            {
                Direction.Up => new Vector2(_tank.X, _tank.Y - 1),
                Direction.Down => new Vector2(_tank.X, _tank.Y + 1),
                Direction.Left => new Vector2(_tank.X - 1, _tank.Y),
                Direction.Right => new Vector2(_tank.X + 1, _tank.Y),
                _ => new Vector2(_tank.X, _tank.Y)
            };

            if (!_map.IsCellPassable(newPosition.X, newPosition.Y))
                return false;

            var tanks = _entityManager.GetEntitiesOfType<TankEntity>();

            foreach (var tank in tanks)
            {
                if (tank != _tank && tank.IsAlive && Math.Abs(tank.X - newPosition.X) < 0.5f && Math.Abs(tank.Y - newPosition.Y) < 0.5f)
                {
                    return false;
                }
            }

            return true;
        }

        private void TryShootAtPlayer()
        {
            var playerTank = _entityManager.GetEntitiesOfType<TankEntity>()
                .FirstOrDefault(t => t.IsPlayer && t.IsAlive);

            if (playerTank == null)
                return;

            float distance = MathF.Sqrt(MathF.Pow(playerTank.X - _tank.X, 2) + MathF.Pow(playerTank.Y - _tank.Y, 2));

            if (distance > _playerDetectionRange)
                return;

            if (IsPlayerInSight(playerTank) && HasClearShotToPlayer(playerTank))
            {
                _tank.Shoot();
            }
        }

        private bool IsPlayerInSight(TankEntity player)
        {
            return _tank.Direction switch
            {
                Direction.Up => player.Y < _tank.Y && Math.Abs(player.X - _tank.X) < 0.5f,
                Direction.Down => player.Y > _tank.Y && Math.Abs(player.X - _tank.X) < 0.5f,
                Direction.Left => player.X < _tank.X && Math.Abs(player.Y - _tank.Y) < 0.5f,
                Direction.Right => player.X > _tank.X && Math.Abs(player.Y - _tank.Y) < 0.5f,
                _ => false
            };
        }

        private bool HasClearShotToPlayer(TankEntity player)
        {
            int startX = _tank.X;
            int startY = _tank.Y;
            int endX = player.X;
            int endY = player.Y;

            // алгоритм Брезенхема для проверки линии видимости.
            int dx = Math.Abs(endX - startX);
            int dy = Math.Abs(endY - startY);
            int sx = startX < endX ? 1 : -1;
            int sy = startY < endY ? 1 : -1;
            int err = dx - dy;

            int x = startX;
            int y = startY;

            while (true)
            {
                if (x == endX && y == endY)
                    break;

                if (x != startX && y != startY)
                {
                    if (!_map.IsCellPassbleForProjectile(x, y))
                        return false;
                }

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                                
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }

            return true;
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}