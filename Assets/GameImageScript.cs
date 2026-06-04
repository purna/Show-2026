using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameImageScript : MonoBehaviour
{
    public GameScriptableObject Game;
    private TextMeshPro TheText;
    // Start is called before the first frame update
    void Start()
    {
        TheText = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = Game.GameImage;
        TheText.text = Game.GameName + "<br> " + "Made by " + Game.Author; 
    }

    public void LoadGame()
    {
        Debug.Log(Game.GameName + " Should be loading");

    }
}
