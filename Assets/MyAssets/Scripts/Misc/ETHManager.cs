using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using UnityEngine.InputSystem;

public class ETHManager : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            Debug.Log(getETHPrice().ToString());
        }
    }


    public int getETHPrice()
    {
        HttpClient client = new HttpClient();
        var responseTask = client.GetAsync("https://api.coinbase.com/v2/exchange-rates?currency=ETH");
        responseTask.Wait();
        if (responseTask.IsCompleted)
        {
            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var message = result.Content.ReadAsStringAsync();
                message.Wait();
                string val = getBetween(message.Result, "\"USD\":\"", "\",\"BTC");
                int price = (int)(float.Parse(val) + 0.5);
                return price;
            }
        }

        return 0;
    }

    public string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }
}
