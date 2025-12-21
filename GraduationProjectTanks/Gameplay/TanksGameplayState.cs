using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;
using GraduationProjectTanks.Systems.Renderer;

namespace GraduationProjectTanks.Gameplay
{
    public class TanksGameplayState : BaseGameState
    {
        private const int BaseEnemyCount = 2;
        private const int MaxEnemyCreationAttempts = 100;
        private const int PlayerStartX = 1;
        private const int PlayerStartY = 1;
        private const int EnemySpawnBorderOffset = 2;
        private const float EnemySpawnMinDistance = 2.0f;
        private const float LevelTextDuration = 2.0f;
        private const float GameOverTextDuration = 5.0f;

        private Map _map;
        private MapRenderer _mapRenderer;
        private bool _isDone;
        private EntityController _entityController;
        private CollisionSystem _collisionSystem;
        private Random _random;
        private int _currentLevel = 1;
        private bool _levelTransition = false;
        private ShowTextState? _levelTextState;
        private ShowTextState? _gameOverState;
        private bool _gameOver = false;

        private readonly MapConfiguration _mapConfig;

        public EntityController EntityController => _entityController;
        
        public TanksGameplayState(MapConfiguration mapConfiguration, IRenderer renderer)
        {
            _mapConfig = mapConfiguration ?? throw new ArgumentNullException(nameof(mapConfiguration));
            
            _random = new Random(mapConfiguration.Seed);
            _mapRenderer = new MapRenderer();
            _entityController = new EntityController();
            _collisionSystem = new CollisionSystem(_entityController);
            
            _map = new Map(mapConfiguration);

            StartLevel();
        }               

        private void StartLevel()
        {
            _entityController = new EntityController();
            _collisionSystem = new CollisionSystem(_entityController);
            
            var levelConfig = new MapConfiguration()
            {
                Width = _mapConfig.Width,
                Height = _mapConfig.Height,
                CellSizeX = _mapConfig.CellSizeX,
                CellSizeY = _mapConfig.CellSizeY,
                MaxWaterSources = _mapConfig.MaxWaterSources,
                MaxWaterAmount = _mapConfig.MaxWaterAmount,
                GenerateMaze = _mapConfig.GenerateMaze,
                GenerateWater = _mapConfig.GenerateWater,
                Seed = _random.Next()
            };

            _map = new Map(levelConfig);

            CreatePlayer();

            int enemyCount = BaseEnemyCount + _currentLevel;
            CreateEnemies(enemyCount);

            int mapPixelWidth = _map.Width * _map.CellSizeX;
            int mapPixelHeight = _map.Height * _map.CellSizeY;

            _levelTransition = true;
            _levelTextState = new ShowTextState($"Level {_currentLevel}", LevelTextDuration, mapPixelWidth, mapPixelHeight);
            _gameOver = false;
        }

        private void CreatePlayer()
        {
            var playerCharacteristics = TankCharacteristics.CreatePlayerCharacteristics();
            Vector2 safePosition = FindSafePosition(PlayerStartX, PlayerStartY);

            var playerTank = new TankEntity(safePosition.X, safePosition.Y, true, playerCharacteristics, _entityController, _map, null);
            _entityController.AddEntity(playerTank);
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

            while (created < count && attempts < MaxEnemyCreationAttempts)
            {
                attempts++;
                // отступ от границы карты для спавна enemies.
                int x = _random.Next(EnemySpawnBorderOffset, _map.Width - EnemySpawnBorderOffset);
                int y = _random.Next(EnemySpawnBorderOffset, _map.Height - EnemySpawnBorderOffset);

                if (!_map.IsCellPassable(x, y))
                    continue;

                bool positionOccupied = _entityController.GetEntitiesOfType<TankEntity>()
                    .Any(t => Math.Abs(t.X - x) < EnemySpawnMinDistance && Math.Abs(t.Y - y) < EnemySpawnMinDistance);

                if (positionOccupied)
                    continue;

                var enemyCharacteristics = TankCharacteristics.CreateEnemyCharacteristics(_random);
                var enemyTank = new TankEntity(x, y, false, enemyCharacteristics, _entityController, _map, _random);
                _entityController.AddEntity(enemyTank);
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

            _entityController.Update(deltaTime);
            _collisionSystem.CheckCollision();

            CheckGameConditions();
        }

        private void CheckGameConditions()
        {
            var tanks = _entityController.GetEntitiesOfType<TankEntity>().ToList();

            bool playerAlive = tanks.Any(t => t.IsPlayer && t.IsAlive);
            bool enemiesAlive = tanks.Any(t => !t.IsPlayer && t.IsAlive);

            if (!playerAlive)
            {
                _gameOver = true;
                int mapPixelWidth = _map.Width * _map.CellSizeX;
                int mapPixelHeight = _map.Height * _map.CellSizeY;
                _gameOverState = new ShowTextState("Game Over", GameOverTextDuration, mapPixelWidth, mapPixelHeight);
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
            _entityController = new EntityController();

            _random = new Random(_mapConfig.Seed);

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
            _mapRenderer.DrawEntities(_entityController.Entities, _map, renderer, 0, 0);

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