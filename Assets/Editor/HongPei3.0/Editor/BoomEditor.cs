using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Boom))]
public class BoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Boom boom = target as Boom;
        if (GUILayout.Button("播放动画"))
        {
            boom.clip.SampleAnimation(boom.go, boom.zenShu / (float)boom.zenLv);
        }
        if (GUILayout.Button("停止动画"))
        {
            boom.clip.SampleAnimation(boom.go, 0f);
        }
    }
}