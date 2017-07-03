using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MatchWindowControl : StepMenuControl
{
    private UnsMachineLogic.MachineLogicMatch info;
    private string str_PropertyName;
    private List<bool> bl_PlanStates;
    private List<List<bool>> bl_PartStates;

    public MatchWindowControl(UnsMachineLogic logic, string name, ModelType type) : base(logic, name, type)
    {
        info = logic.Match;
        str_PropertyName = "Match";
    }

    public override void GetWindow()
    {
        bl_PlanStates = new List<bool>();
        bl_PartStates = new List<List<bool>>();

        if (info.Plan == null)
            info.Plan = new UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan[0];

        if (info.Other == null)
            info.Other = new UnsMachineLogic.MachineLogicMatch.MachineLoMatPlPartOther();

        cc.logic.AnglePos.MatchPos = cc.logic.AnglePos.MatchPos ?? new CameraPosition();
        cc.logic.AnglePos.MatchPreviewPos = cc.logic.AnglePos.MatchPreviewPos ?? new CameraPosition();
        cc.logic.AnglePos.MatchInfoPos = cc.logic.AnglePos.MatchInfoPos ?? new CameraPosition[info.Plan.Length];

        for (int i = 0; i < info.Plan.Length; i++)
        {
            info.Plan[i].c_go_Transparent = ControlTools.FindObject(info.Plan[i].Transparent);
            bl_PlanStates.Add(false);

            List<bool> bYL = new List<bool>();
            for (int j = 0; j < info.Plan[i].Parts.Length; j++)
            {
                info.Plan[i].Parts[j].c_gos_Entity = ControlTools.FindObjects(info.Plan[i].Parts[j].Entity);
                bYL.Add(false);
            }
            bl_PartStates.Add(bYL);
        }

        info.Other.c_gos_Entity = ControlTools.FindObjects(info.Other.Entity);
        info.Other.c_gos_Transparent = ControlTools.FindObjects(info.Other.Transparent);

    }

    public override void OnGUI()
    {
        Resize();
        List<UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan> plans = new List<UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan>(info.Plan);

        GUILayout.BeginVertical(EditorWindowStyle.levelStyle);
        GUILayout.Label("默认设置");

        GUILayout.BeginHorizontal();
        PropertyField(str_PropertyName + ".Other", "c_gos_Transparent", "透明模型");
        SetPath(info.Other.c_gos_Transparent, ref info.Other.Transparent);

        PropertyField(str_PropertyName + ".Other", "c_gos_Entity", "实体模型");
        SetPath(info.Other.c_gos_Entity, ref info.Other.Entity);
        GUILayout.EndHorizontal();

        GUILayout.Box("", EditorWindowStyle.boxLineStyle, GUILayout.Width(fl_LargeBtnWidth), GUILayout.Height(1));

        if (GUILayout.Button("添加选配项", EditorWindowStyle.btnStyle, GUILayout.Width(fl_LargeBtnWidth)))
        {
            plans.Add(new UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan());
            bl_PlanStates.Add(false);
            bl_PartStates.Add(new List<bool>());

            List<CameraPosition> pos = new List<CameraPosition>(cc.logic.AnglePos.MatchInfoPos);
            pos.Add(new CameraPosition());
            cc.logic.AnglePos.MatchInfoPos = pos.ToArray();
        }

        for (int i = 0; i < plans.Count; i++)
        {
            GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);

            GUILayout.BeginHorizontal();
            bl_PlanStates[i] = EditorGUILayout.Foldout(bl_PlanStates[i], "选配项 " + (i + 1));

            if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
            {
                plans.RemoveAt(i);
                bl_PlanStates.RemoveAt(i);
                bl_PartStates.RemoveAt(i);

                List<CameraPosition> pos = new List<CameraPosition>(cc.logic.AnglePos.MatchInfoPos);
                pos.RemoveAt(i);
                cc.logic.AnglePos.MatchInfoPos = pos.ToArray();

                break;
            }
            GUILayout.EndHorizontal();

            if (bl_PlanStates[i])
            {
                GUILayout.BeginVertical(EditorWindowStyle.levelStyle);

                plans[i].c_go_Transparent = EditorGUILayout.ObjectField("透明模型", plans[i].c_go_Transparent, typeof(GameObject), true, GUILayout.Width(400)) as GameObject;
                plans[i].Transparent = ControlTools.GetGameObjectPath(plans[i].c_go_Transparent);

                List<UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan.MachineLoMatPlPartEntity> partList = new List<UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan.MachineLoMatPlPartEntity>();
                if (plans[i].Parts != null)
                    partList.AddRange(plans[i].Parts);

                GUILayout.Box("", EditorWindowStyle.boxLineStyle, GUILayout.Width(fl_LargeBtnWidth), GUILayout.Height(1));
                if (GUILayout.Button("添加选配", EditorWindowStyle.btnStyle, GUILayout.Width(fl_LargeBtnWidth)))
                {
                    partList.Add(new UnsMachineLogic.MachineLogicMatch.MachineLogicMatchPlan.MachineLoMatPlPartEntity());
                    bl_PartStates[i].Add(false);
                }

                for (int j = 0; j < partList.Count; j++)
                {
                    GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);

                    GUILayout.BeginHorizontal();
                    bl_PartStates[i][j] = EditorGUILayout.Foldout(bl_PartStates[i][j], "选配 " + (j + 1));
                    if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(fl_normalBtnWidth)))
                    {
                        partList.RemoveAt(j);
                        bl_PartStates[i].RemoveAt(j);
                        break;
                    }
                    GUILayout.EndHorizontal();
                    if (bl_PartStates[i][j])
                    {
                        GUILayout.BeginVertical(EditorWindowStyle.levelStyle);
                        SerializedObject obj = new SerializedObject(base.cc);
                        SerializedProperty property1 = obj.FindProperty("logic");
                        property1 = property1.FindPropertyRelative(str_PropertyName + ".Plan");
                        property1 = property1.GetArrayElementAtIndex(i);
                        property1 = property1.FindPropertyRelative("Parts");
                        property1 = property1.GetArrayElementAtIndex(j);
                        property1 = property1.FindPropertyRelative("c_gos_Entity");
                        //PropertyField(new string[] { str_PropertyName + ".Plan", "Parts" }, new int[] { i, j }, "gos_Entity", "选配模型");
                        //SerializedObject obj = new SerializedObject(logic);
                        //SerializedProperty property1 = obj.FindProperty(str_PropertyName + ".Plan");
                        //property1= property1.GetArrayElementAtIndex(i);
                        //property1 = property1.FindPropertyRelative("Parts");
                        //property1 = property1.GetArrayElementAtIndex(j);
                        //property1 = property1.FindPropertyRelative("gos_Entity");

                        EditorGUILayout.PropertyField(property1.FindPropertyRelative("Array.size"));
                        for (int k = 0; k < property1.arraySize; k++)
                            EditorGUILayout.PropertyField(property1.GetArrayElementAtIndex(k));
                        obj.ApplyModifiedProperties();

                        SetPath(partList[j].c_gos_Entity, ref partList[j].Entity);
                        GUILayout.EndVertical();
                    }


                    GUILayout.EndVertical();
                }

                plans[i].Parts = partList.ToArray();


                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }


        GUILayout.EndVertical();

        info.Plan = plans.ToArray();

        EndResize();
    }
}
