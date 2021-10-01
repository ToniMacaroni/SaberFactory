using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using SaberFactory.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SaberFactory.UI.Lib.BSML
{
    [ComponentHandler(typeof(Backgroundable))]
    public class BackgroundableHandler : TypeHandler<Backgroundable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "border", new[] { "border" } },
            { "raycast", new[] { "raycast", "block" } },
            { "skew", new[] { "skew" } },
            { "custom_color", new[] { "custom_color" } },
            { "custom_bg", new[] { "custom_bg" } }
        };

        public override Dictionary<string, Action<Backgroundable, string>> Setters => new Dictionary<string, Action<Backgroundable, string>>();
        private readonly Sprite _borderSprite;
        private Material _bgMaterial;
        private Sprite _bgSprite;
        private GameObject _borderTemplate;

        private ImageView _imageViewPrefab;

        public BackgroundableHandler()
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(Utilities.GetResource(Assembly.GetExecutingAssembly(),
                "SaberFactory.Resources.UI.border.png"));
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            _borderSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 32f), 100, 1, SpriteMeshType.FullRect,
                new Vector4(0, 7, 7, 0));
        }

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleType(componentType, parserParams);
            var backgroundable = (Backgroundable)componentType.component;

            if (componentType.data.TryGetValue("custom_bg", out var customBg))
            {
                InitSprite();
                var imageview = GetOrAddImageView(backgroundable);
                if (imageview != null)
                {
                    imageview.overrideSprite = _bgSprite;
                    backgroundable.background = imageview;
                }
            }

            if (componentType.data.TryGetValue("custom_color", out var customColor)) TrySetBackgroundColor(backgroundable, customColor);

            if (componentType.data.TryGetValue("border", out var borderAttr)) AddBorder(backgroundable.gameObject, borderAttr == "square");

            if (componentType.data.TryGetValue("raycast", out var raycastAttr))
                if (backgroundable.background != null)
                    backgroundable.background.raycastTarget = bool.Parse(raycastAttr);

            if (componentType.data.TryGetValue("skew", out var skew))
                if (backgroundable.background is ImageView imageView)
                    imageView.SetSkew(float.Parse(skew));
        }

        private void AddBorder(GameObject go, bool squareSprite = false)
        {
            if (_borderTemplate == null)
            {
                var button = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(x => x.name == "ActionButton");
                var borderTransform = button?.transform.Find("Border");
                if (borderTransform is null) return;
                _borderTemplate = borderTransform.gameObject;
            }

            var borderGo = Object.Instantiate(_borderTemplate, go.transform).GetRect();

            borderGo.transform.SetParent(go.transform, false);

            if (go.GetComponent<HorizontalOrVerticalLayoutGroup>() is { })
            {
                var layout = borderGo.gameObject.AddComponent<LayoutElement>();
                layout.ignoreLayout = true;
            }

            borderGo.anchorMin = Vector2.zero;
            borderGo.anchorMax = Vector2.one;
            borderGo.anchoredPosition = Vector2.zero;
            borderGo.sizeDelta = Vector2.zero;

            var image = borderGo.GetComponent<ImageView>();
            image.SetSkew(0);

            if (squareSprite)
            {
                borderGo.anchorMin = new Vector2(0.015f, -0.01f);
                borderGo.anchorMax = new Vector2(1.006f, 0.97f);

                InitSprite();
                image.sprite = _borderSprite;
                image.overrideSprite = _borderSprite;
                image.material = _bgMaterial;
                //var color = ThemeManager.GetDefinedColor("primary", Color.black);
                var color = image.color;
                color.a = 0.8f;
                image.color = color;
            }
        }

        private void InitSprite()
        {
            if (_bgSprite != null) return;
            var image = Resources.FindObjectsOfTypeAll<GameObject>()
                .FirstOrDefault(x => x.name == "MiddleHorizontalTextSegmentedControlCell")?
                .transform.Find("BG")?
                .GetComponent<ImageView>();
            if (image == null)
            {
                Debug.LogError("Couldn't find background image prefab");
                return;
            }

            _bgSprite = image.sprite;
            _bgMaterial = image.material;
        }

        private ImageView GetOrAddImageView(Backgroundable backgroundable)
        {
            var imageView = backgroundable.GetComponent<ImageView>();
            if (imageView != null) return imageView;
            if (_imageViewPrefab == null)
            {
                _imageViewPrefab = Resources.FindObjectsOfTypeAll<ImageView>().First(x =>
                    x.gameObject?.name == "KeyboardWrapper" && x.sprite?.name == "RoundRect10" &&
                    x.transform.parent?.name == "Wrapper");
                if (_imageViewPrefab == null) return null;
            }

            return backgroundable.gameObject.AddComponent(_imageViewPrefab);
        }

        public static void TrySetBackgroundColor(Backgroundable background, string colorStr)
        {
            if (!ThemeManager.GetColor(colorStr, out var color)) return;
            background.background.color = color;
        }
    }
}