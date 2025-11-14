using GraduationProjectTanks.Tanks;
using static GraduationProjectTanks.Shared.Cell;

namespace GraduationProjectTanks.Shared
{
    public enum CellType
    {
        Empty,
        Brick,
        Water
    }
    
    public enum WallState
    {
        Destroyed,
        Damaged,
        Intact
    }

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

        public override bool Equals(object obj)
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

    public class Map
    {
        public const int CellSize = 3;
        public const int MaxWaterSources = 3;
        public const int MaxWaterAmount = 10;

        private static readonly Cell[] NeighbourCellShifts = new Cell[]
        {
            new(0, -1),
            new(1, 0),
            new(0, 1),
            new(-1, 0)
        };

        public int Width { get; }
        public int Height { get; }
        public CellType[,] Cells { get; }
        public WallState[,] WallStates { get; }
        private Random _random;

        public Map(int width, int height, int seed)
        {
            Width = width;
            Height = height;
            Cells = new CellType[width, height];
            WallStates = new WallState[width, height];
            _random = new Random(seed);

            GenerateWalls();
        }

        private void GenerateWalls()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Cells[x, y] = CellType.Empty;

            GenerateMaze();

            GenerateWater();
        }

        private void GenerateMaze()
        {
            var visited = new bool[Width, Height];
            var stack = new Stack<Cell>();
            var start = new Cell(Width / 2, Height / 2);
            stack.Push(start);
            visited[start.X, start.Y] = true;

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var neighbors = GetUnvisitedNeighbors(current, visited);
                Shuffle(neighbors);

                foreach (var neighbor in neighbors)
                {
                    if (!visited[neighbor.X, neighbor.Y])
                    {
                        var wall = new Cell((current.X + neighbor.X) / 2, (current.Y + neighbor.Y) / 2);
                    
                        if (wall.IsInBounds(0, 0, Width, Height))
                        {
                            Cells[wall.X, wall.Y] = CellType.Brick;
                            WallStates[wall.X, wall.Y] = WallState.Intact;
                        }

                        visited[neighbor.X, neighbor.Y] = true;
                        stack.Push(neighbor);
                    }
                }
            }

            AddRandomWalls();
        }

        private List<Cell> GetUnvisitedNeighbors(Cell cell, bool[,] visited)
        {
            var neighbors = new List<Cell>();

            foreach (var shift in NeighbourCellShifts)
            {
                var neighbor = Cell.Sum(cell, new Cell(shift.X * 2, shift.Y * 2));
                
                if (neighbor.IsInBounds(0, 0, Width, Height) && !visited[neighbor.X, neighbor.Y])
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void AddRandomWalls()
        {
            for (int i = 0; i < Width * Height / 4; i++)
            {
                var x = _random.Next(Width);
                var y = _random.Next(Height);

                if (Cells[x, y] == CellType.Empty)
                {
                    Cells[x, y] = CellType.Brick;
                    WallStates[x, y] = WallState.Intact;
                }
            }
        }

        private void GenerateWater()
        {
            int waterSources = _random.Next(1, MaxWaterSources + 1);

            for (int i = 0; i < waterSources; i++)
            {
                var startX = _random.Next(Width);
                var startY = _random.Next(Height);

                CreateWaterArea(startX, startY);
            }
        }

        private void CreateWaterArea(int startX, int startY)
        {
            var queue = new Queue<Cell>();
            var visited = new bool[Width, Height];
            int waterAmount = _random.Next(3, MaxWaterAmount + 1);
            int created = 0;

            queue.Enqueue(new Cell(startX, startY));
            visited[startX, startY] = true;

            while (queue.Count > 0 && created < waterAmount)
            {
                var current = queue.Dequeue();

                if (Cells[current.X, current.Y] == CellType.Empty)
                {
                    Cells[current.X, current.Y] = CellType.Water;
                    created++;
                }

                foreach (var shift in NeighbourCellShifts)
                {
                    var neighbor = Cell.Sum(current, shift);

                    if (neighbor.IsInBounds(0, 0, Width, Height) && !visited[neighbor.X, neighbor.Y] && _random.NextDouble() > 0.3)
                    {
                        visited[neighbor.X, neighbor.Y] = true;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        public bool IsCellPassable(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;

            return Cells[x, y] == CellType.Empty || Cells[x, y] == CellType.Water;
        }

        public void DamageWall(int x, int y)
        {
            if (Cells[x, y] == CellType.Brick)
            {
                switch (WallStates[x, y])
                {
                    case WallState.Intact:
                        WallStates[x, y] = WallState.Damaged;
                        break;
                    case WallState.Damaged:
                        Cells[x, y] = CellType.Empty;
                        WallStates[x, y] = WallState.Destroyed;
                        break;
                }
            }
        }
    }
}