namespace SaberFactory.Models
{
    internal class SaberModel
    {
        public readonly SaberSection Blade;
        public readonly SaberSection Emitter;
        public readonly SaberSection Handle;
        public readonly SaberSection Pommel;

        private SaberModel()
        {
            Blade = new SaberSection();
            Emitter = new SaberSection();
            Handle = new SaberSection();
            Pommel = new SaberSection();
        }

        internal struct SaberSection
        {
            public BasePieceModel Model;
            public BasePieceModel Halo;
        }
    }
}