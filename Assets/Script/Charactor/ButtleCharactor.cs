using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Collections;


public class ButtleCharactor : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("HPゲージ")]
    [SerializeField] HPGauge hpGauge;
    [Tooltip("攻撃アニメーションなどを扱うアニメーター")]
    [SerializeField] Animator anim;
    [Tooltip("キャラクターの画像のポジション")]
    [SerializeField] RectTransform charactorImgTransform;
    [Tooltip("キャラクターの画像")]
    [SerializeField] Image charactorImg;


    public HPGauge HpGauge
    {
        get { return hpGauge; }
    }

    public Animator Anim
    {
        get { return anim; }
    }
    public RectTransform CharactorImgTransform
    {
        get { return charactorImgTransform; }
    }
    public Image CharactorImg
    {
        get { return charactorImg; }
    }
        


    /*
     * ステータス群
     */

    //HP
    private float maxHp = 100.0f;
    private float Hp = 100.0f;
    public float MAXHP
    {
        get { return maxHp; }
        set { maxHp = value; }
    }
    public float HP
    {
        get { return Hp; }
        set { Hp = value; }
    }

    //攻撃力
    private float atk = 8.0f;
    private float atkRange = 5.0f;
    public float Atk
    {
        get { return atk; }
        set { atk = value; }
    }
    public float AtkRange
    {
        get { return atkRange; }
        set { atkRange = value; }
    }
    //魔法での攻撃力
    private float magicAtk = 20.0f;
    public float MagicAtk
    {
        get { return magicAtk; }
        set { magicAtk = value; }
    }
    


    
    //次の攻撃を行うまでのインターバル
    private float minAtkInterval = 5.0f;
    private float maxAtkInterval = 10.0f;
    public float MinAtkInterval
    {
        get { return minAtkInterval; }
        set { minAtkInterval = value; }
    }
    public float MaxAtkInterval
    {
        get { return maxAtkInterval; }
        set { maxAtkInterval = value; }
    }

    


    //敵をまとめた配列
    private List<ButtleCharactor> enemyCharactors;
    public List<ButtleCharactor> EnemyCharactors
    {
        get { return enemyCharactors; }
        set { enemyCharactors = value; }
    }

    //死んでいるかどうか
    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    //詠唱中かどうか
    private bool isSpell = false;
    public bool IsSpell
    {
        get { return isSpell; }
        set { isSpell = value; }
    }

    //魔法での攻撃かどうか
    private bool isMajicAtk = false;
    public bool IsMajicAtk
    {
        get { return isMajicAtk; } 
        set { isMajicAtk = value; }
    }

    //必殺技かどうか
    private bool isSpecialAtk = false;
    public bool IsSpecialAtk
    {
        get { return isSpecialAtk; }
        set { isSpecialAtk = value; }
    }

    //攻撃ができる状態かどうか
    private bool ableAttack = true;
    public bool AbleAttack
    {
        get { return ableAttack; }
        set { ableAttack = value; }
    }

    //キャラクター初期値
    Vector3 firstPos;
    Vector3 firstScale;
    //攻撃時の大きさ
    Vector3 attackScale;
    public Vector3 FirstPos
    {
        get { return firstPos; } 
        set { firstPos = value;}
    }

    public Vector3 FirstScale
    {
        get { return firstScale; }
        set { firstScale = value;}
    }
    public Vector3 AttackScale
    {
        get { return attackScale; }
        set { attackScale = value; }
    }


    //攻撃モーション
    private Sequence atkAnimSequence;
    public Sequence AtkAnimSequence
    {
        get { return atkAnimSequence; }
        set { atkAnimSequence = value; }
    }
    private float atkMoveAnimSpeed = 0.5f;
    public float AtkMoveAnimSpeed
    {
        get { return atkMoveAnimSpeed; }
        set { atkMoveAnimSpeed = value; }
    }

    //セリフ
    //string majicSeriph = "くらえ！\n炎の剣！";

    virtual public void Start()
    {
        firstPos = transform.position;
        firstScale = transform.localScale;
        attackScale = firstScale;
        Init();
    }


    /*
     * 相手を攻撃する
     *  ・ランダムで敵を選択
     *  ・アニメーション再生
     *  　・アニメーションのコールバックでダメージを与える
     */
    public virtual void Attack()
    {
        if (IsDead)
        {
            ButtleManager.Instance.IsAnimFinish = true;
            return;
        }



        int playerIndex = UnityEngine.Random.Range(0, EnemyCharactors.Count);
        AtkAnimSequence = DOTween.Sequence();

        float waitTime = 0.25f;
        bool isKill = false;
        
        Vector3 targetPos = EnemyCharactors[playerIndex].transform.GetChild(0).transform.position;
        
        AtkAnimSequence.Append(transform.DOMove(targetPos, AtkMoveAnimSpeed))
                        .Join(transform.DOScale(attackScale, AtkMoveAnimSpeed))
                        .AppendCallback(() => { isKill = CauseDamage(playerIndex, atk); })
                        .AppendInterval(waitTime)
                        .Append(transform.DOMove(FirstPos, AtkMoveAnimSpeed))
                        .Join(transform.DOScale(FirstScale, AtkMoveAnimSpeed))
                        .AppendCallback(() => {
                            if (!IsDead && !IsSpell) StartCoroutine(AttackCoroutine());
                            if(!isKill) ButtleManager.Instance.IsAnimFinish = true;
                            else
                            {
                                enemyCharactors[playerIndex].Dead();

                            }

                        }).SetId(this.gameObject);
        


    }

    /*
     * 魔法での攻撃、子クラスで定義
     */
    public virtual void MajicAttack()
    {
        Debug.Log("MajicAttack virutal");
    }

    /*
     * 敵にダメージを与える
     *  enemyIndex : どの敵を攻撃するか
     */
    public virtual bool CauseDamage(int enemyIndex, float a)
    {

        float damage = a + UnityEngine.Random.Range(-atkRange, atkRange);

        return EnemyCharactors[enemyIndex].TakeDamage(damage);



    }
    
    /*
     * 次の攻撃まで待機するコールチン、数秒待って攻撃を実行(子クラスで定義)
     * 
     */

    public virtual IEnumerator AttackCoroutine()
    {
        return null;
    }

    /*
     * 攻撃を止める
     */

    public void StopAttack()
    {
        StopCoroutine("AttackCoroutine");
    }

    /*
     * ダメージを受ける
     *  damage : ダメージ量
     */
     public virtual bool TakeDamage(float damage)
    {
        HP = Mathf.Max(HP - damage, 0.0f);
        HpGauge.GaugeUpdate(HP, MAXHP);
        if (HP <= 0.0f)
        {
            ButtleManager.Instance.IsAnimFinish = false;
            return true;
        }
        Anim.Play("Damaged");
        return false;
    }
    
    /*
     * 死んだ歳の処理
     */
    public virtual void Dead()
    {
        foreach (var p in enemyCharactors) p.enemyCharactors.Remove(this);
        IsDead = true;
        ButtleManager.Instance.Dead(this);
        DOTween.Kill(this.gameObject);
        Anim.Play("FadeOut");
    }

    /*
     */
    public virtual void FinishButtle()
    {
        ButtleManager.Instance.IsAnimFinish = true;
    }



    /*
     * 初期化
     */
    public virtual void Init()
    {
        HpGauge.Init(HP, MAXHP);

        charactorImgTransform.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1.0f);
    }



}
