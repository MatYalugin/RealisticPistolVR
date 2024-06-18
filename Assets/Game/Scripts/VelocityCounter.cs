using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCounter : MonoBehaviour
{
    public Vector3 pos;
    public float distanceToCheckTheHit = 1f;
    private bool isHitting;
    public float Damage;
    public AudioClip hitSound;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SavePos());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("x " + Mathf.Abs((this.transform.localPosition.x - pos.x)));
        //Debug.Log("y " + Mathf.Abs((this.transform.localPosition.y - pos.y)));
        //Debug.Log("z " + Mathf.Abs((this.transform.localPosition.z - pos.z)));
        if (Mathf.Abs((this.transform.position.x - pos.x)) > distanceToCheckTheHit || Mathf.Abs((this.transform.position.y - pos.y)) > distanceToCheckTheHit || Mathf.Abs((this.transform.position.z - pos.z)) > distanceToCheckTheHit)
        {
            isHitting = true;
        }
        else
        {
            isHitting = false;
        }
    }
    public IEnumerator SavePos()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            pos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.tag != null && isHitting)
        {
            if (collision.transform.gameObject.tag.Equals("Enemy"))
            {
                collision.transform.gameObject.GetComponentInParent<Enemy>().Hurt(Damage);
            }
            if (collision.transform.gameObject.tag.Equals("Target"))
            {
                collision.transform.gameObject.GetComponentInParent<Target>().TargetDown();
            }
            AudioSource.PlayClipAtPoint(hitSound, this.transform.position);
            Destroy(gameObject);
        }
    }
}
