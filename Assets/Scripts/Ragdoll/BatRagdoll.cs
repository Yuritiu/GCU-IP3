using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatRagdoll : MonoBehaviour
{
    public static BatRagdoll Instance;
   
    bool triggeredRagdoll = false;
    bool coroutineCalled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("OpponentHead") && !triggeredRagdoll)
        {
            triggeredRagdoll = true;

            if(!coroutineCalled)
            {
                //Unfreeze The Players Camera After Swinging Bat & After Small Delay
                StartCoroutine(UnfreezeCameraDelay());
            }

            //Enable The Ragdoll
            RagdollToggle.Instance.ragdoll = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        triggeredRagdoll = false;
    }

    IEnumerator UnfreezeCameraDelay()
    {
        coroutineCalled = true;

        Freelook.Instance.mouseX = 0;
        Freelook.Instance.mouseY = 0;

        Freelook.Instance.xRotation = 0;
        Freelook.Instance.yRotation = 0;
        Freelook.Instance.currentXRotation = 0;
        Freelook.Instance.currentYRotation = 0;

        yield return new WaitForSeconds(1);
        
        Freelook.Instance.inBatSwing = false;

        coroutineCalled = false;
    }
}
