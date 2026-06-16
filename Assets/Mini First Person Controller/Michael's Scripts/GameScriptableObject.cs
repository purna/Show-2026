using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Game", menuName = "ScriptableObjects/Game", order = 1)]
public class GameScriptableObject : ScriptableObject
{
    public Sprite GameImage;
    public string Author;
    public string GameName;
    [Tooltip("URL to open when Play button is clicked")]
    public string Link;

    [Tooltip("The index number matching the entry in open_app.bat (0 to 40)")]
    public int batchAppIndex; 
}
