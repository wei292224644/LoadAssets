
using UnityEditor;
using UnityEngine;

public class CC : ScriptableObject
{
    public UnsMachineLogic logic;
}

public enum ModelType
{
    None,Explode,Match,Process
}

public abstract class StepMenuControl
{
    public string name;
    public ModelType type;

    public Rect rect_ScrollView = new Rect();
    public float fl_OffsetX = 22f;

    private Rect rect_Size;
    private Vector2 scrollPosition = Vector2.zero;

    private float width = 0f;
    private float height = 0f;

    private float space = 30f;

    protected float fl_LargeBtnWidth = 200f;
    protected float fl_normalBtnWidth = 120f;

    protected CC cc;


    protected StepMenuControl(UnsMachineLogic logic, string name,ModelType type)
    {
        this.name = name;
        this.type = type;
        this.cc = new CC { logic = logic };
    }


    public abstract void GetWindow();

    public abstract void OnGUI();

    public void Resize()
    {
        float offfsetX = StepWindowControl.Instance.position.width / 3;

        width = StepWindowControl.Instance.position.width / 3 * 2;
        height = StepWindowControl.Instance.position.height;

        rect_Size = new Rect(offfsetX, 0, width, height);
        rect_ScrollView.width = StepWindowControl.Instance.position.width / 3 * 2 - fl_OffsetX;
        rect_ScrollView.x = offfsetX - fl_OffsetX;

        scrollPosition = GUI.BeginScrollView(rect_Size, scrollPosition, rect_ScrollView);
        GUILayout.BeginVertical("Box", GUILayout.Width(rect_ScrollView.width), GUILayout.Height(rect_ScrollView.height), GUILayout.MinHeight(height));

        GUILayout.Label(name, EditorWindowStyle.h1FontStyle);
        GUI.Box(new Rect(offfsetX - fl_OffsetX + width / 2 - 100, 28, 200, 1), "");

        GUILayout.BeginVertical(new GUIStyle()
        {
            margin = new RectOffset(0, 0, 20, 0)
        });
    }

    public void EndResize()
    {
        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUI.EndScrollView();

        Rect rect = GUILayoutUtility.GetLastRect();
        if (rect.height > 1)
            rect_ScrollView.height = rect.height;
    }


    public void MachineStateUI(string name, string title, UnsMachineLogic.MachineState state)
    {
        GUILayout.BeginVertical();

        GUILayout.Label(title);

        PropertyField(name, "c_gos_Show", "显示模型");
        PropertyField(name, "c_gos_Hide", "隐藏模型");

        SetPath(state.c_gos_Show, ref state.Show);
        SetPath(state.c_gos_Hide, ref state.Hide);

        GUILayout.EndVertical();
    }

    public void BoundMachineStateUI(UnsMachineLogic.MachineState state)
    {
        state.c_gos_Hide = ControlTools.FindObjects(state.Hide);
        state.c_gos_Show = ControlTools.FindObjects(state.Show);
    }

    public void PropertyField(string name, string name2, string title)
    {
        GUILayout.BeginVertical();

        SerializedObject obj = new SerializedObject(cc);
        SerializedProperty property1 = obj.FindProperty("logic");
        property1 = property1.FindPropertyRelative(name);
        property1 = property1.FindPropertyRelative(name2);

        EditorGUILayout.PropertyField(property1, new GUIContent(title), true, GUILayout.Width(400));
        obj.ApplyModifiedProperties();

        GUILayout.EndVertical();
    }

    public void PropertyField(string[] names, int[] indexs, string name2, string title)
    {
        GUILayout.BeginVertical();

        SerializedObject obj = new SerializedObject(cc);
        SerializedProperty pro = obj.FindProperty("logic");

        pro = pro.FindPropertyRelative(names[0]);
        pro = pro.GetArrayElementAtIndex(indexs[0]);

        for (int i = 1; i < names.Length; i++)
        {
            pro = pro.FindPropertyRelative(names[i]);
            pro = pro.GetArrayElementAtIndex(indexs[i]);
        }
        pro = pro.FindPropertyRelative(name2);
        EditorGUILayout.PropertyField(pro, new GUIContent(title), true, GUILayout.Width(400));
        obj.ApplyModifiedProperties();

        GUILayout.EndVertical();

    }

    public GameObject[] GetGameObject(Behaviour[] gos)
    {
        if (gos == null || gos.Length <= 0)
            return null;

        GameObject[] g = new GameObject[gos.Length];

        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i] == null)
                continue;

            g[i] = gos[i].gameObject;
        }


        return g;
    }

    public void SetPath(GameObject[] gos, ref string[] paths)
    {
        if (gos == null || gos.Length <= 0)
            return;

        paths = new string[gos.Length];

        for (int i = 0; i < gos.Length; i++)
        {
            paths[i] = ControlTools.GetGameObjectPath(gos[i]);
        }
    }
    
}
