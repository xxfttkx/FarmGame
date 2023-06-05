using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Dialogue;


[CreateAssetMenu(fileName = "DialoguePieceList_SO", menuName = "Dialogue/DialoguePieceList")]
public class DialoguePieceList_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieceList;
}
