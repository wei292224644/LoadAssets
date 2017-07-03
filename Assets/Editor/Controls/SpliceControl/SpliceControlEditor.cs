using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpliceControl))]
public class SpliceControlEditor : Editor
{
    private GameObject go_ModelControl;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpliceControl control = target as SpliceControl;

        go_ModelControl = GameObject.Find("Model");

        //if (GUILayout.Button("爆炸组装"))
        //{
        //    ResetModel();
        //    GameObject go = AddGameObject(control.go_Explode, go_ModelControl);
        //    InstantiateModel(go, true);
        //}
        //if (GUILayout.Button("选配组装"))
        //{
        //    ResetModel();
        //    GameObject go = AddGameObject(control.go_Match, go_ModelControl);
        //    InstantiateModel(go, false);
        //}
        //if (GUILayout.Button("加工组装"))
        //{
        //    ResetModel();
        //    GameObject go = AddGameObject(control.go_Process, go_ModelControl);
        //    InstantiateModel(go, true);
        //}

    }


  
}
