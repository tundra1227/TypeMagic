using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class Charactor : ButtleCharactor
{
    // Start is called before the first frame update

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject textObj;
    [SerializeField] string majicAtkText = "喰らえ！<br>炎の剣！";

    private string typedTextColor = "grey";
    //private string unTypedTextColor = "red";

    Coroutine eraseTextCoroutine;
        
    

    /*
     * スタート関数
     *  EnemyをEnemyCharactorsに追加
     *  
     */
    override public void Start()
    {
        base.Start();

        var enemies = GameObject.FindWithTag("Enemy");
        EnemyCharactors = new List<ButtleCharactor>();
        for(int i = 0;i < enemies.transform.childCount; ++i)
        {
            EnemyCharactors.Add(enemies.transform.GetChild(i).GetComponent<Enemy>());
        }
        Init();


    }

    /*
    /*
     * 次の攻撃までのインターバル
     */
    public override IEnumerator AttackCoroutine()
    {

        yield return new WaitForSeconds(UnityEngine.Random.Range(MinAtkInterval, MaxAtkInterval));
        ButtleManager.Instance.AtkQue.Enqueue(this);
    }

    /*
     * 魔法での攻撃
     */
    public override void MajicAttack()
    {

        DrawText(majicAtkText);
        IsMajicAtk = false;
        AbleAttack = false;
        StartCoroutine(MajicTakeDamage());
        eraseTextCoroutine = StartCoroutine(EraseTextCourtine());
        StartCoroutine(MajicAttackInterval());
    }

    /*
     * 呪文を表示
     */
    public void DrawText(string newText)
    {
        if (eraseTextCoroutine != null)
        {
            StopCoroutine(eraseTextCoroutine);
            eraseTextCoroutine = null;
        }
        textObj.SetActive(true);
        text.text = newText;
    }

    public void DrawText(string newText1, string newText2)
    {
        textObj.SetActive(true);
        string s = $"<color=\"{typedTextColor}\">{newText1}</color>{newText2}";
        text.text = s;
    }

    /*
     * 呪文を消す
     */
    public void EraseText()
    {
        textObj.SetActive(false);
    }

    /*
     * 呪文を消すまでのインターバル
     */
    public IEnumerator EraseTextCourtine()
    {
        yield return new WaitForSeconds(3.0f);
        textObj.SetActive(false);
    }

    /*
     *　魔法で攻撃された後、再び攻撃されるまでのインターバル
     */
    public IEnumerator MajicAttackInterval()
    {
        yield return new WaitForSeconds(3.0f);
        ButtleManager.Instance.IsAnimFinish = true;
        yield return new WaitForSeconds(2.0f);
        AbleAttack = true;
       
    }

    /*
     * 魔法での攻撃を与えるコールチン
     */
    public IEnumerator MajicTakeDamage()
    {

        yield return new WaitForSeconds(1.5f);

        int r = UnityEngine.Random.Range(0, EnemyCharactors.Count);
        var targetTransform = EnemyCharactors[r].CharactorImgTransform.transform;
        float damage = MagicAtk + UnityEngine.Random.Range(-AtkRange, AtkRange);
        //EnemyCharactors[r].TakeDamage(damage);
        bool isKill = CauseDamage(r, MagicAtk);
        if (isKill)
        {
            EnemyCharactors[r].Dead();
            
        }
    }

    /*
     * 初期化
     */
    public override void Init()
    {
        base.Init();
        textObj.SetActive(false);
        var canvas = textObj.GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 5;
    }

    /*
     * 呪文を唱え終わった際の処理
     */
    public void FinishSpell()
    {
        Debug.Log($"Charactor : FinishSpell");
        IsSpell = false;
        ButtleManager.Instance.AtkQue.Enqueue(this);
    }

    /*
     * バトルが終わったら結果を出す
     */
    public override void FinishButtle()
    {
        base.FinishButtle();
        if (!ButtleManager.Instance.IsFinish) return; 
        ResultManager.Instance.ShowResult(false);
    }

    /*
     * 死んだ時の処理
     */
    public override void Dead()
    {
        base.Dead();
        EraseText();
    }

    /*
     * 回復
     */
    public void Heal()
    {
        HP += 10;
        HP = Math.Min(MAXHP, HP);
        HpGauge.GaugeUpdate(HP, MAXHP);
    }

}
