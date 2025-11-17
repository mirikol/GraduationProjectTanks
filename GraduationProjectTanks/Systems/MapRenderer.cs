using GraduationProjectTanks.Gameplay.Entities;

namespace GraduationProjectTanks.Systems
{
    public class MapRenderer
    {
        private const char BrickChar = '█';
        private const char WaterChar = '█';
        private const char EmptyChar = ' ';
        private const char DamagedBrickChar = '▒';

        private const char TankUpChar = 'U';
        private const char TankDownChar = 'D';
        private const char TankLeftChar = 'L';
        private const char TankRightChar = 'R';
        private const char ProjectileChar = 'o';

        public void DrawEntities(IEnumerable<IEntity> entities, ConsoleRenderer renderer, int offsetX = 0, int offsetY = 0)
        {
            foreach (var entity in entities.Where(e => e.IsAlive))
            {
                DrawEntity(entity, renderer, offsetX, offsetY);
            }
        }

        private void DrawEntity(IEntity entity, ConsoleRenderer renderer, int offsetX, int offsetY)
        {
            int screenX = offsetX + entity.X * Map.CellSizeX + Map.CellSizeX / 2;
            int screenY = offsetY + entity.Y * Map.CellSizeY + Map.CellSizeY / 2;

            if (screenX >= renderer.Width || screenY >= renderer.Height || screenX < 0 || screenY < 0)
                return;

            char symbol = GetEntitySymbol(entity);
            ConsoleColor color = GetEntityColor(entity);

            renderer.SetPixel(screenX, screenY, symbol, renderer.GetColorIndex(color));
        }

        private char GetEntitySymbol(IEntity entity)
        {
            return entity switch
            {
                TankEntity tank => GetTankSymbol(tank),
                ProjectileEntity => ProjectileChar,
                _ => '?'
            };
        }

        private char GetTankSymbol(TankEntity tank)
        {
            return tank.Direction switch
            {
                Direction.Up => TankUpChar,
                Direction.Down => TankDownChar,
                Direction.Left => TankLeftChar,
                Direction.Right => TankRightChar,
                _ => TankUpChar
            };
        }

        private ConsoleColor GetEntityColor(IEntity entity)
        {
            return entity switch
            {
                TankEntity tank => tank.IsPlayer ? ConsoleColor.Green : ConsoleColor.Red,
                ProjectileEntity => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };
        }

        public void DrawMap(Map map, ConsoleRenderer renderer, int offsetX = 0, int offsetY = 0)
        {
            int maxX = Math.Min(map.Width, (renderer.Width - offsetX) / Map.CellSizeX);
            int maxY = Math.Min(map.Height, (renderer.Height - offsetY) / Map.CellSizeY);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    DrawCell(map, x, y, renderer, offsetX, offsetY);
                }
            }
        }

        private void DrawCell(Map map, int x, int y, ConsoleRenderer renderer, int offsetX, int offsetY)
        {
            int screenX = offsetX + x * Map.CellSizeX;
            int screenY = offsetY + y * Map.CellSizeY;

            if (screenX >= renderer.Width || screenY >= renderer.Height)
                return;

            char symbol = GetCellSymbol(map, x, y);
            ConsoleColor color = GetCellColor(map, x, y);

            for (int dx = 0; dx < Map.CellSizeX; dx++)
            {
                for (int dy = 0; dy < Map.CellSizeY; dy++)
                {
                    if (screenX + dx < renderer.Width && screenY + dy < renderer.Height)
                    {
                        renderer.SetPixel(screenX + dx, screenY + dy, symbol, GetColorIndex(color, renderer));
                    }
                }
            }
        }

        private char GetCellSymbol(Map map, int x, int y)
        {
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
                return EmptyChar;

            if (map.Cells[x, y] == CellType.Brick && map.WallStates[x, y] == WallState.Destroyed)
                return EmptyChar;

            return map.Cells[x, y] switch
            {
                CellType.Brick => map.WallStates[x, y] == WallState.Damaged
                ? DamagedBrickChar
                : BrickChar,
                CellType.Water => WaterChar,
                CellType.Empty => EmptyChar,
                _ => EmptyChar
            };
        }

        private ConsoleColor GetCellColor(Map map, int x, int y)
        {
            if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
                return ConsoleColor.Black;

            return map.Cells[x, y] switch
            {
                CellType.Brick => map.WallStates[x, y] == WallState.Damaged
                ? ConsoleColor.Red
                : ConsoleColor.DarkRed,
                CellType.Water => ConsoleColor.Blue,
                CellType.Empty => ConsoleColor.DarkGray,
                _ => ConsoleColor.DarkGray
            };
        }

        private byte GetColorIndex(ConsoleColor color, ConsoleRenderer renderer)
        {
            return renderer.GetColorIndex(color);
        }
    }
}