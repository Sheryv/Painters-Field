using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Rect GetRect()
        {
            Rect r = new Rect(0,0,X,Y);
            return r;
        }
    }
}
