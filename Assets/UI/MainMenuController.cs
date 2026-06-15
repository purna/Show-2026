using UnityEngine;
using UnityEngine.UIElements; // Crucial namespace for UI Toolkit

public class MainMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement root;

    // UI Elements we want to interact with
    private Button startButton;

    private void Awake()
    {
        // 1. Get a reference to the UIDocument component
        uiDocument = GetComponent<UIDocument>();

        // 2. Extract the 'root' visual element (the top-level container of your UXML)
        root = uiDocument.rootVisualElement;

        // 3. Query (find) elements by their names set in UXML
        startButton = root.Q<Button>("btn-start"); // "btn-start" is the name property in UXML

        // 4. Register event callbacks (like onClick)
        if (startButton != null)
        {
            startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
        }
    }

    private void OnStartButtonClicked(ClickEvent evt)
    {
        Debug.Log("The UI works! System initialized.");
    }
}