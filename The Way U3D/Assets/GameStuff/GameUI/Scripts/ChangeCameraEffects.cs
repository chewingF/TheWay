using UnityEngine;
using UnityEngine.PostProcessing;

[RequireComponent(typeof(PostProcessingBehaviour))]
public class ChangeCameraEffects : MonoBehaviour
{
    PostProcessingProfile m_Profile;

    public float smooth = 1;

    // Focus
    private float maxFocus;
    private float minFocus;
    private float newFocus;

    // Grain
    private float maxGrain;
    private float minGrain;
    private float newGrain;

    // FOV
    private float maxFOV;
    private float minFOV;
    private float newFOV;

    void Start()
    {
        newFocus = m_Profile.depthOfField.settings.focusDistance;
        newGrain = m_Profile.grain.settings.intensity;
        newFOV = this.gameObject.GetComponent<Camera>().fieldOfView;
    }

    void OnEnable()
    {
        var behaviour = GetComponent<PostProcessingBehaviour>();
        if (behaviour.profile == null)
        {
            enabled = false;
            return;
        }

        m_Profile = Instantiate(behaviour.profile);
        behaviour.profile = m_Profile;
    }


    void Update()
    {
        var vignette = m_Profile.vignette.settings;
        var DOF = m_Profile.depthOfField.settings;
        var grain = m_Profile.grain.settings;

        DOF.focusDistance = Mathf.Lerp(DOF.focusDistance, newFocus, Time.unscaledDeltaTime * smooth);
        grain.intensity = Mathf.Lerp(grain.intensity, newGrain, Time.unscaledDeltaTime * smooth);
        this.gameObject.GetComponent<Camera>().fieldOfView = Mathf.Lerp(this.gameObject.GetComponent<Camera>().fieldOfView, newFOV, Time.unscaledDeltaTime * smooth);
        
        m_Profile.depthOfField.settings = DOF;
        m_Profile.grain.settings = grain;

        // vignette.smoothness = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup) * 0.99f) + 0.01f;
        // m_Profile.vignette.settings = vignette;
    }
    public void ChangeFocus(float value, float smoothValue)
    {
        newFocus = value;
        smooth = smoothValue;
    }

    public void ChangeGrain(float value, float smoothValue)
    {
        newGrain = value;
        smooth = smoothValue;
    }

    public void ChangeFOV(float value, float smoothValue)
    {
        newFOV = value;
        smooth = smoothValue;
    }

}
