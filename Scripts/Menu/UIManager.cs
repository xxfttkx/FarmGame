using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private GameObject menuCanvas;
    public GameObject menuPrefab;

    public Button settingsBtn;
    public GameObject pausePanel;
    public GameObject universalUI;
    public GameObject taskUI;
    public Dictionary_SO dictSO;
    private Coroutine fadeUniversalUI;


    protected override void Awake()
    {
        base.Awake();
        settingsBtn.onClick.AddListener(TogglePausePanel);
        pausePanel.GetComponentInChildren<PausePanelUI>(true)?.Init();
    }
    private void Update()
    {

        // if(Input.GetKeyDown(KeyCode.Escape))
        // {
        //     TogglePausePanel();
        // }
        // if (Input.GetKeyDown(KeyCode.F) || 
        //     (taskUI.activeInHierarchy && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))))
        // {
        //     ShowTaskUI();
        // }
    }


    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }


    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuPrefab, menuCanvas.transform);
    }
    private void OnAfterSceneLoadedEvent()
    {
        universalUI.SetActive(false);
        if (menuCanvas.transform.childCount > 0)
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
    }

    public void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;

        if (isOpen)
        {
            pausePanel.SetActive(false);
            // Time.timeScale = 1;
            EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
        }
        else
        {
            System.GC.Collect();
            pausePanel.SetActive(true);
            // Time.timeScale = 0;
            EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        }
    }

    public void GoTo(int num, Action subFun = null, Action cancelFun = null)
    {
        if (universalUI == null) return;
        if (num < 0) universalUI.SetActive(false);
        UniversalUI uniUI = universalUI.GetComponent<UniversalUI>();
        if (uniUI == null) return;
        var dicts = dictSO.dicts;
        var dict = dicts.Find(d => d.num == num);
        if (dict == null) return;

        if (fadeUniversalUI != null)
        {
            var canvas = universalUI.GetComponent<CanvasRenderer>();
            if (canvas == null) return;
            canvas.SetAlpha(1);
            StopCoroutine(fadeUniversalUI);
        }

        universalUI.SetActive(true);

        uniUI.text.text = dict.text;
        if (dict.haveButtons)
        {
            uniUI.submitButton.onClick.RemoveAllListeners();
            uniUI.cancelButton.onClick.RemoveAllListeners();
            uniUI.submitButton.image.enabled = true;
            uniUI.cancelButton.image.enabled = true;
            UnityAction submitFun = new UnityAction(() =>
            {
                universalUI.SetActive(false);
            });
            if (subFun != null)
            {
                submitFun += new UnityAction(subFun);
            }
            uniUI.submitButton.onClick.AddListener(submitFun);

            if (cancelFun != null)
            {
                uniUI.cancelButton.onClick.AddListener(new UnityAction(cancelFun));
            }
            uniUI.cancelButton.onClick.AddListener
                (new UnityAction(() => { universalUI.SetActive(false); }));
        }
        else
        {
            uniUI.submitButton.image.enabled = false;
            uniUI.cancelButton.image.enabled = false;
        }
        if (dict.duration > 0f)
        {
            fadeUniversalUI = StartCoroutine(CancelUniversalUI(dict.duration, dict.fadeDuration));
        }

    }

    IEnumerator CancelUniversalUI(float duration, float fadeDuration)
    {
        yield return new WaitForSeconds(duration);
        float targetAlpha = 0f;
        if (universalUI == null) yield break;
        var canvas = universalUI.GetComponent<CanvasRenderer>();
        if (canvas == null) yield break;
        float speed = Mathf.Abs(canvas.GetAlpha() - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(canvas.GetAlpha(), targetAlpha))
        {
            float currAlpha = Mathf.MoveTowards(canvas.GetAlpha(), targetAlpha, speed * Time.deltaTime);
            canvas.SetAlpha(currAlpha);
            yield return null;
        }
        universalUI.SetActive(false);
        canvas.SetAlpha(1);
    }


    public void ReturnMenuCanvas()
    {
        // Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }

    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        EventHandler.CallUpdateGameStateEvent(GameState.GameEnd);
        EventHandler.CallEndGameEvent();
        yield return new WaitForSeconds(1f);
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    public void ShowTaskUI()
    {
        TaskUI t = taskUI.GetComponent<TaskUI>();
        if (taskUI.activeInHierarchy)//taskUI.transform.localScale.x > 0
        {
            taskUI.SetActive(false);
            // DOTween.To(() => taskUI.transform.localScale,
            //     v => taskUI.transform.localScale = v, new Vector3(0f, 0f, 1f), 0.3f);
            t.CloseTaskContents();
            EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
        }
        else
        {
            taskUI.SetActive(true);
            DOTween.To(() => taskUI.transform.localScale,
                v => taskUI.transform.localScale = v, new Vector3(1f, 1f, 1f), 0.3f);
            t.ShowTaskContents();
            EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
