using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    internal class SubViewSwitcher
    {
        public SubView CurrentSubView { get; private set; }

        private CancellationTokenSource _currentTokenSource;

        private SubView _previousSubView;

        public async void SwitchView(SubView newSubView, bool notify = true)
        {
            if (CurrentSubView == newSubView)
            {
                return;
            }

            CurrentSubView?.Close();
            _previousSubView = CurrentSubView;
            CurrentSubView = newSubView;

            _currentTokenSource?.Cancel();
            _currentTokenSource?.Dispose();

            _currentTokenSource = new CancellationTokenSource();

            await DoTransition(notify, _currentTokenSource.Token);
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

        private async Task DoTransition(
            bool notify,
            CancellationToken cancellationToken)
        {
            if (CurrentSubView is null)
            {
                return;
            }

            var toPresentCG = CurrentSubView.GetComponent<CanvasGroup>();
            toPresentCG.alpha = 0;

            await CurrentSubView.Open(notify);

            float moveOffset = 20;

            if (_previousSubView is { })
            {
                var toDismissCG = _previousSubView.GetComponent<CanvasGroup>();
                var baseCanvasGroupAlpha = toDismissCG.alpha;

                await Animate(t =>
                {
                    toDismissCG.alpha = Mathf.Lerp(baseCanvasGroupAlpha, 0.0f, t);
                    toDismissCG.transform.localEulerAngles = new Vector3(0, -moveOffset * 4 * t, 0.0f);
                }, cancellationToken);
            }

            await Animate(t =>
            {
                toPresentCG.alpha = t;
                toPresentCG.transform.localEulerAngles = new Vector3(0, moveOffset * 4 * (1f - t), 0);
            }, cancellationToken);

            _previousSubView?.gameObject.SetActive(false);
            //ViewControllerTransitionHelpers.ImmediateTransition(toPresentViewController, toDismissViewController);
        }

        private async Task Animate(Action<float> transitionAnimation, CancellationToken cancellationToken)
        {
            var elapsedTime = 0.0f;
            while (elapsedTime < 0.200000005960464)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var num = Easing.OutQuart(elapsedTime / 0.2f);
                transitionAnimation?.Invoke(num);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            transitionAnimation?.Invoke(1f);
        }
    }
}