using Assets.Scripts;
using System;
using UnityEngine;
[Serializable]
public class Player : MonoBehaviour
{
    public PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerData == null)
        {
            playerData = new PlayerData();  
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
