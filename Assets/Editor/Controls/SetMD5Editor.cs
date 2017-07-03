using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetMD5))]
public class SetMD5Editor : Editor
{
    public static string serverPath = "http://192.168.6.168/abs/";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SetMD5 md = target as SetMD5;

        if (GUILayout.Button("排序"))
        {
            md.gos_Object= md.gos_Object.OrderBy(go => go.name).ToArray();
        }

        if (GUILayout.Button("点击"))
        {
            md.infos = new SetMD5.MD5Info[md.gos_Object.Length];
            for (int i = 0; i < md.gos_Object.Length; i++)
            {
                md.infos[i] = new SetMD5.MD5Info
                {
                    Model_Id = i,
                    MD5 = GetMD5(md.gos_Object[i]),
                    Name =  md.gos_Object[i].name,
                    Model_Path = serverPath + md.gos_Object[i].name + ".w3d",
                    Manifest_Path = serverPath + md.gos_Object[i].name + ".w3d.manifest"
                };
            }
            string json = "[";
            for (int i = 0; i < md.infos.Length; i++)
            {
                json += JsonUtility.ToJson(md.infos[i]);
                if (i != md.infos.Length - 1)
                    json += ",";
            }
            json += "]";
            Debug.Log(json);

            FileStream stream = new FileStream(Application.streamingAssetsPath + "/json.json", FileMode.OpenOrCreate);

            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(json);

            writer.Dispose();
            stream.Dispose();
        }
    }

    private string GetMD5(Object go)
    {
        string datapath = Application.dataPath.Replace("/Assets", "/");
        byte[] bytes = File.ReadAllBytes(datapath + AssetDatabase.GetAssetPath(go));

        System.Security.Cryptography.MD5 md5;
        md5 = System.Security.Cryptography.MD5.Create();

        // 生成16位的二进制校验码
        byte[] hashBytes = md5.ComputeHash(bytes);

        // 转为32位字符串
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}