using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class ServerDataModel
    {
        public string MessageId;
        public string Version;
        public long StartDate;
        public long EndDate;
        public bool ShowSecBtn;
        public bool ShowOnEveryStart;
        public List<ServerDataModelLanguage> Items;


        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        public ServerDataModelLanguage GetWithLocal(string code)
        {
            ServerDataModelLanguage defaultItem = Items[0];
            for (int i = 0; i < Items.Count; i++)
            {
                ServerDataModelLanguage s = Items[i];
                if (s.Language == code)
                {
                    return s;
                }
            }
            return defaultItem;
        }

        public static ServerDataModel Generate()
        {
            List<ServerDataModelLanguage> items = new List<ServerDataModelLanguage>();
            items.Add(new ServerDataModelLanguage()
            {
                Language = "en",
                Title = "tit1",
                Content = "cont1",
            });
            items.Add(new ServerDataModelLanguage()
            {
                Language = "pl",
                Title = "tytuł",
                Content = "zawartość",
            });

            ServerDataModel s = new ServerDataModel()
            {
                MessageId = "1483232400",
                Version = "0.1",
                StartDate = 1483228800,
                EndDate = 1485302400,
                ShowSecBtn = false,
                ShowOnEveryStart = false,
                Items = items
            };
            return s;
        }
    }

    [Serializable]
    public class ServerDataModelLanguage
    {
        public string Language;
        public string Title;
        public string Content;
        public string SecBtnText;
    }
    [Serializable]
    public class ServerDataHolder
    {
        private const string ListFieldName = "Messages";
        public List<ServerDataModel> Messages;

        public static string WrapToClass(string json)
        {
            return string.Format("{{ \"{0}\": {1}}}",ListFieldName, json);
        }
    }

}
