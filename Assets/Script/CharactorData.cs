using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Data", menuName = "ScriptableObjects/CharactorData", order = 0)]
public class CharactorData : ScriptableObject
{
    public struct CharactorStatus
    {
        string name;
        float hp;
        float maxHp;
        float atk;
    }

    public List<CharactorStatus> charactorParam;
    

}
