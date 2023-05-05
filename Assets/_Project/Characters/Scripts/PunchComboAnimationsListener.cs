using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchComboAnimationsListener : MonoBehaviour
{
    public event Action<int> onAnimationStarted;
    public event Action<int> onAnimationEnded;
    public event Action<int> onHitPointReach;
    public event Action<int> onHitPointEnd;

    public void AnimationStarted(int anim) => onAnimationStarted?.Invoke(anim);
    public void AnimationEnded(int anim) => onAnimationEnded?.Invoke(anim);
    public void HitPointReached(int anim) => onHitPointReach?.Invoke(anim);
    public void HitPointEnded(int anim) => onHitPointEnd?.Invoke(anim);
}