using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public static PlayerGun instance;
    public bool ForPlayer = true;

    private void Start()
    {
        instance = this;
    }
}


