using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public struct DialogueInfo
{
    public int avatarFace;
    public string speaker;
    public string dialogueText;
    public bool isTutorial;
    public int[] spawnLocs;
    public int[] spawnIDs;
}

[System.Serializable]
public struct TutorialInfo
{
    public bool isPress;
    public int[] pickUpSlots;
    public int[] putDownSlots;
    public int[] pickUpIDs;
    public int[] putDownIDs;
}

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI speakerDisplay;
    [SerializeField] private TextMeshProUGUI dialogueTextDisplay;
    [SerializeField] private Button nextButton;

    [SerializeField] private GridInfo gridInfo;
    [SerializeField] private GameObject bin;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image shade;

    [SerializeField] private Sprite[] avatarFaces;

    [SerializeField] private float typeDelay = 0.1f;

    [SerializeField] private DialogueInfo[] dialogues;
    [SerializeField] private TutorialInfo[] tutorials;

    [SerializeField] private DialogueInfo[] endDialogues;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_FontAsset crabFont;
    [SerializeField] private TMP_FontAsset tutorialFont;

    private int currentLine = -1;
    private int currentTutorial = -1;
    private int currentEndDialogue = -1;

    // Start is called before the first frame update
    public void StartLevel()
    {
        playerController.canPlay = false;
        NextLine();
    }

    public void NextLine()
    {
        StopAllCoroutines();
        currentLine++;
        if (currentLine < dialogues.Length)
        {
            avatar.sprite = avatarFaces[dialogues[currentLine].avatarFace];
            for (int i = 0; i < dialogues[currentLine].spawnIDs.Length; i++)
            {
                gridInfo.SetItemID(dialogues[currentLine].spawnLocs[i], dialogues[currentLine].spawnIDs[i]);
            }
            if (dialogues[currentLine].isTutorial)
            {
                StartTutorial();
                dialogueTextDisplay.font = tutorialFont;
                speakerDisplay.font = tutorialFont;
            }
            else
            {
                dialogueTextDisplay.font = crabFont;
                speakerDisplay.font = crabFont;
            }
            speakerDisplay.text = dialogues[currentLine].speaker;
            StartCoroutine(TypeDialogue(dialogues[currentLine].dialogueText));
        }
        else
        {
            if (!playerController.gameOver)//Tutorial is over
            {
                dialogueTextDisplay.text = dialogues[currentLine-1].dialogueText;
                nextButton.gameObject.SetActive(false);
                playerController.canPlay = true;
            }
            else//Game is over
            {
                currentEndDialogue++;
                dialogueTextDisplay.font = crabFont;
                speakerDisplay.font = crabFont;
                nextButton.gameObject.SetActive(true);
                if (currentEndDialogue < endDialogues.Length)
                {
                    avatar.sprite = avatarFaces[endDialogues[currentEndDialogue].avatarFace];
                    speakerDisplay.text = endDialogues[currentEndDialogue].speaker;
                    StartCoroutine(TypeDialogue(endDialogues[currentEndDialogue].dialogueText));
                }
                else
                {
                    gameManager.NextLevel();
                }
            }
        }
    }

    IEnumerator TypeDialogue(string dialogue)
    {
        //string dialogue = dialogues[currentLine].dialogueText;
        for (int i = 0; i <= dialogue.Length; i++)
        {
            dialogueTextDisplay.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(typeDelay);
        }
    }

    public void FinishTutorial()
    {
        foreach (int slot in tutorials[currentTutorial].pickUpSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(gridInfo.transform);
            }
            else
            {
                gridInfo.grid[slot].GetComponent<RectTransform>().SetParent(gridInfo.transform);
            }
        }
        foreach (int slot in tutorials[currentTutorial].putDownSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(gridInfo.transform);
            }
            else
            {
                gridInfo.grid[slot].GetComponent<RectTransform>().SetParent(gridInfo.transform);
            }
        }
        shade.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        nextButton.gameObject.SetActive(true);
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
        return tutorials[currentTutorial].pickUpSlots.Contains(slot) || tutorials[currentTutorial].pickUpSlots.Length == 0;
    }

    public bool IsPutDown(int slot)
    {
        return tutorials[currentTutorial].putDownSlots.Contains(slot) || tutorials[currentTutorial].putDownSlots.Length == 0;
    }

    public bool ValidPickUp(int itemID)
    {
        return tutorials[currentTutorial].pickUpIDs.Contains(itemID) || tutorials[currentTutorial].pickUpIDs.Length == 0;
    }

    public bool ValidPutDown(int itemID)
    {
        return tutorials[currentTutorial].putDownIDs.Contains(itemID) || tutorials[currentTutorial].putDownIDs.Length == 0;
    }

    public bool GetTutorialIsPress()
    {
        return tutorials[currentTutorial].isPress;
    }

    public bool CompleteTutorial(int selectedSlot, int selectedID)
    {
        return GetTutorialIsPress() && IsPickUp(selectedSlot) && ValidPickUp(selectedID);
    }

    public bool CompleteTutorial(int prevSlot, int selectedSlot, int selectedID, int targetID)
    {
        return !GetTutorialIsPress() && IsPickUp(prevSlot) && IsPutDown(selectedSlot) && ValidPickUp(selectedID) && ValidPutDown(targetID);
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
            else
            {
                gridInfo.grid[slot].GetComponent<RectTransform>().SetParent(shade.transform);
            }
        }
        foreach (int slot in tutorials[currentTutorial].putDownSlots)
        {
            if (slot == -2)
            {
                bin.GetComponent<RectTransform>().SetParent(shade.transform);
            }
            else
            {
                gridInfo.grid[slot].GetComponent<RectTransform>().SetParent(shade.transform);
            }
        }

        nextButton.gameObject.SetActive(false);
        playerController.inTutorial = true;
        playerController.canPlay = true;
        if (tutorials[currentTutorial].pickUpSlots.Length != 0 || tutorials[currentTutorial].pickUpSlots.Length != 0)
        {
            shade.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        }
    }
}
