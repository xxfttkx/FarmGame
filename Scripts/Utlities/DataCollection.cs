using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;
}

[System.Serializable]
public struct InventoryItem : IComparable<InventoryItem>
{
    public int itemID;
    public int itemAmount;

    public bool isEmpty()
    {
        return itemID <= 0 || itemAmount <= 0;
    }

    // public int Compare(InventoryItem x, InventoryItem y)
    // {
    //     if (y.isEmpty()) return -1;
    //     return x.itemID.CompareTo(y.itemID);
    // }

    public int CompareTo(InventoryItem y)
    {
        if (this.isEmpty())
        {
            if (y.isEmpty())
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        if (y.isEmpty())
        {
            return -1;
        }
        int res = this.itemID.CompareTo(y.itemID);
        if (res == 0)
        {
            return -this.itemAmount.CompareTo(y.itemAmount);
        }
        else
        {
            return res;
        }
    }

    public InventoryItem(int ID, int Amount)
    {
        this.itemID = ID;
        this.itemAmount = Amount;
    }
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        // return new Vector2Int((int)x, (int)y);
        return new Vector2Int(Mathf.FloorToInt(x),Mathf.FloorToInt(y));
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;
    public int boxIndex;
}

[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;
}


[System.Serializable]
public class TileDetails
{
    public int girdX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;
    public int daysSinceDug = -1;
    public int daysSinceWatered = -1;
    public int seedItemID = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;
}

[System.Serializable]
public class NPCPosition
{
    public Transform npc;
    public string startScene;
    public Vector3 position;
}

//场景路径
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string gotoSceneName;
    public List<ScenePath> scenePathList;
}

[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int gotoGridCell;
}

[System.Serializable]
public class Task
{
    public string NPCName;
    public int needItemID;
    public int needNum;
    public int day;

    public Task(Task t)
    {
        if (t == null || t.needItemID == 0)
        {
        }
        else
        {
            NPCName = t.NPCName;
            needItemID = t.needItemID;
            needNum = t.needNum;
            day = t.day;
        }

    }
}

[System.Serializable]
public class Dict
{
    public int num;
    public string text;

    public bool haveButtons;

    public float duration;
    public float fadeDuration;
}