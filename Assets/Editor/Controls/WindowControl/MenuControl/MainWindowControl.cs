
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainWindowControl : StepMenuControl
{
    protected string str_PropertyName;
    private UnsMachineLogic.MachineLogicMainInfo info;

    private List<bool> animStateList;
    private List<bool> bl_AnimToggle;

    private List<UnsMachineLogic.MachineLogicMainInfo.AnimationMain> animList;


    public MainWindowControl(UnsMachineLogic logic, string name, ModelType type) : base(logic, name, type)
    {
        info = logic.Main;
        str_PropertyName = "Main";
    }

    public override void GetWindow()
    {
        animList = new List<UnsMachineLogic.MachineLogicMainInfo.AnimationMain>();
        animStateList = new List<bool>();
        bl_AnimToggle = new List<bool>();



        if (info.Animations == null)
            info.Animations = new UnsMachineLogic.MachineLogicMainInfo.AnimationMain[0];

        if (info.State == null)
            info.State = new UnsMachineLogic.MachineState();

        for (int i = 0; i < info.Animations.Length; i++)
        {
            animList.Add(info.Animations[i]);
            info.Animations[i].c_go_Collider = ControlTools.FindObject(info.Animations[i].Collider);
            info.Animations[i].c_animations = ControlTools.FindObjects<Animation>(info.Animations[i].Anims);

            bl_AnimToggle.Add(false);
        }

        info.State.c_gos_Show = ControlTools.FindObjects(info.State.Show);
        info.State.c_gos_Hide = ControlTools.FindObjects(info.State.Hide);

        cc.logic.AnglePos.MainPos = cc.logic.AnglePos.MainPos ?? new CameraPosition();
    }

    public override void OnGUI()
    {
        Resize();

        GUILayout.BeginHorizontal();
        MachineStateUI(str_PropertyName + ".State", "初始操作", info.State);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("播放的动画", GUILayout.Width(60));
        if (GUILayout.Button("+", EditorWindowStyle.btnStyle, GUILayout.Width(120)))
        {
            animList.Add(new UnsMachineLogic.MachineLogicMainInfo.AnimationMain());
            bl_AnimToggle.Add(false);
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < animList.Count; i++)
        {
            GUILayout.BeginVertical(EditorWindowStyle.btnFoldoutStyle);
            DrawAnimationMainUI(i);
            GUILayout.EndVertical();
        }

        info.Animations = animList.ToArray();

        EndResize();
    }

    private void DrawAnimationMainUI(int index)
    {
        UnsMachineLogic.MachineLogicMainInfo.AnimationMain anim = animList[index];

        GUILayout.BeginHorizontal();
        bl_AnimToggle[index] = EditorGUILayout.Foldout(bl_AnimToggle[index], "动画 " + index);
        if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(120)))
        {
            animList.Remove(anim);
            bl_AnimToggle.RemoveAt(index);

            return;
        }

        GUILayout.EndHorizontal();

        if (bl_AnimToggle[index])
        {
            GUILayout.BeginHorizontal(EditorWindowStyle.levelStyle);
            anim.c_go_Collider =
                EditorGUILayout.ObjectField("动画碰撞体", anim.c_go_Collider, typeof(GameObject), true, GUILayout.Width(350)) as GameObject;
            anim.Collider = ControlTools.GetGameObjectPath(anim.c_go_Collider);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (anim.c_animations == null)
                anim.c_animations = new Animation[0];
            if (anim.Anims == null)
                anim.Anims = new string[0];

            if (GUILayout.Button("动画", EditorWindowStyle.btnStyle, GUILayout.Width(120)))
            {
                List<Animation> anims = new List<Animation>(anim.c_animations);
                anims.Add(new Animation());
                anim.c_animations = anims.ToArray();

                List<string> animPaths = new List<string>(anim.Anims);
                animPaths.Add("");
                anim.Anims = animPaths.ToArray();

                animStateList.Add(false);
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < anim.c_animations.Length; i++)
            {
                GUILayout.BeginHorizontal();
                anim.c_animations[i] = EditorGUILayout.ObjectField("动画 " + i, anim.c_animations[i], typeof(Animation), true) as Animation;
                anim.Anims[i] = anim.c_animations[i] == null ? "" : ControlTools.GetGameObjectPath(anim.c_animations[i].gameObject);

                if (GUILayout.Button("-", EditorWindowStyle.btnStyle, GUILayout.Width(60)))
                {
                    List<Animation> anims = new List<Animation>(anim.c_animations);
                    anims.RemoveAt(i);
                    anim.c_animations = anims.ToArray();


                    List<string> animPaths = new List<string>(anim.Anims);
                    animPaths.RemoveAt(i);
                    anim.Anims = animPaths.ToArray();

                    animStateList.RemoveAt(i);

                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }
}