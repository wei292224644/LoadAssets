using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObjectActive))]
public class GameObjectActiveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GameObjectActive go = target as GameObjectActive;

        if (!go) return;
        if (go.gos_Hide == null)
            go.gos_Hide = new GameObject[0];
        if (go.gos_Show == null)
            go.gos_Show = new GameObject[0];

        for (int i = 0; i < go.gos_Hide.Length; i++)
        {
            if (go.gos_Hide[i] == null) continue;
            go.gos_Hide[i].SetActive(!go.bl_IsBoom);
        }
        for (int i = 0; i < go.gos_Show.Length; i++)
        {
            if (go.gos_Show[i] == null) continue;
            go.gos_Show[i].SetActive(go.bl_IsBoom);
        }

        if (GUILayout.Button(go.bl_IsBoom ? "初始" : "切换"))
        {
            go.bl_IsBoom = !go.bl_IsBoom;
        }
    }
}
