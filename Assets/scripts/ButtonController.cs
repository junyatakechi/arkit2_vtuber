using UnityEngine;
using UnityEngine.UI;
using VRM;

public class ButtonController : MonoBehaviour
{
    public Text text;
    public VRMBlendShapeProxy proxy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// Retarget Face tracking
    public void RetargetFaceButton()
    {
        //var f = new FaceTracking();
        //f.InitializeARSession();
    }

    // click button
    public void sad_button()
    {
        RestBlendShape();
        proxy.ImmediatelySetValue(BlendShapePreset.Sorrow, 1.0f);
    }

    public void happy_button()
    {
        RestBlendShape();
        proxy.ImmediatelySetValue(BlendShapePreset.Joy, 1.0f);
    }

    public void angy_button()
    {
        RestBlendShape();
        proxy.ImmediatelySetValue(BlendShapePreset.Angry, 1.0f);
    }

    public void surprise_button()
    {
        RestBlendShape();
        proxy.ImmediatelySetValue(BlendShapePreset.Fun, 1.0f);
    }

    public void neutral_button()
    {
        RestBlendShape();
        proxy.ImmediatelySetValue(BlendShapePreset.Neutral, 1.0f);
    }

    private void RestBlendShape()
    {
        //rest value
        proxy.ImmediatelySetValue(BlendShapePreset.Neutral, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.A, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.I, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.U, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.E, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.O, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_L, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Blink_R, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Angry, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Fun, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Joy, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.Sorrow, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.LookUp, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.LookDown, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.LookLeft, 0.0f);
        proxy.ImmediatelySetValue(BlendShapePreset.LookRight, 0.0f);
    }


}
