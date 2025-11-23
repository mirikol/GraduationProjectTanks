using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay
{
    public class EnemyAI
    {
        private readonly IMoveable _tank;
        private readonly Map _map;
        private readonly EntityController _entityController;
        private readonly Random _random;

        private Vector2 _targetPosition;
        private float _decisionCooldown;
        private float _decisionInterval = 2.0f;
        private float _playerDetectionRange = 5.0f;

        public EnemyAI(IMoveable tank, Map map, EntityController entityController, Random random)
        {
            _tank = tank;
            _map = map;
            _entityController = entityController;
            _random = random;
            _targetPosition = new Vector2(tank.Position.X, tank.Position.Y);
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
            if (Math.Abs(_tank.Position.X - _targetPosition.X) < 1 && Math.Abs(_tank.Position.Y - _targetPosition.Y) < 1)
                return;

            int dx = _targetPosition.X - _tank.Position.X;
            int dy = _targetPosition.Y - _tank.Position.Y;

            Direction preferredDirection;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                preferredDirection = dx > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                preferredDirection = dy > 0 ? Direction.Down : Direction.Up;
            }

            if (_tank.CanMoveInDirection(preferredDirection))
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
                if (_tank.CanMoveInDirection(direction))
                {
                    _tank.Move(direction);
                    return;
                }
            }
        }

        private void TryShootAtPlayer()
        {
            var playerTank = _entityController.GetEntitiesOfType<TankEntity>()
                .FirstOrDefault(t => t.IsPlayer && t.IsAlive);

            if (playerTank == null)
                return;

            float distance = MathF.Sqrt(MathF.Pow(playerTank.X - _tank.Position.X, 2) + MathF.Pow(playerTank.Y - _tank.Position.Y, 2));

            if (distance > _playerDetectionRange)
                return;

            if (IsPlayerInSight(playerTank) && HasClearShotToPlayer(playerTank))
            {
                if (_tank is TankEntity tankEntity)
                {
                    tankEntity.Shoot();
                }
            }
        }

        private bool IsPlayerInSight(TankEntity player)
        {
            return _tank.Direction switch
            {
                Direction.Up => player.Y < _tank.Position.Y && Math.Abs(player.X - _tank.Position.X) < 0.5f,
                Direction.Down => player.Y > _tank.Position.Y && Math.Abs(player.X - _tank.Position.X) < 0.5f,
                Direction.Left => player.X < _tank.Position.X && Math.Abs(player.Y - _tank.Position.Y) < 0.5f,
                Direction.Right => player.X > _tank.Position.X && Math.Abs(player.Y - _tank.Position.Y) < 0.5f,
                _ => false
            };
        }

        private bool HasClearShotToPlayer(TankEntity player)
        {
            int startX = _tank.Position.X;
            int startY = _tank.Position.Y;
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