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
    [Tooltip("HP�Q�[�W")]
    [SerializeField] HPGauge hpGauge;
    [Tooltip("�U���A�j���[�V�����Ȃǂ������A�j���[�^�[")]
    [SerializeField] Animator anim;
    [Tooltip("�L�����N�^�[�̉摜�̃|�W�V����")]
    [SerializeField] RectTransform charactorImgTransform;
    [Tooltip("�L�����N�^�[�̉摜")]
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
     * �X�e�[�^�X�Q
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

    //�U����
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
    //���@�ł̍U����
    private float magicAtk = 20.0f;
    public float MagicAtk
    {
        get { return magicAtk; }
        set { magicAtk = value; }
    }
    


    
    //���̍U�����s���܂ł̃C���^�[�o��
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

    


    //�G���܂Ƃ߂��z��
    private List<ButtleCharactor> enemyCharactors;
    public List<ButtleCharactor> EnemyCharactors
    {
        get { return enemyCharactors; }
        set { enemyCharactors = value; }
    }

    //����ł��邩�ǂ���
    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    //�r�������ǂ���
    private bool isSpell = false;
    public bool IsSpell
    {
        get { return isSpell; }
        set { isSpell = value; }
    }

    //���@�ł̍U�����ǂ���
    private bool isMajicAtk = false;
    public bool IsMajicAtk
    {
        get { return isMajicAtk; } 
        set { isMajicAtk = value; }
    }

    //�K�E�Z���ǂ���
    private bool isSpecialAtk = false;
    public bool IsSpecialAtk
    {
        get { return isSpecialAtk; }
        set { isSpecialAtk = value; }
    }

    //�U�����ł����Ԃ��ǂ���
    private bool ableAttack = true;
    public bool AbleAttack
    {
        get { return ableAttack; }
        set { ableAttack = value; }
    }

    //�L�����N�^�[�����l
    Vector3 firstPos;
    Vector3 firstScale;
    //�U�����̑傫��
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


    //�U�����[�V����
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

    //�Z���t
    //string majicSeriph = "���炦�I\n���̌��I";

    virtual public void Start()
    {
        firstPos = transform.position;
        firstScale = transform.localScale;
        attackScale = firstScale;
        Init();
    }


    /*
     * ������U������
     *  �E�����_���œG��I��
     *  �E�A�j���[�V�����Đ�
     *  �@�E�A�j���[�V�����̃R�[���o�b�N�Ń_���[�W��^����
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
     * ���@�ł̍U���A�q�N���X�Œ�`
     */
    public virtual void MajicAttack()
    {
        Debug.Log("MajicAttack virutal");
    }

    /*
     * �G�Ƀ_���[�W��^����
     *  enemyIndex : �ǂ̓G���U�����邩
     */
    public virtual bool CauseDamage(int enemyIndex, float a)
    {

        float damage = a + UnityEngine.Random.Range(-atkRange, atkRange);

        return EnemyCharactors[enemyIndex].TakeDamage(damage);



    }
    
    /*
     * ���̍U���܂őҋ@����R�[���`���A���b�҂��čU�������s(�q�N���X�Œ�`)
     * 
     */

    public virtual IEnumerator AttackCoroutine()
    {
        return null;
    }

    /*
     * �U�����~�߂�
     */

    public void StopAttack()
    {
        StopCoroutine("AttackCoroutine");
    }

    /*
     * �_���[�W���󂯂�
     *  damage : �_���[�W��
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
     * ���񂾍΂̏���
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
     * ������
     */
    public virtual void Init()
    {
        HpGauge.Init(HP, MAXHP);

        charactorImgTransform.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1.0f);
    }



}
