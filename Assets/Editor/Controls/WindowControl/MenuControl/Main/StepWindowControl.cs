using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class StepWindowControl : EditorWindow
{
    public static StepWindowControl Instance
    {
        get { return instance; }
    }
    private static StepWindowControl instance;

    private SpliceControl spliceControl;

    public List<StepMenuControl> menus;

    private EditorWindowStyle styles;

    private UnsMachineLogic logic;

    private MenuWindowControl menuControl;

    private MainWindowControl mainControl;
    private InfomationWindowControl infomationControl;
    private ProcessWindowControl processControl;
    private MatchWindowControl matchControl;

    private TextAsset obj_MachineStep;
    private bool bl_HasAsset = false;
    private bool bl_CanInit = false;
    private ModelType currentType = ModelType.None;

    private static string modelType = "t33";

    [MenuItem("设置/智能选型编辑工具")]
    static void Init()
    {
        StepWindowControl window = (StepWindowControl)GetWindow(typeof(StepWindowControl), false, "智能选配系统");
        window.spliceControl = GameObject.Find("Splice").GetComponent<SpliceControl>();
        window.Show();
    }

    private void ReStart()
    {
        if (styles == null)
        {
            styles = new EditorWindowStyle();
            styles.Init();
        }

        if (instance == null)
            instance = this;

    }

    private bool InitLogic()
    {
        GUIStyle txtStyle = new GUIStyle(EditorWindowStyle.h3FontStyle);
        txtStyle.alignment = TextAnchor.MiddleCenter;
        txtStyle.normal.textColor = Color.red;
        GUILayout.BeginVertical(EditorWindowStyle.backBlackBoderGray, GUILayout.Width(500));

        if (logic == null)
        {
            obj_MachineStep = EditorGUILayout.ObjectField("打开文件", obj_MachineStep, typeof(TextAsset), true) as TextAsset;
            UnsMachineLogic json = null;

            try
            {
                json = JsonUtility.FromJson<UnsMachineLogic>(obj_MachineStep.text);

                if (json == null)
                {
                    bl_HasAsset = false;
                    GUILayout.Label("文件錯誤", txtStyle);
                }
                else
                    bl_HasAsset = true;
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                bl_HasAsset = false;
                GUILayout.Label("文件錯誤", txtStyle);
            }


            GUILayout.BeginHorizontal();
            if (bl_HasAsset)
            {
                if (GUILayout.Button("打开"))
                {
                    logic = json;
                    bl_CanInit = true;
                }
            }
            if (GUILayout.Button("新建"))
            {
                logic = new UnsMachineLogic();
                bl_CanInit = true;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        return bl_CanInit;
    }

    private void InitMenu()
    {
        if (menus == null)
            menus = new List<StepMenuControl>();

        if (logic.AnglePos == null)
            logic.AnglePos = new UnsMachineLogic.AnglePosition();

        if (mainControl == null)
        {
            if (logic.Main == null)
                logic.Main = new UnsMachineLogic.MachineLogicMainInfo();

            mainControl = new MainWindowControl(logic, "初始界面操作", ModelType.Explode);
            menus.Add(mainControl);
        }

        if (infomationControl == null)
        {
            if (logic.Infomation == null)
                logic.Infomation = new UnsMachineLogic.MachineLogicInfomation();

            infomationControl = new InfomationWindowControl(logic, "参数信息操作", ModelType.Explode);
            menus.Add(infomationControl);
        }

        if (processControl == null)
        {
            if (logic.Process == null)
                logic.Process = new UnsMachineLogic.MachineLogicProcess();

            processControl = new ProcessWindowControl(logic, "加工动画操作", ModelType.Process);
            menus.Add(processControl);
        }

        if (matchControl == null)
        {
            if (logic.Match == null)
                logic.Match = new UnsMachineLogic.MachineLogicMatch();

            matchControl = new MatchWindowControl(logic, "选配操作", ModelType.Match);
            menus.Add(matchControl);
        }
    }

    void OnGUI()
    {
        ReStart();

        if (!InitLogic()) return;

        InitMenu();

        if (menuControl == null)
        {
            menuControl = new MenuWindowControl();
            menuControl.GetWindow(this);
        }

        GUILayout.BeginHorizontal();
        menuControl.OnGUI();

        if (menuControl.bl_Clicked)
        {
            if (menus[menuControl.menuSelected].type != currentType)
            {
                BoundModel(menus[menuControl.menuSelected].type);
                currentType = menus[menuControl.menuSelected].type;
            }

            menus[menuControl.menuSelected].GetWindow();

            menuControl.bl_Clicked = false;
        }

        menus[menuControl.menuSelected].OnGUI();
        GUILayout.EndHorizontal();

        Rect rectBtn = new Rect(this.position.width - 200, 0, 200, 30);

        if (GUI.Button(rectBtn, "保存"))
        {
            UnsMachineLogic.Save(logic);
        }
    }

    private void BoundModel(ModelType type)
    {
        ResetModel();
        GameObject go = null;
        bool isFirst = true;
        switch (type)
        {
            case ModelType.Explode:
                go = AddGameObject(spliceControl.go_Explode, spliceControl.go_ModelControl);
                break;
            case ModelType.Match:
                go = AddGameObject(spliceControl.go_Match, spliceControl.go_ModelControl);
                isFirst = false;
                break;
            case ModelType.Process:
                go = AddGameObject(spliceControl.go_Process, spliceControl.go_ModelControl);

                break;
        }
        InstantiateModel(go, isFirst);
    }

    private void ResetModel()
    {
        for (int i = 0; i < spliceControl.go_ModelControl.transform.childCount; i++)
            DestroyImmediate(spliceControl.go_ModelControl.transform.GetChild(0).gameObject);
    }


    private void InstantiateModel(GameObject parent, bool isFirst)
    {
        UnsModelDescribe describe = parent.GetComponent<UnsModelDescribe>();

        for (int i = 0; i < describe.gos_Skeleton.Length; i++)
        {
            GameObject go = describe.gos_Skeleton[i];

            string regex = @"^" + go.name.Replace("-S", "") + @"(-(\w+))?$";

            for (int j = 0; j < spliceControl.gos_Models.Length; j++)
            {
                if (Regex.IsMatch(spliceControl.gos_Models[j].name, regex))
                {
                    GameObject g = AddGameObject(spliceControl.gos_Models[j], go);

                    if (g.GetComponent<UnsModelDescribe>() != null)
                        InstantiateModel(g, isFirst);

                    if (isFirst)
                        break;
                }
            }
        }
    }

    private GameObject AddGameObject(GameObject example, GameObject content)
    {
        GameObject go = Instantiate(example, content.transform, true) as GameObject;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localPosition = Vector3.zero;
        go.name = go.name.Replace("(Clone)", "");
        return go;
    }
}