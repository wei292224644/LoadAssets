using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;


[CustomEditor(typeof(AutoBake))]
public class AutoBakeEditor : Editor
{
    private readonly string str_AssetMainPath = "Assets/BakePrefab";
    private readonly string str_StreamingAssetsPath = "Assets/StreamingAssets";
    private readonly string str_BakeAssetBundlePath = "bakeABs";

    private readonly string str_IOSPath = "IOS";
    private readonly string str_MACPath = "MAC";
    private readonly string str_WINDOWPath = "WINDOW";

    public static string str_Skeletion = "-S";
    public static string str_AssetBundleVariant = "w3d";

    public static bool bl_BuildMac;
    public static bool bl_BuildWindow;
    public static bool bl_BuildIOS;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty tps = serializedObject.FindProperty("prefabs");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(tps, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        str_Skeletion = EditorGUILayout.TextField("Skeletion", str_Skeletion);
        str_AssetBundleVariant = EditorGUILayout.TextField("AssetBundle后缀", str_AssetBundleVariant);

        AutoBake bake = target as AutoBake;

        EditorGUILayout.Space();
        if (GUILayout.Button("设置所有的GameObject为Prefabs"))
        {
            SetAsset();
            SetPrefabAssetBundle();
        }

        if (GUILayout.Button("排序"))
        {
            bake.prefabs = bake.prefabs.OrderBy(go => go.name).ToArray();
        }

        if (GUILayout.Button("清除重复名称( 1 2 3 4 5)"))
        {
            RemoveErrorName();
        }

        if (GUILayout.Button("添加标识"))
        {
            SetDescribeScript();
        }

        if (GUILayout.Button("设置模型为静态"))
        {
            for (int i = 0; i < bake.prefabs.Length; i++)
                SetLightmapStatic(bake.prefabs[i].transform);
        }

        if (GUILayout.Button("禁止动画自动播放"))
        {
            SetAutoAnimation();
        }

        GUILayout.Space(40f);
        if (GUILayout.Button("烘焙"))
        {
            try
            {
                for (int i = 0; i < bake.prefabs.Length;)
                {
                    GameObject go = SetPrefab(bake.prefabs[i]);
                    Lightmapping.Bake();
                    CopyLightmap(go);
                    try
                    {
                        SetLightmapInfo(go);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }

                    //Apply prefab
                    bake.prefabs[i] = PrefabUtility.ReplacePrefab(go, bake.prefabs[i], ReplacePrefabOptions.ConnectToPrefab);

                    DestroyImmediate(go);
                    i++;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        GUILayout.Space(40f);

        bl_BuildMac = EditorGUILayout.Toggle("BuildMac", bl_BuildMac);
        bl_BuildWindow = EditorGUILayout.Toggle("BuildWindow", bl_BuildWindow);
        bl_BuildIOS = EditorGUILayout.Toggle("BuildIOS", bl_BuildIOS);

        if (GUILayout.Button("打包"))
        {
            CreateBakeFolder();
            BuildAssetsBundle();
        }

        GUILayout.Space(40f);
        SerializedProperty loads = serializedObject.FindProperty("gos_Loads");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(loads, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("预览"))
        {
            List<LightmapData> mapData = new List<LightmapData>();
            for (int i = 0; i < bake.gos_Loads.Length; i++)
            {
                GameObject go = Instantiate(bake.gos_Loads[i]);
                string goPath = AssetDatabase.GetAssetPath(bake.gos_Loads[i]);
                goPath = goPath.Replace("Assets/", "");
                goPath = goPath.Substring(0, goPath.LastIndexOf("/"));
                string[] path = Directory.GetFiles(Application.dataPath + "/" + goPath);
                for (int j = 1; j < path.Length; j++)
                {
                    if (path[j].Contains("meta"))
                        continue;

                    path[j] = path[j].Replace("\\", "/");
                    path[j] = path[j].Substring(path[j].LastIndexOf("Assets/"));

                    LightmapData data = null;
                    Texture2D obj = AssetDatabase.LoadAssetAtPath(path[j], typeof(Texture2D)) as Texture2D;
                    if (obj)
                    {
                        data = data ?? new LightmapData();

                        if (obj.name.Contains("far"))
                            data.lightmapFar = obj;
                        else if (obj.name.Contains("near"))
                            data.lightmapNear = obj;
                    }
                    if (data != null)
                        mapData.Add(data);
                }

                LightmapSettings.lightmaps = mapData.ToArray();


                LightmapInfo[] info = go.GetComponentsInChildren<LightmapInfo>();

                for (int j = 0; j < info.Length; j++)
                {
                    MeshRenderer renderer = info[j].GetComponent<MeshRenderer>();

                    for (int k = 0; k < mapData.Count; k++)
                    {
                        if (info[j].nameFar == mapData[k].lightmapFar.name)
                        {
                            renderer.lightmapIndex = k;
                            break;
                        }
                    }
                    renderer.lightmapScaleOffset = info[j].v4_Offset;
                }
            }
        }
    }

    private void RemoveErrorName()
    {
        AutoBake bake = target as AutoBake;
        for (int i = 0; i < bake.prefabs.Length; i++)
        {
            RemoveErrorName(bake.prefabs[i].transform);
        }
    }

    private void RemoveErrorName(Transform parent)
    {
        string name = parent.name.Substring(parent.name.Length - 2);

        if (Regex.IsMatch(name, @"^ \d?$"))
            parent.name = parent.name.Substring(0, parent.name.Length - 2);

        for (int i = 0; i < parent.childCount; i++)
            RemoveErrorName(parent.GetChild(i));
    }

    /// <summary>
    /// 禁止自动播放动画
    /// </summary>
    private void SetAutoAnimation()
    {
        AutoBake bake = target as AutoBake;

        for (int i = 0; i < bake.prefabs.Length; i++)
        {
            Animation anim = bake.prefabs[i].GetComponent<Animation>();

            if (anim != null)
                anim.playAutomatically = false;
        }
    }



    private void CopyLightmap(GameObject go)
    {
        AutoBake bake = target as AutoBake;

        LightmapData[] datas = LightmapSettings.lightmaps;

        for (int i = 0; i < datas.Length; i++)
        {
            string guid = go.name;

            if (datas[i].lightmapNear != null)
            {
                string nearPath = AssetDatabase.GetAssetPath(datas[i].lightmapNear);

                string nameNear = guid + "_" + i + "_near";
                datas[i].lightmapNear.name = nameNear;

                string fileName = nearPath.Substring(nearPath.LastIndexOf("/") + 1);
                string newPath = str_AssetMainPath + "/" + go.name + "/" + fileName;

                AssetDatabase.DeleteAsset(newPath);

                if (AssetDatabase.CopyAsset(nearPath, newPath))
                {
                    AssetDatabase.Refresh();
                    Texture2D texture2 = AssetDatabase.LoadAssetAtPath<Texture2D>(newPath);
                    texture2.name = nameNear;

                    BindAssetNameAndVariant(go, newPath);
                }
            }

            if (datas[i].lightmapFar != null)
            {
                string farPath = AssetDatabase.GetAssetPath(datas[i].lightmapFar);

                string nameFar = guid + "_" + i + "_far";
                datas[i].lightmapFar.name = nameFar;

                string fileName = farPath.Substring(farPath.LastIndexOf("/") + 1);
                string newPath = str_AssetMainPath + "/" + go.name + "/" + fileName;
                newPath = newPath.Substring(0, newPath.LastIndexOf('/') + 1) + nameFar + ".exr";

                AssetDatabase.DeleteAsset(newPath);

                if (AssetDatabase.CopyAsset(farPath, newPath))
                {
                    AssetDatabase.Refresh();

                    Texture2D texture2 = AssetDatabase.LoadAssetAtPath<Texture2D>(newPath);
                    texture2.name = nameFar;

                    BindAssetNameAndVariant(go, newPath);
                }
            }

        }
    }


    private void SetLightmapInfo(GameObject go)
    {
        try
        {
            MeshRenderer[] meshs = GameObject.FindObjectsOfType<MeshRenderer>();

            //info.maps = new LightmapInfo.MeshLightMap[meshs.Length];
            for (int i = 0; i < meshs.Length; i++)
            {
                if (meshs[i].lightmapIndex < 0)
                    continue;

                LightmapInfo info = meshs[i].GetComponent<LightmapInfo>() ??
                                    meshs[i].gameObject.AddComponent<LightmapInfo>();

                info.index = meshs[i].lightmapIndex;
                info.v4_Offset = meshs[i].lightmapScaleOffset;

                info.nameFar = LightmapSettings.lightmaps[info.index].lightmapFar.name;

                if (LightmapSettings.lightmapsMode != LightmapsMode.NonDirectional)
                    info.nameNear = LightmapSettings.lightmaps[info.index].lightmapNear.name;
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }



    /// <summary>
    /// 将模型转换成 LightmapStatic 模型
    /// </summary>
    /// <param name="parent"></param>
    private void SetLightmapStatic(Transform parent)
    {
        GameObjectUtility.SetStaticEditorFlags(parent.gameObject, StaticEditorFlags.LightmapStatic);

        for (int i = 0; i < parent.childCount; i++)
            SetLightmapStatic(parent.GetChild(i));
    }

    /// <summary>
    ///  将 prefab 放置到场景中
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private GameObject SetPrefab(GameObject go)
    {
        go = Instantiate(go);
        go.transform.name = go.transform.name.Replace("(Clone)", "");

        return go;
    }


    private void CreateBakeFolder()
    {
        //FileUtil.DeleteFileOrDirectory(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath);

        AssetDatabase.Refresh();

        if (!Directory.Exists(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath))
            AssetDatabase.CreateFolder(str_StreamingAssetsPath, str_BakeAssetBundlePath);

        if (bl_BuildWindow)
        {
            FileUtil.DeleteFileOrDirectory(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath + "/" + str_WINDOWPath);
            AssetDatabase.CreateFolder(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath, str_WINDOWPath);
        }
        if (bl_BuildIOS)
        {
            FileUtil.DeleteFileOrDirectory(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath + "/" + str_IOSPath);
            AssetDatabase.CreateFolder(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath, str_IOSPath);
        }
        if (bl_BuildMac)
        {
            FileUtil.DeleteFileOrDirectory(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath + "/" + str_MACPath);
            AssetDatabase.CreateFolder(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath, str_MACPath);
        }
    }


    private void BuildAssetsBundle()
    {
        if (bl_BuildIOS)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
            BuildPipeline.BuildAssetBundles(str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath + "/" + str_IOSPath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
        }
        if (bl_BuildMac)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSXUniversal);
            BuildPipeline.BuildAssetBundles(
                str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath + "/" + str_MACPath,
                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneOSXUniversal);
        }
        if (bl_BuildWindow)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
            BuildPipeline.BuildAssetBundles(
                str_StreamingAssetsPath + "/" + str_BakeAssetBundlePath + "/" + str_WINDOWPath,
                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
        }
    }


    /// <summary>
    ///  添加AssetBundleName 和 AssetBundleVariant
    /// </summary>
    private void BindAssetNameAndVariant(UnityEngine.Object obj, string path)
    {
        //UnityEngine.Object go = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        AssetImporter im = AssetImporter.GetAtPath(path);

        try
        {
            im.SetAssetBundleNameAndVariant(obj.name, str_AssetBundleVariant);
        }
        catch (Exception)
        {
            Debug.Log(obj.name + "--------添加 AssetBundleName  有问题。");
        }
    }

    /// <summary>
    ///  将所有的Prefab添加AssetBundleName 和 AssetBundleVariant
    /// </summary>
    private void SetPrefabAssetBundle()
    {
        AutoBake bake = target as AutoBake;

        for (int i = 0; i < bake.prefabs.Length; i++)
            BindAssetNameAndVariant(bake.prefabs[i], AssetDatabase.GetAssetPath(bake.prefabs[i]));
    }

    /// <summary>
    /// 为所有的Prefab添加  UnsModelDescribe.cs 脚本
    /// </summary>
    /// <param name="tran">添加 脚本的 gameobject</param>
    private void SetDescribeScript()
    {
        AutoBake bake = target as AutoBake;

        for (int i = 0; i < bake.prefabs.Length; i++)
        {
            Transform tran = bake.prefabs[i].transform;
            List<GameObject> goNames = new List<GameObject>();
            AddDescribe(goNames, tran);
            if (goNames.Count > 0)
            {
                UnsModelDescribe describe = null;
                if ((describe = tran.GetComponent<UnsModelDescribe>()) == null)
                    describe = tran.gameObject.AddComponent<UnsModelDescribe>();

                describe.gos_Skeleton = goNames.ToArray();
            }
        }
    }
    private void AddDescribe(List<GameObject> goNames, Transform tran)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            if (tran.GetChild(i).name.LastIndexOf(str_Skeletion, StringComparison.CurrentCulture) == tran.GetChild(i).name.Length - 2)
                goNames.Add(tran.GetChild(i).gameObject);

            if (tran.GetChild(i).childCount > 0)
                AddDescribe(goNames, tran.GetChild(i));
        }
    }

    /// <summary>
    /// 将 模型转换成 prefab 写入到 taget.str_bakePath 文件夹中
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private void SetAsset()
    {
        AutoBake bake = target as AutoBake;

        for (int i = 0; i < bake.prefabs.Length; i++)
        {
            GameObject obj = GameObject.Find(bake.prefabs[i].name) ?? Instantiate(bake.prefabs[i]);
            obj.name = obj.name.Replace("(Clone)", "");

            string path = AssetDatabase.GetAssetPath(obj);

            if (!Directory.Exists(str_AssetMainPath + "/" + obj.name))
                AssetDatabase.CreateFolder(str_AssetMainPath, obj.name);

            AssetDatabase.Refresh();

            string preName = str_AssetMainPath + "/" + obj.name + "/" + obj.name + ".prefab";

            if (File.Exists(preName))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(preName);

                LightmapInfo[] info = go.GetComponentsInChildren<LightmapInfo>();

                for (int j = 0; j < info.Length; j++)
                {
                    GameObject infoGo = GameObject.Find(info[j].gameObject.name);

                    LightmapInfo goInfo = infoGo.GetComponent<LightmapInfo>() ?? infoGo.AddComponent<LightmapInfo>();

                    goInfo.index = info[j].index;
                    goInfo.nameFar = info[j].nameFar;
                    goInfo.nameNear = info[j].nameNear;
                    goInfo.v4_Offset = info[j].v4_Offset;
                }


                if (path.Contains(str_AssetMainPath))
                    bake.prefabs[i] = PrefabUtility.ReplacePrefab(obj, bake.prefabs[i]);
                else
                    bake.prefabs[i] = PrefabUtility.CreatePrefab(preName, obj);
            }
            else
                bake.prefabs[i] = PrefabUtility.CreatePrefab(preName, obj);

            if (GameObject.Find(obj.name))
            {
                DestroyImmediate(obj);
            }
        }
    }
}
