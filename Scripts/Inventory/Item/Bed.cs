using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Transition;
using MFarm.Save;
public class Bed : MonoBehaviour
{
    GameObject sleepUI;
    private void Awake()
    {
        var mainCanvas = GameObject.FindWithTag("MainCanvas");
        if (mainCanvas != null)
            sleepUI = mainCanvas.transform.GetChild(5).gameObject;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.CompareTag("Player") && sleepUI != null)
        //     sleepUI.SetActive(true);

        if (other.CompareTag("Player"))
        {
            UIManager.Instance.GoTo(4, Sleep);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.GoTo(-1);
        }
        // if (other.CompareTag("Player") && sleepUI != null)
        //     sleepUI.SetActive(false);
    }

    void Sleep()
    {
        StartCoroutine(Asleep());
    }
    private IEnumerator Asleep()
    {
        //使用TransitionManager类的Fade
        var transitionManager = TransitionManager.Instance;
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        //submitButton.interactable = false;
        yield return transitionManager.Fade(1);
        TimeManager.Instance.ToNextMorning();
        yield return transitionManager.Fade(0);
        EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
        SaveLoadManager.Instance.Save();
        //submitButton.interactable = true;
        //this.gameObject.SetActive(false);
    }
}
