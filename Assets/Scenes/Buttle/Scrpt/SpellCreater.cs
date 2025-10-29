using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class SpellCreater : MonoBehaviour
{

    private List<Charactor> players;
    public List<Charactor> Players { get { return players; } }
    private int playerIndex = 0;
    public int PlayerIndex { get { return playerIndex; }  set { playerIndex = value; } }

    //タイピングする呪文
    private string spell = "";
    private int MAXSPELLSIZE = 10;
    private int MINSPELLSIZE = 6;

    private Queue<List<string>> typeKey = new Queue<List<string>>();    //タイプするキーを一文字ずつ格納するキュー 
    private Queue<int> textStrIndex = new Queue<int>();                 //画面に表示する文字列保存
    private int typeIndex = 0;                                          //1文字を打つ際に用いる配列のインデックス
    private List<string> nowType;                                       //今実際に操作しているキャラクター


    //呪文生成するインターバル
    private const float MININTERVAL = 1.0f;
    private const float MAXINTERVAL = 2.0f;


    //日本語からローマ字に変換
    private Dictionary<string, string[]> japaneseToRomaji = new Dictionary<string, string[]> {
        {"あ", new string[] {"a" } },
        {"い", new string[] {"i" } },
        {"う", new string[] {"u" } },
        {"え", new string[] {"e" } },
        {"お", new string[] {"o" } },
        {"か", new string[] {"ka", "ca" } },
        {"き", new string[] {"ki" } },
        {"く", new string[] {"ku", "cu" } },
        {"け", new string[] {"ke" } },
        {"こ", new string[] {"ko", "co" } },
        {"さ", new string[] {"sa" } },
        {"し", new string[] {"si", "shi", "ci" } },
        {"す", new string[] {"su" } },
        {"せ", new string[] {"se", "ce" } },
        {"そ", new string[] {"so" } },
        {"た", new string[] {"ta" } },
        {"ち", new string[] {"ti", "chi" } },
        {"つ", new string[] {"tu", "tsu" } },
        {"て", new string[] {"te" } },
        {"と", new string[] {"to" } },
        {"な", new string[] {"na" } },
        {"に", new string[] {"ni" } },
        {"ぬ", new string[] {"nu" } },
        {"ね", new string[] {"ne" } },
        {"の", new string[] {"no" } },
        {"は", new string[] {"ha" } },
        {"ひ", new string[] {"hi" } },
        {"ふ", new string[] {"hu", "fu" } },
        {"へ", new string[] {"he" } },
        {"ほ", new string[] {"ho" } },
        {"ま", new string[] {"ma" } },
        {"み", new string[] {"mi" } },
        {"む", new string[] {"mu" } },
        {"め", new string[] {"me" } },
        {"も", new string[] {"mo" } },
        {"や", new string[] {"ya" } },
        {"ゆ", new string[] {"yu" } },
        {"よ", new string[] {"yo" } },
        {"ら", new string[] {"ra" } },
        {"り", new string[] {"ri" } },
        {"る", new string[] {"ru" } },
        {"れ", new string[] {"re" } },
        {"ろ", new string[] {"ro" } },
        {"わ", new string[] {"wa" } },
        {"を", new string[] {"wo" } },
        {"ん", new string[] {"nn", "xn" } },
        {"が", new string[] {"ga" } },
        {"ぎ", new string[] {"gi" } },
        {"ぐ", new string[] {"gu" } },
        {"げ", new string[] {"ge" } },
        {"ご", new string[] {"go" } },
        {"ざ", new string[] {"za" } },
        {"じ", new string[] {"zi", "ji" } },
        {"ず", new string[] {"zu" } },
        {"ぜ", new string[] {"ze" } },
        {"ぞ", new string[] {"zo" } },
        {"だ", new string[] {"da" } },
        {"ぢ", new string[] {"di" } },
        {"づ", new string[] {"du" } },
        {"で", new string[] {"de" } },
        {"ど", new string[] {"do" } },
        {"ば", new string[] {"ba" } },
        {"び", new string[] {"bi" } },
        {"ぶ", new string[] {"bu" } },
        {"べ", new string[] {"be" } },
        {"ぼ", new string[] {"bo" } },
        {"ぱ", new string[] {"pa" } },
        {"ぴ", new string[] {"pi" } },
        {"ぷ", new string[] {"pu" } },
        {"ぺ", new string[] {"pe" } },
        {"ぽ", new string[] {"po" } },
        {"ぁ", new string[] {"la", "xa" } },
        {"ぃ", new string[] {"li", "xi" } },
        {"ぅ", new string[] {"lu", "xu" } },
        {"ぇ", new string[] {"le", "xe" } },
        {"ぉ", new string[] {"lo", "xo" } },
        {"ゃ", new string[] {"lya", "xya" } },
        {"ゅ", new string[] {"lyu", "xyu" } },
        {"ょ", new string[] {"lyo", "xyo" } },
        {"きゃ", new string[] {"kya" } },
        {"きぃ", new string[] {"kyi" } },
        {"きゅ", new string[] {"kyu" } },
        {"きぇ", new string[] {"kye" } },
        {"きょ", new string[] {"kyo" } },
        {"ぎゃ", new string[] {"gya" } },
        {"ぎぃ", new string[] {"gyi" } },
        {"ぎゅ", new string[] {"gyu" } },
        {"ぎぇ", new string[] {"gye" } },
        {"ぎょ", new string[] {"gyo" } },
        {"しゃ", new string[] {"sya", "sha" } },
        {"しぃ", new string[] {"syi" } },
        {"しゅ", new string[] {"syu", "shu" } },
        {"しぇ", new string[] {"sye", "she" } },
        {"しょ", new string[] {"syo", "sho" } },
        {"じゃ", new string[] {"zya", "ja", "jya" } },
        {"じぃ", new string[] {"zyi", "jyi" } },
        {"じゅ", new string[] {"zyu", "ju", "jyu" } },
        {"じぇ", new string[] {"zye", "jye" } },
        {"じょ", new string[] {"zyo", "jo", "jyo" } },
        {"ちゃ", new string[] {"tya", "cya", "cha" } },
        {"ちぃ", new string[] {"tyi", "cyi" } },
        {"ちゅ", new string[] {"tyu", "cyu", "chu" } },
        {"ちぇ", new string[] {"tye", "cye", "che" } },
        {"ちょ", new string[] {"tyo", "cyo", "cho" } },
        {"てゃ", new string[] {"tha" } },
        {"てぃ", new string[] {"thi" } },
        {"てゅ", new string[] {"thu" } },
        {"てぇ", new string[] {"the" } },
        {"てょ", new string[] {"tho" } },
        {"でゃ", new string[] {"dha" } },
        {"でぃ", new string[] {"dhi" } },
        {"でゅ", new string[] {"dhu" } },
        {"でぇ", new string[] {"dhe" } },
        {"でょ", new string[] {"dho" } },
        {"にゃ", new string[] {"nya" } },
        {"にぃ", new string[] {"nyi" } },
        {"にゅ", new string[] {"nyu" } },
        {"にぇ", new string[] {"nye" } },
        {"にょ", new string[] {"nyo" } },
        {"ひゃ", new string[] {"hya" } },
        {"ひぃ", new string[] {"hyi" } },
        {"ひゅ", new string[] {"hyu" } },
        {"ひぇ", new string[] {"hye" } },
        {"ひょ", new string[] {"hyo" } },
        {"びゃ", new string[] {"bya" } },
        {"びぃ", new string[] {"byi" } },
        {"びゅ", new string[] {"byu" } },
        {"びぇ", new string[] {"bye" } },
        {"びょ", new string[] {"byo" } },
        {"ぴゃ", new string[] {"pya" } },
        {"ぴぃ", new string[] {"pyi" } },
        {"ぴゅ", new string[] {"pyu" } },
        {"ぴぇ", new string[] {"pye" } },
        {"ぴょ", new string[] {"pyo" } },
        {"ふぁ", new string[] {"fa" } },
        {"ふぃ", new string[] {"fi" } },
        {"ふぇ", new string[] {"fe" } },
        {"ふぉ", new string[] {"fo" } },
        {"ヴぁ", new string[] {"va" } },
        {"ヴぃ", new string[] {"vi" } },
        {"ヴ", new string[] {"vu" } },
        {"ヴぇ", new string[] {"ve" } },
        {"ヴぉ", new string[] {"vo" } },
        {"みゃ", new string[] {"mya" } },
        {"みぃ", new string[] {"myi" } },
        {"みゅ", new string[] {"myu" } },
        {"みぇ", new string[] {"mye" } },
        {"みょ", new string[] {"myo" } },
        {"りゃ", new string[] {"rya" } },
        {"りぃ", new string[] {"ryi" } },
        {"りゅ", new string[] {"ryu" } },
        {"りぇ", new string[] {"rye" } },
        {"りょ", new string[] {"ryo" } },


    };

    private string spellCreateChar1 = "あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわをがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ";
    private string spellCreateChar2 = "ぎゃぎゅぎょじゃじゅじぇじょちゃちゅちょにゃにゅにょみゃみゅみょりゃりゅりょ";

    private bool isInputOk = false;

    int correctTypeKey = 0;

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");
        players = new List<Charactor>();
        for(int i = 0;i < player.transform.childCount; ++i)
        {
            players.Add(player.transform.GetChild(i).GetComponent<Charactor>());
        }

    }


    private void Update()
    {
        if (!isInputOk) return;

        InputKey();
    }

    /*
    * 呪文の割り当て
    */
    public IEnumerator SetSpell()
    {
        //System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();

        Init();
        isInputOk = false;
        yield return new WaitForSeconds(UnityEngine.Random.Range(MININTERVAL, MAXINTERVAL));

        if (!ButtleManager.Instance.IsFinish)
        {
            int r = UnityEngine.Random.Range(0, players.Count);

            CreateSpell();
            SetKey(spell);
            SetPlayer();
            isInputOk = true;
        }


    }

    /*
     * 呪文の生成
     */
    private void CreateSpell()
    {
        spell = "";
        int r = UnityEngine.Random.Range(MINSPELLSIZE, MAXSPELLSIZE);
        for (int i = 0; i < r; ++i)
        {
            int r2 = UnityEngine.Random.Range(0, spellCreateChar1.Length);
            spell += spellCreateChar1[r2];
        }
    }



    /*
     * 文字列からタイプする2キーを作成
     *       argStr : 文字列（タイプする呪文）
     */
    public void SetKey(string argStr)
    {

        typeIndex = 0;
        for (int i = 0; i < argStr.Length; ++i)
        {
            string s = "";
            s = argStr[i].ToString();
            //Debug.Log(s);
            if (i != argStr.Length - 1)
            {

            }

            typeKey.Enqueue(japaneseToRomaji[s].ToList<string>());
            textStrIndex.Enqueue(i + 1);
        }
        SetNextType();
    }

    /*
     * キーの入力を識別する
     */
    private void InputKey()
    {

        //Debug.Log($"{nowType[0]}");

        if (nowType == null) return;
        //var copyNowType = new List<string>(nowType);

        if (!Input.anyKeyDown) return;

        bool unCorrectType = true;

        var copyNowType = new List<string>(nowType);

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(key)) continue;

            foreach (var s in copyNowType)
            {
                if (typeIndex >= s.Length) continue;
                if (key.ToString().ToLower() == s[typeIndex].ToString())
                {
                    //Debug.Log(key.ToString().ToLower());
                    EraseKey(s[typeIndex]);
                    typeIndex++;
                    unCorrectType = false;
                    if (typeIndex >= s.Length)
                    {
                        string[] splitText = SplitStr(spell, textStrIndex.Dequeue());
                        
                        players[playerIndex].DrawText(splitText[0], splitText[1]);
                        SetNextType();
                        return;
                    }
                }
            }
        }

        if (unCorrectType)
        {
            correctTypeKey = 0;
            //Debug.Log("間違ったキー!");
        }
        else
        {
            correctTypeKey++;
        }

        if(correctTypeKey != 0 && correctTypeKey % 20 == 0)
        {
            foreach (var p in players) p.Heal();
        }

    }




    /*
     * 入力したキーに対して文字の選択肢を消していく
     *  ・ちゃ(tya, cha)と打つとき、cが打たれた時点でtyaの候補を消す
     *          c : 押されたアルファベット
     */
    private void EraseKey(char c)
    {
        var copyNowType = new List<string>(nowType);

        foreach (string s in copyNowType)
        {
            if (s[typeIndex] != c)
            {
                nowType.Remove(s);
            }
        }
    }


    /*
     * 1文字を入力し終わった際、次の文字列を入力できるようにセットする
     */
    private void SetNextType()
    {
        if (typeKey.Count == 0)
        {
            FinishSpell();
            return;
        }
        nowType = new List<string>(typeKey.Dequeue());
        typeIndex = 0;
    }


    /*
     * 呪文を詠唱するプレイヤーをセット
     */

    private void SetPlayer()
    {

        playerIndex = UnityEngine.Random.Range(0, players.Count);
        players[playerIndex].DrawText(spell);
        players[playerIndex].IsSpell = true;
    }

    /*
     * 呪文の詠唱が終わった際に呼び出す
     */
    private void FinishSpell()
    {
        players[playerIndex].FinishSpell();
        players[playerIndex].IsMajicAtk = true;
    }

    /*
     * 文字列を特定の部分で二つに分割
     *      argS : 分割する文字列
     *      num  : 分割する位置（前半部分の文字列の長さ）
     */
    private string[] SplitStr(string argS, int num)
    {

        string[] s = new string[] { "", "" };

        for (int i = 0; i < argS.Length; ++i)
        {
            if (i < num)
            {
                s[0] += argS[i];
            }
            else
            {
                s[1] += argS[i];
            }
        }
        return s;

    }

    /*
     * 初期化
     */

    private void Init()
    {
        textStrIndex.Clear();
        typeKey.Clear();
        typeIndex = 0;
    }
}
