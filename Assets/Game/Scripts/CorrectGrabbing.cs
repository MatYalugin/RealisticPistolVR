using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class CorrectGrabbing : MonoBehaviour
{
    private Interactable interactable;
    public SteamVR_Action_Boolean buttonAAction;
    public bool PrintLocalPositionAndRotation;
    public bool changeOnlyRotation;
    public Vector3 correctPosition;
    public Quaternion correctRotation;
    void Start()
    {
        interactable = gameObject.GetComponent<Interactable>();
    }
    public void FindInteractableAgain()
    {
        interactable = gameObject.GetComponent<Interactable>();
    }

    void Update()
    {
        if (interactable.attachedToHand != null)
        {
            if (PrintLocalPositionAndRotation)
            {
                SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
                if (buttonAAction[hand].stateDown)
                {

                    Debug.Log("Position: " + gameObject.transform.localPosition);
                    Debug.Log("Rotation: " + gameObject.transform.localRotation);
                }
            }
            else
            {
                if (!changeOnlyRotation)
                {
                    gameObject.transform.localPosition = correctPosition;
                }
                gameObject.transform.localRotation = correctRotation;
            }
        }
    }
}
