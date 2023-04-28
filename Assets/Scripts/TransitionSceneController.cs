using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionSceneController : MonoBehaviour
{
    [SerializeField] private float characterMiddleX = 0f;
    [SerializeField] private float characterEndX = 2f;
    [SerializeField] private float inDuration;
    [SerializeField] private float outDuration;
    [SerializeField] private SmoothUIPop menu;
    [SerializeField] private Text timeText;
    [SerializeField] private Text bestTimeText;
    [SerializeField] private Text levelNameText;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private SpriteRenderer blackSprite;
    [SerializeField] Image rankImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip monkeySound;
    [SerializeField] private GameObject defaultTheme;
    [SerializeField] private GameObject castleTheme;
    [SerializeField] private int firstCastleLevel = 10;
    private int levelId;

    private void Start()
    {
        characterTransform.DOMoveX(characterMiddleX, inDuration);
        blackSprite.color = new Color(0f, 0f, 0f, 1f);
        blackSprite.DOColor(new Color(0f, 0f, 0f, 0f), inDuration).onComplete = ShowMenu;

        bestTimeText.text = 
            SaveController.GetSave().levelTimesData[$"Level{GameController.levelCompletedId}"].ToString("0.00");
        timeText.text = GameController.levelCompletionTime.ToString("0.00");
        levelNameText.text = GameController.levelCompletedId.ToString();

        levelId = GameController.levelCompletedId;

        LevelTimeDataObject timeData = Resources.Load<LevelTimeDataObject>($"LevelTimeObjects/LevelTime{levelId}");

        if (timeData.GetRankId(GameController.levelCompletionTime) == 5)
        {
            ImageAnimation imageAnimation = rankImage.gameObject.AddComponent<ImageAnimation>();
            imageAnimation.Initialize();
            imageAnimation.sprites = Resources.Load<SpriteContainer>("RankImages/Rank5SpriteContainer").sprites;
            audioSource.clip = monkeySound;
            audioSource.Play();
        }
        else
        {
            rankImage.sprite = timeData.GetRankImage(GameController.levelCompletionTime);
        }

        if(GameController.levelCompletedId >= firstCastleLevel)
        {
            SetCastleTheme();
        }
    }
    void ShowMenu()
    {
        menu.Show();
    }
    public void OnContinueButton()
    {
        StartLevel(levelId + 1);
    }
    public void OnRestartButton()
    {
        
        StartLevel(levelId);

    }
    void StartLevel(int levelId)
    {
        menu.Hide();
        characterTransform.DOMoveX(characterEndX, inDuration);
        blackSprite.DOColor(new Color(0f, 0f, 0f, 1f), inDuration).onComplete =
            () => SceneManager.LoadScene(SaveController.GetScene(levelId));
    }
    void SetCastleTheme()
    {
        defaultTheme.SetActive(false);
        castleTheme.SetActive(true);
    }
}
