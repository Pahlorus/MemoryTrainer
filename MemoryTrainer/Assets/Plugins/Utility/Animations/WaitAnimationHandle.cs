using System;
using System.Collections;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Utility.Animations
{
    public class WaitAnimationHandle : CustomYieldInstruction
    {
        private readonly IWaitableAnimation _animation;
        public override bool keepWaiting => _animation.keepWaiting;

        public WaitAnimationHandle(IWaitableAnimation animation)
        {
            _animation = animation;
        }

        public void ContinueWith(Action onComplete, CancellationToken ct = default)
        {
            this.ToUniTask(cancellationToken: ct).ContinueWith(onComplete).Forget();
        }
    }

    public static class WaitAnimationHandleExtensions
    {
        public static WaitAnimationHandle CreateHandle(this IWaitableAnimation waitableAnimation) => new WaitAnimationHandle(waitableAnimation);
    }

    public interface IWaitableAnimation
    {
        bool keepWaiting { get; }
    }
}
