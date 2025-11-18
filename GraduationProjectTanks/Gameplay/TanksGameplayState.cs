using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;
using GraduationProjectTanks.Systems.Renderer;

namespace GraduationProjectTanks.Gameplay
{
    public class TanksGameplayState : BaseGameState
    {
        private Map _map;
        private MapRenderer _mapRenderer;
        private bool _isDone;
        private ConsoleRenderer _renderer;
        private EntityManager _entityManager;
        private CollisionSystem _collisionSystem;
        private Random _random;
        private int _currentLevel = 1;
        private bool _levelTransition = false;
        private ShowTextState? _levelTextState;
        private ShowTextState? _gameOverState;
        private bool _gameOver = false;

        public Map GetMap() => _map;
        public EntityManager EntityManager => _entityManager;
        public int CurrentLevel => _currentLevel;

        public TanksGameplayState(int width, int height, int seed, ConsoleRenderer renderer)
        {
            _map = new Map(width, height, seed);
            _mapRenderer = new MapRenderer();
            _renderer = renderer;
            _entityManager = new EntityManager();
            _collisionSystem = new CollisionSystem(_entityManager);
            _random = new Random(seed);

            StartLevel();
        }

        private void StartLevel()
        {
            _entityManager = new EntityManager();
            _collisionSystem = new CollisionSystem(_entityManager);
            _map = new Map(_map.Width, _map.Height, _random.Next());

            CreatePlayer();

            int enemyCount = 2 + _currentLevel;
            CreateEnemies(enemyCount);

            int mapPixelWidth = _map.Width * Map.CellSizeX;
            int mapPixelHeight = _map.Height * Map.CellSizeY;

            _levelTransition = true;
            _levelTextState = new ShowTextState($"Level {_currentLevel}", 2.0f, mapPixelWidth, mapPixelHeight);
            _gameOver = false;
        }

        private void CreatePlayer()
        {
            var playerCharacteristics = TankCharacteristics.CreatePlayerCharacteristics();
            Vector2 safePosition = FindSafePosition(1, 1);

            var playerTank = new TankEntity(safePosition.X, safePosition.Y, true, playerCharacteristics, _entityManager, _map, null);
            _entityManager.AddEntity(playerTank);
        }

        private Vector2 FindSafePosition(int startX, int startY)
        {
            if (_map.IsCellPassable(startX, startY))
            {
                return new Vector2(startX, startY);
            }

            for (int distance = 1; distance < Math.Max(_map.Width, _map.Height); distance++)
            {
                for (int x = Math.Max(1, startX - distance); x <= Math.Min(_map.Width - 2, startX + distance); x++)
                {
                    for (int y = Math.Max(1, startY - distance); y <= Math.Min(_map.Height - 2, startY + distance); y++)
                    {
                        if (Math.Abs(x - startX) + Math.Abs(y - startY) == distance)
                        {
                            if (_map.IsCellPassable(x, y))
                            {
                                return new Vector2(x, y);
                            }
                        }
                    }
                }
            }

            return new Vector2(startX, startY);
        }

        private void CreateEnemies(int count)
        {
            int created = 0;
            int attempts = 0;
            int maxAttempts = 100;

            while (created < count && attempts < maxAttempts)
            {
                attempts++;

                int x = _random.Next(2, _map.Width - 2);
                int y = _random.Next(2, _map.Height - 2);

                if (!_map.IsCellPassable(x, y))
                    continue;

                bool positionOccupied = _entityManager.GetEntitiesOfType<TankEntity>()
                    .Any(t => Math.Abs(t.X - x) < 2 && Math.Abs(t.Y - y) < 2);

                if (positionOccupied)
                    continue;

                var enemyCharacteristics = TankCharacteristics.CreateEnemyCharacteristics(_random);
                var enemyTank = new TankEntity(x, y, false, enemyCharacteristics, _entityManager, _map, _random);
                _entityManager.AddEntity(enemyTank);
                created++;
            }
        }

        public override void Update(float deltaTime)
        {
            if (_gameOver)
            {
                _gameOverState?.Update(deltaTime);
                return;
            }

            if (_levelTransition)
            {
                _levelTextState?.Update(deltaTime);
                if (_levelTextState?.IsDone() == true)
                {
                    _levelTransition = false;
                }
                return;
            }

            _entityManager.Update(deltaTime);
            _collisionSystem.CheckCollision();

            CheckGameConditions();
        }

        private void CheckGameConditions()
        {
            var tanks = _entityManager.GetEntitiesOfType<TankEntity>().ToList();

            bool playerAlive = tanks.Any(t => t.IsPlayer && t.IsAlive);
            bool enemiesAlive = tanks.Any(t => !t.IsPlayer && t.IsAlive);

            if (!playerAlive)
            {
                _gameOver = true;
                int mapPixelWidth = _map.Width * Map.CellSizeX;
                int mapPixelHeight = _map.Height * Map.CellSizeY;
                _gameOverState = new ShowTextState("Game Over", 5.0f, mapPixelWidth, mapPixelHeight);
                _isDone = true;
            }
            else if (!enemiesAlive)
            {
                _currentLevel++;
                StartLevel();
            }
        }

        public override void Reset()
        {
            _isDone = false;
            _gameOver = false;
            _currentLevel = 1;
            _entityManager = new EntityManager();
            StartLevel();
        }
                
        public override bool IsDone()
        {
            return _isDone;
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            renderer.Clear();
            _mapRenderer.DrawMap(_map, renderer, 0, 0);
            _mapRenderer.DrawEntities(_entityManager.Entities, renderer, 0, 0);

            if (_gameOver)
            {
                _gameOverState?.Draw(renderer);
            }
            else if (_levelTransition)
            {
                _levelTextState?.Draw(renderer);
            }
        }
    }
}