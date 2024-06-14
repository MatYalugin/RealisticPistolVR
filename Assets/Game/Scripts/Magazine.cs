using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Magazine : MonoBehaviour
{
    public int ammo;
    private Interactable interactable;
    private Throwable throwable;
    public Rigidbody body;
    public List<GameObject> bullets;
    public Quaternion correctRot;
    private void Start()
    {
        //bullets.Reverse();
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
    }
    private void Update()
    {
        if(ammo < 0)
        {
            ammo = 0;
        }
        foreach(var obj in bullets)
        {
            if(bullets.IndexOf(obj) + 1 > ammo)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
        if (interactable.attachedToHand != null)
        {
            gameObject.transform.localRotation = correctRot;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag.Equals("MagazinePoint") && interactable.attachedToHand != null)
        {
            if(other.transform.gameObject.GetComponentInParent<Weapon>().GetMagazineGO() == null && other.transform.gameObject.GetComponentInParent<Weapon>().interactable.attachedToHand != null)
            {
                Destroy(throwable);
                Destroy(interactable);
                other.transform.gameObject.GetComponentInParent<Weapon>().InsertMagazine(gameObject);
            }
        }
    }
    public void SetInteractableAndThrowable()
    {
        gameObject.AddComponent<Throwable>();
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
    }
}
