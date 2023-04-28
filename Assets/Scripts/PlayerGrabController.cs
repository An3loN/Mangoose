using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerGrabController : MonoBehaviour
{
    public static PlayerGrabController Instance;

    [SerializeField] float throwForce = 400f;
    [NonSerialized] public Vector2 throwDirection = new Vector2(0f, 0f);
    [NonSerialized] public List<Grabbable> grabPretenders = new List<Grabbable>();
    private bool hasItemEquipped = false;
    Grabbable equippedItem;
    private Rigidbody2D selfRigidBody;

    private void Start()
    {
        Instance = this;
        selfRigidBody = GetComponent<Rigidbody2D>();
    }
    public bool CompareEquipped(GameObject other)
    {
        if(other == null || equippedItem == null) return false;
        return equippedItem.gameObject == other;
    }
    public void OnGrabButtonPressed(CallbackContext context)
    {
        if (!context.performed) return;
        if (equippedItem == null) hasItemEquipped = false;
        if (hasItemEquipped)
        {
            ThrowItem();
        }
        else if(grabPretenders.Count > 0)
        {
            GrabItem(grabPretenders[grabPretenders.Count-1]);
        }
    }
    void GrabItem(Grabbable item)
    {
        equippedItem = item;
        equippedItem.OnGrab();
        equippedItem.transform.position = Vector3.zero;
        equippedItem.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        equippedItem.transform.SetParent(PoseCharacterController.Instance.itemPivotTransform, false);
        grabPretenders.Remove(item);
        hasItemEquipped = true;
    }
    public void ThrowItem()
    {
        if(!hasItemEquipped) return;
        equippedItem.OnDrop();
        equippedItem.transform.SetParent(null, true);
        equippedItem.gameObject.SetActive(false);
        equippedItem.gameObject.SetActive(true);
        if (equippedItem.TryGetComponent(out Rigidbody2D droppedRigidbody))
        {
            if(throwDirection != Vector2.zero)
            {
                droppedRigidbody.velocity = Vector2.zero;
                droppedRigidbody.AddForce(throwDirection.normalized * throwForce);
            }
            else
            {
                droppedRigidbody.velocity = selfRigidBody.velocity;
            }
        }
        hasItemEquipped = false;
        
        equippedItem = null;
    }
    public void DropItem()
    {
        if (!hasItemEquipped) return;
        equippedItem.OnDrop();
        equippedItem.transform.SetParent(null, true);
        hasItemEquipped = false;
        equippedItem = null;
    }
    public void OnThrowDirectionInput(CallbackContext context)
    {
        throwDirection = context.ReadValue<Vector2>();
    }
}
