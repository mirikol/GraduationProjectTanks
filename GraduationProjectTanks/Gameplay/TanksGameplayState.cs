using GraduationProjectTanks.Systems;

namespace GraduationProjectTanks.Gameplay
{
    public class TanksGameplayState : BaseGameState
    {
        private Map _map;
        private MapRenderer _mapRenderer;
        private bool _isDone;
        private ConsoleRenderer _renderer;

        public TanksGameplayState(int width, int heidht, int seed, ConsoleRenderer renderer)
        {
            _map = new Map(width, heidht, seed);
            _mapRenderer = new MapRenderer();
            _renderer = renderer;
        }

        public override void Update(float deltaTime)
        {
        }

        public override void Reset()
        {
            _isDone = false;
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

        public Map GetMap()
        {
            return _map;
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            renderer.Clear();
            _mapRenderer.DrawMap(_map, renderer, 0, 0);
        }
    }
}