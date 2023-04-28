using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class SmoothUIPop : MonoBehaviour
{
    [SerializeField] private float yDelta = -10f;
    [SerializeField] private float transitionDuration = 0.6f;
    [SerializeField] private bool preventPreclick = true;
    private Vector3 initialPosition = new Vector3(0f, 0f, 0f);
    bool firstStart = true;
    //private Image image;
    private void Start()
    {
        if(firstStart)
        {
            initialPosition = transform.localPosition;
            firstStart = false;
        }
    }
    public void Show()
    {
        List<Text> childrenText = new List<Text>();
        List<Image> childrenImages = new List<Image>();
        List<Button> childrenButtons = new List<Button>();
        childrenText.AddRange(GetComponentsInChildren<Text>());
        childrenImages.AddRange(GetComponentsInChildren<Image>());
        childrenButtons.AddRange(GetComponentsInChildren<Button>());
        gameObject.SetActive(true);
        foreach (var childImage in childrenImages)
        {
            Color childInitialColor = childImage.color;
            childImage.color = new Color(childInitialColor.r, childInitialColor.g, childInitialColor.b, 0f);
            childImage.DOColor(childInitialColor, transitionDuration).SetUpdate(true);
        }
        foreach (var childText in childrenText)
        {
            Color childInitialColor = childText.color;
            childText.color = new Color(childInitialColor.r, childInitialColor.g, childInitialColor.b, 0f);
            childText.DOColor(childInitialColor, transitionDuration).SetUpdate(true);
        }
        foreach (var buttonChild in childrenButtons)
        {
            buttonChild.interactable = !preventPreclick;

        }
        if (childrenButtons.Count != 0) childrenButtons[0].Select();
        
        transform.localPosition = initialPosition + Vector3.up * yDelta;
        transform.DOLocalMoveY(initialPosition.y, transitionDuration).SetUpdate(true).onComplete = () =>
        {
            if (!preventPreclick) return;
            foreach (var buttonChild in childrenButtons)
            {
                buttonChild.interactable = true;
            }
        };
    }
    public void Hide()
    {
        List<Text> childrenText = new List<Text>();
        List<Image> childrenImages = new List<Image>();
        List<Button> childrenButtons = new List<Button>();
        List<Color> imageColors = new List<Color>();
        List<Color> textColors = new List<Color>();
        childrenText.AddRange(GetComponentsInChildren<Text>());
        childrenImages.AddRange(GetComponentsInChildren<Image>());
        childrenButtons.AddRange(GetComponentsInChildren<Button>());


        foreach (var childImage in childrenImages)
        {
            Color childInitialColor = childImage.color;
            childImage.DOColor(new Color(childInitialColor.r, childInitialColor.g, childInitialColor.b, 0f), transitionDuration).SetUpdate(true);
            imageColors.Add(childInitialColor);
        }
        foreach (var childText in childrenText)
        {
            Color childInitialColor = childText.color;
            childText.DOColor(new Color(childInitialColor.r, childInitialColor.g, childInitialColor.b, 0f), transitionDuration).SetUpdate(true);
            textColors.Add(childInitialColor);
        }
        foreach(var buttonChild in childrenButtons)
        {
            buttonChild.interactable = false;
        }
        if (TryGetComponent(out Image image))
        {
            Color initialColor = image.color;
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
            image.DOColor(new Color(initialColor.r, initialColor.g, initialColor.b, 0f), transitionDuration).SetUpdate(true);
        }
        transform.DOMoveY(initialPosition.y + yDelta, transitionDuration).SetUpdate(true).onComplete = () =>
        {
            for (int index = 0; index < childrenImages.Count; index++)
            {
                childrenImages[index].color = imageColors[index];
            }
            for (int index = 0; index < childrenText.Count; index++)
            {
                childrenText[index].color = textColors[index];
            }
            gameObject.SetActive(false);
        };
    }
}
