using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSecondGrabPoint : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    public GameObject parentGO;
    void Start()
    {
        startPos = gameObject.transform.localPosition;
        startRot = gameObject.transform.localRotation;
    }

    void Update()
    {
        gameObject.transform.parent = parentGO.transform;
        gameObject.transform.localPosition = startPos;
        gameObject.transform.localRotation = startRot;
    }
}
