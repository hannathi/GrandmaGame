using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro; 

public class CleanPlateGame : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Material cleanMaterial;
    [SerializeField] private TextMeshProUGUI instructionText; 

    [Header("Plate Models (Visual Swap)")]
    [SerializeField] private Mesh brokenPlateMesh;     
    [SerializeField] private Material brokenPlateMaterial; 

    [Header("Game Settings")]
    [SerializeField] private int requiredPresses = 5;
    [SerializeField] private float gameDuration = 8f; 
    [SerializeField] private float penaltyDuration = 1.5f; 
    [SerializeField] private int maxMistakesAllowed = 2; 

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private int cleanProgress = 0;
    private int mistakeCount = 0; 
    private string targetKeyName; 
    
    private bool isClean = false;
    private bool gameActive = false;
    private bool gameFailed = false;

    private float timeRemaining;
    private float penaltyTimer = 0f;
    private bool IsPenalized => penaltyTimer > 0f; 

    private string[] possibleKeys = { "q", "w", "e", "a", "s", "d", "z", "x", "c" };

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        timeRemaining = gameDuration;
       
        if (instructionText != null)
        {
            instructionText.text = "Left click on plate to start washing!";
        }
    }

    void OnMouseDown()
    {
        if (!isClean && !gameActive && !gameFailed)
        {
            StartMinigame();
        }
    }

    void StartMinigame()
    {
        gameActive = true;
        cleanProgress = 0;
        mistakeCount = 0; 
        timeRemaining = gameDuration;
        penaltyTimer = 0f;
        getNewRandomKey();
    }

    void Update()
    {
        if (!gameActive || isClean || gameFailed) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            FailTheGame("TIME'S UP!\nPlate is still dirty.");
            return;
        }

        if (IsPenalized)
        {
            penaltyTimer -= Time.deltaTime;
            
            if (instructionText != null)
            {
                instructionText.text = $"<color=red>WRONG KEY! WAIT!</color>\nMistakes: {mistakeCount}/{maxMistakesAllowed}\nTime Left: {Mathf.Max(0, timeRemaining):F1}s";
            }
            return; 
        }

        UpdateUI();

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        foreach (string keyName in possibleKeys)
        {
            var keyControl = keyboard[keyName] as UnityEngine.InputSystem.Controls.KeyControl;
            
            if (keyControl != null && keyControl.wasPressedThisFrame)
            {
                if (keyName == targetKeyName)
                {
                    advanceCleaning();
                }
                else
                {
                    TriggerPenalty();
                }
                break; 
            }
        }
    }

    void advanceCleaning()
    {
        cleanProgress++;

        if (cleanProgress >= requiredPresses)
        {
            cleantThePlate();
        }
        else
        {
            getNewRandomKey();
        }
    }

    void TriggerPenalty()
    {
        mistakeCount++;
        
        if (mistakeCount >= maxMistakesAllowed)
        {
            BreakThePlate();
        }
        else
        {
            penaltyTimer = penaltyDuration;
        }
    }

    void BreakThePlate()
    {
        // Swap to the broken plate visuals
        if (brokenPlateMesh != null) meshFilter.mesh = brokenPlateMesh;
        if (brokenPlateMaterial != null) meshRenderer.material = brokenPlateMaterial;

        FailTheGame("YOU BROKE THE PLATE!");
    }

    void getNewRandomKey()
    {
        int randomInt = Random.Range(0, possibleKeys.Length);
        targetKeyName = possibleKeys[randomInt];
    }

    void UpdateUI()
    {
        if (instructionText != null)
        {
            instructionText.text = $"PRESS KEY: <color=green>{targetKeyName.ToUpper()}</color>\nProgress: {cleanProgress}/{requiredPresses}\nMistakes: {mistakeCount}/{maxMistakesAllowed}";
        }
    }

    void cleantThePlate()
    {
        meshRenderer.material = cleanMaterial;
        isClean = true;
        gameActive = false;
        
        if (instructionText != null)
        {
            instructionText.text = "<color=green>DISHES WASHED!</color>";
        }
    }

    void FailTheGame(string failureMessage)
    {
        gameActive = false;
        gameFailed = true;

        if (instructionText != null)
        {
            instructionText.text = $"<color=red>{failureMessage}</color>";
        }
    }
}