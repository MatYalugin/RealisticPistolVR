using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Weapon : MonoBehaviour
{
    public SteamVR_Action_Boolean buttonAAction;
    public SteamVR_Action_Boolean buttonBAction;
    public SteamVR_Action_Boolean buttonXAction;
    public SteamVR_Action_Boolean buttonYAction;
    private Interactable interactable;
    private SteamVR_Action_Boolean fireAction;
    public GameObject bulletProjectile;
    public GameObject sleeve;
    public GameObject bullet;
    public GameObject muzzlePoint;
    public GameObject sleevePoint;
    private GameObject magazineGO;
    public GameObject bulletInChamberGO;
    public GameObject secondGrabPoint;
    public Sliding movingPartSliding;
    private Magazine magazine;
    public float delay = 0.4f;
    public float force;
    public string magazineType;
    public ParticleSystem shotEffect;
    public Animator triggerAnimator;
    public Animator hammerAnimator;
    public bool hasHammer;
    public bool stopTimeOnShot;
    private bool movingPartWasInEndPosZ;
    private bool isReadyToShoot = true;
    private bool magazineInserted;
    private bool isMovingToEmptyState;
    public Vector3 magazineInWeaponPos;
    public Quaternion magazineInWeaponRot;
    public Transform magazinePoint;
    public Transform magazinePointDrop;
    public AudioSource magActionsAudio;
    public AudioSource shotSound;
    public AudioClip magInsertAudio;
    public AudioClip magDropAudio;
    void Start()
    {
        interactable = GetComponent<Interactable>();
        fireAction = SteamVR_Actions.default_InteractUI;
    }
    public Interactable returnInteractable()
    {
        return interactable;
    }
    public GameObject GetMagazineGO()
    {
        return magazineGO;
    }

    private void Update()
    {
        CheckIfMagazineGameObjectIsNotNull();
        CheckIfIsMovingToEmptyState();
        CheckIfInteractableHasHand();
        CheckMovingPartMovement();
        CheckIfHasHammer();
    }
    private void CheckIfMagazineGameObjectIsNotNull()
    {
        if (magazineGO != null)
        {
            magazineGO.transform.parent = gameObject.transform;
            magazineGO.transform.localPosition = magazineInWeaponPos;
            magazineGO.transform.localRotation = magazineInWeaponRot;
            magazineGO.GetComponent<Collider>().enabled = false;

            magazineInserted = true;
            magazine = magazineGO.GetComponent<Magazine>();
            magazine.body.isKinematic = true;
        }
        else
        {
            magazineInserted = false;
            magazine = null;
        }
    }
    private void CheckIfIsMovingToEmptyState()
    {
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
    }
    private void CheckExitFromEmptyState(SteamVR_Input_Sources hand)
    {
        if (movingPartSliding.isInEmptyState && (buttonBAction[hand].stateDown || buttonYAction[hand].stateDown))
        {
            movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, Mathf.MoveTowards(gameObject.transform.localPosition.z, movingPartSliding.startPos.z, Time.deltaTime));
            movingPartSliding.playSound();
            if (movingPartSliding.gameObject.transform.localPosition != new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z))
            {
                movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.startPos.z);
            }
        }
    }
    private void CheckIfInteractableHasHand()
    {
        if (interactable.attachedToHand != null)
        {
            secondGrabPoint.SetActive(true);
            SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
            if (fireAction[hand].stateDown && isReadyToShoot)
            {
                triggerAnimator.Play("TriggerMove");
                if (bulletInChamberGO.activeSelf)
                {
                    Shot();
                }
                else
                {
                    if (hasHammer)
                    {
                        hammerAnimator.Play("EmptyShotHammerMove");
                    }
                }
            }
            if (magazineInserted && (buttonAAction[hand].stateDown  || buttonXAction[hand].stateDown))
            {
                DropMagazine();
            }
            CheckExitFromEmptyState(hand);
        }
        else
        {
            Hand hand = secondGrabPoint.GetComponent<Interactable>().attachedToHand;
            if (secondGrabPoint.GetComponent<Interactable>().attachedToHand != null)
            {
                hand.DetachObject(secondGrabPoint);
            }
            secondGrabPoint.SetActive(false);
        }
    }
    private void CheckMovingPartMovement()
    {
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
    }
    private void CheckIfHasHammer()
    {
        if (hasHammer)
        {
            if (bulletInChamberGO.activeSelf && !hammerAnimator.GetCurrentAnimatorStateInfo(0).IsName("EmptyShotHammerMove"))
            {
                hammerAnimator.Play("HammerReadyToHit");
            }
            else
            {
                if (!hammerAnimator.GetCurrentAnimatorStateInfo(0).IsName("EmptyShotHammerMove") && !hammerAnimator.GetCurrentAnimatorStateInfo(0).IsName("None"))
                {
                    hammerAnimator.Play("HammerNotReadyToHit");
                }
            }
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
    public void Shot()
    {
        movingPartSliding.gameObject.transform.localPosition = new Vector3(movingPartSliding.startPos.x, movingPartSliding.startPos.y, movingPartSliding.endPosZ);
        bulletInChamberGO.SetActive(false);
        shotSound.Play();
        shotEffect.Play();
        isReadyToShoot = false;
        Invoke("makeReadyToShoot", delay);
        var newBulletRotation = muzzlePoint.transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
        var newBulletProjectile = Instantiate(bulletProjectile, muzzlePoint.transform.position, newBulletRotation);
        var newSleeve = Instantiate(sleeve, sleevePoint.transform.position, sleevePoint.transform.rotation);
        newBulletProjectile.GetComponent<Rigidbody>().AddForce(muzzlePoint.transform.forward * force, ForceMode.Impulse);
        newSleeve.GetComponent<Rigidbody>().AddForce(sleevePoint.transform.right * 0.1f, ForceMode.Impulse);
        if(stopTimeOnShot)
            Time.timeScale = 0f;

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
