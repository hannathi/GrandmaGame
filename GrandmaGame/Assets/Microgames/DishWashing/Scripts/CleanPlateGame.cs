using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro; 

public class CleanPlateGame : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Material cleanMaterial;
    [SerializeField] private TextMeshProUGUI instructionText; 

    [Header("Game Settings")]
    [SerializeField] private int requiredPresses = 5;
    [SerializeField] private float gameDuration = 8f; 
    [SerializeField] private float penaltyDuration = 1.5f; 

    private MeshRenderer meshRenderer;
    private int cleanProgress = 0;
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
        timeRemaining = gameDuration;
        penaltyTimer = 0f;
        getNewRandomKey();
    }

    void Update()
    {
        if (!gameActive || isClean || gameFailed) return;

        // 1. Handle Game Timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            FailTheGame();
            return;
        }

        // 2. Handle Penalty Cooldown Timer
        if (IsPenalized)
        {
            penaltyTimer -= Time.deltaTime;
            
            // Keep text red and show countdown during penalty
            if (instructionText != null)
            {
                instructionText.text = $"<color=red>JAMMED! WAIT!</color>\nTime Left: {Mathf.Max(0, timeRemaining):F1}s";
            }
            return; 
        }

        // 3. Constantly update the standard UI so the time ticks down smoothly
        UpdateUI();

        // 4. Handle Inputs
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
        penaltyTimer = penaltyDuration;
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
            instructionText.text = $"MASH THIS KEY: <color=green>{targetKeyName.ToUpper()}</color>\nProgress: {cleanProgress}/{requiredPresses}\nTime Left: {Mathf.Max(0, timeRemaining):F1}s";
        }
    }

    void cleantThePlate()
    {
        meshRenderer.material = cleanMaterial;
        isClean = true;
        gameActive = false;
        
        if (instructionText != null)
        {
            instructionText.text = "<color=green> DISHES WASHED!</color>";
        }
    }

    void FailTheGame()
    {
        gameActive = false;
        gameFailed = true;

        if (instructionText != null)
        {
            instructionText.text = "<color=red>TIME'S UP!\nPlate is still dirty.</color>";
        }
    }
}