using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐数据库")]
    public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;
    [Header("Audio Source")]
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapShot;
    public AudioMixerSnapshot ambientSnapShot;
    public AudioMixerSnapshot muteSnapShot;
    private float musicTransitionSecond = 8f;


    public float MusicStartSecond => Random.Range(5f, 15f);

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }



    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        muteSnapShot.TransitionTo(1f);
    }

    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails = soundDetailsData.GetSoundDetails(soundName);
        if (soundDetails != null)
            EventHandler.CallInitSoundEffect(soundDetails);
    }

    private void OnBeforeSceneUnloadEvent()
    {
        audioMixer.SetFloat("MasterVolume", -80f);
        audioMixer.SetFloat("MusicVolume", -80f);
        audioMixer.SetFloat("AmbientVolume", -80f);
    }
    private void OnAfterSceneLoadedEvent()
    {
        //TODO: UImanager awake时没能生效
        
        string currentScene = SceneManager.GetActiveScene().name;

        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
            return;
        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));
        
    }


    private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient)
    {
        if (music != null && ambient != null)
        {
            SetMasterVolume(masterVolume);
            PlayAmbientClip(ambient, 1f);
            SetAmbientVolume(ambientVolume);
            yield return new WaitForSeconds(1f);
            PlayMusicClip(music, musicTransitionSecond);
            SetMusicVolume(musicVolume);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails, float transitionTime)
    {
        // audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled)
            gameSource.Play();

        // normalSnapShot.TransitionTo(transitionTime);
    }


    /// <summary>
    /// 播放环境音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails, float transitionTime)
    {
        //audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));
        // SetAmbientVolume(soundDetails.soundVolume);
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
            ambientSource.Play();

        // ambientSnapShot.TransitionTo(transitionTime);
    }


    private float ConvertSoundVolume(float amount)
    {
        // -80~20   
        return (amount * 100 - 80);
    }

    float masterVolume = 0;
    float musicVolume = 0;
    float ambientVolume = 0;
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        value = 0.1f * value;
        float val = Mathf.Clamp(value, 0.00001f, 1.0f);
        int result = (int)(20 + 20 * Mathf.Log10(val));
        audioMixer.SetFloat("MasterVolume", result);
    }
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        value = 0.1f * value;
        float val = Mathf.Clamp(value, 0.00001f, 1.0f);
        int result = (int)(20 + 20 * Mathf.Log10(val));
        audioMixer.SetFloat("MusicVolume", result);
    }
    public void SetAmbientVolume(float value)
    {
        ambientVolume = value;
        value = 0.1f * value;
        float val = Mathf.Clamp(value, 0.00001f, 1.0f);
        int result = (int)(20 + 20 * Mathf.Log10(val));
        audioMixer.SetFloat("AmbientVolume", result);
    }
}
