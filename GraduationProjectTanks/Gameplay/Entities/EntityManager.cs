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
            _entitiesToAdd.Add(entity);
        }

        public void RemoveEntity(string entityId)
        {
            _entitiesToRemove.Add(entityId);
        }

        public void RemoveEntity(IEntity entity)
        {
            RemoveEntity(entity.Id);
        }

        public T GetEntity<T>(string id) where T : IEntity
        {
            if (_entities.TryGetValue(id, out var entity))
            {
                return (T)entity;
            }
            throw new KeyNotFoundException($"Сущность с Id '{id}' не найдена");
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : IEntity
        {
            return _entities.Values.OfType<T>();
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in _entitiesToAdd)
            {
                _entities[entity.Id] = entity;
                Console.WriteLine($"Добавлено сущность: {entity.GetType().Name} {entity.Id}");
            }
            _entitiesToAdd.Clear();

            foreach (var entity in _entities.Values.ToList())
            {
                if (entity.IsAlive)
                {
                    entity.Update(deltaTime);
                }
                else
                {
                    RemoveEntity(entity.Id);
                }
            }

            foreach (var entityId in _entitiesToRemove)
            {
                if (_entities.Remove(entityId, out var removedEntity))
                {
                    Console.WriteLine($"Удалена сущность: {removedEntity.GetType().Name} {entityId}");
                }
            }
            _entitiesToRemove.Clear();
        }

        public void Draw()
        {
            foreach (var entity in _entities.Values)
            {
                if (entity.IsAlive)
                {
                    entity.Draw();
                }
            }
        }
    }
}