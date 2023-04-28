using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    [SerializeField] Activatable activatable;
    [SerializeField] List<Component> enableOnEquip;
    [SerializeField] List<Component> disableOnEquip;
    private Vector3 initialScale;
    private Quaternion initialRotation;

    private void Awake()
    {
        activatable.onGetAbleActivate = OnGetAbleGrab;
        activatable.onGetUnableActivate = OnGetUnableGrab;
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
    }
    void SetComponentsState(List<Component> components, bool state)
    {
        foreach (var component in components)
        {
            if (component is MonoBehaviour monoBehaviour) monoBehaviour.enabled = state;
            else if (component is Collider2D collider) collider.enabled = state;
            else if (component is Rigidbody2D rigidbody)
            {
                rigidbody.isKinematic = !state;
                rigidbody.simulated = state;
            }
        }
    }
    public void OnGrab()
    {
        SetComponentsState(enableOnEquip, true);
        SetComponentsState(disableOnEquip, false);
    }
    public void OnDrop()
    {
        SetComponentsState(enableOnEquip, false);
        SetComponentsState(disableOnEquip, true);
        transform.localScale = initialScale;
        transform.rotation = initialRotation;
    }
    void OnGetAbleGrab()
    {
        PlayerGrabController.Instance.grabPretenders.Add(this);
    }
    void OnGetUnableGrab()
    {
        PlayerGrabController.Instance.grabPretenders.Remove(this);
    }
}
