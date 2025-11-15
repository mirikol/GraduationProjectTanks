namespace GraduationProjectTanks.Shared
{
    public interface IRenderer
    {
        void SetPixel(int x, int y, char symbol, ConsoleColor color);
        void Render();
        void Clear();
        int Width {  get; }
        int Height { get; }
    }
}