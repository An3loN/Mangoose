using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownContentJustifier : MonoBehaviour
{
    [SerializeField] Dropdown dropdown;
    [SerializeField] ScrollRect scrollRect;
    
    void Start()
    {
        float elementHeight = 0;
        RectTransform rectTransforms = GetComponentInChildren<RectTransform>();
        foreach (RectTransform rectTransform in rectTransforms)
        {
            if (rectTransform.TryGetComponent(out Toggle toggle))
            {
                elementHeight = rectTransform.sizeDelta.y;
                break;
            }
        }
        float contentHeight = dropdown.options.Count * elementHeight;
        GetComponent<RectTransform>().sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, contentHeight);
        RectTransform scrollRectTransform = scrollRect.gameObject.GetComponent<RectTransform>();
        scrollRectTransform.sizeDelta = new Vector2(scrollRectTransform.sizeDelta.x, contentHeight);
    }
}
