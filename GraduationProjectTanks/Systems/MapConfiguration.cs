namespace GraduationProjectTanks.Systems
{
    public class MapConfiguration
    {
        public int Width { get; set; } = 15;
        public int Height { get; set; } = 15;
        public int CellSizeX { get; set; } = 4;
        public int CellSizeY { get; set; } = 2;
        public int MaxWaterSources { get; set; } = 3;
        public int MaxWaterAmount { get; set; } = 10;

        public bool GenerateMaze { get; set; } = true;
        public bool GenerateWater { get; set; } = true;

        public int Seed { get; set; } = 1;

        public static MapConfiguration CreateDefault()
        {
            return new MapConfiguration()
            {
                Width = 15,
                Height = 15,
                CellSizeX = 4,
                CellSizeY = 2,
                MaxWaterSources = 3,
                MaxWaterAmount = 10,
                GenerateMaze = true,
                GenerateWater = true,
                Seed = 1
            };
        }
    }
}