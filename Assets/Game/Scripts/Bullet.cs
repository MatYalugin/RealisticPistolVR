using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Bullet : MonoBehaviour
{
    private Interactable interactable;
    public string caliber;
    private Magazine magazine;
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag.Equals("BulletPoint"))
        {
            magazine = other.transform.gameObject.GetComponentInParent<Magazine>();
            if(interactable.attachedToHand != null && caliber == magazine.caliber && magazine.ReturnInteractable().attachedToHand != null)
            {
                other.transform.gameObject.GetComponentInParent<Magazine>().AddBullet();
                Destroy(gameObject);
            }
        }
    }
}
