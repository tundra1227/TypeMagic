
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] State state;


    //最初の大きさ
    Vector3 firstScale;
    //選択されているときに何倍大きくするか
    float zoomSize = 1.2f;
    //何秒かけるか
    float zoomTime = 0.3f;

    //サイズを変更するアニメーション
    Sequence changeSize;

    private void Start()
    {
        firstScale = transform.localScale;

    }

    //ゲームの状態を表す
    enum State
    {
        tutorial,
        play,
        quit,
        again,
        title,
        resume
    }

    /*
     * ポインターが重なると大きくなる
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill(changeSize);
        changeSize = DOTween.Sequence().Append(transform.DOScale(firstScale * zoomSize, zoomTime)).SetLink(gameObject);
    }
    
    /*
     * ポインターが離れると元の大きさに戻る
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(changeSize);
        changeSize = DOTween.Sequence().Append(transform.DOScale(firstScale, zoomTime)).SetLink(gameObject);
    }


    /*
     * クリックされた際、自身が持っているstateの行動をする
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        DOTween.KillAll();

        switch(state)
        {
            case State.tutorial:
                SceneManager.LoadScene("TextEvent");
                break;
            case State.play:
                ButtleManager.IsTutorial = false;
                SceneManager.LoadScene("ButtleScene");
                break;
            case State.quit:
                Application.Quit();
                break;
            case State.again:
                Scene current = SceneManager.GetActiveScene();
                SceneManager.LoadScene(current.name);
                break;
            case State.title:
                SceneManager.LoadScene("Title");
                break;
            case State.resume:
                ButtleManager.Instance.IsPause = false;
                this.gameObject.SetActive(false);
                ButtleManager.Instance.ResumeSpell();
                break;
        }
    }
    

    
}
