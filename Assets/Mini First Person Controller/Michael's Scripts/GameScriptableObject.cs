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
}
