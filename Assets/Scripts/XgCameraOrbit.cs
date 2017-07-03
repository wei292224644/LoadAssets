using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Adaptation of the standard MouseOrbit script to use the finger drag gesture to rotate the current object using
/// the fingers/mouse around a target object
/// </summary>
public class XgCameraOrbit : MonoBehaviour
{
    public enum PanMode
    {
        Disabled,
        OneFinger,
        TwoFingers
    }

    /// <summary>
    /// The object to orbit around
    /// </summary>
    public Transform target;

    public Transform camRoot;
    public Transform camOrigin;

    //private XgIntellControl xg_IC;
    //private XgPictrueControl xg_Pc;

    /// <summary>
    /// Initial camera distance to target
    /// </summary>
    public float initialDistance = 10.0f;

    /// <summary>
    /// Minimum distance between camera and target
    /// </summary>
    public float minDistance = 1.0f;

    /// <summary>
    /// Maximum distance between camera and target
    /// </summary>
    public float maxDistance = 30.0f;

    /// <summary>
    /// Affects horizontal rotation speed (in degrees per cm)
    /// </summary>
    public float yawSensitivity = 45.0f;

    /// <summary>
    /// Affects vertical rotation speed (in degrees per cm)
    /// </summary>
    public float pitchSensitivity = 45.0f;

    /// <summary>
    /// Keep yaw angle value between minYaw and maxYaw?
    /// </summary>
    public bool clampYawAngle = false;

    public float minYaw = -75;
    public float maxYaw = 75;

    /// <summary>
    /// Keep pitch angle value between minPitch and maxPitch?
    /// </summary>
    public bool clampPitchAngle = true;

    public float minPitch = -20;
    public float maxPitch = 80;

    /// <summary>
    /// Allow the user to affect the orbit distance using the pinch zoom gesture
    /// </summary>
    public bool allowPinchZoom = true;

    /// <summary>
    /// Affects pinch zoom speed
    /// </summary>
    public float pinchZoomSensitivity = 5.0f;

    private float pinchZoomSensMax;
    private float pinchZoomSens;

    /// <summary>
    /// Use smooth camera motions?
    /// </summary>
    public bool smoothMotion = true;

    public float smoothZoomSpeed = 5.0f;
    public float smoothOrbitSpeed = 10.0f;

    /// <summary>
    /// Two-Finger camera panning.
    /// Panning will apply an offset to the pivot/camera target point
    /// </summary>
    public bool allowPanning = false;

    public bool invertPanningDirections = false;
    public float panningSensitivity = 1.0f;
    private float panningSensMax;
    private float panningSens;

    public Transform panningPlane;
    // reference transform used to apply the panning translation (using panningPlane.right and panningPlane.up vectors)

    public bool smoothPanning = true;
    public float smoothPanningSpeed = 12.0f;

    // collision test
    public LayerMask collisionLayerMask;
    private Transform myTrans;
    private float distance = 10.0f;
    private float yaw = 0;
    private float pitch = 0;
    private float idealDistance = 0;
    private float idealYaw = 0;
    private float idealPitch = 0;
    private Vector3 idealPanOffset = Vector3.zero;
    private Vector3 panOffset = Vector3.zero;
    private Vector3 rPanOffset;
    private bool slideCentered = false;
    private Vector3 slideOffset;
    private PinchRecognizer pinchRecognizer;
    private bool UIdrag = false;
    private bool canControl = true;
    private bool canInput = true;

    private float fl_IdeaMovePos;

    //messager
    //SLCameraMessager sCm;


    public bool UIDrag
    {
        get { return UIdrag; }
        set { UIdrag = value; }
    }

    public float Distance
    {
        get { return distance; }
    }

    public float IdealDistance
    {
        get { return idealDistance; }
        set { idealDistance = Mathf.Clamp(value, minDistance, maxDistance); }
    }

    public float Yaw
    {
        get { return yaw; }
    }

    public float IdealYaw
    {
        get { return idealYaw; }
        set { idealYaw = clampYawAngle ? ClampAngle(value, minYaw, maxYaw) : value; }
    }

    public float Pitch
    {
        get { return pitch; }
    }

    public float IdealPitch
    {
        get { return idealPitch; }
        set { idealPitch = clampPitchAngle ? ClampAngle(value, minPitch, maxPitch) : value; }
    }

    public Vector3 IdealPanOffset
    {
        get { return idealPanOffset; }
        set { idealPanOffset = value; }
    }

    public Vector3 PanOffset
    {
        get { return panOffset; }
    }

    public Vector3 ROffset
    {
        get { return rPanOffset; }
    }

    private void InstallGestureRecognizers()
    {
        //xg_IC = GameObject.Find(str_Intell_Control).GetComponent<XgIntellControl>();
        //xg_Pc = GameObject.Find(str_Pic_Control).GetComponent<XgPictrueControl>();

        List<GestureRecognizer> recogniers = new List<GestureRecognizer>(GetComponents<GestureRecognizer>());
        DragRecognizer drag = recogniers.Find(r => r.EventMessageName == "OnDrag") as DragRecognizer;
        DragRecognizer twoFingerDrag = recogniers.Find(r => r.EventMessageName == "OnTwoFingerDrag") as DragRecognizer;
        PinchRecognizer pinch = recogniers.Find(r => r.EventMessageName == "OnPinch") as PinchRecognizer;

        // check if we need to automatically add a screenraycaster
        if (OnlyRotateWhenDragStartsOnObject)
        {
            ScreenRaycaster raycaster = gameObject.GetComponent<ScreenRaycaster>();

            if (!raycaster)
                raycaster = gameObject.AddComponent<ScreenRaycaster>();
        }

        if (!drag)
        {
            drag = gameObject.AddComponent<DragRecognizer>();
            drag.RequiredFingerCount = 1;
            drag.IsExclusive = true;
            drag.MaxSimultaneousGestures = 1;
            drag.SendMessageToSelection = GestureRecognizer.SelectionType.None;
        }

        if (!pinch)
            pinch = gameObject.AddComponent<PinchRecognizer>();

        if (!twoFingerDrag)
        {
            twoFingerDrag = gameObject.AddComponent<DragRecognizer>();
            twoFingerDrag.RequiredFingerCount = 2;
            twoFingerDrag.IsExclusive = true;
            twoFingerDrag.MaxSimultaneousGestures = 1;
            twoFingerDrag.ApplySameDirectionConstraint = true;
            twoFingerDrag.EventMessageName = "OnTwoFingerDrag";
        }
    }

    public void Start()
    {


        InstallGestureRecognizers();

        myTrans = transform;

        if (!panningPlane)
            panningPlane = myTrans;

        Vector3 angles = myTrans.eulerAngles;

        distance = IdealDistance = initialDistance;
        yaw = IdealYaw = angles.y;
        pitch = IdealPitch = angles.x;

        pinchZoomSensMax = pinchZoomSensitivity;
        panningSensMax = panningSensitivity;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        Apply();

    }

    #region Gesture Event Messages

    private float nextDragTime = 0.5f;
    public bool OnlyRotateWhenDragStartsOnObject = false;

    private void OnDrag(DragGesture gesture)
    {
        if (canControl)
        {
            if (!canInput) return;
            if (!UIdrag)
            {
                // don't rotate unless the drag started on our target object
                if (OnlyRotateWhenDragStartsOnObject)
                {
                    if (gesture.Phase == ContinuousGesturePhase.Started)
                    {
                        if (!gesture.Recognizer.Raycaster)
                        {
                            Debug.LogWarning("The drag recognizer on " + gesture.Recognizer.name +
                                             " has no ScreenRaycaster component set. This will prevent OnlyRotateWhenDragStartsOnObject flag from working.");
                            OnlyRotateWhenDragStartsOnObject = false;
                            return;
                        }

                        if (target && !target.GetComponent<Collider>())
                        {
                            Debug.LogWarning(
                                "The target object has no collider set. OnlyRotateWhenDragStartsOnObject won't work.");
                            OnlyRotateWhenDragStartsOnObject = false;
                            return;
                        }
                    }

                    if (!target || gesture.StartSelection != target.gameObject)
                        return;
                }

                // wait for drag cooldown timer to wear off
                //  used to avoid dragging right after a pinch or pan, when lifting off one finger but the other one is still on screen
                if (Time.time < nextDragTime)
                    return;

                if (target)
                {
                    IdealYaw += gesture.DeltaMove.x.Centimeters() * yawSensitivity;
                    IdealPitch -= gesture.DeltaMove.y.Centimeters() * pitchSensitivity;

                }
            }
        }
    }

    private void OnPinch(PinchGesture gesture)
    {
        if (canControl)
        {
            if (!canInput) return;
            if (allowPinchZoom)
            {
                pinchZoomSens = Distance / 5.0f * pinchZoomSensMax;
                IdealDistance -= gesture.Delta.Centimeters() * pinchZoomSens;
                nextDragTime = Time.time + 0.25f;
            }
        }
    }

    private void OnTwoFingerDrag(DragGesture gesture)
    {
        if (canControl)
        {
            if (!canInput) return;
            if (allowPanning)
            {

                panningSens = Distance / 5.0f * panningSensMax;
                Vector3 move = -panningSens *
                                  (panningPlane.right * gesture.DeltaMove.x.Centimeters() +
                                   panningPlane.up * gesture.DeltaMove.y.Centimeters());

                if (invertPanningDirections)
                    IdealPanOffset = -move;
                else
                    IdealPanOffset = move;

                //reset idealPanPositon
                if (gesture.Phase == ContinuousGesturePhase.Ended)
                    IdealPanOffset = Vector3.zero;

                nextDragTime = Time.time + 0.25f;
            }
        }
    }

    private void OnTap(TapGesture gesture)
    {

    }


    //IN assemble mode, double tap to auto disassemble
    private void OnDoubleTap(TapGesture gesture)
    {

    }
    #endregion

    private void Apply()
    {

        if (smoothMotion)
        {
            distance = Mathf.Lerp(distance, IdealDistance, Time.deltaTime * smoothZoomSpeed);
            yaw = Mathf.LerpAngle(yaw, IdealYaw, Time.deltaTime * smoothOrbitSpeed);
            pitch = Mathf.LerpAngle(pitch, IdealPitch, Time.deltaTime * smoothOrbitSpeed);
        }
        else
        {
            distance = IdealDistance;
            yaw = IdealYaw;
            pitch = IdealPitch;
        }
        //print(IdealYaw);
        if (smoothPanning)
            panOffset = Vector3.Lerp(panOffset, idealPanOffset, Time.deltaTime * smoothPanningSpeed);
        else
            panOffset = idealPanOffset;
        if (canInput)
            rPanOffset = camOrigin.InverseTransformPoint(panOffset + myTrans.position);

        //rotate the orignal camera
        camOrigin.rotation = Quaternion.Euler(pitch, yaw, 0);

        //change rotation to the orginal camera
        myTrans.rotation = camOrigin.rotation;

        camOrigin.position = target.position - camOrigin.forward * distance;

        //set position for camera
        myTrans.position = camOrigin.TransformPoint(rPanOffset);

        //Debug.Log(rPanOffset);
    }


    private void LateUpdate()
    {
        if (canControl)
            Apply();
    }

    public void CameraProperty(bool pan, bool zoom)
    {

        allowPanning = pan;
        allowPinchZoom = zoom;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }


    public void SlideCenter(bool enable, Vector3 offset)
    {

        slideCentered = enable;
        if (enable)
        {
            slideOffset = offset;
        }
        else
        {
            slideOffset = Vector3.zero;
        }
    }

    public void CanControl(bool enable)
    {

        canControl = enable;
    }

    // recenter the camera
    public void ResetPanning()
    {
        IdealPanOffset = Vector3.zero;
    }


    //获取角度
    public float GetCamYawAngle()
    {
        return yaw % 360 < 0 ? 360 + yaw % 360 : Yaw % 360;
    }

    public float GetCamPitchAngle()
    {
        return pitch;
    }

    public IEnumerator PauseControl(float time)
    {

        canControl = false;

        yield return new WaitForSeconds(time + 0.1f);

        canControl = true;
    }


    public void PauseInput(float time)
    {
        StopCoroutine("PauseCamInput");
        StartCoroutine("PauseCamInput", time);
    }

    IEnumerator PauseCamInput(float time)
    {
        canInput = false;
        yield return new WaitForSeconds(time);
        canInput = true;
    }

    public void TeleportCam(Transform pivot, Vector3 originPos, Vector3 rot, float dis, Vector3 pan)
    {

        target = pivot;
        distance = idealDistance = dis;

        yaw = idealYaw = rot.y >= 180 ? rot.y - 360 : rot.y;
        pitch = idealPitch = rot.x >= 180 ? rot.x - 360 : rot.x;

        panOffset = idealPanOffset = Vector3.zero;

        rPanOffset = pan;

        camOrigin.position = originPos;
        camOrigin.eulerAngles = rot;
    }

    public void OnDistance(float dis)
    {

        //Debug.Log("distance: " + dis);
        distance = dis;
        idealDistance = dis;
    }

    public void OnViewAngle(Vector3 angle)
    {
        //Debug.Log("angle: " + angle);
        camOrigin.eulerAngles = angle;
        yaw = idealYaw = angle.y >= 180 ? angle.y - 360 : angle.y;
        pitch = idealPitch = angle.x >= 180 ? angle.x - 360 : angle.x;
    }

    public void OnPanOffset(Vector3 offset)
    {
        panOffset = idealPanOffset = Vector3.zero;
        rPanOffset = offset;
    }

    public void SetPointLimit(Vector2 distance, Vector2 pitch)
    {
        minDistance = distance.x;
        maxDistance = distance.y;

        minPitch = pitch.x;
        maxPitch = pitch.y;
    }
}
//381 355
