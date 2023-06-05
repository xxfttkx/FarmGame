using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DragItemUI : MonoBehaviour
{
    public Text numText;
    public Image dragItemImage;

    public void MoveByMouse()
    {
        dragItemImage.transform.position = Input.mousePosition;
    }
    public void SetNumText(int num)
    {
        numText.text = num.ToString();
    }

    public void SetDragItemImage(Sprite sprite)
    {
        dragItemImage.sprite = sprite;
        dragItemImage.enabled = true;
        dragItemImage.SetNativeSize();
    }
    public void ShowDragItemImage(Sprite sprite, int num)
    {
        SetDragItemImage(sprite); 
        SetNumText(num);
    }
    public void CloseDragItemImage()
    {
        dragItemImage.enabled = false;
        numText.text = string.Empty;
    }
}
