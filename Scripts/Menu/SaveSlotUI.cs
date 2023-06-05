using MFarm.Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SaveSlotUI : MonoBehaviour, IPointerDownHandler
{
    public Text dataTime, dataScene;
    private Button currentButton;
    private DataSlot currentData;

    private int Index => transform.GetSiblingIndex();

    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }

    private void OnEnable()
    {
        SetupSlotUI();
    }

    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];

        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            dataTime.text = "这个世界还没开始";
            dataScene.text = "梦还没开始";
        }
    }

    private void LoadGameData()
    {
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(Index);
        }
        else
        {
            Debug.Log("新游戏");
            EventHandler.CallStartNewGameEvent(Index);
            //EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
            //改在Fade(0)之后了
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right)
            UIManager.Instance.GoTo(6, CancelGameData);
    }

    private void CancelGameData()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];

        if (currentData == null)
        {
            UIManager.Instance.GoTo(3);
        }
        else
        {
            SaveLoadManager.Instance.Kill(Index);
            SetupSlotUI();
        }
    }
}
