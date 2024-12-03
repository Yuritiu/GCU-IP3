using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThwackSFX : MonoBehaviour
{
    public static ThwackSFX Instance;

    [SerializeField] AudioSource thwackSFX;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayThwackSFX()
    {
        thwackSFX.Play();
    }
}
