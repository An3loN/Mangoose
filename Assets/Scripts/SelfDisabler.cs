using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisabler : MonoBehaviour
{
    public void SelfDisable()
    {
        gameObject.SetActive(false);
    }
}
