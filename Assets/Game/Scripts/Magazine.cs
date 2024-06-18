using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Magazine : MonoBehaviour
{
    public int ammo;
    public string caliber;
    public string magazineType;
    private Interactable interactable;
    public SteamVR_Action_Boolean buttonAAction;
    public SteamVR_Action_Boolean buttonXAction;
    private Throwable throwable;
    public Rigidbody body;
    public List<GameObject> bullets;
    public Quaternion correctRot;
    public GameObject bullet;
    public Transform bulletPoint;
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
        KeepAmmoBetweenZeroAndBulletsCount();
        DisplayBulletsInMagazine();
        SetCorrectRotationByHolding();
        CheckBulletDropButton();
    }
    private void CheckBulletDropButton()
    {
        if (interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
            if (ammo != 0 && (buttonAAction[hand].stateDown || buttonXAction[hand].stateDown))
            {
                DropBullet();
            }
        }
    }
    private void KeepAmmoBetweenZeroAndBulletsCount()
    {
        if (ammo < 0)
        {
            ammo = 0;
        }
        if(ammo > bullets.Count)
        {
            ammo = bullets.Count;
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
            if(other.transform.gameObject.GetComponentInParent<Weapon>().GetMagazineGO() == null && other.transform.gameObject.GetComponentInParent<Weapon>().returnInteractable().attachedToHand != null && magazineType == other.transform.gameObject.GetComponentInParent<Weapon>().magazineType)
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
        var rightRotation = Quaternion.Euler(-90, bulletPoint.transform.rotation.y, bulletPoint.transform.rotation.z);
        var newBullet = Instantiate(bullet, bulletPoint.position, rightRotation);
        newBullet.GetComponent<Rigidbody>().AddForce(bulletPoint.up * 0.02f, ForceMode.Impulse);
    }
    public void SetInteractableAndThrowable()
    {
        gameObject.AddComponent<Throwable>();
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
    }
}
