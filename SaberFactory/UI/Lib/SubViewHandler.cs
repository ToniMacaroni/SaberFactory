using System;
using System.Collections;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class SubViewHandler
    {
        public SubView CurrentSubView { get; private set; }

        private SubView _previousSubView;

        public SubViewHandler()
        {

        }

        public void SwitchView(SubView newSubView, bool notify = true)
        {
            if (CurrentSubView == newSubView) return;

            CurrentSubView?.Close();
            _previousSubView = CurrentSubView;
            CurrentSubView = newSubView;
            CurrentSubView.Open(notify);
        }

        public void NotifyDidOpen()
        {
            CurrentSubView?.DidOpen();
        }

        public void NotifyDidClose()
        {
            CurrentSubView?.DidClose();
        }

        public void GoBack()
        {
            SwitchView(_previousSubView);
        }

        //private static IEnumerator DoHorizontalTransition(
        //    SubView toPresentViewController,
        //    SubView toDismissViewController,
        //    float moveOffsetMultiplier)
        //{
        //    toPresentViewController.canvasGroup.alpha = 0.0f;
        //    float baseCanvasGroupAlpha = toDismissViewController.canvasGroup.alpha;
        //    float moveOffset = 2f * moveOffsetMultiplier;
        //    yield return (object)ViewControllerTransitionHelpers.AnimationCoroutine((Action<float>)(t =>
        //    {
        //        toPresentViewController.canvasGroup.alpha = t;
        //        toPresentViewController.transform.localPosition = new Vector3(moveOffset * (1f - t), 0.0f, 0.0f);
        //        toDismissViewController.canvasGroup.alpha = Mathf.Lerp(baseCanvasGroupAlpha, 0.0f, t);
        //        toDismissViewController.transform.localPosition = new Vector3(-moveOffset * t, 0.0f, 0.0f);
        //    }));
        //    ViewControllerTransitionHelpers.ImmediateTransition(toPresentViewController, toDismissViewController);
        //}

        private static IEnumerator AnimationCoroutine(Action<float> transitionAnimation)
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
    }
}