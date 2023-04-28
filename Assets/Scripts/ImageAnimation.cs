using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    public float sampleTime = 0.15f;
    private Image image;
    public List<Sprite> sprites;
    // Start is called before the first frame update

    private void Start()
    {
        LoopSprites();
    }
    public void Initialize()
    {
        image = GetComponent<Image>();

    }
    public void LoopSprites()
    {
        StartCoroutine(SpriteChangeCoroutine(sprites, true));
    }
    IEnumerator SpriteChangeCoroutine(List<Sprite> sprites, bool loop)
    {
        do {
            foreach (Sprite sprite in sprites)
            {
                image.sprite = sprite;
                yield return new WaitForSeconds(sampleTime);
            }
        } while (loop);
    }
}
