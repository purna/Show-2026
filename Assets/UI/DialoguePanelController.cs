using UnityEngine;
using UnityEngine.UIElements;

public class DialoguePanelController : MonoBehaviour
{
    private VisualElement dialogueBox;
    private Label npcNameLabel;
    private Label dialogueTextLabel;
    private VisualElement choicesContainer;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        dialogueBox = root.Q<VisualElement>("local-dialogue-box");
        npcNameLabel = root.Q<Label>("npc-name-display");
        dialogueTextLabel = root.Q<Label>("bubble-text");
        choicesContainer = root.Q<VisualElement>("bubble-choices");
    }

    public void OpenDialogueConnection(string npcName, string speechPrompt, string[] choices, System.Action<int> onChoiceSelected)
    {
        dialogueBox.style.display = DisplayStyle.Flex;
        npcNameLabel.text = npcName;
        dialogueTextLabel.text = speechPrompt;
        choicesContainer.Clear();

        for (int i = 0; i < choices.Length; i++)
        {
            int choiceIndex = i; 
            Button choiceBtn = new Button();
            choiceBtn.text = choices[i];
            choiceBtn.AddToClassList("choice-button");
            choiceBtn.RegisterCallback<ClickEvent>(evt => {
                onChoiceSelected?.Invoke(choiceIndex);
            });
            choicesContainer.Add(choiceBtn);
        }

        // Add a clean disconnect connection button when dialogue tree reaches its end
        if (choices.Length == 0)
        {
            Button closeBtn = new Button(() => CloseDialogueConnection());
            closeBtn.text = "[CLOSE CONNECTION]";
            closeBtn.AddToClassList("choice-button");
            closeBtn.style.width = Length.Percent(100);
            choicesContainer.Add(closeBtn);
        }
    }

    public void CloseDialogueConnection()
    {
        dialogueBox.style.display = DisplayStyle.None;
    }
}