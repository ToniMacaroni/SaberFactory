using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BeatSaberMarkupLanguage;
using SaberFactory.Loaders;
using SaberFactory.UI;
using UnityEngine;

namespace SaberFactory.Models
{
    internal class PreloadMetaData : ICustomListItem
    {
        public readonly AssetMetaPath AssetMetaPath;

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
            LoadFromFile(assetMetaPath.MetaDataPath);
        }

        public PreloadMetaData(AssetMetaPath assetMetaPath, ICustomListItem customListItem)
        {
            AssetMetaPath = assetMetaPath;
            _name = customListItem.ListName;
            _author = customListItem.ListAuthor;
            _coverSprite = customListItem.ListCover;
        }

        public void SaveToFile()
        {
            if (AssetMetaPath.HasMetaData)
            {
                File.Delete(AssetMetaPath.MetaDataPath);
            }

            var ser = new SerializableMeta();
            ser.Name = _name;
            ser.Author = _author;

            if (_coverSprite != null && _coverSprite.texture.isReadable)
            {
                ser.CoverData = _coverSprite?.texture.EncodeToPNG();
            }


            var fs = new FileStream(AssetMetaPath.MetaDataPath, FileMode.Create, FileAccess.Write, FileShare.Write);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, ser);
            fs.Close();
        }

        private void LoadFromFile(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var formatter = new BinaryFormatter();
            var ser = (SerializableMeta)formatter.Deserialize(fs);
            fs.Close();

            _name = ser.Name;
            _author = ser.Author;
            _coverData = ser.CoverData;
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
        }
    }
}