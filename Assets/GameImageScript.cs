using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameImageScript : MonoBehaviour
{
    public Sprite GameImage;
    public string Author;
    public string GameName;
    private TextMeshPro TheText;
    // Start is called before the first frame update
    void Start()
    {
        TheText = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().sprite = GameImage;
        TheText.text = GameName + "<br> " + "Made by " + Author; 
    }

    public void LoadGame()
    {
        Debug.Log(GameName + " Should be loading");

    }
}
