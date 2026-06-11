using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameImageScript : MonoBehaviour
{
    [Header("Game Data")]
    public GameScriptableObject Game;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gameText;
    [SerializeField] private Image gameImage;
    [SerializeField] private Button playButton;

    private void Start()
    {
        UpdateUI();

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OpenGameLink);
        }
    }

    private void UpdateUI()
    {
        if (Game == null)
        {
            Debug.LogWarning($"GameScriptableObject asset is missing on {gameObject.name}!", gameObject);
            return;
        }

        if (gameImage != null)
        {
            gameImage.sprite = Game.GameImage;
        }

        if (gameText != null)
        {
            gameText.text = $"{Game.GameName}\nMade by {Game.Author}";
        }
    }

    public void OpenGameLink()
    {
        if (Game == null || string.IsNullOrWhiteSpace(Game.Link))
        {
            Debug.LogWarning("No game link assigned.");
            return;
        }

        Application.OpenURL(Game.Link);
    }

    public void LoadGame()
    {
        if (Game != null)
        {
            Debug.Log(Game.GameName + " Should be loading");
        }
    }

}