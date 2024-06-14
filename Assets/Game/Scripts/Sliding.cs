using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Sliding : MonoBehaviour
{
    private Interactable interactable;
    public Quaternion startRot;
    public Vector3 startPos;
    public float endPosZ;
    public float currentPosZ;
    public Rigidbody body;
    public GameObject parentGO;
    public bool isInEmptyState;
    public Collider _collider;
    public Weapon weapon;
    public bool activateCollider;
    public AudioSource audioSource;
    private bool soundPlayed;


    void Start()
    {
        interactable = GetComponent<Interactable>();
        startPos = gameObject.transform.localPosition;
        startRot = gameObject.transform.localRotation;
    }

    void Update()
    {
        gameObject.transform.localRotation = startRot;
        gameObject.transform.parent = weapon.gameObject.transform;
        if (weapon.interactable.attachedToHand != null)
        {
            gameObject.transform.localRotation = startRot;
        }
        if (weapon.interactable.attachedToHand != null || activateCollider)
        {
            _collider.enabled = true;
        }
        else
        {
            _collider.enabled = false;
        }
        gameObject.transform.SetParent(parentGO.transform);
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

        }
        else
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, gameObject.transform.localPosition.z);
        }
        if(interactable.attachedToHand == null && gameObject.transform.localPosition.z < startPos.z && !isInEmptyState)
        {
            gameObject.transform.localPosition = new Vector3(startPos.x, startPos.y, Mathf.MoveTowards(gameObject.transform.localPosition.z, startPos.z, Time.deltaTime));
        }

        if(gameObject.transform.localPosition.z == endPosZ && interactable.attachedToHand != null && !soundPlayed)
        {
            playSound();
        }
        else
        {
            soundPlayed = false;
        }
    }
    public void playSound()
    {
        audioSource.Play();
        soundPlayed = true;
    }
}
