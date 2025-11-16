namespace GraduationProjectTanks.Gameplay
{
    public interface IEntity
    {
        public string Id { get; }
        public int X { get; }
        public int Y { get; }
        public int Health { get; }
        public bool IsAlive { get; }
        public bool IsSolid {  get; }
        public bool CanTakeDamage { get; }

        public void TakeDamage(int  damage);
        public void Update(float deltaTime);
        public void Draw();
        public void OnCollision(IEntity other);
    }

    public abstract class BaseEntity : IEntity
    {
        private static int _nextId = 1;

        public string Id { get; }
        public int X { get; protected set; }

        public int Y { get; protected set; }

        public int Health { get; protected set; }

        public bool IsAlive => Health > 0;

        public abstract bool IsSolid { get; }

        public abstract bool CanTakeDamage { get;}

        protected BaseEntity(int x, int y, int health)
        {
            Id = $"Entity_{_nextId++}";
            X = x;
            Y = y;
            Health = health;
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public abstract void Update(float deltaTime);

        public abstract void Draw();

        public abstract  void OnCollision(IEntity other);

        protected bool CheckCollision(IEntity other, float tolerance = 1.0f)
        {
            float dx = Math.Abs(X - other.X);
            float dy = Math.Abs(Y - other.Y);
            return dx < tolerance && dy < tolerance;
        }
    }
}