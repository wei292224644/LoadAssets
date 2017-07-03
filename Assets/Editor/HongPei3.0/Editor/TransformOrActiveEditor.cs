using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TransformOrActive))]
public class TransformOrActiveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TransformOrActive transformOrActive = target as TransformOrActive;

        if (!transformOrActive) return;

        if (transformOrActive.tOAInfos == null)
            transformOrActive.tOAInfos = new List<TransformOrActiveInfo>();

        List<TransformOrActiveInfo> infos = transformOrActive.tOAInfos;

        if (infos.Count < 0) return;

        //        GUILayout.Label("Error!Error!注意：模型拖拽错误时，请点击右侧的Remove，请勿直接替换模型。Error!Error!Error!Error!Error!");

        for (int i = 0; i < infos.Count; i++)
        {
            TransformOrActiveInfo info = infos[i];
            EditorGUILayout.BeginHorizontal();
            info.bl_TranState = EditorGUILayout.Foldout(info.bl_TranState, info.go ? info.go.name : "");
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                info.bl_IsInit = false;
                if (info.go)
                {
                    info.go.transform.localPosition = info.v3_TransformInit;
                    info.go.transform.localEulerAngles = info.v3_TransformAngleInit;
                    info.v3_TransformOffset = Vector3.zero;
                    info.v3_TransfromAngle = Vector3.zero;
                }
                infos.Remove(info);
            }
            EditorGUILayout.EndHorizontal();

            if (info.bl_TranState)
            {
                EditorGUI.indentLevel++;

                info.go = EditorGUILayout.ObjectField("模型", info.go, typeof(GameObject), true) as GameObject;
                info.v3_TransformOffset = EditorGUILayout.Vector3Field("偏移", info.v3_TransformOffset);
                info.v3_TransfromAngle = EditorGUILayout.Vector3Field("旋转", info.v3_TransfromAngle);
                EditorGUI.indentLevel--;
            }

            if (!info.go) continue;

            if (info.go != info.currentGo)
            {
                info.bl_IsInit = false;
                if (info.currentGo)
                {
                    info.currentGo.transform.localPosition = info.v3_TransformInit;
                    info.currentGo.transform.localEulerAngles = info.v3_TransformAngleInit;
                    info.v3_TransformInit = Vector3.zero;
                    info.v3_TransformAngleInit = Vector3.zero;
                    info.v3_TransformOffset = Vector3.zero;
                    info.v3_TransfromAngle = Vector3.zero;
                }
                info.currentGo = info.go;
            }
            else
            {
                if (!info.bl_IsInit)
                {
                    info.v3_TransformInit = info.go.transform.localPosition;
                    info.v3_TransformAngleInit = info.go.transform.localEulerAngles;
                    info.bl_IsInit = true;
                }

                if (info.bl_IsInit)
                {
                    info.go.transform.localPosition = info.v3_TransformInit +
                                                      (transformOrActive.bl_IsBoom
                                                          ? info.v3_TransformOffset
                                                          : Vector3.zero);
                    info.go.transform.localEulerAngles = info.v3_TransformAngleInit + (transformOrActive.bl_IsBoom
                        ? info.v3_TransfromAngle
                        : Vector3.zero);

                }
            }
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("添加模型"))
            infos.Add(new TransformOrActiveInfo());

        if (GUILayout.Button(transformOrActive.bl_IsBoom ? "模型归位" : "模型位移"))
            transformOrActive.bl_IsBoom = !transformOrActive.bl_IsBoom;
        //base.OnInspectorGUI();
    }
}
