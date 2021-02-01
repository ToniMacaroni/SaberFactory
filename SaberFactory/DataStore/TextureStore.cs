using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaberFactory.Helpers;

namespace SaberFactory.DataStore
{
    internal class TextureStore
    {
        public Task CurrentLoadingTask => _currentLoadingTask ?? Task.CompletedTask;
        public bool IsLoading { get; private set; }

        private readonly Dictionary<string, TextureAsset> _textureAssets;
        private readonly DirectoryInfo _textureDirectory;
        private Task _currentLoadingTask;

        private TextureStore(DirectoryInfo textureDirectory)
        {
            _textureAssets = new Dictionary<string, TextureAsset>();
            _textureDirectory = textureDirectory;
        }

        public Task<TextureAsset> this[string path] => GetTexture(path);

        public async Task<TextureAsset> GetTexture(string path)
        {
            return await LoadTexture(path);
        }

        public bool HasTexture(string path)
        {
            return _textureAssets.ContainsKey(path);
        }

        public IEnumerable<TextureAsset> GetAllTextures()
        {
            return _textureAssets.Values;
        }

        public async Task LoadAllTexturesAsync()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                _currentLoadingTask = LoadAllTexturesAsyncInternal();
            }

            await _currentLoadingTask;
            IsLoading = false;
        }

        public void UnloadAll()
        {
            // TODO: Check if valid
            _textureAssets.Clear();
        }

        private async Task<TextureAsset> LoadTexture(string path)
        {
            if (HasTexture(path)) return _textureAssets[path];

            var fullPath = PathTools.ToFullPath(path);
            if (!File.Exists(fullPath)) return null;

            var tex = await Readers.ReadTexture(path);
            if (!tex) return null;

            var texAsset = new TextureAsset(Path.GetFileName(path), path, tex, EAssetOrigin.FileSystem);

            _textureAssets.Add(texAsset.Path, texAsset);
            return texAsset;
        }

        private async Task LoadAllTexturesAsyncInternal()
        {
            foreach (var texFile in _textureDirectory.EnumerateFiles("*.png", SearchOption.AllDirectories))
            {
                await LoadTexture(texFile.FullName);
            }
        }
    }
}