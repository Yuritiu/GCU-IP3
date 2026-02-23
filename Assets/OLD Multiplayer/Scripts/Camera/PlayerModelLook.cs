using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerModelLook : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform headBone;
    [SerializeField] public Transform cameraTransform;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public float maxPitch = 120f;
    public float maxYaw = 180f;

    private Quaternion initialLocalRotation;

    //NetworkVariable to Sync Head Rotation (world rotation of head bone)
    private NetworkVariable<Quaternion> networkedHeadWorldRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Quaternion targetRemoteRotation;

    void Start()
    {
        if(headBone == null ||  cameraTransform == null)
        {
            Debug.LogError("Head bone OR camera transform missing reference!");
            return;
        }

        initialLocalRotation = headBone.localRotation;
        targetRemoteRotation = initialLocalRotation;
        //Listen For Changes From Network & Apply Them on Non Owner Players
        networkedHeadWorldRotation.OnValueChanged += OnNetworkedHeadRotationChanged;
    }

    void OnDestroy()
    {
        networkedHeadWorldRotation.OnValueChanged -= OnNetworkedHeadRotationChanged;
    }

    void LateUpdate()
    {
        if (IsOwner)
        {
            //Calculate Rotation & Apply It Locally
            Quaternion newRotation = RotateHeadTowardCamera();
            headBone.localRotation = Quaternion.Slerp(headBone.localRotation, targetRemoteRotation * newRotation, Time.deltaTime * rotationSpeed);
            //Send The Owner's World Head Rotation to The Network
            networkedHeadWorldRotation.Value = headBone.rotation;
        }
        else
        {
            Quaternion newRotation = RotateHeadTowardCamera();
            // Apply smooth rotation toward latest received rotation
            headBone.rotation = Quaternion.Slerp(headBone.rotation,networkedHeadWorldRotation.Value,Time.deltaTime * rotationSpeed);
        }
    }

    #region Network Head Rotation
    private void OnNetworkedHeadRotationChanged(Quaternion previous, Quaternion current)
    {
        if (IsOwner) return;

        //Debug.Log("Received head rotation: " + current.eulerAngles);
        //Apply Received Rotation on Remote Clients Smoothly
        targetRemoteRotation = current;
    }
    #endregion

    #region Handle Head Rotation
    private Quaternion RotateHeadTowardCamera()
    {
        Vector3 directionToLook = cameraTransform.forward;
        Vector3 localDirection = transform.InverseTransformDirection(directionToLook);
        Quaternion targetRotation = Quaternion.LookRotation(localDirection, Vector3.up);

        Vector3 targetEuler = targetRotation.eulerAngles;
        targetEuler.x = NormalizeAngle(targetEuler.x);
        targetEuler.y = NormalizeAngle(targetEuler.y);

        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxPitch, maxPitch);
        targetEuler.y = Mathf.Clamp(targetEuler.y, -maxYaw, maxYaw);

        return Quaternion.Euler(-targetEuler.y, 0f, targetEuler.x);
    }

    float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
    #endregion
}