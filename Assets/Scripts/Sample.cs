using UnityEngine;

public enum ConnectType
{
    DEVELOPMENT,
    AZURE,
}

public class Sample : MonoBehaviour
{
    public ConnectType m_connectType = ConnectType.DEVELOPMENT;
    public string m_devUrl;
    public string m_azureUrl;
    public string m_uuid;
    public string m_nickName;
    private bool m_connect;

    public string Url { get { return m_connectType == ConnectType.DEVELOPMENT ? m_devUrl : m_azureUrl; } }
    public string Uuid { get { return string.IsNullOrEmpty(m_uuid) ? SystemInfo.deviceUniqueIdentifier : m_uuid; } }

    private void Start()
    {
        Wendy.Instance.OnErrorHandler = message =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog("Error", message, "OK");
#endif
        };
        Wendy.Instance.Url = Url;
        new WendySender<PacketToken>("/device/token/" + Uuid)
            .OnSuccess(tokenResult =>
            {
                Wendy.Instance.Authorization = tokenResult.token;
                m_connect = true;
            }).OnError((error) => m_connect = true).Send();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0.0f, 0.0f, 100.0f, 20.0f), m_connect ? "Connect" : "Disonnect");
        if (m_connect == false) return;

        if (Wendy.Instance.IsConnect == false)
        {
            m_nickName = GUI.TextField(new Rect(0.0f, 50.0f, 200.0f, 20.0f), m_nickName);
            if (GUI.Button(new Rect(200.0f, 50.0f, 200.0f, 20.0f), "계정만들기(닉네임)"))
            {
                new WendySender<PacketDevice>("/device")
                    .AddField("UUID", Uuid)
                    .AddField("DeviceType", (int)DeviceType.EDITOR)
                    .OnSuccess(deviceResult =>
                    {
                        new WendySender<PacketUserRegister>("/user")
                        .AddField("NickName", m_nickName)
                        .AddField("Locale", "ko-kr")
                        .AddField("UUID", Uuid)
                        .AddField("OffsetTime", "9")
                        .OnSuccess((userRegisterResult) =>
                        {
                            Wendy.Instance.Authorization = userRegisterResult.token;
                        }).Send();
                    }).Send();
            }
        }
        else
        {
            if (GUI.Button(new Rect(0.0f, 20.0f, 100.0f, 20.0f), "user"))
            {
                new WendySender<PacketUser>("/user")
                    .AddAuthorization()
                    .OnSuccess(userResult =>
                    {
                        m_nickName = userResult.UserInfo.NickName;
                    }).Send();
            }
            if (GUI.Button(new Rect(0.0f, 40.0f, 100.0f, 20.0f), "currency/own"))
            {
                new WendySender<PacketCurrencyOwn>("/currency/own")
                    .AddAuthorization()
                    .OnSuccess(currencyOwnResult => { }).Send();
            }
        }
    }
}