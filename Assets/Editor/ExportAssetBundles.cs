using UnityEngine;
using UnityEditor;

public class ExportAssetBundles : MonoBehaviour
{

    [MenuItem("Custom Editor/Build AssetBundles")]
    public static void CreateAssetBundlesMain()
    {
        //        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets");
    }

    [MenuItem("Custom Editor/SetLightmapInfo")]
    public static void SetLightmapIndex()
    {
        MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            LightmapInfo info = renderers[i].GetComponent<LightmapInfo>();

            if (info != null)
                DestroyImmediate(info);

            info = renderers[i].gameObject.AddComponent<LightmapInfo>();
            //info.nameFar = LightmapSettings.lightmaps[renderers[i].lightmapIndex].lightmapFar.name;
            //info.v4_Offset = renderers[i].lightmapScaleOffset;
        }
    }

    [MenuItem("Custon Editor/SetLightmap")]
    static void SetLightmap()
    {
        MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            LightmapInfo info = renderers[i].GetComponent<LightmapInfo>();


            //renderers[i].lightmapIndex = info.index;
            //renderers[i].lightmapScaleOffset = info.v4_Offset;
        }
    }

    [MenuItem("Custom Editor/GetLightmap")]
    static void GetLightmap()
    {
        MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Debug.Log("index:" + renderers[i].lightmapIndex + "offset:" + renderers[i].lightmapScaleOffset);
        }
    }
}
