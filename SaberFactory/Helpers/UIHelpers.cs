using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using SaberFactory.UI.Lib;
using UnityEngine;

namespace SaberFactory.Helpers
{
    internal static class UIHelpers
    {
        public static async Task<BSMLParserParams> ParseFromResourceAsync(string resource, GameObject parent, object host)
        {
            var data = await Readers.ReadResourceAsync(resource);
            var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
            return BSMLParser.instance.Parse(content, parent, host);
        }

        public static BSMLParserParams ParseFromResource(string resource, GameObject parent, object host)
        {
            var data = Readers.ReadResource(resource);
            var content = Encoding.UTF8.GetString(data, 3, data.Length - 3);
            return BSMLParser.instance.Parse(content, parent, host);
        }

        public static void SetSkew(this ImageView image, float skew)
        {
            image.SetField("_skew", skew);
        }

        public static void SetGradient(this ImageView image, bool gradient)
        {
            image.SetField("_gradient", gradient);
            image.SetVerticesDirty();
        }

        public static IEnumerator AnimationCoroutine(Action<float> transitionAnimation)
        {
            yield return null;
            float elapsedTime = 0.0f;
            while (elapsedTime < 0.400000005960464)
            {
                float num = Easing.OutQuart(elapsedTime / 0.4f);
                transitionAnimation?.Invoke(num);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transitionAnimation?.Invoke(1f);
        }

        public static IEnumerator DoHorizontalTransition(this Transform transform)
        {
            float moveOffset = 20f;
            yield return AnimationCoroutine(t =>
            {
                transform.localPosition = new Vector3(moveOffset * (1f - t), 0, 0);
            });
        }

        public static IEnumerator DoVerticalTransition(this Transform transform)
        {
            float moveOffset = 20f;
            yield return AnimationCoroutine(t =>
            {
                transform.localPosition = new Vector3(0, moveOffset * (1f - t), 0);
            });
        }

        public static IEnumerator DoZTransition(this Transform transform)
        {
            float moveOffset = 20f;
            yield return AnimationCoroutine(t =>
            {
                transform.localPosition = new Vector3(0, 0, moveOffset * (1f - t));
            });
        }

        public static IEnumerator Animate(this IAnimatableUi animatable)
        {
            if(!(animatable is MonoBehaviour comp)) yield break;

            switch (animatable.AnimationType)
            {
                case IAnimatableUi.EAnimationType.Horizontal:
                    yield return DoHorizontalTransition(comp.transform);
                    break;
                case IAnimatableUi.EAnimationType.Vertical:
                    yield return DoVerticalTransition(comp.transform);
                    break;
                case IAnimatableUi.EAnimationType.Z:
                    yield return DoZTransition(comp.transform);
                    break;
            }
        }
    }
}