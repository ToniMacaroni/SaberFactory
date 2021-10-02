using System;
using System.Threading;
using System.Threading.Tasks;

namespace SaberFactory.Helpers
{
    public class AnimationManager
    {
        private readonly Action<float> _inAnimation;
        private readonly Action<float> _outAnimation;

        private readonly float _speedDivision;
        private CancellationTokenSource _cancellationTokenSource;

        public AnimationManager(float speedDivision, Action<float> inAnimation, Action<float> outAnimation)
        {
            _speedDivision = speedDivision;
            _inAnimation = inAnimation;
            _outAnimation = outAnimation;
        }

        public async Task AnimateIn()
        {
            CancelCurrent();

            await AnimationHelper.AsyncAnimation(_speedDivision, _cancellationTokenSource.Token, _inAnimation);
        }

        public async Task AnimateOut()
        {
            CancelCurrent();

            await AnimationHelper.AsyncAnimation(_speedDivision, _cancellationTokenSource.Token, _outAnimation);
        }

        private void CancelCurrent()
        {
            if (_cancellationTokenSource is { })
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}