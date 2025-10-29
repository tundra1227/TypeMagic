using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ResultManager : MonoBehaviour
{
    static ResultManager insntance;
    public static ResultManager Instance
    {
        get { return insntance; } 
    }

    private void Awake()
    {
        if (insntance != null) return;
        insntance = this;
    }

   


    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject resultObj;
    [SerializeField] GameObject tutorial;
    [SerializeField] GameObject menu;

    Vector3 firstScale;         //�\�������O�̑傫��
    Vector3 lastScale;          //�\����̑傫��
    float showSpeed = 0.3f;     //�\�������܂ł̎���



    private void Start()
    {

        firstScale = new Vector3(0.25f, 0.0f, 1.0f);
        resultObj.transform.localScale = firstScale;
        lastScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    /*
     * ���ʂ�\��
     */
    public void ShowResult(bool isPlayerWin)
    {
        //System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        //Debug.Log(stackTrace.ToString());
        ButtleManager.Instance.gameObject.SetActive(false);

        resultObj.SetActive(true);
        if(isPlayerWin)
        {
            text.text = "�����I�I";
        }
        else
        {
            text.text = "�s�k�E�E�E";
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(resultObj.transform.DOScaleY(lastScale.y, showSpeed)).
                Append(resultObj.transform.DOScaleX(lastScale.x, showSpeed))
                .OnComplete( () =>
                {
                    Debug.Log("ShowResult finish");
                    tutorial.SetActive(true);
                });

        
    }

    /*
     * ���ʂ̕\��
     * �E�L�^��\��
     */
    public void ShowResult(int wave)
    {
        ButtleManager.Instance.gameObject.SetActive(false);

        resultObj.SetActive(true);

        text.text = $"����̋L�^�@�F�@{wave} ��G��ނ����I";
        text.fontSize = 16;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(resultObj.transform.DOScaleY(lastScale.y, showSpeed)).
                Append(resultObj.transform.DOScaleX(lastScale.x, showSpeed))
                .OnComplete(() =>
                {
                    Debug.Log("ShowResult finish");
                });

        menu.SetActive(true);
    }
    

    
}