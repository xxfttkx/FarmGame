using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MFarm.Transition;
public class SleepUI : MonoBehaviour
{

    //TODO: 删掉
    // Start is called before the first frame update
    public Button submitButton;
    public Button cancelButton;
    void Awake()
    {
        submitButton = transform.GetChild(1).GetComponent<Button>();
        cancelButton = transform.GetChild(2).GetComponent<Button>();
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(Sleep);
        }
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelSleepUI);
        }

    }

    // Update is called once per frame
    void Sleep()
    {
        StartCoroutine(Asleep());
    }
    private IEnumerator Asleep()
    {
        //使用TransitionManager类的Fade
        var transitionManager = TransitionManager.Instance;
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        submitButton.interactable = false;
        yield return transitionManager.Fade(1);
        TimeManager.Instance.ToNextMorning();
        yield return transitionManager.Fade(0);
        EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
        submitButton.interactable = true;
        //this.gameObject.SetActive(false);
    }
    void CancelSleepUI()
    {
        this.gameObject.SetActive(false);
    }
}
