namespace GraduationProjectTanks.Gameplay
{
    public class CollisionSystem
    {
        private readonly EntityManager _entityManager;

        public CollisionSystem(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public void CheckCollision()
        {
            var entities = _entityManager.Entities.Where(e => e.IsAlive).ToList();

            for (int i = 0; i < entities.Count; i++)
            {
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
        private bool AreColliding(IEntity a, IEntity b)
        {
            float distance = MathF.Sqrt(MathF.Pow(a.X - b.X, 2) + MathF.Pow(a.Y - b.Y, 2));
            return distance < 1.0f;
        }
        
        private void HandleCollision(IEntity a, IEntity b)
        {
            Console.WriteLine($"Коллизия: {a.GetType().Name} {a.Id} ↔ {b.GetType().Name} {b.Id}");

            a.OnCollision(b);
            b.OnCollision(a);
        }
    }
}