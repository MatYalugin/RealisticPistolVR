using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timeToDestroySelf;
    private void Start()
    {
        Invoke("Destroy", timeToDestroySelf);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
