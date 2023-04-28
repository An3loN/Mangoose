using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionController : MonoBehaviour
{
    [SerializeField] Animator emotionCloudAnimator;
    public enum Emotions
    {
        Exclamation = 0,
        Question,
        Gold,
        Zzz,
        Disable
    }

    public Func<bool> PlayEmote(Emotions emotion)
    {
        emotionCloudAnimator.gameObject.SetActive(true);
        Func<bool> waiter = () => !emotionCloudAnimator.gameObject.activeSelf;
        emotionCloudAnimator.SetTrigger(emotion.ToString());
        return waiter;
    }
    public Func<bool> PlayEmote(string emote)
    {
        Emotions emotion = (Emotions) Enum.Parse(typeof(Emotions), emote);
        return PlayEmote(emotion);
    }
}
