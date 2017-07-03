using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadAssets : MonoBehaviour
{
    void Start()
    {
        //StartCoroutine(LoadLightmap("lightmap1", "model1"));
        //StartCoroutine(LoadLightmap("lightmap2", "model2"));
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        IEnumerator ie = LoadMat();
        yield return ie;

		ie = LoadModel("t33_waifanghu.w3d");
        yield return ie;
    }

    IEnumerator LoadModel(string name)
    {
		WWW www = new WWW("file://" + Application.streamingAssetsPath + "/bakeABs/WINDOW/" + name);

        yield return www;

        var bundle = www.assetBundle;
        string[] assetNames = bundle.GetAllAssetNames();

        List<LightmapData> data = new List<LightmapData>(LightmapSettings.lightmaps);

        GameObject go = null;
        for (int i = 0; i < assetNames.Length; i++)
        {
            Object obj = bundle.LoadAsset(assetNames[i]);

            if (obj as Texture2D)
            {
                LightmapData lidata = new LightmapData
                {
                    lightmapFar = obj as Texture2D,
                    lightmapNear = null
                };

                data.Add(lidata);
            }
            else
            {
                go = Instantiate(obj) as GameObject;
            }
        }
        LightmapSettings.lightmaps = null;
        LightmapSettings.lightmaps = data.ToArray();

        Renderer[] rndrs = GameObject.FindObjectsOfType<Renderer>();
        for (int y = 0; y < rndrs.Length; y++)
        {
            Renderer renderer = rndrs[y];
            LightmapInfo info = rndrs[y].GetComponent<LightmapInfo>();
            renderer.gameObject.isStatic = true;
            renderer.lightmapIndex = 0;
            renderer.lightmapScaleOffset = info.v4_Offset;
            renderer.realtimeLightmapIndex = 0;
            renderer.realtimeLightmapScaleOffset = info.v4_Offset;

            //for (int k = 0; k < data.Count; k++)
            //{
            //    if (info.nameFar == data[k].lightmapFar.name)
            //    {
            //        break;
            //    }
            //}
        }


    }

    IEnumerator LoadGo(string path)
    {
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/" + path);

        yield return www;

        var bundle = www.assetBundle;
        string[] assetNames = bundle.GetAllAssetNames();
        for (int i = 0; i < assetNames.Length; i++)
        {
            GameObject go = Instantiate(bundle.LoadAsset(assetNames[i])) as GameObject;
        }

        LightmapInfo[] info = GameObject.FindObjectsOfType<LightmapInfo>();
        LightmapData[] datas = LightmapSettings.lightmaps;
        for (int j = 0; j < info.Length; j++)
        {
            MeshRenderer renderer = info[j].GetComponent<MeshRenderer>();
            //renderer.lightmapScaleOffset = info[j].v4_Offset;
            for (int k = 0; k < datas.Length; k++)
                if (info[j].name == datas[k].lightmapFar.name)
                    renderer.lightmapIndex = k;
        }
    }

    IEnumerator LoadMat()
    {
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/bakeABs/IOS/material.w3d");

        yield return www;

        var bundle = www.assetBundle;
        //bundle.LoadAsset(name);
    }

    IEnumerator LoadTexture(string path)
    {
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/" + path);

        yield return www;

        var bundle = www.assetBundle;
        bundle.LoadAllAssets();
    }

    IEnumerator LoadLightmap(string path, string modelname)
    {
        WWW www = new WWW("file://" + Application.streamingAssetsPath + "/" + path);

        yield return www;

        var bundle = www.assetBundle;
        bundle.LoadAllAssets();
        string[] assetNames = bundle.GetAllAssetNames();
        List<LightmapData> lightmapDatas = new List<LightmapData>(LightmapSettings.lightmaps);
        for (int i = 0; i < assetNames.Length; i++)
        {
            LightmapData data = new LightmapData();
            data.lightmapFar = bundle.LoadAsset(assetNames[i]) as Texture2D;
            lightmapDatas.Add(data);
        }

        LightmapSettings.lightmaps = lightmapDatas.ToArray();

        StartCoroutine(LoadGo(modelname));
    }
}
