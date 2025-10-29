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

    //�^�C�s���O�������
    private string spell = "";
    private int MAXSPELLSIZE = 10;
    private int MINSPELLSIZE = 6;

    private Queue<List<string>> typeKey = new Queue<List<string>>();    //�^�C�v����L�[���ꕶ�����i�[����L���[ 
    private Queue<int> textStrIndex = new Queue<int>();                 //��ʂɕ\�����镶����ۑ�
    private int typeIndex = 0;                                          //1������łۂɗp����z��̃C���f�b�N�X
    private List<string> nowType;                                       //�����ۂɑ��삵�Ă���L�����N�^�[


    //������������C���^�[�o��
    private const float MININTERVAL = 1.0f;
    private const float MAXINTERVAL = 2.0f;


    //���{�ꂩ�烍�[�}���ɕϊ�
    private Dictionary<string, string[]> japaneseToRomaji = new Dictionary<string, string[]> {
        {"��", new string[] {"a" } },
        {"��", new string[] {"i" } },
        {"��", new string[] {"u" } },
        {"��", new string[] {"e" } },
        {"��", new string[] {"o" } },
        {"��", new string[] {"ka", "ca" } },
        {"��", new string[] {"ki" } },
        {"��", new string[] {"ku", "cu" } },
        {"��", new string[] {"ke" } },
        {"��", new string[] {"ko", "co" } },
        {"��", new string[] {"sa" } },
        {"��", new string[] {"si", "shi", "ci" } },
        {"��", new string[] {"su" } },
        {"��", new string[] {"se", "ce" } },
        {"��", new string[] {"so" } },
        {"��", new string[] {"ta" } },
        {"��", new string[] {"ti", "chi" } },
        {"��", new string[] {"tu", "tsu" } },
        {"��", new string[] {"te" } },
        {"��", new string[] {"to" } },
        {"��", new string[] {"na" } },
        {"��", new string[] {"ni" } },
        {"��", new string[] {"nu" } },
        {"��", new string[] {"ne" } },
        {"��", new string[] {"no" } },
        {"��", new string[] {"ha" } },
        {"��", new string[] {"hi" } },
        {"��", new string[] {"hu", "fu" } },
        {"��", new string[] {"he" } },
        {"��", new string[] {"ho" } },
        {"��", new string[] {"ma" } },
        {"��", new string[] {"mi" } },
        {"��", new string[] {"mu" } },
        {"��", new string[] {"me" } },
        {"��", new string[] {"mo" } },
        {"��", new string[] {"ya" } },
        {"��", new string[] {"yu" } },
        {"��", new string[] {"yo" } },
        {"��", new string[] {"ra" } },
        {"��", new string[] {"ri" } },
        {"��", new string[] {"ru" } },
        {"��", new string[] {"re" } },
        {"��", new string[] {"ro" } },
        {"��", new string[] {"wa" } },
        {"��", new string[] {"wo" } },
        {"��", new string[] {"nn", "xn" } },
        {"��", new string[] {"ga" } },
        {"��", new string[] {"gi" } },
        {"��", new string[] {"gu" } },
        {"��", new string[] {"ge" } },
        {"��", new string[] {"go" } },
        {"��", new string[] {"za" } },
        {"��", new string[] {"zi", "ji" } },
        {"��", new string[] {"zu" } },
        {"��", new string[] {"ze" } },
        {"��", new string[] {"zo" } },
        {"��", new string[] {"da" } },
        {"��", new string[] {"di" } },
        {"��", new string[] {"du" } },
        {"��", new string[] {"de" } },
        {"��", new string[] {"do" } },
        {"��", new string[] {"ba" } },
        {"��", new string[] {"bi" } },
        {"��", new string[] {"bu" } },
        {"��", new string[] {"be" } },
        {"��", new string[] {"bo" } },
        {"��", new string[] {"pa" } },
        {"��", new string[] {"pi" } },
        {"��", new string[] {"pu" } },
        {"��", new string[] {"pe" } },
        {"��", new string[] {"po" } },
        {"��", new string[] {"la", "xa" } },
        {"��", new string[] {"li", "xi" } },
        {"��", new string[] {"lu", "xu" } },
        {"��", new string[] {"le", "xe" } },
        {"��", new string[] {"lo", "xo" } },
        {"��", new string[] {"lya", "xya" } },
        {"��", new string[] {"lyu", "xyu" } },
        {"��", new string[] {"lyo", "xyo" } },
        {"����", new string[] {"kya" } },
        {"����", new string[] {"kyi" } },
        {"����", new string[] {"kyu" } },
        {"����", new string[] {"kye" } },
        {"����", new string[] {"kyo" } },
        {"����", new string[] {"gya" } },
        {"����", new string[] {"gyi" } },
        {"����", new string[] {"gyu" } },
        {"����", new string[] {"gye" } },
        {"����", new string[] {"gyo" } },
        {"����", new string[] {"sya", "sha" } },
        {"����", new string[] {"syi" } },
        {"����", new string[] {"syu", "shu" } },
        {"����", new string[] {"sye", "she" } },
        {"����", new string[] {"syo", "sho" } },
        {"����", new string[] {"zya", "ja", "jya" } },
        {"����", new string[] {"zyi", "jyi" } },
        {"����", new string[] {"zyu", "ju", "jyu" } },
        {"����", new string[] {"zye", "jye" } },
        {"����", new string[] {"zyo", "jo", "jyo" } },
        {"����", new string[] {"tya", "cya", "cha" } },
        {"����", new string[] {"tyi", "cyi" } },
        {"����", new string[] {"tyu", "cyu", "chu" } },
        {"����", new string[] {"tye", "cye", "che" } },
        {"����", new string[] {"tyo", "cyo", "cho" } },
        {"�Ă�", new string[] {"tha" } },
        {"�Ă�", new string[] {"thi" } },
        {"�Ă�", new string[] {"thu" } },
        {"�Ă�", new string[] {"the" } },
        {"�Ă�", new string[] {"tho" } },
        {"�ł�", new string[] {"dha" } },
        {"�ł�", new string[] {"dhi" } },
        {"�ł�", new string[] {"dhu" } },
        {"�ł�", new string[] {"dhe" } },
        {"�ł�", new string[] {"dho" } },
        {"�ɂ�", new string[] {"nya" } },
        {"�ɂ�", new string[] {"nyi" } },
        {"�ɂ�", new string[] {"nyu" } },
        {"�ɂ�", new string[] {"nye" } },
        {"�ɂ�", new string[] {"nyo" } },
        {"�Ђ�", new string[] {"hya" } },
        {"�Ђ�", new string[] {"hyi" } },
        {"�Ђ�", new string[] {"hyu" } },
        {"�Ђ�", new string[] {"hye" } },
        {"�Ђ�", new string[] {"hyo" } },
        {"�т�", new string[] {"bya" } },
        {"�т�", new string[] {"byi" } },
        {"�т�", new string[] {"byu" } },
        {"�т�", new string[] {"bye" } },
        {"�т�", new string[] {"byo" } },
        {"�҂�", new string[] {"pya" } },
        {"�҂�", new string[] {"pyi" } },
        {"�҂�", new string[] {"pyu" } },
        {"�҂�", new string[] {"pye" } },
        {"�҂�", new string[] {"pyo" } },
        {"�ӂ�", new string[] {"fa" } },
        {"�ӂ�", new string[] {"fi" } },
        {"�ӂ�", new string[] {"fe" } },
        {"�ӂ�", new string[] {"fo" } },
        {"����", new string[] {"va" } },
        {"����", new string[] {"vi" } },
        {"��", new string[] {"vu" } },
        {"����", new string[] {"ve" } },
        {"����", new string[] {"vo" } },
        {"�݂�", new string[] {"mya" } },
        {"�݂�", new string[] {"myi" } },
        {"�݂�", new string[] {"myu" } },
        {"�݂�", new string[] {"mye" } },
        {"�݂�", new string[] {"myo" } },
        {"���", new string[] {"rya" } },
        {"�股", new string[] {"ryi" } },
        {"���", new string[] {"ryu" } },
        {"�肥", new string[] {"rye" } },
        {"���", new string[] {"ryo" } },


    };

    private string spellCreateChar1 = "�����������������������������������ĂƂȂɂʂ˂̂͂Ђӂւق܂݂ނ߂�������������������������������������Âłǂ΂тԂׂڂς҂Ղ؂�";
    private string spellCreateChar2 = "���Ⴌ�ガ�傶�Ⴖ�ザ�����傿�Ⴟ�タ��ɂ�ɂ�ɂ�݂�݂�݂�������";

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
    * �����̊��蓖��
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
     * �����̐���
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
     * �����񂩂�^�C�v����2�L�[���쐬
     *       argStr : ������i�^�C�v��������j
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
     * �L�[�̓��͂����ʂ���
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
            //Debug.Log("�Ԉ�����L�[!");
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
     * ���͂����L�[�ɑ΂��ĕ����̑I�����������Ă���
     *  �E����(tya, cha)�ƑłƂ��Ac���ł��ꂽ���_��tya�̌�������
     *          c : �����ꂽ�A���t�@�x�b�g
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
     * 1��������͂��I������ہA���̕��������͂ł���悤�ɃZ�b�g����
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
     * �������r������v���C���[���Z�b�g
     */

    private void SetPlayer()
    {

        playerIndex = UnityEngine.Random.Range(0, players.Count);
        players[playerIndex].DrawText(spell);
        players[playerIndex].IsSpell = true;
    }

    /*
     * �����̉r�����I������ۂɌĂяo��
     */
    private void FinishSpell()
    {
        players[playerIndex].FinishSpell();
        players[playerIndex].IsMajicAtk = true;
    }

    /*
     * ����������̕����œ�ɕ���
     *      argS : �������镶����
     *      num  : ��������ʒu�i�O�������̕�����̒����j
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
     * ������
     */

    private void Init()
    {
        textStrIndex.Clear();
        typeKey.Clear();
        typeIndex = 0;
    }
}
