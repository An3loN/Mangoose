using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructor : MonoBehaviour
{
    [SerializeField] private float delay = 0;
    private void Start()
    {
        if(delay != 0)
        {
            StartCoroutine(DelayedDestruct());
        }
    }
    IEnumerator DelayedDestruct()
    {
        yield return new WaitForSeconds(delay);
        SelfDestruct();
    }
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
