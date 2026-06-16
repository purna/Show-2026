using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Game", menuName = "ScriptableObjects/Game", order = 1)]
public class GameScriptableObject : ScriptableObject
{
    public Sprite GameImage;
    public string Author;

    
    public string GameName0;

    public string GameName1;

    
    [Tooltip("URL to open when Play button is clicked")]
    public string Link0;
    public string Link1;


    [Tooltip("The index number matching the entry in open_app.bat (0 to 40)")]
    public int batchAppIndex0; 
    public int batchAppIndex1; 
}
