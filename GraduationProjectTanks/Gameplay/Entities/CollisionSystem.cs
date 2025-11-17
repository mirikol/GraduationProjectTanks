namespace GraduationProjectTanks.Gameplay.Entities
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
            var entities = _entityManager.Entities.Where(e => e.IsAlive && e.IsSolid).ToList();

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
            return distance < 0.8f;
        }
        
        private void HandleCollision(IEntity a, IEntity b)
        {
            if (a is TankEntity && b is TankEntity)
            {
                return;
            }

            a.OnCollision(b);
            b.OnCollision(a);
        }
    }
}