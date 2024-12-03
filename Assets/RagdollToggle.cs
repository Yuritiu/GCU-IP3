using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] Transform rootBone;

    public bool ragdoll = false;

    Rigidbody[] rbs;
    CharacterJoint[] joints;
    Collider[] colliders;

    void Awake()
    {
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
    }

    public void EnableRagdoll(bool enabled)
    {
        animator.enabled = !enabled;

        foreach (Collider collider in colliders) 
        {
            if(collider.name == "spine")
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
}
