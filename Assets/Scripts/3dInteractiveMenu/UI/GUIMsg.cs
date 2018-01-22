using System;
using UnityEngine;

namespace BeatsFever.UI
{
    class XGUIMsg : GUIMsg
    {
        public XGUIMsg(string name) : base (name)
        {
        }
        
        public int param { get; set; }

        public Vector3 pos { get; set; }

        public Vector2 dir { get; set; }
    }
    
    class IntStringParamsMsg : GUIMsg
    {
        public IntStringParamsMsg(string name) : base (name)
        {
        }
        
        public int int1 { get; set; }

        public int int2 { get; set; }

        public int int3 { get; set; }
        
        public string string1 { get; set; }

        public string string2 { get; set; }

        public string string3 { get; set; }

        public long long1 { get; set; }
    }

    
    class QuickParamMsg : GUIMsg
    {
        
        public QuickParamMsg(string name) : base (name)
        {
        }
        
        public int _int { get; set; }

        public long _long { get; set; }

        public float _float { get; set; }

        public string _string { get; set; }
    }
}