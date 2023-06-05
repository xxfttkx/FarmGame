using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
//using UnityEditor;
using UnityEngine;
using MFarm.Inventory;

//[ExecuteInEditMode]
public class SortSomething : MonoBehaviour
{
    // Start is called before the first frame update
    public ItemDataList_SO itemDataList_SO;
    public CropDataList_SO cropDataList_SO;
    public bool canSort = false;
    public bool changeName = false;
    public ScheduleDataList_SO momoiS;
    public bool writeMomoiS = false;
    public bool writeItemsToFile = false;
    public bool getSomething =false;
    public int id;
    public int num;
    
    private void Update() {

    }
    void ChangeSomeName()
    {
        string targetDirectory = @"E:\UnityHub\Projects\Farm\Assets\M Studio\Animations\NPC\Momoi";
        string searchPattern = "*.anim";
        DirectoryInfo directory = new DirectoryInfo(targetDirectory); // 创建目录实例
        FileInfo[] files = directory.GetFiles(searchPattern); // 获取目录下符合搜索模式的所有文件
        foreach (FileInfo file in files)
        {
            string oldName = file.Name;
            string newName = oldName.Replace("Midori", "Momoi"); // 将文件名中的"midori"替换为"momoi"

            string newPath = Path.Combine(file.DirectoryName, newName); // 拼接新的文件路径

            File.Move(file.FullName, newPath); // 修改文件名
        }
    }
    void Start()
    {

    }

    void OnEnable()
    {
        if(canSort)
        {
            canSort = false;
            InventoryManager.Instance.ArrangeAndMergePlayerBag();
        }
        if(changeName)
        {
            changeName = false;
            ChangeSomeName();
        }
        if(writeMomoiS)
        {
            writeMomoiS = false;
            WriteMomoiS();
        }
        if(getSomething)
        {
            getSomething = false;
            InventoryManager.Instance.AddItem(id,num);
        }
        if(writeItemsToFile)
        {
            writeItemsToFile = false;
            WriteItemsToFile();
        }

        // return;
        
        // if (cropDataList_SO != null)
        // {
        //     var details = cropDataList_SO.cropDetailsList;
        //     if (details != null)
        //     {
        //         details.Sort((a, b) => a.seedItemID.CompareTo(b.seedItemID));
        //         //EditorUtility.SetDirty(itemDataList_SO);
        //     }
        // }
    }
    public void WriteItemsToFile()
    {
        if (itemDataList_SO != null)
        {
            var dataFolder = Application.persistentDataPath + "/Some Data/";
            var resultPath = dataFolder + "itemDataList_SO" + ".json";
            var itemData = "";
            //JsonConvert.SerializeObject(itemDataList_SO.itemDetailsList, Formatting.Indented);
            foreach (var item in itemDataList_SO.itemDetailsList)
            {
                itemData += item.itemID + " " + item.itemName + "\n";
            }
            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(dataFolder);
            }
            File.WriteAllText(resultPath, itemData);
        }
    }

    void WriteMomoiS()
    {
        //6 6.5 7 7.5           ------         6-1 7-2   23-18    size = 
        var list = momoiS.scheduleList;
        var first = new Vector2Int(-11, -7);
        var second = new Vector2Int(-11, -17);
        var third = new Vector2Int(10, -17);
        var fourth = new Vector2Int(10, -7);
        int hour = 6;
        int minute = 0;
        var newList = new List<ScheduleDetails>(17*4);
        while (newList.Count < 17*4)
        {
            newList.Add(null); // 向列表中添加元素，直到数量达到36
        }
        for (int i = 0; i < 17; ++i)
        {
            newList[i * 4] = new ScheduleDetails(hour, minute, 0, 0, Season.春天, "01.Field", first, null, false);
            minute+=20;
            newList[i * 4 + 1] = new ScheduleDetails(hour, minute, 0, 0, Season.春天, "01.Field", second, null, false);
            minute+=10;
            newList[i * 4 + 2] = new ScheduleDetails(hour, minute, 0, 0, Season.春天, "01.Field", third, null, false);
            minute+=20;
            newList[i * 4 + 3] = new ScheduleDetails(hour, minute, 0, 0, Season.春天, "01.Field", fourth, null, false);
            minute=0;
            hour += 1;
        }
        momoiS.scheduleList = newList;
    }

    // struct TroopItem
    // {
    //     int id; int type;
    //     int level; public int load;
    //     int force; public int own_num; public int select_num;
    //     public TroopItem(TroopItem t, int select_num)
    //     {
    //         this.id = t.id; this.type = t.type; this.level = t.level;
    //         this.load = t.load; this.force = t.force; this.own_num = t.own_num;
    //         this.select_num = select_num;
    //     }
    // }
    // List<TroopItem> QuickSelectTroopList(int res_max, int march_size_max, List<TroopItem> own_troop_list)
    // {
    //     List<TroopItem> res = new List<TroopItem>(own_troop_list);
    //     //load从高到低排序
    //     res.Sort((a, b) => { return b.load.CompareTo(a.load); });
    //     int remain_load = res_max;
    //     int remain_size = march_size_max;
    //     for (int i = 0; i < res.Count; ++i)
    //     {
    //         if (remain_load > 0 && res[i].own_num > 0 && remain_size > 0)
    //         {
    //             int need_num = remain_load / res[i].load;
    //             //先判断当前TroopItem的load够不够
    //             //再判断当前TroopItem的own_num会不会超出
    //             if (res[i].own_num * res[i].load >= remain_load)
    //             {
    //                 int select_num = 0;
    //                 select_num = remain_load / res[i].own_num;
    //                 if (remain_load % res[i].own_num != 0) select_num += 1;
    //                 if (remain_size >= select_num)
    //                 {
    //                     res[i] = new TroopItem(res[i], select_num);
    //                     // remain_size -= res[i].select_num;
    //                     remain_load = 0;
    //                 }
    //                 else
    //                 {
    //                     res[i] = new TroopItem(res[i], remain_size);
    //                     // remain_load -= remain_size * res[i].load;
    //                     remain_size = 0;
    //                 }
    //             }
    //             else
    //             {
    //                 if (remain_size >= res[i].own_num)
    //                 {
    //                     res[i] = new TroopItem(res[i], res[i].own_num);
    //                     remain_load -= res[i].own_num * res[i].load;
    //                     remain_size -= res[i].own_num;
    //                 }
    //                 else
    //                 {
    //                     res[i] = new TroopItem(res[i], remain_size);
    //                     // remain_load -= remain_size * res[i].load;
    //                     remain_size = 0;
    //                 }

    //             }
    //         }
    //         else
    //         {
    //             res[i] = new TroopItem(res[i], 0);
    //         }
    //     }
    //     return res;
    // }




}
