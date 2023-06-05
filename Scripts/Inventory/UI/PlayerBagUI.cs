using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBagUI : MonoBehaviour
{
    public Button[] buttons;
    public GoodOpinionUI goodOpinionUI;
    public ItemLogUI itemLogUI;
    public int currIndex = -1;
    // Start is called before the first frame update
    private void Awake() 
    {
        buttons[0].onClick.AddListener(()=>SwtichPanel(0));
        buttons[1].onClick.AddListener(()=>SwtichPanel(1));
        buttons[2].onClick.AddListener(()=>SwtichPanel(2));
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SwtichPanel(int index)
    {
        switch (index)
        {
            case 0:
                goodOpinionUI.gameObject.SetActive(false);
                itemLogUI.CloseItemLogUI();
                break;
            case 1:
                goodOpinionUI.gameObject.SetActive(true);
                itemLogUI.CloseItemLogUI();
                break;
            case 2:
                itemLogUI.ShowItemLogUI();
                break;
            default: break;
        }
        //TODO:通过currIndex能优化
        currIndex = index;
    }
}
