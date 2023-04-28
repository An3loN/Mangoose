using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChooseMenuController : MonoBehaviour
{
    [SerializeField] LevelChooseButtonController buttonPrefabController;
    [SerializeField] Transform buttonsParent;
    private Dictionary<string, float> times;

    // Start is called before the first frame update
    void Start()
    {
        times = SaveController.GetSave().levelTimesData;
        InitializeButtons();
    }

    void InitializeButtons()
    {
        foreach (var level in times.Keys)
        {
            var time = times[level];
            if (time < 0) continue;
            var button = Instantiate(buttonPrefabController.gameObject, buttonsParent).GetComponent<LevelChooseButtonController>();
            button.Initialize(level, time);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
