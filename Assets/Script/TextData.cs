using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScritableObject/Text")]

public class TextData : ScriptableObject
{
    [System.Serializable]
    public struct data
    {
        public string name;
        public string text;
        public charactorName charactor1Name;
        public charactorName charactor2Name;
        public showState charactorShow1;
        public showState charactorShow2;
        public bool blackOut;
        public bool MessageAnim;
        public bool CharactorAnim1;
        public bool CharactorAnim2;
    }
    [System.Serializable]
    public enum showState
    {
        hide = 0,
        show = 1,
        transparent = 2
    }
    [System.Serializable]
    public enum charactorName
    {
        same,
        you,
        flugel,
        Rei,
        Sena
    }

    public data[] textDatas;
    public Sprite[] charactorImg;
    
}
