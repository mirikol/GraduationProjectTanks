namespace GraduationProjectTanks.Gameplay.Entities
{
    public class EntityManager
    {
        private readonly Dictionary<string, IEntity> _entities = new();
        private readonly List<IEntity> _entitiesToAdd = new();
        private readonly List<string> _entitiesToRemove = new();

        public IReadOnlyCollection<IEntity> Entities => _entities.Values;

        public void AddEntity(IEntity entity)
        {
            if (entity == null)
                return;

            _entitiesToAdd.Add(entity);
        }

        public void RemoveEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId))
                return;

            _entitiesToRemove.Add(entityId);
        }

        public void RemoveEntity(IEntity entity)
        {
            if (entity == null)
                return;

            RemoveEntity(entity.Id);
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : IEntity
        {
            return _entities.Values.OfType<T>();
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in _entitiesToAdd)
            {
                if (entity != null && !string.IsNullOrEmpty(entity.Id))
                {
                    _entities[entity.Id] = entity;
                }
            }
            _entitiesToAdd.Clear();

            foreach (var entity in _entities.Values.ToList())
            {
                if (entity?.IsAlive == true)
                {
                    entity.Update(deltaTime);
                }
                else if (entity != null)
                {
                    RemoveEntity(entity.Id);
                }
            }

            foreach (var entityId in _entitiesToRemove)
            {
                if (!string.IsNullOrEmpty(entityId))
                {
                    _entities.Remove(entityId);
                }
            }
            _entitiesToRemove.Clear();
        }

        public void Draw()
        {
            foreach (var entity in _entities.Values)
            {
                if (entity?.IsAlive == true)
                {
                    entity.Draw();
                }
            }
        }
    }
}