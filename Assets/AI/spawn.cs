using UnityEngine;
using System.Collections.Generic;

public class Spawn : MonoBehaviour
{
    [SerializeField] private Transform transformE;
    [SerializeField] private Transform transformF;
    [SerializeField] private float swapInterval = 2f;
    [SerializeField] private Player player;
    public string wpNameSelected;
    [SerializeField] private Transform[] transformPairs;

    private void Start()
    {
        playerSpawn();
        if (transformE != null && transformF != null)
        {
            InvokeRepeating(nameof(SwapPositions2), swapInterval, swapInterval);
        }
        if (transformPairs != null && transformPairs.Length >= 2)
        {
            InvokeRepeating(nameof(SwapAllPositions), swapInterval, swapInterval);
        }

    }
    private void Update()
    {
        if (player.isDead)
        {
            playerSpawn();
            player.heath = 100;
            player.isDead = false;

        }
    }
    private void playerSpawn()
    {
        if (player)
        {
            player.wpName = wpNameSelected;
            player.transform.localPosition = transformE.localPosition;
            player.ReloadInfo();
        }
    }

    private void SwapPositions2()
    {
        Vector3 tempPosition2 = transformE.localPosition;
        transformE.localPosition = transformF.localPosition;
        transformF.localPosition = tempPosition2;
    }
    private void SwapAllPositions()
    {
        Vector3 lastPosition = transformPairs[transformPairs.Length - 1].localPosition;

        for (int i = transformPairs.Length - 1; i > 0; i--)
        {
            transformPairs[i].localPosition = transformPairs[i - 1].localPosition;
        }
        transformPairs[0].localPosition = lastPosition;
    }

}
