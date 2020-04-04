using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using UnityEngine.XR.iOS;
using UniRx;


public class FaceTracking : MonoBehaviour
{
    [Header("VRMモデル")]
    public Transform Head;
    public Transform Chest;
    public Transform Hip;

    public VRMBlendShapeProxy proxy;

    private UnityARSessionNativeInterface Session;


    // Start is called before the first frame update
    void Start()
    {
        InitializeARSession();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void InitializeARSession()
    {
        Session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        ARKitFaceTrackingConfiguration configuration = new ARKitFaceTrackingConfiguration();
        configuration.alignment = UnityARAlignment.UnityARAlignmentGravity;
        configuration.enableLightEstimation = true;

        if (configuration.IsSupported)
        {
            UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdd;
            UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdate;
            UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaeRemoved;
        }
        else
        {
            // 利用できない場合の処理
        }

        Session.RunWithConfig(configuration);
    }


    void FaceAdd(ARFaceAnchor anchorData)
    {
        UpdateHead(anchorData);
        UpdateFace(anchorData);
    }

    void FaceUpdate(ARFaceAnchor anchorData)
    {
        UpdateHead(anchorData);
        UpdateFace(anchorData);
    }

    void FaeRemoved(ARFaceAnchor anchorData)
    {
        // 顔の認識ができなくなった場合の処理
    }

    void UpdateHead(ARFaceAnchor anchorData)
    {
        // ARKitが右手軸なのをUnityの左手軸に変更と水平坑はミラーにするように変更
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        var rot = UnityARMatrixOps.GetRotation(anchorData.transform);
        rot.ToAngleAxis(out angle, out axis);
        axis.x = -axis.x;
        axis.z = -axis.z;
        //head
        Head.localRotation = Quaternion.AngleAxis(angle, axis);
        //chest
        Vector3 chest_angle = axis;
        Observable.TimerFrame(6).Subscribe(_ =>
            {
                chest_angle.x = NormalizationNumbers(chest_angle.x, 1.0f, 0.5f, -0.5f);
                chest_angle.y = NormalizationNumbers(chest_angle.y, 1.0f, 0.5f, -0.5f);
                chest_angle.z = 0.0f;
                Chest.localRotation = Quaternion.AngleAxis(angle, axis);
            }
        );
        //Observable.TimerFrame(12).Subscribe(_ =>
        //    {
        //        //hip
        //        axis.x = 0;
        //        axis.y = NormalizationNumbers(axis.y, 1.0f, 3.0f, 0.0f);
        //        axis.z = 0;
        //        Hip.localRotation = Quaternion.AngleAxis(angle, axis);
        //    }
        //);
    }

    void UpdateFace(ARFaceAnchor anchorData)
    {
        var blendShapes = anchorData.blendShapes;
        if (blendShapes == null || blendShapes.Count == 0)
            return;
        //mouth shape
        proxy.ImmediatelySetValue(BlendShapePreset.O, NormalizationNumbers(blendShapes[ARBlendShapeLocation.JawOpen], 0.7f));
        proxy.ImmediatelySetValue(BlendShapePreset.I, NormalizationNumbers(blendShapes[ARBlendShapeLocation.JawForward], 0.7f));
        proxy.ImmediatelySetValue(BlendShapePreset.U, NormalizationNumbers(blendShapes[ARBlendShapeLocation.MouthPucker], 0.7f));
        //blink shape
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_L, blendShapes[ARBlendShapeLocation.EyeBlinkLeft]);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_R, blendShapes[ARBlendShapeLocation.EyeBlinkRight]);
        //eyes shape
        Vector3 eyes_points = anchorData.lookAtPoint;
        if (eyes_points.y > 0.0f)
        {
            proxy.ImmediatelySetValue(BlendShapePreset.LookUp, eyes_points.y);
        }
        else {
            proxy.ImmediatelySetValue(BlendShapePreset.LookDown, Mathf.Abs(eyes_points.y));
        }

        if (eyes_points.x > 0.0f)
        {
            proxy.ImmediatelySetValue(BlendShapePreset.LookRight, eyes_points.x);
        }
        else
        {
            proxy.ImmediatelySetValue(BlendShapePreset.LookLeft, Mathf.Abs(eyes_points.x));
        }
    }

    private float NormalizationNumbers(float input, float coeffient, float max=1.0f, float min=0.0f)
    {
        var output = (Mathf.Pow(input, coeffient) - Mathf.Pow(min, coeffient)) / (Mathf.Pow(max, coeffient) - Mathf.Pow(min, coeffient));
        return output;
    }
}
