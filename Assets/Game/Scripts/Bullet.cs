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
            if(interactable.attachedToHand != null && caliber == magazine.caliber && magazine.ReturnInteractable().attachedToHand != null && magazine.ammo != magazine.bullets.Count)
            {
                other.transform.gameObject.GetComponentInParent<Magazine>().AddBullet();
                Destroy(gameObject);
            }
        }
        if (other.transform.gameObject.tag.Equals("WeaponChamberPoint"))
        {
            var weapon = other.GetComponentInParent<Weapon>();
            var slidingScript = weapon.movingPartSliding;
            var endPosZ = slidingScript.endPosZ + 0.01f;
            if (interactable.attachedToHand != null && weapon.returnInteractable().attachedToHand != null)
            {
                slidingScript.isLoadingBulletByHand = true;
                slidingScript.gameObject.transform.localPosition = new Vector3(slidingScript.startPos.x, slidingScript.startPos.y, Mathf.MoveTowards(slidingScript.gameObject.transform.localPosition.z, endPosZ, Time.timeScale));
                if (slidingScript.gameObject.transform.localPosition != new Vector3(slidingScript.startPos.x, slidingScript.startPos.y, endPosZ))
                {
                    slidingScript.gameObject.transform.localPosition = new Vector3(slidingScript.startPos.x, slidingScript.startPos.y, endPosZ);                 
                }
                if(!weapon.bulletInChamberGO.activeSelf)
                    StartCoroutine(PutMeInChamber(weapon));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.tag.Equals("WeaponChamberPoint"))
        {
            other.GetComponentInParent<Weapon>().movingPartSliding.isLoadingBulletByHand = false;
        }
    }
    private IEnumerator PutMeInChamber(Weapon weapon)
    {
        yield return new WaitForSeconds(0.8f);
        if(weapon.movingPartSliding.isLoadingBulletByHand)
        {
            weapon.bulletInChamberGO.SetActive(true);
            weapon.movingPartSliding.isLoadingBulletByHand = false;
            Destroy(gameObject);
        }
    }
}
