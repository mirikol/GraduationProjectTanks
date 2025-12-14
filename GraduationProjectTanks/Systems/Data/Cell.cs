namespace GraduationProjectTanks.Systems.Data
{
    public struct Cell
    {
        public int X;
        public int Y;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Cell Sum(Cell a, Cell b) => new(a.X + b.X, a.Y + b.Y);
        public static Cell Sub(Cell a, Cell b) => new(a.X - b.X, a.Y - b.Y);

        public bool IsInBounds(int minX, int minY, int maxX, int maxY)
        {
            return X >= minX && X < maxX && Y >= minY && Y < maxY;
        }

        public override bool Equals(object? obj)
        {
            return obj is Cell cell && X == cell.X && Y == cell.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !left.Equals(right);
        }
    }
}