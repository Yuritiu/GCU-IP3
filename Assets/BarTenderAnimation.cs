using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BarTenderAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private float danceCount = 120f;
    private float timeStore;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        StandingTr();
        timeStore = danceCount;
    }

    private void FixedUpdate()
    {
        if (timeStore < Time.time) 
        {
            DanceTr();
            timeStore = Time.time + danceCount;
        }
    }

    public void DanceTr()
    {
        animator.SetTrigger("TrDance");
    }

    public void BottleTr()
    {
        animator.SetTrigger("TrBottle");
    }

    public void StandingTr()
    {
        animator.SetTrigger("TrStanding");
    }
}
