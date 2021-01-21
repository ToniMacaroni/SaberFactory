using System.Threading.Tasks;
using SaberFactory.Saving;
using Zenject;

namespace SaberFactory.Models
{
    internal class SaberSet
    {
        private readonly SaveManager _saveManager;
        public SaberModel LeftSaber { get; set; }
        public SaberModel RightSaber { get; set; }

        public Task CurrentLoadingTask { get; private set; }

        private SaberSet(
            [Inject(Id = "LeftSaberModel")] SaberModel leftSaber,
            [Inject(Id = "RightSaberModel")] SaberModel rightSaber,
            SaveManager saveManager)
        {
            _saveManager = saveManager;
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
            _saveManager.SaveSaber(this, fileName);
        }

        public async Task Load(string fileName)
        {
            CurrentLoadingTask = _saveManager.LoadSaber(this, fileName);
            await CurrentLoadingTask;
        }

        public void Sync(SaberModel fromModel)
        {
            fromModel.Sync();
            var otherSaber = fromModel == LeftSaber ? RightSaber : LeftSaber;
            otherSaber.SaberWidth = fromModel.SaberWidth;
        }
    }
}