using System;
using UnityEngine;

public class XgCameraControl : MonoBehaviour
{
    private static XgCameraControl _instacne;

    public static XgCameraControl instance
    {
        get { return _instacne; }
    }

    public delegate void TravelCallback();
    public delegate void CACProcess();

    public float fl_Time;
    private float time_Shift = 0.8f;

    private static iTween.EaseType posEaseType = iTween.EaseType.easeOutQuad;
    private static iTween.EaseType rotEaseType = iTween.EaseType.easeOutQuad;

    private XgCameraOrbit orbit
    {
        get
        {
            if (_orbit == null)
            {
                _orbit = tran_Camera.GetComponent<XgCameraOrbit>();
            }
            return _orbit;
        }
    }

    private XgCameraOrbit _orbit;

    private Transform m_tranCamera;
    private Transform tran_Camera
    {
        get
        {
            if (m_tranCamera == null)
                m_tranCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

            return m_tranCamera;
        }
    }

    void Awake()
    {
        _instacne = this;
    }

    /// <summary>
    /// 设置相机的Yaw
    /// </summary>
    /// <param name="yaw"></param>
    public void CamAutoCorrection(float yaw)
    {
        orbit.IdealYaw = yaw;
    }

    /// <summary>
    /// 获取相机角度
    /// </summary>
    /// <returns></returns>
    public float GetCamYawAngle()
    {
        return orbit.GetCamYawAngle();
    }

    public float GetCamPitchAngle()
    {
        return orbit.GetCamPitchAngle();
    }

    public void SetPointLimit(Vector2 distance, Vector2 pitch)
    {
        orbit.minDistance = distance.x;
        orbit.maxDistance = distance.y;

        orbit.minPitch = pitch.x;
        orbit.maxPitch = pitch.y;
    }

    public void TravelToPoint(CameraPosition pos)
    {
        SetPointLimit(pos.v2_Distance, pos.v2_PitchAngle);

        Transform camTrans = tran_Camera.transform;

        Vector3 desiredPos = pos.v3_CenterPos - Vector3.Project(pos.v3_CenterPos - pos.v3_TranPos, pos.v3_TranFor);
        float newDistance = Vector3.Distance(desiredPos, pos.v3_CenterPos);
        Vector3 newOffset = pos.v3_TranOffset == Vector3.zero ? pos.v3_TranPos - desiredPos : pos.v3_TranOffset;
        Vector3 newViewAngle = pos.v3_TranRot;

        Vector3 oldViewAngle = camTrans.eulerAngles;
        //print(newViewAngle - oldViewAngle);
        float oldDistance = orbit.Distance;
        Vector3 oldOffset = orbit.ROffset;

        float time = time_Shift;

        StopAllCoroutines();
        orbit.PauseInput(time + 0.1f);

        //shifting
        iTween.MoveTo(orbit.target.gameObject, iTween.Hash("position", pos.v3_CenterPos, "time", time, "easeType", posEaseType));

        //shift view angles
        float anplyYDiff = oldViewAngle.y - newViewAngle.y;
        float anglyXDiff = oldViewAngle.x - newViewAngle.x;

        if (Mathf.Abs(anplyYDiff) > 180)
            newViewAngle = new Vector3(newViewAngle.x, oldViewAngle.y + (anplyYDiff > 0 ? 360 - anplyYDiff : -(360 + anplyYDiff)), newViewAngle.z);

        if (Mathf.Abs(anglyXDiff) > 180)
            newViewAngle = new Vector3(oldViewAngle.x + (anglyXDiff > 0 ? 360 - anglyXDiff : -(360 + anglyXDiff)), newViewAngle.y, newViewAngle.z);

        iTween.ValueTo(tran_Camera.gameObject, iTween.Hash(
                 "from", oldViewAngle,
                 "to", newViewAngle,
                 "time", time,
                 "onupdatetarget", tran_Camera.gameObject,
                 "onupdate", "OnViewAngle",
                 "easetype", rotEaseType));

        //shift view distance
        iTween.ValueTo(tran_Camera.gameObject, iTween.Hash(
            "from", oldDistance,
            "to", newDistance,
            "time", time,
            "onupdatetarget", tran_Camera.gameObject,
            "onupdate", "OnDistance",
            "easetype", rotEaseType));

        //shift view offset
        iTween.ValueTo(tran_Camera.gameObject, iTween.Hash(
            "from", oldOffset,
            "to", newOffset,
            "time", time,
            "onupdatetarget", tran_Camera.gameObject,
            "onupdate", "OnPanOffset",
            "easetype", rotEaseType));

    }
}

[Serializable]
public class TravelPosition
{
    public Transform tran_Center;
    public Transform tran_Pos;
    public Vector3 v3_Offset;
}