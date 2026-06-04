using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Game", menuName = "ScriptableObjects/Game", order = 1)]
public class GameScriptableObject : ScriptableObject
{
    public Sprite GameImage;
    public string Author;
    public string GameName;
}
