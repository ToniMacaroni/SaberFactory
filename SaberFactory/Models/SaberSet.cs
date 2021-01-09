using Zenject;

namespace SaberFactory.Models
{
    internal class SaberSet
    {
        public SaberModel LeftSaber { get; set; }
        public SaberModel RightSaber { get; set; }

        private SaberSet([Inject(Id = "LeftSaberModel")] SaberModel leftSaber, [Inject(Id = "RightSaberModel")] SaberModel rightSaber)
        {
            LeftSaber = leftSaber;
            RightSaber = rightSaber;
        }

        public void SetModelComposition(ModelComposition modelComposition)
        {
            LeftSaber.SetModelComposition(modelComposition);
            RightSaber.SetModelComposition(modelComposition);
        }
    }
}