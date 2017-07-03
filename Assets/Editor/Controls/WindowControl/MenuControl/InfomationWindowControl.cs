using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InfomationWindowControl : StepMenuControl
{
    private UnsMachineLogic.MachineLogicInfomation info;
    private string str_PropertyName;

    private List<UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle> sizeList;

    private List<bool> bl_XAngleState;
    private List<List<bool>> bl_YAngleState;
    private List<bool> bl_InfoMessage;

    public InfomationWindowControl(UnsMachineLogic logic, string name, ModelType type) : base(logic, name, type)
    {
        info = logic.Infomation;
        str_PropertyName = "Infomation";
    }

    public override void GetWindow()
    {
        sizeList = new List<UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle>();
        bl_XAngleState = new List<bool>();
        bl_YAngleState = new List<List<bool>>();
        bl_InfoMessage = new List<bool>();

        if (info.SizeInfo == null)
            info.SizeInfo = new UnsMachineLogic.MachineLogicInfomation.MachineSize();
        if (info.SizeInfo.AfterChipState == null)
            info.SizeInfo.AfterChipState = new UnsMachineLogic.MachineState();
        if (info.SizeInfo.SideChipState == null)
            info.SizeInfo.SideChipState = new UnsMachineLogic.MachineState();
        if (info.SizeInfo.SizeAngle == null)
            info.SizeInfo.SizeAngle = new UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle[0];

        if (info.ExplodeInfo == null)
            info.ExplodeInfo = new UnsMachineLogic.MachineLogicInfomation.MachineExplode();
        if (info.ExplodeInfo.c_animations == null)
            info.ExplodeInfo.c_animations = new Animation[0];
        if (info.ExplodeInfo.InfoOther == null)
            info.ExplodeInfo.InfoOther = new UnsMachineLogic.MachineState();
        if (info.ExplodeInfo.InfoMessage == null)
            info.ExplodeInfo.InfoMessage = new UnsMachineLogic.MachineLogicInfomation.MachineExplode.MachineExplodeMessage[0];

        if (info.ProcessRangeInfo == null)
            info.ProcessRangeInfo = new UnsMachineLogic.MachineLogicInfomation.MachineProcessRange();
        if (info.ProcessRangeInfo.State == null)
            info.ProcessRangeInfo.State = new UnsMachineLogic.MachineState();

        cc.logic.AnglePos.MainPos = cc.logic.AnglePos.MainPos ?? new CameraPosition();
        cc.logic.AnglePos.SizePos = cc.logic.AnglePos.SizePos ?? new CameraPosition();
        cc.logic.AnglePos.ExplodePos = cc.logic.AnglePos.ExplodePos ?? new CameraPosition();
        cc.logic.AnglePos.ProcessSizePos = cc.logic.AnglePos.ProcessSizePos ?? new CameraPosition();
        cc.logic.AnglePos.ExplodeInfoPos = cc.logic.AnglePos.ExplodeInfoPos ?? new CameraPosition[info.ExplodeInfo.InfoMessage.Length];

        //初始化SizeInfo
        for (int i = 0; i < info.SizeInfo.SizeAngle.Length; i++)
        {
            sizeList.Add(info.SizeInfo.SizeAngle[i]);
            bl_XAngleState.Add(false);

            List<bool> blY = new List<bool>();
            for (int j = 0; j < info.SizeInfo.SizeAngle[i].Yaw.Length; j++)
            {
                info.SizeInfo.SizeAngle[i].Yaw[j].c_gos_AfterChip = ControlTools.FindObjects(info.SizeInfo.SizeAngle[i].Yaw[j].Models_AfterChip);
                info.SizeInfo.SizeAngle[i].Yaw[j].c_gos_SideChip = ControlTools.FindObjects(info.SizeInfo.SizeAngle[i].Yaw[j].Models_SideChip);
                blY.Add(false);
            }
            bl_YAngleState.Add(blY);

        }
        BoundMachineStateUI(info.SizeInfo.AfterChipState);
        BoundMachineStateUI(info.SizeInfo.SideChipState);


        //初始化ExplodeInfo
        info.ExplodeInfo.c_animations = ControlTools.FindObjects<Animation>(info.ExplodeInfo.Animations);
        for (int i = 0; i < info.ExplodeInfo.InfoMessage.Length; i++)
        {
            info.ExplodeInfo.InfoMessage[i].c_go_Collider =
                ControlTools.FindObject(info.ExplodeInfo.InfoMessage[i].Collider);

            info.ExplodeInfo.InfoMessage[i].c_go_Model =
                ControlTools.FindObject(info.ExplodeInfo.InfoMessage[i].Model);

            bl_InfoMessage.Add(false);


        }
        BoundMachineStateUI(info.ExplodeInfo.InfoOther);


        //初始化ProcessRangeInfo
        info.ProcessRangeInfo.c_animations = ControlTools.FindObjects<Animation>(info.ProcessRangeInfo.Animations);
        BoundMachineStateUI(info.ProcessRangeInfo.State);



    }

    public override void OnGUI()
    {
        Resize();

        GUILayout.Label("尺寸操作", EditorWindowStyle.h1FontStyle, GUILayout.Width(60));
        DrawSizeUI();
        GUILayout.Box("", GUILayout.Width(base.rect_ScrollView.width - fl_OffsetX), GUILayout.Height(1));

        GUILayout.Space(20);

        GUILayout.Label("爆炸操作", EditorWindowStyle.h1FontStyle, GUILayout.Width(60));
        DrawBoomUI();
        GUILayout.Box("", GUILayout.Width(base.rect_ScrollView.width - fl_OffsetX), GUILayout.Height(1));

        GUILayout.Space(20);

        GUILayout.Label("加工范围操作", EditorWindowStyle.h1FontStyle, GUILayout.Width(92));
        DrawProcessRangeUI();

        EndResize();
    }

    private void DrawSizeUI()
    {

        GUILayout.BeginVertical(EditorWindowStyle.levelStyle);
        // 初始状态
        GUILayout.BeginHorizontal();
        MachineStateUI(str_PropertyName + ".SizeInfo.AfterChipState", "后排泄操作", info.SizeInfo.AfterChipState);
        MachineStateUI(str_PropertyName + ".SizeInfo.SideChipState", "侧排泄操作", info.SizeInfo.SideChipState);
        GUILayout.EndHorizontal();

        GUILayout.Box("", EditorWindowStyle.boxLineStyle, GUILayout.Width(fl_LargeBtnWidth), GUILayout.Height(1));

        //角度操作
        if (GUILayout.Button("添加X轴角度事件", GUILayout.Width(fl_LargeBtnWidth)))
        {
            sizeList.Add(new UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle());

            bl_XAngleState.Add(false);
            bl_YAngleState.Add(new List<bool>());
        }

        for (int i = 0; i < sizeList.Count; i++)
        {
            UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle angle = sizeList[i];

            GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);

            GUILayout.BeginHorizontal();
            bl_XAngleState[i] = EditorGUILayout.Foldout(bl_XAngleState[i], "X角度事件 " + (i + 1));
            if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
            {
                sizeList.RemoveAt(i);

                bl_XAngleState.RemoveAt(i);
                bl_YAngleState.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();

            List<UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle.MachineSizeAngleYaw> ylist = new List<UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle.MachineSizeAngleYaw>();
            if (angle.Yaw != null)
                ylist.AddRange(angle.Yaw);

            if (bl_XAngleState[i])
            {
                GUILayout.BeginHorizontal(EditorWindowStyle.levelStyle);
                angle.MaxPitch = EditorGUILayout.FloatField("最大角度", angle.MaxPitch, GUILayout.Width(300));
                angle.MinPitch = EditorGUILayout.FloatField("最小角度", angle.MinPitch, GUILayout.Width(300));
                GUILayout.EndHorizontal();

                if (angle.Yaw == null)
                    angle.Yaw = new UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle.MachineSizeAngleYaw[0];

                if (GUILayout.Button("添加Y角度事件", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
                {
                    ylist.Add(new UnsMachineLogic.MachineLogicInfomation.MachineSize.MachineSizeAngle.MachineSizeAngleYaw());
                    bl_YAngleState[i].Add(false);

                }

                for (int j = 0; j < angle.Yaw.Length; j++)
                {
                    GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);

                    GUILayout.BeginHorizontal();
                    bl_YAngleState[i][j] = EditorGUILayout.Foldout(bl_YAngleState[i][j], "Y Angle " + (i + 1));
                    if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
                    {
                        ylist.RemoveAt(j);
                        bl_YAngleState[i].RemoveAt(j);
                        break;
                    }
                    GUILayout.EndHorizontal();

                    if (bl_YAngleState[i][j])
                    {
                        GUILayout.BeginVertical(EditorWindowStyle.levelStyle);

                        GUILayout.BeginHorizontal();
                        angle.Yaw[j].MaxAngle = EditorGUILayout.FloatField("最大角度", angle.Yaw[j].MaxAngle, GUILayout.Width(300));
                        angle.Yaw[j].MinAngle = EditorGUILayout.FloatField("最小角度", angle.Yaw[j].MinAngle, GUILayout.Width(300));
                        GUILayout.EndHorizontal();

                        //logic.SizeInfo.SizeAngle[0].Yaw[j].gos_AfterChip

                        string[] names = new[] { str_PropertyName + ".SizeInfo.SizeAngle", "Yaw" };
                        int[] idxs = new[] { i, j };


                        PropertyField(names, idxs, "c_gos_AfterChip", "后排泄动画模型");
                        PropertyField(names, idxs, "c_gos_SideChip", "侧排屑动画模型");

                        SetPath(angle.Yaw[j].c_gos_AfterChip, ref angle.Yaw[j].Models_AfterChip);
                        SetPath(angle.Yaw[j].c_gos_SideChip, ref angle.Yaw[j].Models_SideChip);

                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();

            angle.Yaw = ylist.ToArray();
        }
        GUILayout.EndVertical();

        info.SizeInfo.SizeAngle = sizeList.ToArray();
    }

    private void DrawBoomUI()
    {
        List<UnsMachineLogic.MachineLogicInfomation.MachineExplode.MachineExplodeMessage> infoMessages = new List<UnsMachineLogic.MachineLogicInfomation.MachineExplode.MachineExplodeMessage>(info.ExplodeInfo.InfoMessage);


        GUILayout.BeginVertical(EditorWindowStyle.levelStyle);

        GUILayout.BeginHorizontal();
        MachineStateUI(str_PropertyName + ".ExplodeInfo.InfoOther", "初始操作", info.ExplodeInfo.InfoOther);
        PropertyField(str_PropertyName + ".ExplodeInfo", "c_animations", "初始动画");
        SetPath(GetGameObject(info.ExplodeInfo.c_animations), ref info.ExplodeInfo.Animations);
        GUILayout.EndHorizontal();

        GUILayout.Box("", EditorWindowStyle.boxLineStyle, GUILayout.Width(fl_LargeBtnWidth), GUILayout.Height(1));

        if (GUILayout.Button("添加爆炸点击操作", EditorWindowStyle.btnStyle, GUILayout.Width(fl_LargeBtnWidth)))
        {
            infoMessages.Add(new UnsMachineLogic.MachineLogicInfomation.MachineExplode.MachineExplodeMessage());

            bl_InfoMessage.Add(false);

            List<CameraPosition> pos = new List<CameraPosition>(cc.logic.AnglePos.ExplodeInfoPos);
            pos.Add(new CameraPosition());
            cc.logic.AnglePos.ExplodeInfoPos = pos.ToArray();
        }

        for (int i = 0; i < infoMessages.Count; i++)
        {
            GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);

            GUILayout.BeginHorizontal();
            bl_InfoMessage[i] = EditorGUILayout.Foldout(bl_InfoMessage[i], "爆炸信息 " + (i + 1));

            if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
            {
                infoMessages.RemoveAt(i);
                bl_InfoMessage.RemoveAt(i);

                List<CameraPosition> pos = new List<CameraPosition>(cc.logic.AnglePos.ExplodeInfoPos);
                pos.RemoveAt(i);
                cc.logic.AnglePos.ExplodeInfoPos = pos.ToArray();

                break;
            }
            GUILayout.EndHorizontal();

            if (bl_InfoMessage[i])
            {
                GUILayout.BeginVertical(EditorWindowStyle.levelStyle);

                infoMessages[i].c_go_Collider = EditorGUILayout.ObjectField("碰撞体", infoMessages[i].c_go_Collider, typeof(GameObject), true) as GameObject;
                infoMessages[i].c_go_Model = EditorGUILayout.ObjectField("模型", infoMessages[i].c_go_Model, typeof(GameObject), true) as GameObject;

                infoMessages[i].Collider = ControlTools.GetGameObjectPath(infoMessages[i].c_go_Collider);
                infoMessages[i].Model = ControlTools.GetGameObjectPath(infoMessages[i].c_go_Model);

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();
        info.ExplodeInfo.InfoMessage = infoMessages.ToArray();
    }

    private void DrawProcessRangeUI()
    {
        GUILayout.BeginHorizontal(EditorWindowStyle.levelStyle);
        MachineStateUI(str_PropertyName + ".ProcessRangeInfo.State", "初始操作", info.ProcessRangeInfo.State);
        PropertyField(str_PropertyName + ".ProcessRangeInfo", "c_animations", "初始动画");
        SetPath(GetGameObject(info.ProcessRangeInfo.c_animations), ref info.ProcessRangeInfo.Animations);

        GUILayout.EndHorizontal();
    }

}
