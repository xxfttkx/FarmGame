using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TaskDataList_SO", menuName = "Task/TaskDataList_SO")]
public class TaskDataList_SO : ScriptableObject
{
    public List<Task> taskList;
}