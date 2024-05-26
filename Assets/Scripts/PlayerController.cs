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

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GridInfo gridInfo;

    [SerializeField] private GameObject selected;

    private int selectedID;
    private int prevSlot;

    private Vector3 selectedOffset;

    public bool inTutorial = false;
    public bool canPlay = false;
    public bool gameOver = false;
    [SerializeField] private TutorialController tutorialController;

    // Start is called before the first frame update
    void Update()
    {
        if (selected.activeInHierarchy)
        {
            selected.GetComponent<RectTransform>().position = Input.mousePosition + selectedOffset;
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
                        break;
                    case 2:
                        gridInfo.Tap(selectedSlot);
                        break;
                    case 7:

                    case 8:
                    case 9:
                    case 10:

                    default:
                        selected.SetActive(true);
                        selected.GetComponent<Image>().sprite = gridInfo.grid[selectedSlot].GetSprite();
                        selected.GetComponent<RectTransform>().sizeDelta = gridInfo.cellSize;
                        selectedOffset = Vector3.zero;
                        selectedID = targetID;
                        gridInfo.SetItemID(selectedSlot, 0);
                        prevSlot = selectedSlot;
                        break;
                }
                if (inTutorial && tutorialController.GetTutorialIsPress() && tutorialController.IsPickUp(selectedSlot))
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
                if (targetID == 0)
                {
                    gridInfo.SetItemID(selectedSlot, selectedID);
                    //If the item is a puffer fish tri to combine three
                    if (selectedID == 3 || selectedID == 5)
                    {
                        gridInfo.TryFuseBomb(selectedSlot, selectedID);
                    }
                }
                else if (TryMerge(selectedID, targetID))
                {
                    //merge scenario
                }
                else
                {
                    gridInfo.SetItemID(prevSlot, targetID);
                    gridInfo.SetItemID(selectedSlot, selectedID);
                    if (selectedID == 3 || selectedID == 5)
                    {
                        gridInfo.TryFuseBomb(selectedSlot, selectedID);
                    }
                }
            }
            else if (selectedSlot == -2)//Mouse is on the bin
            {

            }
            else//The pointer is not in on a slot
            {
                gridInfo.SetItemID(prevSlot, selectedID);
            }
            selected.SetActive(false);

            if (inTutorial && !tutorialController.GetTutorialIsPress() && tutorialController.IsPickUp(prevSlot) && tutorialController.IsPutDown(selectedSlot))
            {
                tutorialController.FinishTutorial();
            }
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

    bool TryMerge(int itemLeft, int itemRight)
    {
        return false;
    }

    void Win()
    {
        canPlay = false;
        tutorialController.NextLine();
    }
}
