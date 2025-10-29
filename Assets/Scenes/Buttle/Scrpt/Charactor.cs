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
    [SerializeField] string majicAtkText = "��炦�I<br>���̌��I";

    private string typedTextColor = "grey";
    //private string unTypedTextColor = "red";

    Coroutine eraseTextCoroutine;
        
    

    /*
     * �X�^�[�g�֐�
     *  Enemy��EnemyCharactors�ɒǉ�
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
     * ���̍U���܂ł̃C���^�[�o��
     */
    public override IEnumerator AttackCoroutine()
    {

        yield return new WaitForSeconds(UnityEngine.Random.Range(MinAtkInterval, MaxAtkInterval));
        ButtleManager.Instance.AtkQue.Enqueue(this);
    }

    /*
     * ���@�ł̍U��
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
     * ������\��
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
     * ����������
     */
    public void EraseText()
    {
        textObj.SetActive(false);
    }

    /*
     * �����������܂ł̃C���^�[�o��
     */
    public IEnumerator EraseTextCourtine()
    {
        yield return new WaitForSeconds(3.0f);
        textObj.SetActive(false);
    }

    /*
     *�@���@�ōU�����ꂽ��A�ĂэU�������܂ł̃C���^�[�o��
     */
    public IEnumerator MajicAttackInterval()
    {
        yield return new WaitForSeconds(3.0f);
        ButtleManager.Instance.IsAnimFinish = true;
        yield return new WaitForSeconds(2.0f);
        AbleAttack = true;
       
    }

    /*
     * ���@�ł̍U����^����R�[���`��
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
     * ������
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
     * �����������I������ۂ̏���
     */
    public void FinishSpell()
    {
        Debug.Log($"Charactor : FinishSpell");
        IsSpell = false;
        ButtleManager.Instance.AtkQue.Enqueue(this);
    }

    /*
     * �o�g�����I������猋�ʂ��o��
     */
    public override void FinishButtle()
    {
        base.FinishButtle();
        if (!ButtleManager.Instance.IsFinish) return; 
        ResultManager.Instance.ShowResult(false);
    }

    /*
     * ���񂾎��̏���
     */
    public override void Dead()
    {
        base.Dead();
        EraseText();
    }

    /*
     * ��
     */
    public void Heal()
    {
        HP += 10;
        HP = Math.Min(MAXHP, HP);
        HpGauge.GaugeUpdate(HP, MAXHP);
    }

}
