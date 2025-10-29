
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] State state;


    //�ŏ��̑傫��
    Vector3 firstScale;
    //�I������Ă���Ƃ��ɉ��{�傫�����邩
    float zoomSize = 1.2f;
    //���b�����邩
    float zoomTime = 0.3f;

    //�T�C�Y��ύX����A�j���[�V����
    Sequence changeSize;

    private void Start()
    {
        firstScale = transform.localScale;

    }

    //�Q�[���̏�Ԃ�\��
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
     * �|�C���^�[���d�Ȃ�Ƒ傫���Ȃ�
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        DOTween.Kill(changeSize);
        changeSize = DOTween.Sequence().Append(transform.DOScale(firstScale * zoomSize, zoomTime)).SetLink(gameObject);
    }
    
    /*
     * �|�C���^�[�������ƌ��̑傫���ɖ߂�
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(changeSize);
        changeSize = DOTween.Sequence().Append(transform.DOScale(firstScale, zoomTime)).SetLink(gameObject);
    }


    /*
     * �N���b�N���ꂽ�ہA���g�������Ă���state�̍s��������
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
