using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro; 

public class CleanPlateGame : MonoBehaviour
{
    [SerializeField] private Material cleanMaterial;
    [SerializeField] private TextMeshProUGUI instructionText; 

    private MeshRenderer meshRenderer;

    private int cleanProgress = 0;
    private int requiredPresses = 5;
    private string targetKeyName; 
    private bool isClean = false;
    private bool gameActive = false;
    private string[] possibleKeys = { "q", "w", "e", "a", "s", "d", "z", "x", "c" };

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
       
        if (instructionText != null)
        {
            instructionText.text = "Left click on plate to start washing!";
        }
    }

    void OnMouseDown()
    {
        if (!isClean && !gameActive)
        {
            StartMinigame();
        }
    }

    void StartMinigame()
    {
        gameActive = true;
        cleanProgress = 0;
        getNewRandomKey();
        Debug.Log("Minigame Started!");
    }

    void Update()
    {
        if (!gameActive || isClean) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        var targetKeyControl = keyboard[targetKeyName] as UnityEngine.InputSystem.Controls.KeyControl;

        if (targetKeyControl != null && targetKeyControl.wasPressedThisFrame)
        {
            advanceCleaning();
        }
    }

    void advanceCleaning()
    {
        cleanProgress++;
        Debug.Log("Scrubbed! Progress: " + cleanProgress + "/" + requiredPresses);

        if (cleanProgress >= requiredPresses)
        {
            cleantThePlate();
        }
        else
        {
            getNewRandomKey();
        }
    }

    void getNewRandomKey()
    {
        int randomInt = Random.Range(0, possibleKeys.Length);
        targetKeyName = possibleKeys[randomInt];
        
        if (instructionText != null)
        {
            instructionText.text = $"MASH THIS KEY: <color=green>{targetKeyName.ToUpper()}</color>\nProgress: {cleanProgress}/{requiredPresses}";
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
}