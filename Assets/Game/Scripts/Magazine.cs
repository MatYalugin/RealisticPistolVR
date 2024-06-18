using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Magazine : MonoBehaviour
{
    public int ammo;
    public string caliber;
    private Interactable interactable;
    public SteamVR_Action_Boolean buttonAAction;
    private Throwable throwable;
    public Rigidbody body;
    public List<GameObject> bullets;
    public Quaternion correctRot;
    public GameObject bullet;
    public Transform bulletPoint;
    public Quaternion bulletRotOnDrop;
    public AudioSource bulletPointAudioSource;
    private void Start()
    {
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
    }
    public Interactable ReturnInteractable()
    {
        return interactable;
    }
    private void Update()
    {
        DoNotAllowAmmoGoBelowZero();
        DisplayBulletsInMagazine();
        SetCorrectRotationByHolding();
        CheckBulletDropButton();
    }
    private void CheckBulletDropButton()
    {
        if (interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
            if (buttonAAction[hand].stateDown && ammo != 0)
            {
                DropBullet();
            }
        }
    }
    private void DoNotAllowAmmoGoBelowZero()
    {
        if (ammo < 0)
        {
            ammo = 0;
        }
    }
    private void DisplayBulletsInMagazine()
    {
        foreach (var obj in bullets)
        {
            if (bullets.IndexOf(obj) + 1 > ammo)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }
    private void SetCorrectRotationByHolding()
    {
        if (interactable.attachedToHand != null)
        {
            //localRotation!!
            gameObject.transform.localRotation = correctRot;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag.Equals("MagazinePoint") && interactable.attachedToHand != null)
        {
            if(other.transform.gameObject.GetComponentInParent<Weapon>().GetMagazineGO() == null && other.transform.gameObject.GetComponentInParent<Weapon>().returnInteractable().attachedToHand != null)
            {
                Destroy(throwable);
                Destroy(interactable);
                other.transform.gameObject.GetComponentInParent<Weapon>().InsertMagazine(gameObject);
            }
        }
    }
    public void AddBullet()
    {
        ammo++;
        bulletPointAudioSource.Play();
    }
    public void DropBullet()
    {
        ammo--;
        bulletPointAudioSource.Play();
        var newBullet = Instantiate(bullet, bulletPoint.position, bulletRotOnDrop);
        newBullet.GetComponent<Rigidbody>().AddForce(bulletPoint.up * 0.02f, ForceMode.Impulse);
    }
    public void SetInteractableAndThrowable()
    {
        gameObject.AddComponent<Throwable>();
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
    }
}
