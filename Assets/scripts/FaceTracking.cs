using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using UnityEngine.XR.iOS;
using UniRx;
using UnityEngine.UI;


public class FaceTracking : MonoBehaviour
{
    [Header("VRMモデル")]
    public Transform Head;
    public Transform Chest;
    public Text view_angle;
    public Text view_axisz;
    public Text view_axisy;
    public Text view_axisx;

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
        //オプションを追加
        var options = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;

        if (configuration.IsSupported)
        {
            UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdd;
            UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdate;
            UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
        }
        else
        {
            // 利用できない場合の処理
        }

        //オプションをセット
        Session.RunWithConfigAndOptions(configuration, options);
    }

    public void RestARFace() {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdd;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdate;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent -= FaceRemoved;
        Session = null;
        InitializeARSession();
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

    void FaceRemoved(ARFaceAnchor anchorData)
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
        view_axisz.text = string.Format("axisz:{0}", axis.z); //debug////////////////////////////////
        view_axisy.text = string.Format("axisy:{0}", axis.y); //debug////////////////////////////////
        view_axisx.text = string.Format("axisx:{0}", axis.x); //debug////////////////////////////////
        view_angle.text = string.Format("angle:{0}", angle); //debug////////////////////////////////
        //head
        Head.localRotation = Quaternion.AngleAxis(angle, axis);
        //chest
        Vector3 chest_axis = axis;
        float chest_angle = angle;
        if (chest_angle < 345f) {chest_angle = 345f;} //limited
        Observable.TimerFrame(5).Subscribe(_ =>
            {
                Chest.localRotation = Quaternion.AngleAxis(chest_angle, chest_axis);
            }
        );
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
            proxy.ImmediatelySetValue(BlendShapePreset.LookRight, eyes_points.x * 1.5f);
        }
        else
        {
            proxy.ImmediatelySetValue(BlendShapePreset.LookLeft, Mathf.Abs(eyes_points.x * 1.5f));
        }
    }

    private float NormalizationNumbers(float input, float coeffient, float max=1.0f, float min=0.0f)
    {
        var output = (Mathf.Pow(input, coeffient) - Mathf.Pow(min, coeffient)) / (Mathf.Pow(max, coeffient) - Mathf.Pow(min, coeffient));
        return output;
    }


}
