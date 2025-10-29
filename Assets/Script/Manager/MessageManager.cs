using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    //メッセージボックス、キャラクター
    [SerializeField] GameObject messageBox, charator1, charactor2;
    //暗い画面
    [SerializeField] Image blackImage;
    
    [SerializeField] Color transColor;
    
    
    private GameObject messageNextTextUI;

    private TextMeshProUGUI messageName, messageText;

    private Image charactorImage1, charactorImage2;
    private Animator messageAnim, charactorAnim1, charactorAnim2;

    //実際に表示するテキストデータ
    [SerializeField] TextData textDatas;
    private TextData.data textData;
    private TextData.data prevTextData;
    Queue<TextData.data> textQue;

    //アニメーション
    private Coroutine textCoroutine;
    [SerializeField] float textSpeed = 0.25f;
    private float fadeTime = 1.5f;
    private float textShowFinishInterval = 0.5f;
    private WaitForSeconds textInterval;
    private WaitForSeconds nextTextInterval;

    private int maxTextLen;
    private bool isTextShowFinish = false;
    private bool isAnimFinish = true;

    /*
     * スタート関数
     */
    private void Start()
    {
        messageName = messageBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        messageText = messageBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        messageNextTextUI = messageBox.transform.GetChild(2).gameObject;
        charactorImage1 = charator1.transform.GetChild(0).GetComponent<Image>();
        charactorImage2 = charactor2.transform.GetChild(0).GetComponent<Image>();
        
        messageAnim = messageBox.GetComponent<Animator>();
        charactorAnim1 = charator1.GetComponent<Animator>();
        charactorAnim2 = charactor2.GetComponent<Animator>();

        textInterval = new WaitForSeconds(textSpeed);
        nextTextInterval = new WaitForSeconds(textShowFinishInterval);
        textQue = new Queue<TextData.data>();
        for(int i = 0;i < textDatas.textDatas.Length; ++i)
        {
            textQue.Enqueue(textDatas.textDatas[i]);
        }
        TextDataInit();
    }

    /*
     * アップデート関数
     * 　・ボタンを押したらテキストを次に進める
     * 　
     */
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!isAnimFinish) return;
            if (isTextShowFinish)
            {
                TextDataInit();
            }
            else
            {
                StopCoroutine(TextCoroutine());
                messageText.maxVisibleCharacters = maxTextLen;
                FinishTextShow();
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SceneMove();
        }
    }

    /*
     * テキストの右下に表示している三角形を表示
     */
    private void FinishTextShow()
    {
        isTextShowFinish = true;
        messageNextTextUI.SetActive(true);

        
    }

    /*
     * だんだんテキストを表示するコールチン
     */
    private IEnumerator TextCoroutine()
    {
        messageText.maxVisibleCharacters = messageText.maxVisibleCharacters + 1;

        yield return textInterval;
        
        if (messageText.maxVisibleCharacters >= maxTextLen)
        {
            yield return nextTextInterval;
            FinishTextShow();
            
        }
        else
        {
            TextStartCoroutine();
        }

    }

    /*
     * キューからテキストを更新したり、キャラクターの表示を変更したりする
     */
    private void TextDataInit()
    {
        messageNextTextUI.SetActive(false);
        messageAnim.Play("Wait");
        if(textQue.Count == 0)
        {
            FinishScene();
            return;
        }
        isTextShowFinish = false;
        prevTextData = textData;
        messageText.maxVisibleCharacters = 0;
        
        textData = textQue.Dequeue();
        messageName.text = textData.name;
        maxTextLen = textData.text.Length;
        messageText.text = textData.text;

        CharactorShow(charactorImage1, textData.charactorShow1);
        CharactorShow(charactorImage2, textData.charactorShow2);
        if (textData.charactor1Name != TextData.charactorName.same && textData.charactor1Name != TextData.charactorName.you) CharactorChange(charactorImage1, textDatas.charactorImg[(int)(textData.charactor1Name) - 2]);
        if (textData.charactor2Name != TextData.charactorName.same && textData.charactor2Name != TextData.charactorName.you) CharactorChange(charactorImage2, textDatas.charactorImg[(int)(textData.charactor2Name)-2]);

        if (prevTextData.text != null)
        {
            if (prevTextData.blackOut && !textData.blackOut)
            {
                FadeIn();
                return;
            }
        }
        if(textData.MessageAnim)
        {
            messageAnim.Play("TextShake");
        }

        if(textData.CharactorAnim1)
        {
            charactorAnim1.Play("Jump");
        }
        if (textData.CharactorAnim2)
        {
            charactorAnim2.Play("Jump");
        }


        TextStartCoroutine();


    }

    /*
     * キャラクターを透過させたり完全に表示する
     *  ・img   : 透過させる画像
     *  ・state : 画像の状態 
     */
    private void CharactorShow(Image img, TextData.showState state)
    {
        switch(state)
        {
            case TextData.showState.transparent:
                img.color = transColor;
                break;
            case TextData.showState.show:
                img.color = new Color(1.0f,1.0f,1.0f);
                break;
            default:
                img.color = new Color(0.0f,0.0f,0.0f, 0.0f);
                break;


        }
    }

    /*
     * キャラクターの画像を入れ替える
     *  ・img    ： 入れ替える画像
     *  ・sprite :　変更後の画像 
     */
    private void CharactorChange(Image img, Sprite sprite)
    {
        img.sprite = sprite;
    }

    /*
     *  テキスト表示アニメーション再生
     */
    private void TextStartCoroutine()
    {
        textCoroutine = StartCoroutine(TextCoroutine());
    }

    /*
     * シーン終了時の処理
     */
    private void FinishScene()
    {
        FadeOut();
        messageBox.SetActive(false);
        Debug.Log("Scene Finish");
    }

    /*
     * シーンの移動
     */

    private void SceneMove()
    {
        ButtleManager.IsTutorial = true;
        SceneManager.LoadScene("ButtleScene");
    }

    /*
     * フェードインアニメーション
     */
    private void FadeIn()
    {
        isAnimFinish = false;
        Debug.Log("FADE IN");
        blackImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 0.0f), fadeTime).OnComplete(() =>
        {
            if (textData.CharactorAnim1)
            {
                charactorAnim1.Play("Jump");
            }
            if (textData.CharactorAnim2)
            {
                charactorAnim2.Play("Jump");
            }
            isAnimFinish = true;
            TextStartCoroutine();
        });
    }

    /*
     * フェードアウトアニメーション
     */
    private void FadeOut()
    {
        isAnimFinish = false;
        blackImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), fadeTime).OnComplete(() => { SceneMove(); });
    }




}
