using System;
using UnityEngine;

public class SetMD5 : MonoBehaviour
{
    public UnityEngine.Object[] gos_Object;
    public MD5Info[] infos;

    [Serializable]
    public class MD5Info
    {
        public int Model_Id;
        public string MD5;
        public string Name;
        public string Manifest_Path;
        public string Model_Path;
    }

}


