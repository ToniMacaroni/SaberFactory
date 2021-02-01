using System.Threading.Tasks;
using SaberFactory.Saving;
using Zenject;

namespace SaberFactory.Models
{
    /// <summary>
    /// Stores currently used left and right saber model implementation
    /// </summary>
    internal class SaberSet
    {
        private readonly PresetSaveManager _presetSaveManager;
        public SaberModel LeftSaber { get; set; }
        public SaberModel RightSaber { get; set; }

        public Task CurrentLoadingTask { get; private set; }

        private SaberSet(
            [Inject(Id = ESaberSlot.Left)] SaberModel leftSaber,
            [Inject(Id = ESaberSlot.Right)] SaberModel rightSaber,
            PresetSaveManager presetSaveManager)
        {
            _presetSaveManager = presetSaveManager;
            LeftSaber = leftSaber;
            RightSaber = rightSaber;

            Load();
        }

        public void SetModelComposition(ModelComposition modelComposition)
        {
            LeftSaber.SetModelComposition(modelComposition);
            RightSaber.SetModelComposition(modelComposition);
        }

        public async void Load()
        {
            await Load("default");
        }

        public void Save()
        {
            Save("default");
        }

        public void Save(string fileName)
        {
            _presetSaveManager.SaveSaber(this, fileName);
        }

        public async Task Load(string fileName)
        {
            CurrentLoadingTask = _presetSaveManager.LoadSaber(this, fileName);
            await CurrentLoadingTask;
        }

        public void Sync(SaberModel fromModel)
        {
            fromModel.Sync();
            var otherSaber = fromModel == LeftSaber ? RightSaber : LeftSaber;
            otherSaber.SaberWidth = fromModel.SaberWidth;
        }

        public bool IsEmpty => LeftSaber.IsEmpty && RightSaber.IsEmpty;
    }
}