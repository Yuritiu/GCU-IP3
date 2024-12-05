using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    public static RagdollToggle Instance;

    [Header("References")]
    Animator animator;
    Transform rootBone;

    [SerializeField] public bool ragdoll = false;
    bool calledCoroutine = false;

    Rigidbody[] rbs;
    CharacterJoint[] joints;
    Collider[] colliders;

    void Awake()
    {
        Instance = this;

        animator = GetComponentInChildren<Animator>();
        rootBone = GameObject.FindGameObjectWithTag("RootBone").GetComponent<Transform>();
        rbs = rootBone.GetComponentsInChildren<Rigidbody>();
        joints = rootBone.GetComponentsInChildren<CharacterJoint>();
        colliders = rootBone.GetComponentsInChildren<Collider>();

        if (ragdoll)
        {
            EnableRagdoll(true);
        }
        else
        {
            EnableRagdoll(false);
        }
    }

    void Update()
    {
        if (ragdoll)
        {
            EnableRagdoll(true);
        }
        else
        {
            EnableRagdoll(false);
        }

        //Reset Ragdoll When It's Turn To Play Again
        if(GameManager.Instance.aiSkippedTurns <= 0 && ragdoll && !calledCoroutine)
        {
            calledCoroutine = true;
            StartCoroutine(DisableAIRagdollToPlayTurn());
        }
    }

    public void EnableRagdoll(bool enabled)
    {
        animator.enabled = !enabled;

        foreach (Collider collider in colliders) 
        {
            if(collider.name == "spine" || collider.tag == "OpponentHead")
            {
                collider.enabled = true;
            }
            else
            {
                collider.enabled = enabled;
            }
        }

        foreach (CharacterJoint joint in joints)
        {
            joint.enableCollision = enabled;
        }

        foreach(Rigidbody rb in rbs)
        {
            if (rb.name == "spine")
            {
                rb.isKinematic = true;
            }
            else
            {
                rb.useGravity = enabled;
                rb.detectCollisions = enabled;
                //rb.velocity = Vector3.zero;
            }
        }
    }

    IEnumerator DisableAIRagdollToPlayTurn()
    {
        yield return new WaitForSeconds(3);

        ragdoll = false;
        calledCoroutine = false;
    }
}
