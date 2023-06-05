using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;

public class UIOpenCloseManager : Singleton<UIOpenCloseManager>
{
    public InventoryUI inventoryUI;
    public TaskUI taskUI;
    public GameObject baseBag;
    protected override void Awake()
    {
        base.Awake();
    }
    void Update()
    {
        if (Player.Instance.gameState == GameState.GameEnd) return;
        bool hasUIOpen = false;
        if (inventoryUI.bagOpened)
        {
            hasUIOpen = true;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
            {
                inventoryUI.CloseBagUI();
            }
        }
        if (taskUI.gameObject.activeInHierarchy)
        {
            hasUIOpen = true;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.ShowTaskUI();
            }
        }
        if (UIManager.Instance.pausePanel.activeInHierarchy)
        {
            hasUIOpen = true;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.TogglePausePanel();
            }
        }
        if(baseBag.activeInHierarchy)
        {
            hasUIOpen = true;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                EventHandler.CallBaseBagCloseEvent(SlotType.Shop, null);
                EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
            }
        }
        if(!hasUIOpen)
        {//什么UI都没打开
            if (Input.GetKeyDown(KeyCode.E) ||Input.GetKeyDown(KeyCode.B))
            {
                inventoryUI.OpenBagUI();
            }
            if(Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.ShowTaskUI();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.TogglePausePanel();
            }
        }
    }
}
