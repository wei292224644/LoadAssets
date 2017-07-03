using UnityEngine;
using System.Collections;

public class GTAnimation : MonoBehaviour
{

    /// <summary>
    /// Play default clip
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    public static void PlayAnimation(Animation anim, AnimationClip clip)
    {

        anim.clip = clip;

        anim[clip.name].normalizedTime = 0.0f;
        anim[clip.name].speed = 1.0f;

        anim.Play();
    }
    public static void PlayAnimation(AnimationInfo animInfo, bool expend, float progress = -1f)
    {
        PlayAnimation(animInfo.anim, animInfo.clip, expend, animInfo.speed, progress);
    }

    public static void PlayAnimation(Animation anim, AnimationClip clip, bool expend, float speed = 1f, float progress = -1f)
    {
        if (progress != -1f)
            anim[clip.name].normalizedTime = progress;
        else
            anim[clip.name].normalizedTime = anim[clip.name].normalizedTime == 0f ? (expend ? 0f : 1f) : anim[clip.name].normalizedTime;
        anim[clip.name].speed = expend ? speed : -speed;
        anim.clip = clip;

        anim.CrossFade(clip.name);
    }

    public static void AutoAnimation(AnimationInfo info, bool flag)
    {
        if (info.bl_AnimState == flag) return;
        info.bl_AnimState = flag;

        PlayAnimation(info.anim, info.clip, info.bl_AnimState, info.speed);
    }

    /// <summary>
    /// Plaies the animation.
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    /// <param name="speed">Speed with 100 faster.</param>
    public static void PlayAnimation(float speed, Animation anim, AnimationClip clip)
    {
        anim.clip = clip;

        anim[clip.name].normalizedTime = 0.0f;
        anim[clip.name].speed = speed;

        anim.Play();
    }

    /// <summary>
    /// animtion with pre-saved progress
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    /// <param name="progress">Progress.</param>
    public static void ReverseAnimation(Animation anim, AnimationClip clip, float progress)
    {

        anim.clip = clip;

        anim[clip.name].normalizedTime = progress;
        anim[clip.name].speed = -1.0f;

        anim.Play();
    }

    public static void PlayAnimation(Animation anim, AnimationClip clip, float progress)
    {

        anim.clip = clip;

        anim[clip.name].normalizedTime = progress;
        anim[clip.name].speed = 1.0f;

        anim.Play();

    }

    /// <summary>
    /// pause default clip
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    public static void PauseAnimation(Animation anim, AnimationClip clip)
    {

        anim[clip.name].speed = 0.0f;
    }

    /// <summary>
    /// pause default clip
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    public static void ResumeAnimation(float speed, Animation anim, AnimationClip clip)
    {

        anim[clip.name].speed = speed;
    }


    /// <summary>
    /// skip the animation to the last frame
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    public static void SkipAnimtion(Animation anim, AnimationClip clip)
    {

        anim.clip = clip;

        anim[clip.name].normalizedTime = 1.0f;
        anim[clip.name].speed = 0.0f;

        anim.CrossFade(clip.name);
        //anim.Sample ();
    }


    /// <summary>
    /// Resets the animation.
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    public static void ResetAnimation(Animation anim, AnimationClip clip)
    {
        switch (anim[clip.name].wrapMode)
        {
            case WrapMode.Loop:

                anim.clip = clip;
                anim[clip.name].normalizedTime = 0.0f;
                anim[clip.name].speed = 0.0f;
                anim.CrossFade(clip.name);

                break;
            case WrapMode.Default:
                anim.clip = clip;
                anim[clip.name].normalizedTime = 0f;
                anim[clip.name].speed = 0f;
                anim.CrossFade(clip.name);
                break;
        }
        //anim.Sample ();
    }

    /// <summary>
    /// get a progress of the entered clip on the anim obj
    /// </summary>
    /// <returns>The progress.</returns>
    public static float ClipProgress(Animation anim, AnimationClip clip)
    {
        float p;
        p = anim[clip.name].normalizedTime;

        //Debug.Log("anim progress: " + p);
        return p;
    }


    /// <summary>
    /// Samples the animation.
    /// used for pausing animation or expend slider animation
    /// </summary>
    /// <param name="anim">Animation.</param>
    /// <param name="clip">Clip.</param>
    /// <param name="progress">Progress.</param>
    public static void SampleAnimation(Animation anim, AnimationClip clip, float progress)
    {

        anim.clip = clip;

        anim[clip.name].speed = 0.0f;
        anim[clip.name].normalizedTime = progress;

        anim.Sample();
        anim.Play();
    }

    public static float GetAnimmationClipLength(Animation anim, AnimationClip clip, float speed = 1f)
    {
        return anim[clip.name].length / speed;
    }

    [System.Serializable]
    public class AnimationInfo
    {
        public Animation anim;
        public AnimationClip clip;
        [HideInInspector]
        public bool bl_AnimState = false;
        public float speed = 1f;
    }
}
