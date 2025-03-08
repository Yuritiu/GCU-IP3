using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
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
