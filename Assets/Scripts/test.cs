using UnityEngine;
using System.Collections;
using DG.Tweening;

public class test : MonoBehaviour
{
    public Animation anim;
    public AnimationClip clip;
    private bool flag = false;

    void OnGUI()
    {
        if (GUILayout.Button("1231asdxcv"))
        {

            //clip.SetCurve("",typeof(Transform),"localPosition.x",curve);
            flag = !flag;
            float value = flag ? 0 : 1;
            //GTAnimation.PlayAnimation(anim, clip, flag,2f);
            DOTween.To(() => value, x => value = x, flag ? 1 : 0, 0.4f).OnUpdate(delegate
            {
                print(value);
                GTAnimation.SampleAnimation(anim, clip, value);
            }).SetEase(Ease.OutQuad);
        }
    }
}
