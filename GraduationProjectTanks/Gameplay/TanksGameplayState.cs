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
        private ShowTextState _levelTextState;

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

            _levelTransition = true;
            _levelTextState = new ShowTextState($"Level {_currentLevel}", 2.0f);
        }

        private void CreatePlayer()
        {
            var playerCharacteristics = TankCharacteristics.CreatePlayerCharacteristics();
            var playerTank = new TankEntity(1, 1, true, playerCharacteristics, _entityManager, _map);
            _entityManager.AddEntity(playerTank);
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
            if (_levelTransition)
            {
                _levelTextState.Update(deltaTime);
                if (_levelTextState.IsDone())
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

            if (_levelTransition)
            {
                _levelTextState.Draw(renderer);
            }
        }

        public bool CanMoveTo(int x, int y) => _map.IsCellPassable(x, y);
        public void DamageWall(int x, int y) => _map.DamageWall(x, y);
    }
}