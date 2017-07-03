using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

[Serializable]
public class UnsMachineLogic
{
    public AnglePosition AnglePos;
    public MachineLogicMainInfo Main;
    public MachineLogicInfomation Infomation;
    public MachineLogicProcess Process;
    public MachineLogicMatch Match;

    public class LogicParent
    {

    }

    [Serializable]
    public class AnglePosition
    {
        public CameraPosition MainPos;
        public CameraPosition InfoPos;
        public CameraPosition ProcessPos;
        public CameraPosition ProcessPartPos;
        public CameraPosition MatchPos;
        public CameraPosition MatchPreviewPos;
        public CameraPosition SizePos;
        public CameraPosition ExplodePos;
        public CameraPosition ProcessSizePos;
        public CameraPosition[] ExplodeInfoPos;
        public CameraPosition[] MatchInfoPos;
    }

    [Serializable]
    public class MachineLogicMainInfo : LogicParent
    {
        public AnimationMain[] Animations;
        public MachineState State;

        [Serializable]
        public class AnimationMain
        {
            public string Collider;
            public string[] Anims;

            public GameObject c_go_Collider;
            public Animation[] c_animations;
        }
    }

    [Serializable]
    public class MachineLogicInfomation : LogicParent
    {
        public MachineSize SizeInfo;
        public MachineExplode ExplodeInfo;
        public MachineProcessRange ProcessRangeInfo;

        [Serializable]
        public class MachineSize
        {
            public MachineState SideChipState;
            public MachineState AfterChipState;
            public MachineSizeAngle[] SizeAngle;


            [Serializable]
            public class MachineSizeAngle
            {
                public float MinPitch;
                public float MaxPitch;

                public MachineSizeAngleYaw[] Yaw;

                [Serializable]
                public class MachineSizeAngleYaw
                {
                    public float MinAngle;
                    public float MaxAngle;
                    public string[] Models_AfterChip;
                    public string[] Models_SideChip;

                    public GameObject[] c_gos_AfterChip;
                    public GameObject[] c_gos_SideChip;
                }
            }
        }

        [Serializable]
        public class MachineExplode
        {
            public MachineExplodeMessage[] InfoMessage;
            public MachineState InfoOther;
            public string[] Animations;

            public Animation[] c_animations;

            [Serializable]
            public class MachineExplodeMessage
            {
                public string Collider;
                public string Model;

                public GameObject c_go_Collider;
                public GameObject c_go_Model;
            }
        }

        [Serializable]
        public class MachineProcessRange
        {
            public MachineState State;
            public string[] Animations;

            public Animation[] c_animations;
        }
    }

    [Serializable]
    public class MachineLogicProcess : LogicParent
    {
        public MachineLogicProcInfo[] Infomation;
        public string[] Animations;
        public MachineState PartState;

        public Animation[] c_animations;

        [Serializable]
        public class MachineLogicProcInfo
        {
            public string[] Model;
            public string Name;
            public int Image_ID;
            public int Video_ID;

            public GameObject[] c_gos_Model;
        }
    }

    [Serializable]
    public class MachineLogicMatch : LogicParent
    {
        public MachineLogicMatchPlan[] Plan;
        public MachineLoMatPlPartOther Other;

        [Serializable]
        public class MachineLogicMatchPlan
        {
            public string Transparent;
            public MachineLoMatPlPartEntity[] Parts;

            public GameObject c_go_Transparent;

            [Serializable]
            public class MachineLoMatPlPartEntity
            {
                public string[] Entity;

                public GameObject[] c_gos_Entity;
            }
        }

        [Serializable]
        public class MachineLoMatPlPartOther
        {
            public string[] Transparent;
            public string[] Entity;

            public GameObject[] c_gos_Transparent;
            public GameObject[] c_gos_Entity;
        }
    }

    [Serializable]
    public class MachineState
    {
        public string[] Hide;
        public string[] Show;

        public GameObject[] c_gos_Hide;
        public GameObject[] c_gos_Show;
    }

    private static string dataPath = "/Resources/EditorJson/";
    private static string assetPath = Application.dataPath + dataPath;
    private static string dataHead = "Assets";


    public static void Save(UnsMachineLogic logic)
    {
        string json = RemoveOther(JsonUtility.ToJson(logic));

        FileStream fs = new FileStream(assetPath + DateTime.Now.ToString("yyyyMMdhhmmss") + ".json", FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(json);
        sw.Dispose();
        fs.Dispose();

        AssetDatabase.Refresh();
    }

    private static string RemoveOther(string json)
    {
        //((\,)?)(\"(\w+)\":)({"instanceID":(-?)(\d+)\}(\,)?)+
        //((\,)?)(\"(\w+)\":)\[({"instanceID":(\-)?\d+\}(\,)?)*]

        json = Regex.Replace(json, "(\\\"c_(\\w+)\\\":)({\"instanceID\":(-?)(\\d+)\\})+", "");
        json = Regex.Replace(json, "(\\\"c_(\\w+)\\\":)\\[({\"instanceID\":(\\-)?\\d+\\}(\\,)?)*]", "");
        json = Regex.Replace(json, "(\\,){2,}", ",");
        json = Regex.Replace(json, "(\\,})", "}");
        return json;
    }
}