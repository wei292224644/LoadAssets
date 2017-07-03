using UnityEngine;
using System.Collections.Generic;

public class TransformOrActive : MonoBehaviour
{
    public List<TransformOrActiveInfo> tOAInfos;
    [HideInInspector]
    public bool bl_IsBoom = false;

    public GameObject[] gos;
    public bool bl_IsActice = false;
}

[System.Serializable]
public class TransformOrActiveInfo
{
    public GameObject go;
    public Vector3 v3_TransformOffset;
    public Vector3 v3_TransfromAngle;

    [HideInInspector]
    public Vector3 v3_TransformInit;
    
    [HideInInspector]
    public Vector3 v3_TransformAngleInit;

    [HideInInspector]
    public bool bl_IsInit = false;
    [HideInInspector]
    public bool bl_TranState = false;
    [HideInInspector]
    public GameObject currentGo;
}