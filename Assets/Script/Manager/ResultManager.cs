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

    Vector3 firstScale;         //表示される前の大きさ
    Vector3 lastScale;          //表示後の大きさ
    float showSpeed = 0.3f;     //表示されるまでの時間



    private void Start()
    {

        firstScale = new Vector3(0.25f, 0.0f, 1.0f);
        resultObj.transform.localScale = firstScale;
        lastScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    /*
     * 結果を表示
     */
    public void ShowResult(bool isPlayerWin)
    {
        //System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        //Debug.Log(stackTrace.ToString());
        ButtleManager.Instance.gameObject.SetActive(false);

        resultObj.SetActive(true);
        if(isPlayerWin)
        {
            text.text = "勝利！！";
        }
        else
        {
            text.text = "敗北・・・";
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
     * 結果の表示
     * ・記録を表示
     */
    public void ShowResult(int wave)
    {
        ButtleManager.Instance.gameObject.SetActive(false);

        resultObj.SetActive(true);

        text.text = $"今回の記録　：　{wave} 回敵を退けた！";
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