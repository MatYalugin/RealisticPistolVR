using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Weapon : MonoBehaviour
{
    public SteamVR_Action_Boolean buttonAAction;
    public SteamVR_Action_Boolean buttonBAction;
    public Interactable interactable;
    private SteamVR_Action_Boolean fireAction;
    private bool isReadyToShoot = true;
    private float delay = 0.4f; //must be greater than the shot animation length
    public GameObject bulletProjectile;
    public GameObject sleeve;
    public GameObject bullet;
    public GameObject muzzlePoint;
    public GameObject sleevePoint;
    public float force = 3f;
    public AudioSource shotSound;
    public ParticleSystem shotEffect;
    public Animator triggerAnimator;
    public bool stopTimeOnShot;
    private GameObject magazineGO;
    private Magazine magazine;
    private bool magazineInserted;
    private bool isMovingToEmptyState;
    public GameObject bulletInChamberGO;

    public bool hasHammer;
    public Animator hammerAnimator;

    private bool movingPartWasInEndPosZ;
    public Vector3 magazineInWeaponPos;
    public Quaternion magazineInWeaponRot;
    public Transform magazinePoint;
    public Transform magazinePointDrop;
    public Sliding movingPartSliding;
    public GameObject secondGrabPoint;
    public AudioSource magActionsAudio;
    public AudioClip magInsertAudio;
    public AudioClip magDropAudio;
    public Quaternion correctRot;
    void Start()
    {
        interactable = GetComponent<Interactable>();
        fireAction = SteamVR_Actions.default_InteractUI;
    }
    public GameObject GetMagazineGO()
    {
        return magazineGO;
    }

    void Update()
    {
        if (magazineGO != null)
        {
            magazineGO.transform.parent = gameObject.transform;
            magazineGO.transform.localPosition = magazineInWeaponPos;
            magazineGO.transform.localRotation = magazineInWeaponRot;
            magazineGO.GetComponent<Collider>().enabled = false;
        }
        //localPosition!!
        if (isMovingToEmptyState)
        {
            movingPartSliding.isInEmptyState = true;
            movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, Mathf.MoveTowards(gameObject.transform.localPosition.z, movingPartSliding.endPosZ, Time.deltaTime));
            if (movingPartSliding.gameObject.transform.localPosition != new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.endPosZ))
            {
                movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.endPosZ);
                isMovingToEmptyState = false;
            }
        }
        if (movingPartSliding.gameObject.transform.localPosition != new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.endPosZ) && movingPartSliding.isInEmptyState)
        {
            movingPartSliding.isInEmptyState = false;
        }



        if (Input.GetKeyDown(KeyCode.K) && magazineInserted)
        {
            DropMagazine();
        }
        if (Input.GetKeyDown(KeyCode.N) && isReadyToShoot && bulletInChamberGO.activeSelf)
        {
            Shot();
        }
        if (Input.GetKeyDown(KeyCode.V) && movingPartSliding.isInEmptyState)
        {
            movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, Mathf.MoveTowards(gameObject.transform.localPosition.z, movingPartSliding.startPos.z, Time.deltaTime));
            if (movingPartSliding.gameObject.transform.localPosition != new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z))
            {
                movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z);
                movingPartSliding.playSound();
            }
        }



        if (magazineGO != null)
        {
            magazineInserted = true;
            magazine = magazineGO.GetComponent<Magazine>();
            magazine.body.isKinematic = true;
        }
        else
        {
            magazineInserted = false;
            magazine = null;
        }

        if (interactable.attachedToHand != null)
        {
            secondGrabPoint.SetActive(true);
            SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
            if (fireAction[hand].stateDown && isReadyToShoot && bulletInChamberGO.activeSelf)
            {
                Shot();
            }
            if (buttonAAction[hand].stateDown && magazineInserted)
            {
                DropMagazine();
            }
            if (buttonBAction[hand].stateDown && movingPartSliding.isInEmptyState)
            {
                movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, Mathf.MoveTowards(gameObject.transform.localPosition.z, movingPartSliding.startPos.z, Time.deltaTime));
                if (movingPartSliding.gameObject.transform.localPosition != new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z))
                {
                    movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z);
                    movingPartSliding.playSound();
                }
            }
        }
        else
        {
            Hand hand = secondGrabPoint.GetComponent<Interactable>().attachedToHand;
            if(secondGrabPoint.GetComponent<Interactable>().attachedToHand == secondGrabPoint)
            {
                hand.DetachObject(secondGrabPoint);
            }
            secondGrabPoint.SetActive(false);
        }
        //localPosition!!
        if (movingPartSliding.gameObject.transform.localPosition.z == movingPartSliding.endPosZ)
        {
            movingPartWasInEndPosZ = true;
            if (bulletInChamberGO.activeSelf)
            {
                bulletInChamberGO.SetActive(false);
                var newBullet = Instantiate(bullet, sleevePoint.transform.position, sleevePoint.transform.rotation);
                newBullet.GetComponent<Rigidbody>().AddForce(sleevePoint.transform.right * 0.1f, ForceMode.Impulse);
            }
        }
        if (movingPartSliding.gameObject.transform.localPosition.z == movingPartSliding.startPos.z && movingPartWasInEndPosZ)
        {
            TryToLoadBulletInChamber();
        }
        if (bulletInChamberGO.activeSelf && hasHammer)
        {
            hammerAnimator.Play("HammerReadyToHit");
        }
        else
        {
            hammerAnimator.Play("HammerNotReadyToHit");
        }
    }
    public void DropMagazine()
    {
        magazineGO.transform.position = magazinePointDrop.position;
        magazineGO.GetComponent<Rigidbody>().isKinematic = false;
        magazineGO.transform.SetParent(null);
        magazine.SetInteractableAndThrowable();
        magazineGO.GetComponent<Collider>().enabled = true;
        magazineGO = null;
        magActionsAudio.clip = magDropAudio;
        magActionsAudio.Play();
    }
    public void InsertMagazine(GameObject mag)
    {
        magazineGO = mag;
        magActionsAudio.clip = magInsertAudio;
        magActionsAudio.Play();
    }
    public void TryToLoadBulletInChamber()
    {
        movingPartWasInEndPosZ = false;
        if (magazineInserted)
        {
            if (magazine.ammo > 0)
            {
                bulletInChamberGO.SetActive(true);
                magazine.ammo--;
                movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z);
            }
        }
    }
    public void ExitFromEmptyState()
    {
        movingPartSliding.isInEmptyState = false;
    }
    public void Shot()
    {
        movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.endPosZ);
        bulletInChamberGO.SetActive(false);
        triggerAnimator.Play("Shot");
        shotSound.Play();
        shotEffect.Play();
        isReadyToShoot = false;
        Invoke("makeReadyToShoot", delay);
        var newBulletRotation = muzzlePoint.transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
        var newBulletProjectile = Instantiate(bulletProjectile, muzzlePoint.transform.position, newBulletRotation);
        var newSleeve = Instantiate(sleeve, sleevePoint.transform.position, sleevePoint.transform.rotation);
        newBulletProjectile.GetComponent<Rigidbody>().AddForce(muzzlePoint.transform.forward * force, ForceMode.Impulse);
        newSleeve.GetComponent<Rigidbody>().AddForce(sleevePoint.transform.right * 0.1f, ForceMode.Impulse);
        if (stopTimeOnShot)
        {
            Time.timeScale = 0f;
        }

        if (magazineInserted)
        {
            if (magazine.ammo == 0)
            {
                isMovingToEmptyState = true;
                movingPartSliding.playSound();
            }
        }
        else
        {
            isMovingToEmptyState = true;
            movingPartSliding.playSound();
        }
    }
    public void makeReadyToShoot()
    {
        isReadyToShoot = true;
    }
}
