using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class WeaponSecondGrabPoint : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    public GameObject parentGO;
    private Interactable interactable;
    void Start()
    {
        startPos = gameObject.transform.localPosition;
        startRot = gameObject.transform.localRotation;
        interactable = gameObject.GetComponent<Interactable>();
    }
    public Interactable returnInteractable()
    {
        return interactable;
    }

    void Update()
    {
        gameObject.transform.parent = parentGO.transform;
        gameObject.transform.localPosition = startPos;
        gameObject.transform.localRotation = startRot;
    }
}
