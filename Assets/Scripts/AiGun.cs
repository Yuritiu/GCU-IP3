using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGun : MonoBehaviour
{
    public static AiGun instance;
    public bool ForBot = true;

    private void Start()
    {
        instance = this;
    }
}
