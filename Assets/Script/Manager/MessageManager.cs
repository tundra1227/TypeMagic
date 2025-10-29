using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageManager : MonoBehaviour
{
    //���b�Z�[�W�{�b�N�X�A�L�����N�^�[
    [SerializeField] GameObject messageBox, charator1, charactor2;
    //�Â����
    [SerializeField] Image blackImage;
    
    [SerializeField] Color transColor;
    
    
    private GameObject messageNextTextUI;

    private TextMeshProUGUI messageName, messageText;

    private Image charactorImage1, charactorImage2;
    private Animator messageAnim, charactorAnim1, charactorAnim2;

    //���ۂɕ\������e�L�X�g�f�[�^
    [SerializeField] TextData textDatas;
    private TextData.data textData;
    private TextData.data prevTextData;
    Queue<TextData.data> textQue;

    //�A�j���[�V����
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
     * �X�^�[�g�֐�
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
     * �A�b�v�f�[�g�֐�
     * �@�E�{�^������������e�L�X�g�����ɐi�߂�
     * �@
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
     * �e�L�X�g�̉E���ɕ\�����Ă���O�p�`��\��
     */
    private void FinishTextShow()
    {
        isTextShowFinish = true;
        messageNextTextUI.SetActive(true);

        
    }

    /*
     * ���񂾂�e�L�X�g��\������R�[���`��
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
     * �L���[����e�L�X�g���X�V������A�L�����N�^�[�̕\����ύX�����肷��
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
     * �L�����N�^�[�𓧉߂������芮�S�ɕ\������
     *  �Eimg   : ���߂�����摜
     *  �Estate : �摜�̏�� 
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
     * �L�����N�^�[�̉摜�����ւ���
     *  �Eimg    �F ����ւ���摜
     *  �Esprite :�@�ύX��̉摜 
     */
    private void CharactorChange(Image img, Sprite sprite)
    {
        img.sprite = sprite;
    }

    /*
     *  �e�L�X�g�\���A�j���[�V�����Đ�
     */
    private void TextStartCoroutine()
    {
        textCoroutine = StartCoroutine(TextCoroutine());
    }

    /*
     * �V�[���I�����̏���
     */
    private void FinishScene()
    {
        FadeOut();
        messageBox.SetActive(false);
        Debug.Log("Scene Finish");
    }

    /*
     * �V�[���̈ړ�
     */

    private void SceneMove()
    {
        ButtleManager.IsTutorial = true;
        SceneManager.LoadScene("ButtleScene");
    }

    /*
     * �t�F�[�h�C���A�j���[�V����
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
     * �t�F�[�h�A�E�g�A�j���[�V����
     */
    private void FadeOut()
    {
        isAnimFinish = false;
        blackImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), fadeTime).OnComplete(() => { SceneMove(); });
    }




}
