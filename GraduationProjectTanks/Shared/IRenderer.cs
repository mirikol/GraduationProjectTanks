namespace GraduationProjectTanks.Shared
{
    public interface IRenderer
    {
        public void SetPixel(int x, int y, char symbol, ConsoleColor color);
        public void Render();
        public void Clear();
        public int Width {  get; }
        public int Height { get; }
    }
}