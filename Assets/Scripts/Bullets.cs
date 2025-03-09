using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    private GameManager gameManager;
    public List<GameObject> bulletObjects;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        int bulletsInGun = gameManager.bullets;

        if (bulletsInGun > 0 && bulletsInGun <= bulletObjects.Count)
        {
            bulletObjects[bulletsInGun - 1].SetActive(false);
        }
    }
}
