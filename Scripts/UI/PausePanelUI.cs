using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider masterVolumeSlider;
    public Slider musicVolumeolumeSlider;
    public Slider ambientVolumeSlider;

    private void Awake() 
    {
        
    }
    private void Start()
    {
        
    }
    public void Init()
    {
        masterVolumeSlider?.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        musicVolumeolumeSlider?.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        ambientVolumeSlider?.onValueChanged.AddListener(AudioManager.Instance.SetAmbientVolume);
        masterVolumeSlider?.SetValueWithoutNotify(0.3f);
        musicVolumeolumeSlider?.SetValueWithoutNotify(0.3f);
        ambientVolumeSlider?.SetValueWithoutNotify(0.3f);
        masterVolumeSlider?.onValueChanged.Invoke(0.3f);
        musicVolumeolumeSlider?.onValueChanged.Invoke(0.3f);
        ambientVolumeSlider?.onValueChanged.Invoke(0.3f);
    }
}
