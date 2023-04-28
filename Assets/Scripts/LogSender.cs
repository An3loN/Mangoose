using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSender : MonoBehaviour
{
    public static LogSender Instance;
    [SerializeField] string token;
    [SerializeField] string userId;
    [SerializeField] bool sendFullDataOnStart = false;
    struct LogData
    {
        public string userName;
        public string overAllTime;
        public Dictionary<string, float> times;
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        if (sendFullDataOnStart)
        {
            SendTelegramMessage(GetFullData());
        }
    }

    string GetFullData()
    {
        LogData data = new LogData();
        data.userName = Environment.UserName;
        data.overAllTime = (Time.time - TimerController.newGameStartTime).ToString("00.00");
        data.times = SaveController.GetSave().levelTimesData;
        string msg = JsonConvert.SerializeObject(data, Formatting.Indented);
        //string msg = "text";
        return msg;
    }

    void SendTelegramMessage(string msg)
    {
        //HTTPClient.SendMessage(token, userId, msg);
    }

    public void SendChallangeCompletionData()
    {
        string msg = "WE'VE GOT A WINNER!!!\n\n";
        msg += GetFullData();
        SendTelegramMessage(msg);
    }
}
