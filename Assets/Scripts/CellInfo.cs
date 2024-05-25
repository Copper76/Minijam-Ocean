using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellInfo : MonoBehaviour
{
    public int cellItemID;

    private Image image;

    private float cooldown;
    private float cooldownLimit;

    private int tapLimit;
    private int tapCount;

    private int[] spawnList;
    private int spawnCounter;

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0) 
            {
                cooldown = 0;
                image.color = Color.white;
            }
        }
    }

    public void SetImage(Image image)
    {
        this.image = image;
    }

    public Sprite GetSprite()
    {
        return image.sprite;
    }

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetLimits(int tapLimit, float cooldownLimit)
    {
        this.tapLimit = tapLimit;
        this.cooldownLimit = cooldownLimit;
    }

    //Check if the tap is successful
    public bool Tap()
    {
        if (cooldown > 0)
        {
            return false;
        }
        tapCount++;
        if (tapCount >= tapLimit)
        {
            tapCount = 0;
            cooldown = cooldownLimit;
            image.color = Color.gray;
        }
        return true;
    }

    public int GetNextSpawn()
    {
        int spawnID = spawnList[spawnCounter++];
        spawnCounter %= spawnList.Length;
        return spawnID;
    }

    public void SetSpawnList(int[] spawnList)
    {
        this.spawnList = spawnList;
        spawnCounter = 0;
    }
}

