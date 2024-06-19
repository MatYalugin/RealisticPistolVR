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
        ammo = Mathf.Clamp(ammo, 0, bullets.Count);
        DisplayBulletsInMagazine();
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
        var newBullet = Instantiate(bullet, bulletPoint.position, bulletPoint.transform.localRotation);
        newBullet.GetComponent<Rigidbody>().AddForce(bulletPoint.up * 0.02f, ForceMode.Impulse);
    }
    public void SetInteractableAndThrowable()
    {
        gameObject.AddComponent<Throwable>();
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
        gameObject.GetComponent<CorrectGrabbing>().FindInteractableAgain();
    }
}
