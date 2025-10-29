using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMark : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] Tutorial tutorial;

    private Image img;
    private float alpha = 0.7f;
    private float fadeTime = 0.5f;

    [SerializeField] string s;
    void Start()
    {
        img = GetComponent<Image>();
    }



    /*
     * �}�E�X�����ƐF����������
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        img.DOFade(alpha, fadeTime);
    }

    /*
     * �}�E�X�������ƌ��̐F�ɖ߂�
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        img.DOFade(1.0f, fadeTime);
    }

    /*
     * �N���b�N���ꂽ��`���[�g���A����ʂ�����
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        tutorial.gameObject.SetActive(false);
        ButtleManager.Instance.IsPause = false;
        if (s == "Title") SceneManager.LoadScene(s);
    }

}
