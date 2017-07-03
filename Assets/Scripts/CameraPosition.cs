using System;
using UnityEngine;

[Serializable]
public class CameraPosition 
{
    public float[] CenterPos;
    private Vector3 m_v3CenterPos;

    public float[] TranPos;
    private Vector3 m_v3TranPos;

    public float[] TranFor;
    private Vector3 m_v3TranFor;

    public float[] TranRot;
    private Vector3 m_v3TranRot;

    public float[] TranOffset;
    private Vector3 m_v3TranOffset;

    public float[] Distance;
    private Vector2 m_v2Distance;

    public float[] PitchAngle;
    private Vector2 m_v2PitchAngle;

    [NonSerialized]
    public Vector3 v3_CenterPos;
    [NonSerialized]
    public Vector3 v3_TranPos;
    [NonSerialized]
    public Vector3 v3_TranFor;
    [NonSerialized]
    public Vector3 v3_TranRot;
    [NonSerialized]
    public Vector3 v3_TranOffset;
    [NonSerialized]
    public Vector2 v2_Distance;
    [NonSerialized]
    public Vector2 v2_PitchAngle;
    [NonSerialized]
    public string remark = "";
    [NonSerialized]
    public bool bl_Editor = false;
}