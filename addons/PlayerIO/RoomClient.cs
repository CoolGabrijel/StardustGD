using System.Collections.Generic;

namespace PIOMP
{
    public class RoomClient
    {
        public RoomClient(int _id, string _username)
        {
            Id = _id;
            Username = _username;
        }

        public int Id;
        public string Username;

        Dictionary<string, string> data = new();

        public void AddData(string _dataName, string _data)
        {
            data.Add(_dataName, _data);
        }

        public void RemoveData(string _dataName)
        {
            data.Remove(_dataName);
        }

        public string GetData(string _dataName)
        {
            if (data.TryGetValue(_dataName, out string result))
            {
                return result;
            }
            else throw new System.Exception($"Cannot find {_dataName} data for {Username} {Id}");
        }
    }
}