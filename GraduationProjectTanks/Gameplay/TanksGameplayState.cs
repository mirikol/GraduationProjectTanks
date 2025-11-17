using GraduationProjectTanks.Gameplay.Entities;
using GraduationProjectTanks.Systems;

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

        public Map GetMap() => _map;
        public EntityManager EntityManager => _entityManager;

        public TanksGameplayState(int width, int height, int seed, ConsoleRenderer renderer)
        {
            _map = new Map(width, height, seed);
            _mapRenderer = new MapRenderer();
            _renderer = renderer;
            _entityManager = new EntityManager();
            _collisionSystem = new CollisionSystem(_entityManager);
        }

        public override void Update(float deltaTime)
        {
            _entityManager.Update(deltaTime);
            _collisionSystem.CheckCollision();

            if (!_entityManager.GetEntitiesOfType<TankEntity>().Any(t => t.IsPlayer && t.IsAlive))
            {
                _isDone = true;
            }
        }

        public override void Reset()
        {
            _isDone = false;
            _entityManager = new EntityManager();
        }
                
        public override bool IsDone()
        {
            return _isDone;
        }

        public bool CanMoveTo(int x, int y)
        {
            return _map.IsCellPassable(x, y);
        }

        public void DamageWall(int x, int y)
        {
            _map.DamageWall(x, y);
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            renderer.Clear();
            _mapRenderer.DrawMap(_map, renderer, 0, 0);
            _mapRenderer.DrawEntities(_entityManager.Entities, renderer, 0, 0);
        }
    }
}