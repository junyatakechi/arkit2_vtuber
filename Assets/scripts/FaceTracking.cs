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
        proxy.ImmediatelySetValue(BlendShapePreset.O, blendShapes[ARBlendShapeLocation.JawOpen]);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_L, blendShapes[ARBlendShapeLocation.EyeBlinkLeft]);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_R, blendShapes[ARBlendShapeLocation.EyeBlinkRight]);
    }
}
