using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameImageScript : MonoBehaviour
{
    public GameScriptableObject Game;
    
    private TextMeshProUGUI TheText; 
    private Image TheImage;

    void Start()
    {
        // ✔️ FIXED: No more transform.GetChild() index breaking.
        // It reads directly from its own children.
        TheText = GetComponentInChildren<TextMeshProUGUI>();
        TheImage = GetComponentInChildren<Image>();

        if (Game != null)
        {
            if (TheImage != null)
            {
                TheImage.sprite = Game.GameImage;
            }

            if (TheText != null)
            {
                TheText.text = Game.GameName + "<br> " + "Made by " + Game.Author;
            }
        }
        else
        {
            Debug.LogWarning($"GameScriptableObject asset is missing on {gameObject.name}!", gameObject);
        }
    }

    public void LoadGame()
    {
        if (Game != null)
        {
            Debug.Log(Game.GameName + " Should be loading");
        }
    }
}