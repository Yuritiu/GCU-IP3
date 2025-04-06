using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAnimationController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private RagdollToggle ragdoll;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        //Had to add this for the Animation Capture Scene, sorry!
        if (gameObject.GetComponent<RagdollToggle>() != null)
        {
            ragdoll = gameObject.GetComponent<RagdollToggle>();
            ragdoll.enabled = true;
        }
        IdleTr();
    }

    public void IdleTr()
    {
        animator.SetTrigger("TrIdle");
    }

    public void CigarTr()
    {
        animator.SetTrigger("TrCigar");
    }

    public void KnifeTr()
    {
        animator.SetTrigger("TrKnife");
    }

    public void GunTr()
    {
        animator.SetTrigger("TrGun");
    }
}
