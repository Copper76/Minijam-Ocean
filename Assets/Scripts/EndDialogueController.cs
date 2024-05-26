using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class EndDialogueController : MonoBehaviour
{
    [SerializeField] private Image avatar;
    [SerializeField] private TextMeshProUGUI speakerDisplay;
    [SerializeField] private TextMeshProUGUI dialogueTextDisplay;
    [SerializeField] private GameObject nextButton;

    [SerializeField] private Sprite[] avatarFaces;

    [SerializeField] private float typeDelay = 0.1f;

    [SerializeField] private DialogueInfo[] dialogues;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private int currentLine = -1;

    // Start is called before the first frame update
    void Start()
    {
        NextLineEnd();
    }

    IEnumerator TypeDialogue()
    {
        string dialogue = dialogues[currentLine].dialogueText;
        for (int i = 0; i<= dialogue.Length; i++)
        {
            dialogueTextDisplay.text = dialogue.Substring(0,i);
            yield return new WaitForSeconds(typeDelay);
        }
    }

    public void NextLineEnd()
    {
        StopAllCoroutines();
        currentLine++;
        if (currentLine < dialogues.Length)
        {
            avatar.sprite = avatarFaces[dialogues[currentLine].avatarFace];
            speakerDisplay.text = dialogues[currentLine].speaker;
            StartCoroutine(TypeDialogue());
        }
        else
        {
            dialogueTextDisplay.text = "";
            nextButton.SetActive(false);
            gameManager.NextLevel();
        }
    }
}
