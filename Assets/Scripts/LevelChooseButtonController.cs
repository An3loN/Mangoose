using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooseButtonController : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] Text levelNumberText;
    [SerializeField] Image image;
    [SerializeField] Image rankImage;
    private string levelName;
    
    public void Initialize(string levelName, float time)
    {
        this.levelName = levelName;
        timeText.text = time.ToString("00.00");
        string levelId = levelName.Split("Level")[1];
        levelNumberText.text = levelId;
        image.sprite = Resources.Load<Sprite>($"LevelPreviews/{levelName}"); //

        LevelTimeDataObject timeData = Resources.Load<LevelTimeDataObject>($"LevelTimeObjects/LevelTime{levelId}");
        if (timeData.GetRankId(time) == 5)
        {
            ImageAnimation imageAnimation = rankImage.gameObject.AddComponent<ImageAnimation>();
            imageAnimation.Initialize();
            imageAnimation.sprites = Resources.Load<SpriteContainer>("RankImages/Rank5SpriteContainer").sprites;
        }
        else
        {
            rankImage.sprite = timeData.GetRankImage(time);
        }
        
    }

    public void OnClick()
    {
        MainMenuController.Instance.StartLevel(levelName);
    }
}
