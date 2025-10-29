using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ButtleCharactor
{



    override public void Start()
    {
        
        base.Start();


        var enemiesObj = GameObject.FindWithTag("Player");
        EnemyCharactors = new List<ButtleCharactor>();
        for(int i = 0;i < enemiesObj.transform.childCount; ++i)
        {
            EnemyCharactors.Add(enemiesObj.transform.GetChild(i).GetComponent<Charactor>());
        }

        AttackScale = FirstScale * 0.5f;


    }



    /*
     * UŒ‚‚ÌƒR[ƒ‹ƒ`ƒ“
     */
    public override IEnumerator AttackCoroutine() 
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(MinAtkInterval, MaxAtkInterval));
        ButtleManager.Instance.AtkQue.Enqueue(this);
    }


    /*
     * “G‚ÌƒŒƒxƒ‹İ’è
     */
    public void LevelSetting(int level)
    {
        if(level < 10)
        {
            Atk = 8.0f;
        }
        else if(level < 20)
        {
            Atk = 12.0f;
        }
        else if(level < 50)
        {
            Atk = 20.0f;
        }
        else
        {
            Atk = 25.0f;
        }

    }

    /*
     * “G‚Ì‰æ‘œ‚ğİ’è
     */
    public void ChangeImg(Sprite sprite)
    {
        CharactorImg.sprite = sprite;
    }



}