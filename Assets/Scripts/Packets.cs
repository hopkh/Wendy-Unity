using System;

[Serializable]
public class Packet
{
}

[Serializable]
public class PacketError : Packet
{
    public int result;
    public string message;

    [Serializable]
    public class Data
    {
        public string name;
        public int code;
    }

    public Data error;
}

[Serializable]
public class PacketToken : Packet
{
    public int result;
    public string token;
}


public class PacketDevice : Packet
{
}

[Serializable]
public class PacketUserRegister : Packet
{
    public int result;
    public string token;
}

[Serializable]
public class PacketUser : Packet
{
    [Serializable]
    public class Data
    {
        public int GameUserID;
        public string NickName;
        public string Locale;
        public int OffsetTime;
        public string OffsetTimeUpdateAt;
        public string createAt;
        public string loginAt;
    }

    public Data UserInfo;
}

[Serializable]
public class PacketCurrencyOwn : Packet
{
    public int result;
    [Serializable]
    public class Data
    {
        public int TotalQNTY;
        public int OwnCurrencyUID;
        public int CurrencyID;
        public int CurrentQNTY;
        public int NowMaxQNTY;
        public int AddMaxQNTY;
        public string UpdateTimeStamp;
        public int GameUserID;
    }

    public Data[] list;
}