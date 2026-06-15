using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("Data Architecture Structure Templates")]
    public List<QuestData> activeQuests = new List<QuestData>();
    public List<string> inventoryItems = new List<string>();
    public List<ActionData> contextActions = new List<ActionData>();

    private UIDocument uiDocument;
    private VisualElement root;

    // Structural View Panels
    private VisualElement panelQuest;
    private VisualElement panelInv;
    private VisualElement panelActions;
    private VisualElement startScreen;
    private VisualElement winScreen;
    private VisualElement wipeOverlay;
    private VisualElement locBox;
    private Label locNameLabel;

    // List Containers
    private ScrollView questListContainer;
    private ScrollView invListContainer;
    private ScrollView actionsListContainer;

    // Interactive Toggle Handle Elements
    private Button btnQuestToggle;
    private Button btnInvToggle;
    private Button btnActionsToggle;
    private Button btnSoundToggle;

    private bool isAudioMuted = false;

    [System.Serializable]
    public class QuestData
    {
        public string id;
        public string labelID;
        public string description;
        public int currentProgress;
        public int targetCount;
        public bool isCompleted;
    }

    [System.Serializable]
    public class ActionData
    {
        public string name;
        public string actionType;
        public string visualIconIndicator;
        public bool isConsumable;
    }

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        CacheVisualTreeReferences();
        RegisterInterfaceInputCallbacks();
        InitializePrototypeMockData();
    }

    private void CacheVisualTreeReferences()
    {
        panelQuest = root.Q<VisualElement>("panel-quest");
        panelInv = root.Q<VisualElement>("panel-inv");
        panelActions = root.Q<VisualElement>("panel-actions");
        
        startScreen = root.Q<VisualElement>("start-screen");
        winScreen = root.Q<VisualElement>("win-screen");
        wipeOverlay = root.Q<VisualElement>("wipe-overlay");
        locBox = root.Q<VisualElement>("loc-box");
        locNameLabel = root.Q<Label>("loc-name");

        questListContainer = root.Q<ScrollView>("quest-list");
        invListContainer = root.Q<ScrollView>("inv-list");
        actionsListContainer = root.Q<ScrollView>("actions-list");

        btnQuestToggle = root.Q<Button>("btn-quest-toggle");
        btnInvToggle = root.Q<Button>("btn-inv-toggle");
        btnActionsToggle = root.Q<Button>("btn-actions-toggle");
        btnSoundToggle = root.Q<Button>("btn-sound-toggle");
    }

    private void RegisterInterfaceInputCallbacks()
    {
        btnQuestToggle.RegisterCallback<ClickEvent>(evt => TogglePanel(panelQuest, btnQuestToggle));
        btnInvToggle.RegisterCallback<ClickEvent>(evt => TogglePanel(panelInv, btnInvToggle));
        btnActionsToggle.RegisterCallback<ClickEvent>(evt => TogglePanel(panelActions, btnActionsToggle));
        
        btnSoundToggle.RegisterCallback<ClickEvent>(evt => ToggleAudioEngineMute());

        root.Q<Button>("btn-start").RegisterCallback<ClickEvent>(evt => StartMatchLoopSequence());
        root.Q<Button>("btn-replay").RegisterCallback<ClickEvent>(evt => ReinitializeActiveScene());
    }

    private void InitializePrototypeMockData()
    {
        // Populate system mimicking javascript baseline arrays
        activeQuests.Add(new QuestData { id = "RECON", labelID = "NORTH_RECON", description = "Reach the North Spire", currentProgress = 0, targetCount = 1 });
        activeQuests.Add(new QuestData { id = "CELLS", labelID = "CELL_RECOVERY", description = "Collect Data Cells", currentProgress = 1, targetCount = 3 });
        activeQuests.Add(new QuestData { id = "SHARDS", labelID = "SIGNAL_FRAGMENTS", description = "Collect Purple Shards", currentProgress = 0, targetCount = 3 });

        inventoryItems.Add("DATA_CELL_01");
        inventoryItems.Add("SIGNAL_SHARD_A");

        contextActions.Add(new ActionData { name = "Scan Area", actionType = "scan", isConsumable = false });
        contextActions.Add(new ActionData { name = "Repair Gear", actionType = "repair", isConsumable = true });
        contextActions.Add(new ActionData { name = "Hack Terminal", actionType = "hack", isConsumable = true });

        RenderActiveSystemLists();
    }

    private void TogglePanel(VisualElement targetPanel, Button associatedToggle)
    {
        // Close other windows to replicate single-focus menu style 
        if (!targetPanel.ClassListContains("sliding-panel--open"))
        {
            panelQuest.RemoveFromClassList("sliding-panel--open");
            panelInv.RemoveFromClassList("sliding-panel--open");
            panelActions.RemoveFromClassList("sliding-panel--open");
            
            btnQuestToggle.RemoveFromClassList("ui-toggle--active");
            btnInvToggle.RemoveFromClassList("ui-toggle--active");
            btnActionsToggle.RemoveFromClassList("ui-toggle--active");

            targetPanel.AddToClassList("sliding-panel--open");
            associatedToggle.AddToClassList("ui-toggle--active");
            PlayInterfaceAudioTrigger(true);
        }
        else
        {
            targetPanel.RemoveFromClassList("sliding-panel--open");
            associatedToggle.RemoveFromClassList("ui-toggle--active");
            PlayInterfaceAudioTrigger(false);
        }
    }

    public void RenderActiveSystemLists()
    {
        // 1. Quests System
        questListContainer.Clear();
        foreach (var q in activeQuests)
        {
            Label qLabel = new Label($"{q.labelID}\n{q.description} [{q.currentProgress}/{q.targetCount}]");
            qLabel.AddToClassList("quest-element");
            if (q.currentProgress >= q.targetCount)
            {
                qLabel.AddToClassList("quest-element--completed");
                q.isCompleted = true;
            }
            questListContainer.Add(qLabel);
        }

        // 2. Inventory System
        invListContainer.Clear();
        if (inventoryItems.Count == 0)
        {
            Label emptyLabel = new Label("0_CELLS");
            emptyLabel.AddToClassList("item-element");
            invListContainer.Add(emptyLabel);
        }
        foreach (var item in inventoryItems)
        {
            Label itemLabel = new Label(item);
            itemLabel.AddToClassList("item-element");
            invListContainer.Add(itemLabel);
        }

        // 3. Actions Processing Engine
        actionsListContainer.Clear();
        foreach (var act in contextActions)
        {
            Button actBtn = new Button(() => ExecuteContextAction(act)) { text = act.name };
            actBtn.AddToClassList("choice-button");
            actBtn.style.width = Length.Percent(100);
            actionsListContainer.Add(actBtn);
        }

        EvaluateWinConditions();
    }

    private void ExecuteContextAction(ActionData action)
    {
        Debug.Log($"Running System Action Command: {action.name}");
        GetComponent<FloatingActionEffect>()?.SpawnActionGlyphEffect(action.name);

        if (action.isConsumable)
        {
            contextActions.Remove(action);
            RenderActiveSystemLists();
        }
    }

    public void DisplayDynamicZoneBanner(string zoneName, bool visible)
    {
        locNameLabel.text = zoneName.Replace("LOC_", "").Replace("_", " ");
        locBox.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void ToggleAudioEngineMute()
    {
        isAudioMuted = !isAudioMuted;
        btnSoundToggle.text = isAudioMuted ? "MUTED" : "VOL";
        // Connect hook output directly to your preferred game audio manager here
    }

    private void StartMatchLoopSequence()
    {
        // Smoothly fade screen elements using class toggles
        startScreen.style.opacity = 0;
        startScreen.style.display = DisplayStyle.None;
        Debug.Log("Game Engine Dispatched & Running.");
    }

    private void EvaluateWinConditions()
    {
        bool missionSuccess = activeQuests.TrueForAll(q => q.currentProgress >= q.targetCount);
        if (missionSuccess && winScreen.style.display == DisplayStyle.None)
        {
            TriggerVictorySequence();
        }
    }

    private void TriggerVictorySequence()
    {
        panelQuest.RemoveFromClassList("sliding-panel--open");
        panelInv.RemoveFromClassList("sliding-panel--open");
        panelActions.RemoveFromClassList("sliding-panel--open");

        // Slide the screen transition wipe horizontally
        wipeOverlay.AddToClassList("wipe-overlay--active");

        // Display victory screen panel overlay behind transition wipe frame
        Invoke(nameof(RevealWinScreenOverlay), 0.5f);
    }

    private void RevealWinScreenOverlay()
    {
        winScreen.style.display = DisplayStyle.Flex;
        winScreen.style.opacity = 1;
    }

    private void ReinitializeActiveScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void PlayInterfaceAudioTrigger(bool openMode)
    {
        // Custom sound hook placeholders
    }
}