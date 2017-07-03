
using UnityEngine;

public class MenuWindowControl
{
    private Rect rect_MenuScrollView;
    private Rect rect_MenuSize;
    private Vector2 scrollPosition = Vector2.zero;

    private float fl_MenuBtnHeight = 40;
    private float fl_OffsetX = 22f;
    private float width = 0f;
    private float height = 0f;

    private string[] str_Menus;
    public int menuSelected = 0;
    private int currentSelected = -1;
    public bool bl_Clicked = false;

    public void GetWindow(StepWindowControl stepWindowControl)
    {
        str_Menus = new string[stepWindowControl.menus.Count];
        for (int i = 0; i < str_Menus.Length; i++)
            str_Menus[i] = stepWindowControl.menus[i].name;
    }

    public void OnGUI()
    {
        Resize();

        GUILayout.BeginVertical("Box", GUILayout.Width(width - fl_OffsetX), GUILayout.ExpandHeight(true));

        GUIStyle selectionStyle = new GUIStyle(EditorWindowStyle.selectionGridView);
        selectionStyle.border = new RectOffset(2, 2, 2, 2);
        selectionStyle.margin = new RectOffset(0, 0, 7, 7);
        selectionStyle.fontSize = 11;
        menuSelected = GUILayout.SelectionGrid(menuSelected, str_Menus, 1, selectionStyle, GUILayout.Height(fl_MenuBtnHeight * str_Menus.Length));

        GUILayout.EndVertical();
        GUI.EndScrollView();

        if (menuSelected!=currentSelected)
        {
            currentSelected = menuSelected;
            bl_Clicked = true;
        }
    }

    private void Resize()
    {
        width = StepWindowControl.Instance.position.width / 3;
        height = StepWindowControl.Instance.position.height;

        rect_MenuSize = new Rect(0, 0, width, height);
        rect_MenuScrollView = new Rect(0, 0, width - fl_OffsetX, fl_MenuBtnHeight * str_Menus.Length);

        scrollPosition = GUI.BeginScrollView(rect_MenuSize, scrollPosition, rect_MenuScrollView);
    }
    
}