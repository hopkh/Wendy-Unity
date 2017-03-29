using System;
using System.Collections.Generic;
using UnityEngine;

public class WendySender<TPacket> where TPacket : Packet
{
    public string Path { get; private set; }
    private Action<TPacket> m_onSuccess;
    private Action<PacketError> m_onError;
    private Action m_onFail;
    private readonly WWWForm m_wwwForm = new WWWForm();
    private readonly Dictionary<string, string> m_headers = new Dictionary<string, string>();

    public byte[] PostData { get { return m_wwwForm.data.Length > 0 ? m_wwwForm.data : null; } }
    public Dictionary<string, string> Headers { get { return m_headers.Count > 0 ? m_headers : null; } }

    public WendySender(string path)
    {
        Path = path;
    }

    public WendySender<TPacket> AddAuthorization()
    {
        m_headers.Add("Authorization", Wendy.Instance.Authorization);
        return this;
    }

    public WendySender<TPacket> AddField(string fieldName, string value)
    {
        m_wwwForm.AddField(fieldName, value);
        return this;
    }

    public WendySender<TPacket> AddField(string fieldName, int value)
    {
        m_wwwForm.AddField(fieldName, value);
        return this;
    }

    public WendySender<TPacket> Send()
    {
        Wendy.Instance.Send(this);
        return this;
    }

    public WendySender<TPacket> OnSuccess(Action<TPacket> onSuccess)
    {
        m_onSuccess = onSuccess;
        return this;
    }

    public WendySender<TPacket> OnError(Action<PacketError> onError)
    {
         m_onError = onError;
        return this;
    }

    public WendySender<TPacket> OnFail(Action onFail)
    {
        m_onFail = onFail;
        return this;
    }

    public void ExecuteOnSuccess(TPacket packet)
    {
        if (m_onSuccess == null) return;
        m_onSuccess(packet);
    }

    public void ExecuteOnError(PacketError error)
    {
        if (m_onError != null) m_onError(error);
        if (Wendy.Instance.OnErrorHandler != null) Wendy.Instance.OnErrorHandler(error.message);
    }

    public void ExecuteOnFail()
    {
        if (m_onFail == null) return;
        m_onFail();
    }
}