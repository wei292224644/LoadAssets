using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProcessWindowControl : StepMenuControl
{
    private UnsMachineLogic.MachineLogicProcess info;
    private string str_PropertyName;

    private List<bool> bl_InfoStates;

    public ProcessWindowControl(UnsMachineLogic logic, string name, ModelType type) : base(logic, name, type)
    {
        info = cc.logic.Process;
        str_PropertyName = "Process";
    }

    public override void GetWindow()
    {
        bl_InfoStates = new List<bool>();

        if (info.PartState == null)
            info.PartState = new UnsMachineLogic.MachineState();

        if (info.Infomation == null)
            info.Infomation = new UnsMachineLogic.MachineLogicProcess.MachineLogicProcInfo[0];

        cc.logic.AnglePos.ProcessPos = cc.logic.AnglePos.ProcessPos ?? new CameraPosition();
        cc.logic.AnglePos.ProcessPartPos = cc.logic.AnglePos.ProcessPartPos ?? new CameraPosition();


        info.c_animations = ControlTools.FindObjects<Animation>(info.Animations);

        for (int i = 0; i < info.Infomation.Length; i++)
        {
            info.Infomation[i].c_gos_Model = ControlTools.FindObjects(info.Infomation[i].Model);
            bl_InfoStates.Add(false);
        }

        BoundMachineStateUI(info.PartState);

    }

    public override void OnGUI()
    {
        Resize();
        DrawProcessUI();

        EndResize();
    }

    private void DrawProcessUI()
    {
        List<UnsMachineLogic.MachineLogicProcess.MachineLogicProcInfo> infos = new List<UnsMachineLogic.MachineLogicProcess.MachineLogicProcInfo>(info.Infomation);

        GUILayout.BeginVertical(EditorWindowStyle.levelStyle);

        GUILayout.BeginHorizontal();
        MachineStateUI(str_PropertyName + ".PartState", "初始操作", info.PartState);
        PropertyField(str_PropertyName, "c_animations", "加工动画");
        SetPath(GetGameObject(info.c_animations), ref info.Animations);
        GUILayout.EndHorizontal();

        GUILayout.Box("", EditorWindowStyle.boxLineStyle, GUILayout.Width(fl_LargeBtnWidth), GUILayout.Height(1));

        if (GUILayout.Button("添加加工部件", EditorWindowStyle.btnStyle, GUILayout.Width(fl_LargeBtnWidth)))
        {
            infos.Add(new UnsMachineLogic.MachineLogicProcess.MachineLogicProcInfo());

            bl_InfoStates.Add(false);
        }
        for (int i = 0; i < infos.Count; i++)
        {
            GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);

            GUILayout.BeginHorizontal();
            bl_InfoStates[i] = EditorGUILayout.Foldout(bl_InfoStates[i], "加工部件 " + (i + 1));

            if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
            {
                infos.RemoveAt(i);
                bl_InfoStates.RemoveAt(i);
                break;
            }
            GUILayout.EndHorizontal();

            if (bl_InfoStates[i])
            {
                info.Infomation[i].Name = EditorGUILayout.TextField("部件名称", info.Infomation[i].Name,
                    GUILayout.Width(350));
                PropertyField(new[] { str_PropertyName + ".Infomation" }, new int[] { i }, "c_gos_Model", "部件模型");
                //PropertyField(logic.Infomation[i], "gos_Model", "部件模型");
                SetPath(info.Infomation[i].c_gos_Model, ref info.Infomation[i].Model);
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();

        info.Infomation = infos.ToArray();
    }
}
