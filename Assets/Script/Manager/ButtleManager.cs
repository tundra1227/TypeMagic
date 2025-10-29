using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtleManager : MonoBehaviour
{
    //インスタンス
    static ButtleManager instance;
    public static ButtleManager Instance
    {
        get { return instance; }
    }

    [SerializeField] SpellCreater spellCreater;
    Coroutine spellCreaterCoroutine;

    //チュートリアルかどうか
    private static bool isTutorial = false; 
    public static bool IsTutorial
    {
        get { return isTutorial; }
        set { isTutorial = value; }
    }
    
    //プレイが止まっているかどうか
    private bool isPause = false;
    public bool IsPause 
    { 
        get { return isPause; } 
        set { isPause = value; }
    }

    //攻撃が実行できるかどうか
    private static bool ableAttack = true;
    public static bool AbleAttack
    {
        get { return ableAttack; }
        set { ableAttack = value; }
    }

    bool spellStart = false;

    //攻撃する順にキャラクターをキューに入れる
    private Queue<ButtleCharactor> atkQue;
    public Queue<ButtleCharactor> AtkQue
    {
        get { return atkQue; }
        set {  atkQue = value; }
    }

    //攻撃アニメーションが終了しているかどうか
    private bool isAnimFinish = true;
    public bool IsAnimFinish
    {
        get { return isAnimFinish; }
        set { isAnimFinish = value; }
    }

    private bool isFinish = false;
    public bool IsFinish
    {
        get { return isFinish; }
        set { isFinish = value; }
    }

    private bool playerWin = false;
    public bool PlayerWin
    {
        get { return playerWin; }
        set { playerWin = value; }
    }

    //何回選目か
    private int wave = 0;
    public int Wave
    {
        get { return wave; }
        set { wave = value; }
    }

    
    //キャラクターの情報
    private List<ButtleCharactor> enemys;
    private List<ButtleCharactor> players;
    private GameObject enemyObj;
    public List<ButtleCharactor> Players { get { return players; } }
    //チュートリアルgameObj
    [SerializeField] GameObject tutorialObj;
    //ポーズ画面obj
    [SerializeField] GameObject pauseObj;



    //敵のプレファブ
    [SerializeField] GameObject enemyPrefab;
    //敵のポジション
    [SerializeField] Vector3[] enemyPosition;
    //敵のスプライト
    [SerializeField] Sprite[] enemySprite;
    //エネミーを生成するまでのインターバル
    float enemyCreateInterval = 1.0f;
    //エネミーをクリエイトできているのか
    bool enemyCreate = false;

    //起動時にインスタンス化
    private void Awake()
    {
        if (instance != null) return;
        instance = this;
    }


    /*
     * スタート関数
     */
    void Start()
    {
        players = new List<ButtleCharactor>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        for(int i = 0;i < p.transform.childCount; ++i)
        {
            players.Add(p.transform.GetChild(i).GetComponent<Charactor>());

        }
        enemys = new List<ButtleCharactor>();
        GameObject e = GameObject.FindGameObjectWithTag("Enemy");
        enemyObj = e;
        for(int i = 0;i < e.transform.childCount; ++i)
        {
            enemys.Add(e.transform.GetChild(i).GetComponent<Enemy>());
        }


     
        atkQue = new Queue<ButtleCharactor>();

        if (IsTutorial)
        {
            isPause = true;
            tutorialObj.SetActive(true);
            
        }

    }


    /*
     * アップデート関数
     */
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            DOTween.KillAll();
            SceneManager.LoadScene("Title");
        }
        if (isFinish) return;
        if (isPause) return;

        if (!spellStart)
        {
            Restart();
            atkQue.Clear();
            spellCreaterCoroutine = StartCoroutine(spellCreater.SetSpell());
            spellStart = true;
        }
        

        AttackProcess();

    }



    /*
     * キューにたまった攻撃を実行する
     * アニメーション実行中などあれば待機する
     */
    void AttackProcess()
    {
        

        if(isFinish)
        {

            return;
        }



        if (IsAnimFinish && atkQue.Count != 0)
        {

            if(AtkQue.Peek() == null)
            {
                AtkQue.Dequeue();
                return;
            }

            if (AtkQue.Peek().IsDead)
            {
                if(atkQue.Peek().IsMajicAtk)
                {
                    if(spellCreaterCoroutine != null) StopCoroutine(spellCreaterCoroutine);
                    spellCreaterCoroutine = StartCoroutine(spellCreater.SetSpell());
                }
                AtkQue.Dequeue();
                
                return;
            }

            if (AtkQue.Peek().IsSpecialAtk)
            {
                AtkQue.Peek().IsMajicAtk = false;
            }
            else if (AtkQue.Peek().IsMajicAtk)
            {
                AtkQue.Dequeue().MajicAttack();
                spellCreaterCoroutine = StartCoroutine(spellCreater.SetSpell());
            }
            else
            {
                if (!AtkQue.Peek().IsSpell && AtkQue.Peek().AbleAttack)
                {
                    AtkQue.Dequeue().Attack();
                }
                else
                {
                    AtkQue.Dequeue();
                    return;
                }
            }
            IsAnimFinish = false;
        }
    }


    /*
     * キャラクターが死んだ時の処理
     *  charactor : 死んだキャラクター
     */
    public void Dead(ButtleCharactor charactor)
    {
        if (charactor is Charactor)
        {
            Debug.Log("BM : DEAD");
            players.Remove(charactor);
            int ind = spellCreater.Players.IndexOf((Charactor)charactor);
            if (ind != -1 && ind < spellCreater.PlayerIndex) spellCreater.PlayerIndex--;
            spellCreater.Players.Remove((Charactor)charactor);
            if(charactor.IsSpell)
            {
                StopCoroutine(spellCreaterCoroutine);
                spellCreaterCoroutine = StartCoroutine(spellCreater.SetSpell());
            }
        }
        else enemys.Remove(charactor);

        Destroy(charactor.gameObject, enemyCreateInterval-0.1f);
        FinishCheck();
    }   


    /*
     * ゲームが終了しているか判定
     */
    public void FinishCheck()
    {
        if(players.Count == 0)
        {
            if(!IsTutorial)
            {
                ResultManager.Instance.ShowResult(wave);
                IsFinish = true;
                return;
            }

            PlayerWin = false;
            ResultManager.Instance.ShowResult(false);
            IsFinish = true;
        }
        else if(enemys.Count == 0)
        {
            if(isTutorial)
            {
                ResultManager.Instance.ShowResult(true);
                IsFinish = true;
            }
            else
            {

                if (!enemyCreate)
                {
                    
                    Invoke("CreateEnemy", enemyCreateInterval);
                    ++wave;
                    enemyCreate = true;
                }
                    //CreateEnemy();
            }
            PlayerWin = true;
        }
        else
        {
            IsAnimFinish = true;
            return;
        }
        //isFinish = true;
    }

    public void CreateEnemy()
    {
        IsAnimFinish = false;
        int r = UnityEngine.Random.Range(0, 3);
        switch(r)
        {
            case 0:
                GameObject obj = Instantiate(enemyPrefab, enemyObj.transform);
                obj.transform.localPosition = enemyPosition[0];
                Enemy e = obj.GetComponent<Enemy>();
                enemys.Add(e);
                e.ChangeImg(enemySprite[UnityEngine.Random.Range(0, enemySprite.Length)]);
                e.Anim.Play("FadeIn");
                e.LevelSetting(wave);
                break;
            case 1:
                for(int i = 0;i < 2; ++i)
                {
                    GameObject o = Instantiate(enemyPrefab, enemyObj.transform);
                    o.transform.localPosition = enemyPosition[i+1];
                    Enemy e2 = o.GetComponent<Enemy>();
                    e2.ChangeImg(enemySprite[UnityEngine.Random.Range(0, enemySprite.Length)]);
                    e2.Anim.Play("FadeIn");
                    e2.LevelSetting(wave);
                    enemys.Add(e2);
                }
                break;
            default:
                for(int i = 0;i < 3; ++i)
                {
                    GameObject o = Instantiate(enemyPrefab, enemyObj.transform);
                    o.transform.localPosition = enemyPosition[i];
                    Enemy e2 = o.GetComponent<Enemy>();
                    e2.ChangeImg(enemySprite[UnityEngine.Random.Range(0, enemySprite.Length)]);
                    e2.Anim.Play("FadeIn");
                    enemys.Add(e2);
                    e2.LevelSetting(wave);
                }
                break;

        }
        foreach(var p in players) 
        {        
            p.EnemyCharactors = enemys;
        }
        Restart();
        enemyCreate = false;

    }

    /*
     * 敵を生成してから改めて戦闘開始
     */
    private void Restart()
    {
        foreach (var p in players)
        {
            p.StopAttack();
            StartCoroutine(p.AttackCoroutine());
        }
        foreach (var p in enemys) StartCoroutine(p.AttackCoroutine());

        AtkQue.Clear();
        for(int i = 0;i < players.Count; ++i)
        {
            if (players[i].IsMajicAtk) AtkQue.Enqueue(players[i]);
        }
        IsAnimFinish = true;
    }



    public void ResumeSpell()
    {
        spellCreater.enabled = true;
    }

}
