using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Data", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    public int SizeX, SizeY;
    public float CellSize;
    public int ColorCount;
    public int FirstIconThreshold, SecondIconThreshold, ThirdIconThreshold;
}
