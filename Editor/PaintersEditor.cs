using System.Collections.Generic;
using Assets.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor
{
    public class PaintersEditor
    {
        [MenuItem("PaintersEditor/Get All Locs")]
        public static void GetAllLocs()
        {
            Dictionary<string, string> Loc = new Dictionary<string, string>();
            Dictionary<string, string> Loc2 = new Dictionary<string, string>();

            Text[] bt = (Text[]) Resources.FindObjectsOfTypeAll(typeof (Text));
            //            Assets.Worldblade.GameEvents[] gt = (GameEvents[]) Resources.FindObjectsOfTypeAll(typeof (GameEvents));
            //            GameEvents gc = gt[0];
            //            //        GameObject[] bt = GameObject.FindGameObjectsWithTag("Button");
            //            //       gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            //            EventDelegate del = new EventDelegate(gc, "PanelOpened");
            //            EventDelegate del2 = new EventDelegate(gc, "PanelOpened");
            //            Debug.Log("ge is null: " + (gc == null) + " btn: " + (bt == null) + " del: " + (del.parameters == null));
            //
            //            UIPlayAnimation[] a;
            string s = "";
            for (int i = 0; i < bt.Length; i++)
            {
                Text t = bt[i];
                Localization l = t.gameObject.GetComponent<Localization>();
                if (l != null)
                {
                    if (!Loc2.ContainsKey(t.text))
                    {
                        Loc2.Add(t.text, "");
                    }
                }
                else
                {
                    
                if (!Loc.ContainsKey(t.text))
                {
                Loc.Add(t.text, "");
                }
                }
            }
            foreach (KeyValuePair<string, string> kv in Loc)
            {
                s += "{ \"" + kv.Key + "\", \"\"},\n";
            }
            Debug.Log(s);
            foreach (KeyValuePair<string, string> kv in Loc2)
            {
                s += "{ \"" + kv.Key + "\", \"\"},\n";
            }
            Debug.Log(s);
        }
    }
}