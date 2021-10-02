using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SaberFactory.Helpers
{
    public static class AnimationHelper
    {
        public static async Task AsyncAnimation(float speedDivision, CancellationToken cancelToken, Action<float> transitionAnimation)
        {
            var elapsedTime = 0.0f;
            var cutoff = speedDivision - 0.1f;
            while (elapsedTime < cutoff)
            {
                if (cancelToken.IsCancellationRequested) break;

                var num = Easing.OutQuart(elapsedTime / speedDivision);
                transitionAnimation?.Invoke(num);
                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            transitionAnimation?.Invoke(1f);
        }
    }
}