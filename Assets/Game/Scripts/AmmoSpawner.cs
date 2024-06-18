using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class AmmoSpawner : MonoBehaviour
{
    public GameObject pickZone;
    private Interactable pickZoneInteractable;
    private Vector3 pickZonePos;
    private Quaternion pickZoneRot;
    public GameObject magazine;
    public AudioSource audioSource;
    public int count;

    void Start()
    {
        pickZonePos = pickZone.transform.position;
        pickZoneRot = pickZone.transform.rotation;
        pickZoneInteractable = pickZone.GetComponent<Interactable>();
    }
    void Update()
    {
        pickZone.transform.position = pickZonePos;
        pickZone.transform.rotation = pickZoneRot;
        if (pickZoneInteractable.attachedToHand != null && count != 0)
        {
            count--;
            Hand hand = pickZoneInteractable.attachedToHand;
            var newMag = Instantiate(magazine, pickZone.transform.position, Quaternion.identity);
            var newMagInteractable = newMag.GetComponent<Interactable>();
            audioSource.Play();
            if (newMagInteractable != null)
            {
                hand.DetachObject(pickZone);
                hand.AttachObject(newMag, GrabTypes.Grip);
            }
        }
    }
}
