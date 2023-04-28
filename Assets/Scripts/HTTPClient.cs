using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;

public static class HTTPClient
{
    public async static void SendMessage(string token, string userId, string msg)
    {
        HttpClient client = new HttpClient();
        var responseString = await client.GetStringAsync($"https://api.telegram.org/bot{token}/sendMessage?chat_id={userId}&text={msg}");
    }
}
