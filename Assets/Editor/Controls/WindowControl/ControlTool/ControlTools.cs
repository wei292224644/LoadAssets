using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ControlTools
{
    private static string str_MainPath = "Assets/";

    public static string GetModelTree(string tree, Transform tran)
    {
        if (tran.parent != null)
        {
            tree.Insert(0, tran.parent.name + "/");
            GetModelTree(tree, tran.parent);
        }

        return tree;
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        if (obj == null)
            return "";

        string path = "/" + obj.name;
        while (obj.transform.parent != null && obj.transform.parent.name != "Model")
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path.Substring(1);
    }

    public static Texture2D SetTextureForColor(Color color, Color borderColor, int ii)
    {
        Texture2D texture2d = new Texture2D(16, 16);
        Color[] colors = texture2d.GetPixels();
        //for (int i = 0; i < colors.Length; i++)
        //    colors[i] = color;

        for (int i = 0; i < texture2d.width; i++)
        {
            for (int j = 0; j < texture2d.height; j++)
            {
                if (i < ii || i >= (texture2d.width - ii))
                    colors[j + i * texture2d.width] = borderColor;
                else
                    colors[j + i * texture2d.width] = color;

            }
        }
        texture2d.SetPixels(colors);
        return texture2d;
    }

    //public static Object GetAssetsByPath(string path)
    //{
    //    return AssetDatabase.FindAssets(str_MainPath + path, typeof(Object));
    //}

    public static void SaveAssetsByPath(string path, Object obj)
    {
        if (!AssetDatabase.Contains(obj))
            AssetDatabase.CreateAsset(obj, str_MainPath + path);

        AssetDatabase.SaveAssets();
    }

    public static T FindObject<T>(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go != null)
            return GameObject.Find(path).GetComponent<T>();

        return default(T);
    }

    public static GameObject FindObject(string path)
    {
        GameObject go = GameObject.Find(path);
        if (go != null)
            return GameObject.Find(path);
        return null;
    }

    public static GameObject[] FindObjects(string[] path)
    {
        if (path==null||path.Length==0)
            return null;

        GameObject[] gos = new GameObject[path.Length];

        for (int i = 0; i < path.Length; i++)
            gos[i] = FindObject(path[i]);

        return gos;
    }

    public static T[] FindObjects<T>(string[] path)
    {
        if (path == null || path.Length == 0)
            return null;

        T[] t = new T[path.Length];

        for (int i = 0; i < path.Length; i++)
            t[i] = FindObject<T>(path[i]);

        return t;
    }
}
