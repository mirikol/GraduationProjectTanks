namespace GraduationProjectTanks.Gameplay.Entities
{
    public class EntityController
    {
        // Сущности храняться в словаре, для быстрого доступа по id. Гарантирует уникальность сущностей.
        private readonly Dictionary<string, IEntity> _entities = new();

        // Буферные списки. Исключают изменение коллекций во время итераций. Все сущности в одном кадре видят одинаковое состояние мира.
        private readonly List<IEntity> _entitiesToAdd = new();
        private readonly List<string> _entitiesToRemove = new();

        //Инкапсуляция доступа к коллекции сущностей.
        public IReadOnlyCollection<IEntity> Entities => _entities.Values;

        // Метод для добавление сущностей.
        public void AddEntity(IEntity entity)
        {
            if (entity == null)
                return;

            _entitiesToAdd.Add(entity);
        }

        // Перегрузка методов для удаления сущностей по id (строке) или удаление по объекту.
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

        // Дженерик-метод для фильтрации сущностей по типу (IEntity).
        public IEnumerable<T> GetEntitiesOfType<T>() where T : IEntity
        {
            return _entities.Values.OfType<T>();
        }

        public void Update(float deltaTime)
        {            
            foreach (var entity in _entitiesToAdd)
            {
                // Проверка гарантирует целостность данных.
                if (entity != null && !string.IsNullOrEmpty(entity.Id))
                {
                    // Добавление новых сущностей в основную коллекцию.
                    _entities[entity.Id] = entity;
                }
            }
            _entitiesToAdd.Clear();

            // Обновление существующих сущностей. Важно через - ToList() - основная коллекция не изменяется во время итерации.
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity?.IsAlive == true)
                {
                    // Основная игровая логика (движение, стрельба, AI).
                    entity.Update(deltaTime);
                }
                // Сущность мертва или уничтожена.
                else if (entity != null)
                {
                    // Помечаем на удаление.
                    RemoveEntity(entity.Id);
                }
            }

            // Удаление.
            foreach (var entityId in _entitiesToRemove)
            {
                if (!string.IsNullOrEmpty(entityId))
                {
                    // Фактическое удаление из коллекции.
                    _entities.Remove(entityId);
                }
            }
            _entitiesToRemove.Clear();
        }
    }
}