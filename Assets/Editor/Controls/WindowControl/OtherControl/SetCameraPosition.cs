using System.Collections.Generic;
using DG.DemiEditor;
using UnityEngine;
using UnityEditor;

public class SetCameraPosition : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private Cam_SceneType sceneType;
    private Animation anim_Boom;

    private UnsMachineLogic.AnglePosition anglePosition = new UnsMachineLogic.AnglePosition();

    private Camera mainCamera;
    private XgCameraOrbit camOrbit;
    private GameObject go_CameraCenter;
    private XgCameraControl camControl;

    private Vector2 scroll;

    private bool bl_ExplodeInfo = false;
    private bool bl_MatchInfo = false;

    [MenuItem("设置/设置项目的相机角度")]
    static void Init()
    {
        SetCameraPosition window = (SetCameraPosition)EditorWindow.GetWindow(typeof(SetCameraPosition));
        window.Show();
    }

    void OnGUI()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        go_CameraCenter = GameObject.FindGameObjectWithTag("CameraCenter");
        camOrbit = mainCamera.GetComponent<XgCameraOrbit>();
        camControl = mainCamera.GetComponent<XgCameraControl>();

        scroll = GUILayout.BeginScrollView(scroll);
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.wordWrap = true;
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 12;

        GUILayout.Label("快捷键：", labelStyle);
        GUILayout.Label("\t定位中心点--->\t点击一个物体(Scene场景中)，按下(F)键位锁定物体视角,然后选择 (Hierarchy) 中的 (CameraCenter) 物体，按下 (ctrl+alt+f) 键位将 (CameraCenter) 锁定在选中的物体中心点( 如果中心点有偏移的话需要手动调节位置)。", labelStyle);
        GUILayout.Space(8);
        GUILayout.Label("\t定位相机位置--->\t点击 (Hierarchy) 中的 (Main Camera) 物体，然后在 (Scene) 中定位视角， 按下 (shift+ctrl+f) 将相机移动到指定位置。", labelStyle);
        GUILayout.Space(20);

        GUIStyle verticalStyle = new GUIStyle();
        verticalStyle.normal.background = Texture2D.blackTexture;
        verticalStyle.margin = new RectOffset(20, 20, 0, 40);

        GUIStyle verticalChildStyle = new GUIStyle();
        verticalChildStyle.normal.background = Texture2D.blackTexture;
        verticalChildStyle.margin = new RectOffset(10, 10, 0, 20);

        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("初始-->MainPos", ref anglePosition.MainPos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("样件加工-->ProcessPos", ref anglePosition.ProcessPos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("选型配置-->MatchPos", ref anglePosition.MatchPos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("选型配置-->选型选配预览-->MatchPreviewPos", ref anglePosition.MatchPreviewPos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("机床信息-->InfoPos", ref anglePosition.InfoPos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("机床信息-->机床尺寸-->SizePos", ref anglePosition.SizePos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("机床信息-->加工范围-->ProcessSizePos", ref anglePosition.ProcessSizePos);
        GUILayout.EndVertical();
        GUILayout.BeginVertical(verticalStyle);
        SetDefaultCameraPosition("爆炸-->ExplodePos", ref anglePosition.ExplodePos);
        GUILayout.EndVertical();

        GUILayout.BeginVertical(verticalStyle);
        if (bl_ExplodeInfo = GUILayout.Toggle(bl_ExplodeInfo, "爆炸选配-->ExplodeInfoPos"))
        {
            anglePosition.ExplodeInfoPos = anglePosition.ExplodeInfoPos ?? new CameraPosition[0];

            if (GUILayout.Button("+"))
            {
                List<CameraPosition> explodeInfos = new List<CameraPosition>(anglePosition.ExplodeInfoPos);
                explodeInfos.Add(new CameraPosition());
                anglePosition.ExplodeInfoPos = explodeInfos.ToArray();
            }
            for (int i = 0; i < anglePosition.ExplodeInfoPos.Length; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical(verticalChildStyle);
                SetDefaultCameraPosition("爆炸-->ExplodeInfoPos " + (i + 1),ref anglePosition.ExplodeInfoPos[i]);
                GUILayout.EndVertical();

                if (GUILayout.Button("-"))
                {
                    List<CameraPosition> explodeInfos = new List<CameraPosition>(anglePosition.ExplodeInfoPos);
                    explodeInfos.RemoveAt(i);
                    anglePosition.ExplodeInfoPos = explodeInfos.ToArray();
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();


        GUILayout.BeginVertical(verticalStyle);
        if (bl_MatchInfo = GUILayout.Toggle(bl_MatchInfo, "选配-->MatchInfoPos"))
        {
            anglePosition.MatchInfoPos = anglePosition.MatchInfoPos ?? new CameraPosition[0];

            if (GUILayout.Button("+"))
            {
                List<CameraPosition> matchInfos = new List<CameraPosition>(anglePosition.MatchInfoPos);
                matchInfos.Add(new CameraPosition());
                anglePosition.MatchInfoPos = matchInfos.ToArray();
            }
            for (int i = 0; i < anglePosition.MatchInfoPos.Length; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical(verticalChildStyle);
                SetDefaultCameraPosition("爆炸选配-->MatchInfoPos " + (i + 1), ref anglePosition.MatchInfoPos[i]);
                GUILayout.EndVertical();

                if (GUILayout.Button("-"))
                {
                    List<CameraPosition> matchInfos = new List<CameraPosition>(anglePosition.MatchInfoPos);
                    matchInfos.RemoveAt(i);
                    anglePosition.MatchInfoPos = matchInfos.ToArray();
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
    }

    private void SetDefaultCameraPosition(string title, ref CameraPosition cameraPos)
    {
        cameraPos = cameraPos ?? new CameraPosition();

        cameraPos.remark = string.IsNullOrEmpty(cameraPos.remark) ? title : cameraPos.remark;
        if (cameraPos.bl_Editor = GUILayout.Toggle(cameraPos.bl_Editor, cameraPos.remark))
        {
            cameraPos.remark = EditorGUILayout.TextField("标注", cameraPos.remark);
            cameraPos.v3_CenterPos = EditorGUILayout.Vector3Field("中心点", cameraPos.v3_CenterPos);
            cameraPos.v3_TranPos = EditorGUILayout.Vector3Field("相机位置", cameraPos.v3_TranPos);
            cameraPos.v3_TranRot = EditorGUILayout.Vector3Field("相机角度", cameraPos.v3_TranRot);
            cameraPos.v3_TranOffset = EditorGUILayout.Vector3Field("相机偏移位置", cameraPos.v3_TranOffset);
            cameraPos.v3_TranFor = EditorGUILayout.Vector3Field("相机的向前向量", cameraPos.v3_TranFor);
            cameraPos.v2_Distance = EditorGUILayout.Vector2Field("相机距离中心点的范围", cameraPos.v2_Distance);
            cameraPos.v2_PitchAngle = EditorGUILayout.Vector2Field("相机上下活动角度", cameraPos.v2_PitchAngle);

            if (GUILayout.Button("定位"))
            {
                cameraPos.v3_CenterPos = go_CameraCenter.transform.position;
                cameraPos.v3_TranPos = mainCamera.transform.position;
                cameraPos.v3_TranRot = mainCamera.transform.eulerAngles;
                cameraPos.v3_TranFor = mainCamera.transform.forward;
                cameraPos.v2_Distance = new Vector2(camOrbit.minDistance, camOrbit.maxDistance);
                cameraPos.v2_PitchAngle = new Vector2(camOrbit.minPitch, camOrbit.maxPitch);
            }
            if (GUILayout.Button("相机追踪"))
            {
                //go_CameraCenter.transform.position = cameraPos.v3_CenterPos;
                //go_CameraCenter.transform.position = cameraPos.v3_CenterPos;
                //mainCamera.transform.position = cameraPos.v3_TranPos;
                //mainCamera.transform.eulerAngles = cameraPos.v3_TranRot;
                camControl.TravelToPoint(cameraPos);
                //camOrbit.minDistance = cameraPos.v2_Distance.x;
                //camOrbit.maxDistance = cameraPos.v2_Distance.y;
                //camOrbit.minPitch = cameraPos.v2_PitchAngle.x;
                //camOrbit.maxPitch = cameraPos.v2_PitchAngle.y;
            }
        }
    }

    private void BoomCameraPosition()
    {
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }
}

public enum Cam_SceneType
{
    爆炸, 选配
}