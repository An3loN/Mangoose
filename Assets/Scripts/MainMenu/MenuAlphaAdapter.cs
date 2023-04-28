using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAlphaAdapter : MonoBehaviour
{
    private List<Image> imageChildren = new List<Image>();
    private List<Text> textChildren = new List<Text>();
    public void Init()
    {
        imageChildren.AddRange(gameObject.GetComponentsInChildren<Image>());
        textChildren.AddRange(gameObject.GetComponentsInChildren<Text>());
    }
    public void SetAlpha(float alpha)
    {
        foreach (var image in imageChildren)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
        foreach (var text in textChildren)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
    }
}
