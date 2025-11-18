namespace GraduationProjectTanks.Systems.Renderer
{
    public class ConsoleRenderer : IRenderer
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private const int MaxColors = 16;
        private readonly ConsoleColor[] _colors;
        private readonly char[,] _pixels;
        private readonly byte[,] _pixelColors;
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public ConsoleColor BgColor { get; set; }

        public char this[int w, int h]
        {
            get { return _pixels[w, h]; }
            set { _pixels[w, h] = value; }
        }

        public ConsoleRenderer(ConsoleColor[] colors)
        {
            if (colors == null || colors.Length == 0)
            {
                colors = CreateDefaultPalette();
            }

            if (colors.Length > MaxColors)
            {
                var tmp = new ConsoleColor[MaxColors];
                Array.Copy(colors, tmp, tmp.Length);
                colors = tmp;
            }

            _colors = colors;

            try
            {
                _maxWidth = Console.LargestWindowWidth;
                _maxHeight = Console.LargestWindowHeight;
                Width = Console.WindowWidth;
                Height = Console.WindowHeight;
            }
            catch
            {
                _maxWidth = 80;
                _maxHeight = 24;
                Width = 80;
                Height = 24;
            }

            _pixels = new char[_maxWidth, _maxHeight];
            _pixelColors = new byte[_maxWidth, _maxHeight];

            Console.CursorVisible = false;
            BgColor = ConsoleColor.Black;
        }

        public ConsoleRenderer() : this(CreateDefaultPalette())
        {
        }

        private static ConsoleColor[] CreateDefaultPalette()
        {
            return new ConsoleColor[]
            {
                ConsoleColor.Black,
                ConsoleColor.Red,
                ConsoleColor.DarkRed,
                ConsoleColor.Blue,
                ConsoleColor.White,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Cyan,
                ConsoleColor.Magenta,
                ConsoleColor.Gray,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkCyan,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkGray
            };
        }

        public void SetPixel(int w, int h, char val, byte colorIdx)
        {
            if (w >= 0 && w < Width && h >= 0 && h < Height)
            {
                _pixels[w, h] = val;
                if (colorIdx < _colors.Length)
                {
                    _pixelColors[w, h] = colorIdx;
                }
            }
        }

        public void SetPixel(int x, int y, char symbol, ConsoleColor color)
        {
            byte colorIdx = GetColorIndex(color);
            SetPixel(x, y, symbol, colorIdx);
        }

        public byte GetColorIndex(ConsoleColor color)
        {
            for (byte i = 0; i < _colors.Length; i++)
            {
                if (_colors[i] == color)
                    return i;
            }
            return 0;
        }

        public void Render()
        {
            Console.BackgroundColor = BgColor;

            for (var h = 0; h < Height; h++)
            {
                for (var w = 0; w < Width; w++)
                {
                    var colorIdx = _pixelColors[w, h];
                    if (colorIdx >= _colors.Length)
                        continue;

                    var color = _colors[colorIdx];
                    var symbol = _pixels[w, h];

                    if (symbol == 0 || symbol == ' ' && color == BgColor)
                    {
                        continue;
                    }

                    Console.ForegroundColor = color;
                    Console.SetCursorPosition(w, h);
                    Console.Write(symbol);
                }
            }
            Console.ResetColor();
        }

        public void DrawString(string text, int atWidth, int atHeight, ConsoleColor color)
        {
            var colorIdx = GetColorIndex(color);

            for (int i = 0; i < text.Length && atWidth + i < Width; i++)
            {
                if (atWidth + i >= 0 && atHeight >= 0 && atHeight < Height)
                {
                    _pixels[atWidth + i, atHeight] = text[i];
                    _pixelColors[atWidth + i, atHeight] = colorIdx;
                }
            }
        }

        public void DrawString(string text, int atWidth, int atHeight, byte colorIdx)
        {
            if (colorIdx >= _colors.Length)
                colorIdx = 0;

            for (int i = 0; i < text.Length && atWidth + i < Width; i++)
            {
                if (atWidth + i >= 0 && atHeight >= 0 && atHeight < Height)
                {
                    _pixels[atWidth + i, atHeight] = text[i];
                    _pixelColors[atWidth + i, atHeight] = colorIdx;
                }
            }
        }

        public void Clear()
        {
            for (int w = 0; w < Width; w++)
                for (int h = 0; h < Height; h++)
                {
                    _pixelColors[w, h] = 0;
                    _pixels[w, h] = (char)0;
                }
        }

        public void Clear(int x, int y, int width, int height)
        {
            int endX = Math.Min(x + width, Width);
            int endY = Math.Min(y + height, Height);

            for (int w = Math.Max(x, 0); w < endX; w++)
            {
                for (int h = Math.Max(y, 0); h < endY; h++)
                {
                    _pixelColors[w, h] = 0;
                    _pixels[w, h] = (char)0;
                }
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ConsoleRenderer casted)
                return false;

            if (_maxWidth != casted._maxWidth || _maxHeight != casted._maxHeight ||
                Width != casted.Width || Height != casted.Height ||
                _colors.Length != casted._colors.Length)
            {
                return false;
            }

            for (int i = 0; i < _colors.Length; i++)
            {
                if (_colors[i] != casted._colors[i])
                    return false;
            }

            for (int w = 0; w < Width; w++)
                for (var h = 0; h < Height; h++)
                {
                    if (_pixels[w, h] != casted._pixels[w, h] || _pixelColors[w, h] != casted._pixelColors[w, h])
                    {
                        return false;
                    }
                }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = HashCode.Combine(_maxWidth, _maxHeight, Width, Height);

            for (int i = 0; i < _colors.Length; i++)
            {
                hash = HashCode.Combine(hash, _colors[i]);
            }

            for (int w = 0; w < Width; w++)
                for (var h = 0; h < Height; h++)
                {
                    hash = HashCode.Combine(hash, _pixelColors[w, h], _pixels[w, h]);
                }

            return hash;
        }
    }
}