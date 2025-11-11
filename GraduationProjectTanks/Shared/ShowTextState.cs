namespace GraduationProjectTanks.Shared
{
    internal class ShowTextState : BaseGameState
    {
        public string Text { get; set; }

        private float _duration = 1f;
        private float _timeLeft = 0f;

        public ShowTextState(float duration) : this(string.Empty, duration) { }

        public ShowTextState(string text, float duration)
        {
            Text = text;
            _duration = duration;

            Reset();
        }

        public override void Draw(ConsoleRenderer renderer)
        {
            var textHalfLength = Text.Length / 2;
            var textY = renderer.Height / 2;
            var textX = renderer.Width / 2 - textHalfLength;
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