using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Net;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GridInfo gridInfo;

    [SerializeField] private GameObject selected;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ClipDepot clipDepot;

    private int selectedID;
    private int prevSlot;

    public bool inTutorial = false;
    public bool canPlay = false;
    public bool gameOver = false;
    [SerializeField] private TutorialController tutorialController;

    [SerializeField] private int[] winningCondition;
    private List<int> winningCells = new List<int>();

    // Start is called before the first frame update
    void Update()
    {
        if (selected.activeInHierarchy)
        {
            selected.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void Press(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int selectedSlot = IsPointerOverSlot();
            if (selectedSlot > -1)
            {
                int targetID = gridInfo.GetItemID(selectedSlot);
                switch (targetID)
                {
                    case 0://Nothing happens if there is nothing on the slot
                        break;
                    case 1:
                        gridInfo.Tap(selectedSlot);
                        audioSource.PlayOneShot(clipDepot.clips[3]);
                        break;
                    case 2:
                        gridInfo.Tap(selectedSlot);
                        audioSource.PlayOneShot(clipDepot.clips[4]);
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11:
                        break;
                    case 12:
                        break;
                    case 13:
                        break;
                    case 14:
                        break;
                    default:
                        selected.SetActive(true);
                        selected.GetComponent<Image>().sprite = gridInfo.grid[selectedSlot].GetSprite();
                        selected.GetComponent<RectTransform>().sizeDelta = gridInfo.cellSize;
                        selectedID = targetID;
                        gridInfo.SetItemID(selectedSlot, 0);
                        prevSlot = selectedSlot;
                        break;
                }
                if (inTutorial && tutorialController.CompleteTutorial(selectedSlot, targetID))
                {
                    tutorialController.FinishTutorial();
                }
            }
        }
    }

    public void Release(InputAction.CallbackContext context)
    {
        if (context.performed && selected.activeInHierarchy)
        {
            int selectedSlot = IsPointerOverSlot();
            if (selectedSlot > -1)//The pointer is on a valid slot
            {
                int targetID = gridInfo.GetItemID(selectedSlot);
                switch (targetID)
                {
                    case 1:
                        gridInfo.SetItemID(prevSlot, selectedID);
                        break;
                    case 2:
                        gridInfo.SetItemID(prevSlot, selectedID);
                        break;
                    case 6:
                        if (selectedID >=19 && selectedID<23)
                        {
                            gridInfo.SetItemID(selectedSlot, selectedID + 4);
                            audioSource.PlayOneShot(clipDepot.clips[5]);
                        }
                        break;
                    case 7:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot, 7))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 8:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot-1, 7))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 9:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot-gridInfo.gridWidth, 7))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 10:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot-gridInfo.gridWidth-1, 7))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 11:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot, 11))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 12:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot - gridInfo.gridWidth, 11))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 13:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot, 13))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 14:
                        if (selectedID == 4)
                        {
                            if(gridInfo.TryBlow(selectedSlot - 1, 13))
                            {
                                audioSource.PlayOneShot(clipDepot.clips[1]);
                            }
                        }
                        else
                        {
                            gridInfo.SetItemID(prevSlot, selectedID);
                        }
                        break;
                    case 19:
                        if (selectedID == 6)
                        {
                            gridInfo.SetItemID(selectedSlot, 23);
                            audioSource.PlayOneShot(clipDepot.clips[5]);
                        }
                        break;
                    case 20:
                        if (selectedID == 6)
                        {
                            gridInfo.SetItemID(selectedSlot, 24);
                            audioSource.PlayOneShot(clipDepot.clips[5]);
                        }
                        break;
                    case 21:
                        if (selectedID == 6)
                        {
                            gridInfo.SetItemID(selectedSlot, 25);
                            audioSource.PlayOneShot(clipDepot.clips[5]);
                        }
                        break;
                    case 22:
                        if (selectedID == 6)
                        {
                            gridInfo.SetItemID(selectedSlot, 26);
                            audioSource.PlayOneShot(clipDepot.clips[5]);
                        }
                        break;
                    default:
                        gridInfo.SetItemID(prevSlot, targetID);
                        gridInfo.SetItemID(selectedSlot, selectedID);
                        gridInfo.TryFuse(selectedSlot, selectedID);
                        break;
                }

                CheckWin();

                if (inTutorial && tutorialController.CompleteTutorial(prevSlot, selectedSlot, selectedID, targetID))
                {
                    tutorialController.FinishTutorial();
                }
            }
            else if (selectedSlot == -2)//Mouse is on the bin
            {
                audioSource.PlayOneShot(clipDepot.clips[6]);
                if (inTutorial && tutorialController.CompleteTutorial(prevSlot, selectedSlot, selectedID, -2))
                {
                    tutorialController.FinishTutorial();
                }
            }
            else//The pointer is not in on a slot
            {
                gridInfo.SetItemID(prevSlot, selectedID);
            }
            selected.SetActive(false);
        }
    }

    public void Shuffle()
    {
        if (canPlay && (!inTutorial || (inTutorial && tutorialController.IsValidSlot(-3))))
        {
            gridInfo.Shuffle();
        }
    }

    public int IsPointerOverSlot()
    {
        return IsPointerOverSlot(GetEventSystemRaycastResults());
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    private int IsPointerOverSlot(List<RaycastResult> eventSystemRaysastResults)
    {
        if (!canPlay) return -1;
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("Slot"))
            {
                int id = Convert.ToInt32(curRaysastResult.gameObject.name);
                if (!inTutorial || (inTutorial && tutorialController.IsValidSlot(id)))
                {
                    return id;
                }
            }
        }
        return -1;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    void Win()
    {
        audioSource.PlayOneShot(clipDepot.clips[7]);
        canPlay = false;
        gameOver = true;
        tutorialController.NextLine();
    }

    void CheckWin()
    {
        winningCells = winningCondition.ToList();
        foreach (CellInfo cell in gridInfo.grid)
        {
            winningCells.Remove(cell.cellItemID);
            if (winningCells.Count == 0)
            {
                Win();
                break;
            }
        }
    }
}
