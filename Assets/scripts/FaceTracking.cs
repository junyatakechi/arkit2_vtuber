using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using UnityEngine.XR.iOS;


public class FaceTracking : MonoBehaviour
{
    [Header("VRMモデル")]
    public Transform Head;
    public Transform Chest;

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
        axis.x = NormalizationNumbers(axis.x, 1.0f, 10f, 0f);
        axis.y = NormalizationNumbers(axis.y, 1.0f, 15f, 0f);
        axis.z = NormalizationNumbers(axis.z, 1.0f, 10f, 0f);
        Chest.localRotation = Quaternion.AngleAxis(angle, axis);
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
        float lookDown = (blendShapes[ARBlendShapeLocation.EyeLookDownLeft] + blendShapes[ARBlendShapeLocation.EyeLookDownRight]) / 2.0f;
        float lookUp = (blendShapes[ARBlendShapeLocation.EyeLookUpLeft] + blendShapes[ARBlendShapeLocation.EyeLookUpRight]) / 2.0f;
        float lookLeft = (blendShapes[ARBlendShapeLocation.EyeLookOutLeft] + blendShapes[ARBlendShapeLocation.EyeLookInRight]) / 2.0f;
        float lookRight = (blendShapes[ARBlendShapeLocation.EyeLookInLeft] + blendShapes[ARBlendShapeLocation.EyeLookOutRight]) / 2.0f;
        proxy.ImmediatelySetValue(BlendShapePreset.LookDown, lookDown);
        proxy.ImmediatelySetValue(BlendShapePreset.LookUp, lookUp);
        proxy.ImmediatelySetValue(BlendShapePreset.LookLeft, lookLeft);
        proxy.ImmediatelySetValue(BlendShapePreset.LookRight, lookRight);
    }

    private float NormalizationNumbers(float input, float coeffient, float max=1.0f, float min=0.0f)
    {
        var output = (Mathf.Pow(input, coeffient) - Mathf.Pow(min, coeffient)) / (Mathf.Pow(max, coeffient) - Mathf.Pow(min, coeffient));
        return output;
    }
}
