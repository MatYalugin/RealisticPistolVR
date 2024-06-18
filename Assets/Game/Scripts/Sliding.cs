using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Sliding : MonoBehaviour
{
    private Interactable interactable;
    public Quaternion startRot;
    public Vector3 startPos;
    public float weaponJammingPosZ;
    public float endPosZ;
    public float currentPosZ;
    public Rigidbody body;
    public GameObject parentGO;
    public Collider _collider;
    public Weapon weapon;
    public AudioSource audioSource;
    public bool isInEmptyState;
    public bool isInJammingState;
    public bool isLoadingBulletByHand;
    private bool soundPlayed;


    void Start()
    {
        interactable = GetComponent<Interactable>();
        startPos = gameObject.transform.localPosition;
        startRot = gameObject.transform.localRotation;
    }
    public Interactable returnInteractable()
    {
        return interactable;
    }
    private void Update()
    {
        SetRightRotationAndParent();
        CheckIfInteractableHasHand();
        SetRightPosition();
        MoveOnStartPositionWhenInteractableHasNotHand();
        DecideWhenPlaySound();
    }
    private void SetRightRotationAndParent()
    {
        gameObject.transform.localRotation = startRot;
        gameObject.transform.SetParent(parentGO.transform);
    }
    private void CheckIfInteractableHasHand()
    {
        if (weapon.returnInteractable().attachedToHand != null)
        {
            gameObject.transform.localRotation = startRot;
            _collider.enabled = true;
        }
        else
        {
            _collider.enabled = false;
        }
    }
    private void SetRightPosition()
    {
        //localPosition!!
        currentPosZ = gameObject.transform.localPosition.z;
        if (gameObject.transform.localPosition.z > startPos.z)
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, startPos.z);
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, gameObject.transform.localPosition.z);
        }
        if (gameObject.transform.localPosition.z < endPosZ)
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, endPosZ);
            soundPlayed = false;
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, gameObject.transform.localPosition.z);
        }
    }
    private void MoveOnStartPositionWhenInteractableHasNotHand()
    {
        if (interactable.attachedToHand == null && gameObject.transform.localPosition.z < startPos.z && !isInEmptyState && !isInJammingState && !isLoadingBulletByHand)
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, Mathf.MoveTowards(gameObject.transform.localPosition.z, startPos.z, Time.deltaTime));
        }
    }
    private void DecideWhenPlaySound()
    {
        if (gameObject.transform.localPosition.z == endPosZ && interactable.attachedToHand != null && !soundPlayed)
        {
            playSound();
        }
    }
    public void playSound()
    {
        audioSource.Play();
        soundPlayed = true;
    }
}
