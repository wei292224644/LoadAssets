using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class UnsFileControl
{
    public static UnsFileControl instance
    {
        get { return _instacne ?? (_instacne = new UnsFileControl()); }
        set { _instacne = value; }
    }

    private static UnsFileControl _instacne;

    public readonly string str_SteamingAssetsPath = Application.streamingAssetsPath + "/";
    public readonly string str_PersistentPath = Application.persistentDataPath + "/";

    public delegate void W3Delgate(WWW www);

    /// <summary>
    /// 创建文件夹 (persistentDataPath)
    /// </summary>
    /// <param name="path">相对路径</param>
    public void CreateFolder(string path)
    {
        if (!Directory.Exists(str_PersistentPath + path))
            Directory.CreateDirectory(str_PersistentPath + path);
    }

    /// <summary>
    /// 创建文件 (persistentDataPath)
    /// </summary>
    /// <param name="path">相对路径</param>
    public void CreateFile(string path)
    {
        string[] folderAry = path.Split('/');

        string str_folderPath = "";

        for (int i = 0; i < folderAry.Length - 1; i++)
        {
            str_folderPath += folderAry[i] + "/";
            CreateFolder(str_folderPath);
        }

        if (!File.Exists(str_PersistentPath + path))
        {
            FileStream fs = File.Create(str_PersistentPath + path);

            fs.Close();
            fs.Dispose();
        }
    }

    /// <summary>
    /// 修改文件内容(persistentDataPath)
    /// </summary>
    /// <param name="path">相对路径</param>
    /// <param name="bytes">内容</param>
    /// <param name="callback">回调函数</param>
    public void SaveFile(string path, Byte[] bytes, Action callback = null)
    {
        CreateFile(path);

        using (FileStream fileStream = new FileStream(str_PersistentPath + path, FileMode.Open, FileAccess.Write))
        {
            fileStream.SetLength(0);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }
        if (callback != null)
            callback();
    }


    /// <summary>
    /// 获取文件(Stream)
    /// </summary>
    /// <param name="path"></param>
    public string GetFile(string path)
    {
        var outStr = "";
        var sr = new StreamReader(path);
        outStr = sr.ReadToEnd();
        sr.Close();

        return outStr;
    }

    /// <summary>
    /// 获取文件(www)
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="callback">下载完的回调函数</param>
    /// <param name="start">下载前的回调函数</param>
    /// <returns></returns>
    public IEnumerator GetFile(string path, W3Delgate callback, W3Delgate start = null)
    {

        WWW www = new WWW(path);

        if (start != null)
            start(www);

        yield return www;

        if (callback != null)
            callback(www);
    }

    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="path"></param>
    public void delFolder(string path)
    {
        if (Directory.Exists(str_PersistentPath + path))
            return;
    }
}
