using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLandSound : MonoBehaviour
{
    public AudioSource audioSource;
    private bool canPlaySound;
    private void Start()
    {
        Invoke("canPlaySoundTrue", 0.2f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.tag.Equals(null) && canPlaySound)
        {
            audioSource.Play();
            canPlaySound = false;
        }
    }
    private void canPlaySoundTrue()
    {
        canPlaySound = true;
    }
}
