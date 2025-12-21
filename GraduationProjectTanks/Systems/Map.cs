using GraduationProjectTanks.Systems.Data;

namespace GraduationProjectTanks.Systems
{
    public class Map
    {
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

        private readonly MapConfiguration _mapConfig;
        private Random _random;

        public int CellSizeX => _mapConfig.CellSizeX;
        public int CellSizeY => _mapConfig.CellSizeY;

        public Map(MapConfiguration mapConfig)
        {
            _mapConfig = mapConfig;
            Width = mapConfig.Width;
            Height = mapConfig.Height;

            Cells = new CellType[Width, Height];
            WallStates = new WallState[Width, Height];
            _random = new Random(mapConfig.Seed);

            GenerateWalls();
        }

        private void GenerateWalls()
        {
            // Инициализация всех ячеек как пустых.
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Cells[x, y] = CellType.Empty;

            GeneratePerimeterWall();

            if (_mapConfig.GenerateMaze)
                GenerateMaze();

            if (_mapConfig.GenerateWater)
            GenerateWater();
        }

        private void GeneratePerimeterWall()
        {
            for (int x = 0; x < Width; x++)
            {
                Cells[x, 0] = CellType.Brick;
                Cells[x, Height -1] = CellType.Brick;
                WallStates[x, 0] = WallState.Intact;
                WallStates[x, Height -1] = WallState.Intact;
            }

            for (int y = 0; y < Height; y++)
            {
                Cells[0, y] = CellType.Brick;
                Cells[Width - 1, y] = CellType.Brick;
                WallStates[0, y] = WallState.Intact;
                WallStates[Width - 1, y] = WallState.Intact;
            }
        }

        private void GenerateMaze()
        {
            var visited = new bool[Width, Height];
            var stack = new Stack<Cell>();

            var start = new Cell(_random.Next(1, Width -1), _random.Next(1, Height - 1));

            if (start.X % 2 != 0) start.X--;
            if (start.Y % 2 != 0) start.Y--;

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
                    
                        if (wall.IsInBounds(1, 1, Width -1, Height - 1))
                        {
                            Cells[wall.X, wall.Y] = CellType.Brick;
                            WallStates[wall.X, wall.Y] = WallState.Intact;
                        }

                        visited[neighbor.X, neighbor.Y] = true;
                        stack.Push(neighbor);
                    }
                }
            }
        }

        private List<Cell> GetUnvisitedNeighbors(Cell cell, bool[,] visited)
        {
            var neighbors = new List<Cell>();

            foreach (var shift in NeighbourCellShifts)
            {
                var neighbor = Cell.Sum(cell, new Cell(shift.X * 2, shift.Y * 2));
                
                if (neighbor.IsInBounds(1, 1, Width - 1, Height - 1) && !visited[neighbor.X, neighbor.Y])
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

        private void GenerateWater()
        {
            var emptyCells = new List<Cell>();

            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    if (Cells[x, y] == CellType.Empty)
                        emptyCells.Add(new Cell(x, y));
                }
            }

            if (emptyCells.Count == 0)
                return;

            int waterSources = Math.Min(_random.Next(1, _mapConfig.MaxWaterSources + 1), emptyCells.Count);

            Shuffle(emptyCells);

            for (int i = 0; i < waterSources; i++)
            {
                var cell = emptyCells[i];
                CreateWaterArea(cell.X, cell.Y);
            }
        }

        private void CreateWaterArea(int startX, int startY)
        {
            if (Cells[startX, startY] != CellType.Empty)
                return;

            var queue = new Queue<Cell>();
            var visited = new bool[Width, Height];
            int waterAmount = _random.Next(3, _mapConfig.MaxWaterAmount + 1);
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

                    if (neighbor.IsInBounds(1, 1, Width - 1, Height - 1) && !visited[neighbor.X, neighbor.Y] && _random.NextDouble() > 0.3)
                    {
                        visited[neighbor.X, neighbor.Y] = true;
                        queue.Enqueue(neighbor);
                    }
                }

                if (created >= waterAmount)
                    break;
            }    
        }

        public bool IsCellPassable(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;

            return Cells[x, y] == CellType.Empty;
        }

        public bool IsCellPassableForProjectile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return false;

            return Cells[x, y] == CellType.Empty || Cells[x, y] == CellType.Water;
        }

        public void DamageWall(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

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