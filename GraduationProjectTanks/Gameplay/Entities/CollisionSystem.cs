namespace GraduationProjectTanks.Gameplay.Entities
{
    public class CollisionSystem
    {
        // 0.8f для плавного столкновения с небольшим визуальным буфером между сущностями.
        private const float CollisionThreshold = 0.8f;
        private readonly EntityController _entityController;

        //Использует dependency injection через конструктор.
        public CollisionSystem(EntityController entityController)
        {
            _entityController = entityController;
        }

        public void CheckCollision()
        {
            //Фильтруем только те сущности которые могут сталкиваться и преобразуем их в список для безопасной итерации.
            var entities = _entityController.Entities.Where(e => e.IsAlive && e.IsSolid).ToList();

            // Двойной цикл проверки столкновений.
            for (int i = 0; i < entities.Count; i++)
            {
                // (j = i + 1) - избегает дублирующих проверок (А+Б и Б+А).
                for (int j = i + 1; j < entities.Count; j++)
                {
                    var entityA = entities[i];
                    var entityB = entities[j];

                    if (AreColliding(entityA, entityB))
                    {
                        HandleCollision(entityA, entityB);
                    }
                }
            }
        }

        // Метод обнаружения столкновений. Используется евклидово растояние между центрами сущностей.
        private bool AreColliding(IEntity a, IEntity b)
        {
            float distance = MathF.Sqrt(MathF.Pow(a.X - b.X, 2) + MathF.Pow(a.Y - b.Y, 2));            
            return distance < CollisionThreshold;
        }
        
        // Обработка столкновение. Танки не сталкиваются друг с другом.
        private void HandleCollision(IEntity a, IEntity b)
        {
            if (a is TankEntity && b is TankEntity)
            {
                return;
            }

            // OnCollision - каждая сущност сама решает как реагировать на столкновение.
            a.OnCollision(b);
            b.OnCollision(a);
        }
    }
}