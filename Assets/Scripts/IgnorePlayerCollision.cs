using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class IgnorePlayerCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider2D playerCollider = PoseCharacterController.Instance.gameObject.GetComponent<Collider2D>();
        Collider2D selfCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
