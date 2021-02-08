using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using IPA.Utilities.Async;
using SaberFactory.Loaders;
using SaberFactory.UI;
using UnityEngine;

namespace SaberFactory.Models
{
    internal class PreloadMetaData : ICustomListItem
    {
        public readonly AssetMetaPath AssetMetaPath;
        public AssetTypeDefinition AssetTypeDefinition { get; private set; }

        private string _name;
        private string _author;

        private byte[] _coverData;
        private Texture2D _coverTex;
        private Sprite _coverSprite;

        public Texture2D CoverTex => _coverTex ??= LoadTexture();
        public Sprite CoverSprite => _coverSprite ??= LoadSprite();

        public PreloadMetaData(AssetMetaPath assetMetaPath)
        {
            AssetMetaPath = assetMetaPath;
        }

        public PreloadMetaData(AssetMetaPath assetMetaPath, ICustomListItem customListItem, AssetTypeDefinition assetTypeDefinition)
        {
            AssetMetaPath = assetMetaPath;
            AssetTypeDefinition = assetTypeDefinition;
            _name = customListItem.ListName;
            _author = customListItem.ListAuthor;
            _coverSprite = customListItem.ListCover;
        }

        public async Task SaveToFile()
        {
            await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
            {
                if (AssetMetaPath.HasMetaData)
                {
                    File.Delete(AssetMetaPath.MetaDataPath);
                }

                var ser = new SerializableMeta();
                ser.Name = _name;
                ser.Author = _author;
                ser.AssetTypeDefinition = AssetTypeDefinition;

                if (_coverSprite != null)
                {
                    var tex = _coverSprite.texture;
                    ser.CoverData = GetTextureData(tex);
                }

                var fs = new FileStream(AssetMetaPath.MetaDataPath, FileMode.Create, FileAccess.Write, FileShare.Write);
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, ser);
                fs.Close();

            });

        }

        public async Task LoadFromFile()
        {
            await LoadFromFile(AssetMetaPath.MetaDataPath);
        }

        public async Task LoadFromFile(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var formatter = new BinaryFormatter();
            var ser = (SerializableMeta)formatter.Deserialize(fs);
            fs.Close();

            await UnityMainThreadTaskScheduler.Factory.StartNew(() =>
            {
                _name = ser.Name;
                _author = ser.Author;
                _coverData = ser.CoverData;
                AssetTypeDefinition = ser.AssetTypeDefinition;

                LoadSprite();
            });
        }

        public void SetFavorite(bool isFavorite)
        {
            IsFavorite = isFavorite;
        }

        /// <summary>
        /// Get Texture png data from non-readable texture
        /// </summary>
        /// <param name="tex">The texture to read from</param>
        /// <returns>png bytes</returns>
        private byte[] GetTextureData(Texture2D tex)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                tex.width,
                tex.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Default);

            Graphics.Blit(tex, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(tex.width, tex.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D.EncodeToPNG();
        }

        private Texture2D LoadTexture()
        {
            return _coverData==null?null:Utilities.LoadTextureRaw(_coverData);
        }

        private Sprite LoadSprite()
        {
            return CoverTex==null?null:Utilities.LoadSpriteFromTexture(CoverTex);
        }

        public string ListName => _name;

        public string ListAuthor => _author;

        public Sprite ListCover => CoverSprite;

        public bool IsFavorite { get; set; }

        [Serializable]
        internal class SerializableMeta
        {
            public string Name;
            public string Author;
            public byte[] CoverData;
            public AssetTypeDefinition AssetTypeDefinition;
        }
    }
}