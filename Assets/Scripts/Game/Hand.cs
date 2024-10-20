using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] List<GameObject> fingers;

    public void RemoveFinger()
    {
        Destroy(fingers[0]);
        fingers.Remove(fingers[0]);
    }
}
