using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class MatchData
    {
        public int RoundsCount = 1;
        public int CurrRound = 0;
        public float MatchDuration = 12f;

        public MatchMode Mode;


        public bool WasLastRound()
        {
            if (RoundsCount <= CurrRound)
            {
                return true;
            }
            return false;
        }
    }


       public enum MatchMode
        {
            Single = 0,
            Tournament =1
        }

}
