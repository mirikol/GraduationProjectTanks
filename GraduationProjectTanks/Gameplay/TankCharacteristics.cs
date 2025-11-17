namespace GraduationProjectTanks.Gameplay
{
    public class TankCharacteristics
    {
        public int MaxHealth { get; set; }
        public float MoveDelay { get; set; }
        public float ShootDelay { get; set; }
        public float ProjectileSpeed { get; set; }
        public int Damage { get; set; }

        public static TankCharacteristics CreatePlayerCharacteristics()
        {
            return new TankCharacteristics()
            {
                MaxHealth = 3,
                MoveDelay = 0.1f,
                ShootDelay = 0.3f,
                ProjectileSpeed = 6.0f,
                Damage = 1
            };
        }

        public static TankCharacteristics CreateEnemyCharacteristics(Random random)
        {
            return new TankCharacteristics()
            {
                MaxHealth = random.Next(1, 4),
                MoveDelay = random.Next(2, 6) * 0.1f,
                ShootDelay = random.Next(3, 8) * 0.1f,
                ProjectileSpeed = random.Next(4, 7),
                Damage = 1
            };
        }

        public static TankCharacteristics CreateDefault()
        {
            return new TankCharacteristics()
            {
                MaxHealth = 3,
                MoveDelay = 0.2f,
                ShootDelay = 0.5f,
                ProjectileSpeed = 5.0f,
                Damage = 1
            };
        }
    }
}