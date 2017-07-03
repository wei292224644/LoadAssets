
using UnityEditor;
using UnityEngine;

public class EditorWindowStyle
{
    private static string str_ImagePath = "Assets/Editor/Controls/WindowControl/Images/";
    private static Texture2D text_backBlueBorderBlack;
    private static Texture2D text_backBlackBoderGray;
    private static Texture2D text_white;
    private static Texture2D text_gray;

    public void Init()
    {
        //        text_BgBlueBorderBlack = new Texture2D(0x2a, 0x2a, TextureFormat.ASTC_RGBA_5x5, false, true);

        text_backBlueBorderBlack = AssetDatabase.LoadAssetAtPath<Texture2D>(str_ImagePath + "backBlueBorderBlack.png");
        text_backBlackBoderGray = AssetDatabase.LoadAssetAtPath<Texture2D>(str_ImagePath + "backBlackBoderGray.png");
        text_white = AssetDatabase.LoadAssetAtPath<Texture2D>(str_ImagePath + "white.png");
        text_gray = AssetDatabase.LoadAssetAtPath<Texture2D>(str_ImagePath + "gray.png");

        GUIStyle style2 = new GUIStyle();
        style2.normal.textColor = Color.white;
        style2.alignment = TextAnchor.MiddleCenter;
        style2.fontSize = 16;
        h1FontStyle = style2;

        h2FontStyle = new GUIStyle();
        h2FontStyle.normal.textColor = Color.white;
        h2FontStyle.alignment = TextAnchor.MiddleLeft;
        h2FontStyle.fontSize = 14;

        h3FontStyle = new GUIStyle();
        h3FontStyle.normal.textColor = Color.white;
        h3FontStyle.alignment = TextAnchor.MiddleLeft;
        h3FontStyle.fontSize = 12;

        GUIStyle style3 = new GUIStyle(style2);
        style3.normal.background = text_backBlackBoderGray;
        style3.border = new RectOffset(2, 2, 2, 2);
        backBlackBoderGray = style3;

        GUIStyle style4 = new GUIStyle();
        style4.normal.background = text_white;
        style4.onNormal.background = text_backBlueBorderBlack;
        style4.border = new RectOffset(10, 10, 10, 10);
        style4.alignment = TextAnchor.MiddleCenter;
        selectionGridView = style4;

        GUIStyle style5 = new GUIStyle(GUI.skin.button);
        style5.alignment = TextAnchor.MiddleCenter;
        style5.margin = new RectOffset(0, 0, 0, 0);
        btnStyle = style5;

        GUIStyle style7 = new GUIStyle();
        style7.margin = new RectOffset(15, 10, 7, 0);
        style7.padding = new RectOffset(4, 4, 3, 4);
        levelStyle = style7;

        GUIStyle style6 = new GUIStyle(style7);
        style6.border = new RectOffset(2, 2, 2, 2);
        style6.normal.background = text_gray;
        btnFoldoutStyle = style6;

        boxLineStyle = new GUIStyle(GUI.skin.box);
        boxLineStyle.margin = new RectOffset(0, 0, 10, 10);
    }

    public static GUIStyle backBlackBoderGray { get; private set; }

    public static GUIStyle selectionGridView { get; private set; }

    public static GUIStyle btnStyle { get; private set; }

    public static GUIStyle btnFoldoutStyle { get; private set; }

    public static GUIStyle levelStyle { get; private set; }

    public static GUIStyle h1FontStyle { get; private set; }

    public static GUIStyle h2FontStyle { get; private set; }

    public static GUIStyle h3FontStyle { get; private set; }

    public static GUIStyle boxLineStyle { get; private set; }
}
