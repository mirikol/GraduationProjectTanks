using GraduationProjectTanks.Gameplay;

namespace GraduationProjectTanks.Systems.Renderer
{
    internal class ShowTextState : BaseGameState
    {
        public string Text { get; set; }
        private float _duration = 1f;
        private float _timeLeft = 0f;
        private int _mapWidth;
        private int _mapHeight;

        public ShowTextState(float duration) : this(string.Empty, duration) { }

        public ShowTextState(string text, float duration, int mapWidth = 0, int mapHeight = 0)
        {
            Text = text;
            _duration = duration;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;

            Reset();
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            int mapCenterX = _mapWidth / 2;
            int mapCenterY = _mapHeight / 2;
            var textHalfLength = Text.Length / 2;
            var textX = mapCenterX - textHalfLength;
            var textY = mapCenterY;

            textX = Math.Max(0, Math.Min(textX, _mapWidth - Text.Length));
            textY = Math.Max(0, Math.Min(textY, _mapHeight - 1));

            for (int i = -1; i <= Text.Length + 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (textX + i >= 0 && textX + i < renderer.Width && textY + j >= 0 && textY + j < renderer.Height)
                    {
                        renderer.SetPixel(textX + i, textY + j, ' ', ConsoleColor.Black);
                    }
                }
            }

            renderer.DrawString(Text, textX, textY, ConsoleColor.White);
        }

        public override void Reset()
        {
            _timeLeft = _duration;
        }

        public override void Update(float deltaTime)
        {
            _timeLeft -= deltaTime;
        }

        public override bool IsDone()
        {
            return _timeLeft <= 0f;
        }
    }
}