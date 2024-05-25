using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.PackageManager;
using System.Linq;

[System.Serializable]
public struct DialogueInfo
{
    public int mamaFace;
    public string speaker;
    public string dialogueText;
    public bool isTutorial;

    public DialogueInfo(string dialogueText, string speaker, bool isTutorial, int mamaFace = 0)
    {
        this.dialogueText = dialogueText;
        this.speaker = speaker;
        this.mamaFace = mamaFace;
        this.isTutorial = isTutorial;
    }
}

[System.Serializable]
public struct TutorialInfo
{
    public bool isPress;
    public int[] pickUpSlots;
    public int[] putDownSlots;
}

public class DialogueController : MonoBehaviour
{
    [SerializeField] private Image mamaCrab;
    [SerializeField] private TextMeshProUGUI speakerDisplay;
    [SerializeField] private TextMeshProUGUI dialogueTextDisplay;
    [SerializeField] private GameObject nextButton;

    [SerializeField] private Sprite[] mamaCrabFaces;

    [SerializeField] private string dialogueFile;

    [SerializeField] private float typeDelay;

    [SerializeField] private DialogueInfo[] dialogues;
    [SerializeField] private TutorialInfo[] tutorials;
    [SerializeField] private Transform grid;
    [SerializeField] private GameObject bin;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image shade;
    [SerializeField] private Image shuffle;
    private List<GameObject> highlights = new List<GameObject>();

    private int currentLine = -1;
    private int currentTutorial = -1;

    // Start is called before the first frame update
    void Start()
    {
        NextLine();
    }

    IEnumerator TypeDialogue(Action onComplete)
    {
        string dialogue = dialogues[currentLine].dialogueText;
        for (int i = 0; i<= dialogue.Length; i++)
        {
            dialogueTextDisplay.text = dialogue.Substring(0,i);
            yield return new WaitForSeconds(typeDelay);
        }
        onComplete?.Invoke();
    }

    public void NextLine()
    {
        StopAllCoroutines();
        currentLine++;
        if (currentLine < dialogues.Length)
        {
            mamaCrab.sprite = mamaCrabFaces[dialogues[currentLine].mamaFace];
            speakerDisplay.text = dialogues[currentLine].speaker;
            StartCoroutine(TypeDialogue(()=>FinishTyping()));
            if (dialogues[currentLine].isTutorial)
            {
                StartTutorial();
            }
        }
        else
        {
            dialogueTextDisplay.text = "";
            nextButton.SetActive(false);
            playerController.canPlay = true;
        }
    }

    void FinishTyping()
    {

    }

    public void FinishTutorial()
    {
        foreach (int slot in tutorials[currentTutorial].pickUpSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(grid.transform);
            }
            else if (slot == -3)
            {
                shuffle.GetComponent<RectTransform>().SetParent(grid.transform);
            }
            else
            {
                grid.GetComponent<GridInfo>().grid[slot].GetComponent<RectTransform>().SetParent(grid.transform);
            }
        }
        foreach (int slot in tutorials[currentTutorial].putDownSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(grid.transform);
            }
            else if (slot == -3)
            {
                shuffle.GetComponent<RectTransform>().SetParent(grid.transform);
            }
            else
            {
                grid.GetComponent<GridInfo>().grid[slot].GetComponent<RectTransform>().SetParent(grid.transform);
            }
        }
        shade.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        nextButton.SetActive(true);
        playerController.inTutorial = false;
        playerController.canPlay = false;
        NextLine();
    }

    public bool IsValidSlot(int slot)
    {
        return IsPickUp(slot) || IsPutDown(slot);
    }

    public bool IsPickUp(int slot)
    {
        return tutorials[currentTutorial].pickUpSlots.Contains(slot);
    }

    public bool IsPutDown(int slot)
    {
        return tutorials[currentTutorial].putDownSlots.Contains(slot);
    }

    public bool GetTutorialIsPress()
    {
        return tutorials[currentTutorial].isPress;
    }

    public 

    void StartTutorial()
    {
        currentTutorial++;
        foreach (int slot in tutorials[currentTutorial].pickUpSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(shade.transform);
            }
            else if (slot == -3)
            {
                shuffle.GetComponent<RectTransform>().SetParent(shade.transform);
            }
            else
            {
                grid.GetComponent<GridInfo>().grid[slot].GetComponent<RectTransform>().SetParent(shade.transform);
            }
        }
        foreach (int slot in tutorials[currentTutorial].putDownSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(shade.transform);
            }
            else if (slot == -3)
            {
                shuffle.GetComponent<RectTransform>().SetParent(shade.transform);
            }
            else
            {
                grid.GetComponent<GridInfo>().grid[slot].GetComponent<RectTransform>().SetParent(shade.transform);
            }
        }
        nextButton.SetActive(false);
        playerController.inTutorial = true;
        playerController.canPlay = true;
        shade.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
    }
}
