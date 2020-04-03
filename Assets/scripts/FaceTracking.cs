using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using UnityEngine.XR.iOS;

public class FaceTracking : MonoBehaviour
{
    [Header("VRMモデル")]
    public Transform Head;
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
        Head.localRotation = Quaternion.AngleAxis(angle, axis);

    }

    void UpdateFace(ARFaceAnchor anchorData)
    {
        var blendShapes = anchorData.blendShapes;
        if (blendShapes == null || blendShapes.Count == 0)
            return;
        //mouth shape
        proxy.ImmediatelySetValue(BlendShapePreset.A, NormalizationNumbers(blendShapes[ARBlendShapeLocation.JawOpen], (float)0.7));
        proxy.ImmediatelySetValue(BlendShapePreset.I, NormalizationNumbers(blendShapes[ARBlendShapeLocation.JawForward], (float)0.7));
        proxy.ImmediatelySetValue(BlendShapePreset.U, NormalizationNumbers(blendShapes[ARBlendShapeLocation.MouthPucker], (float)0.7));
        //blink shape
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_L, blendShapes[ARBlendShapeLocation.EyeBlinkLeft]);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_R, blendShapes[ARBlendShapeLocation.EyeBlinkRight]);
        //eyes shape
        float lookDown = (blendShapes[ARBlendShapeLocation.EyeLookDownLeft] + blendShapes[ARBlendShapeLocation.EyeLookDownRight]) / 2;
        float lookUp = (blendShapes[ARBlendShapeLocation.EyeLookUpLeft] + blendShapes[ARBlendShapeLocation.EyeLookUpRight]) / 2;
        float lookLeft = (blendShapes[ARBlendShapeLocation.EyeLookOutLeft] + blendShapes[ARBlendShapeLocation.EyeLookInRight]) / 2;
        float lookRight = (blendShapes[ARBlendShapeLocation.EyeLookInLeft] + blendShapes[ARBlendShapeLocation.EyeLookOutRight]) / 2;
        proxy.ImmediatelySetValue(BlendShapePreset.LookDown, NormalizationNumbers(lookDown, (float)0.7));
        proxy.ImmediatelySetValue(BlendShapePreset.LookUp, NormalizationNumbers(lookUp, (float)0.7));
        proxy.ImmediatelySetValue(BlendShapePreset.LookLeft, NormalizationNumbers(lookLeft, (float)0.7));
        proxy.ImmediatelySetValue(BlendShapePreset.LookRight, NormalizationNumbers(lookRight, (float)0.7));
    }

    private float NormalizationNumbers(float input, float coeffient)
    {
        var max = 1;
        var min = 0;
        var output = (Mathf.Pow(input, coeffient) - Mathf.Pow(min, coeffient)) / (Mathf.Pow(max, coeffient) - Mathf.Pow(min, coeffient));
        return output;
    }
}
