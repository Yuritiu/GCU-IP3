using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] List<GameObject> fingers;

    public void RemoveFinger(int num)
    {
        Destroy(fingers[num]);
        fingers.Remove(fingers[num]);
    }
}
