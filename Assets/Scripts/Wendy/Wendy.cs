using System;
using System.Collections;
using UnityEngine;

public enum DeviceType
{
    AOS = 1,
    IOS = 2,
    EDITOR = 10,
}

public class Wendy : MonoBehaviour
{
    private static Wendy instance;
    public static Wendy Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<Wendy>();
            if (instance == null)
            {
                var newGameObject = new GameObject("Wendy");
                instance = newGameObject.AddComponent<Wendy>();
            }
            return instance;
        }
    }

    public string Url { get; set; }
    public string Authorization { get; set; }
    public bool IsConnect { get { return string.IsNullOrEmpty(Authorization) == false; } }
    public Action<string> OnErrorHandler { get; set; }

    public void Send<TPacket>(WendySender<TPacket> sender) where TPacket : Packet
    {
        StartCoroutine(WaitForRequest(sender));
    }

    public IEnumerator WaitForRequest<TPacket>(WendySender<TPacket> sender) where TPacket : Packet
    {
        var www = new WWW(Url + sender.Path, sender.PostData, sender.Headers);
        yield return www;

        if (string.IsNullOrEmpty(www.error) == false)
        {
            try
            {
                var error = JsonUtility.FromJson<PacketError>(www.text);
                if (error != null)
                {
                    Debug.Log(sender.Path + "...." + www.text);
                    sender.ExecuteOnError(error);
                }
            }
            catch (Exception)
            {
                Debug.LogError(sender.Path + ":" + www.error);
                sender.ExecuteOnFail();
            }
            yield break;
        }

        var packet = JsonUtility.FromJson<Packet>(www.text);
        if (packet == null)
        {
            sender.ExecuteOnFail();
            yield break;
        }

        Debug.Log(sender.Path + "...." + www.text);
        sender.ExecuteOnSuccess(JsonUtility.FromJson<TPacket>(www.text));
    }
}
