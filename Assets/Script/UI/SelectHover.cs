using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class SelectHover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    // Start is called before the first frame update

    [SerializeField] direction dir;
    [SerializeField] Tutorial tutorial;
    private Image img;
    Sequence HoverSequence, LandSequence;

    private float fadeTime = 0.25f;
    private float fadeValue = 0.7f;

    enum direction
    {
        left,
        right
    }

    private void Start()
    {
        img = GetComponent<Image>();

    }

    /*
     * ������
     */
    private void OnEnable()
    {
        if (img == null) return;
        Color c = img.color;
        c.a = 1.0f;
        img.color = c;
    }

    /*
     * �J�[�\�������Ə������߂���
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        img.DOFade(fadeValue, fadeTime);
        
    }


    /*
     * �J�[�\���������ƐF�����ɖ߂�
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        img.DOFade(1.0f, fadeTime);
    }

    /*
     * �N���b�N�����ۂ̏���
     *  dir��left�Ȃ�y�[�W��߂�
     *  dir��right�Ȃ�y�[�W��i�߂�
     */
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(dir)
        {
            case direction.left:
                tutorial.TurnLeftPage();
                break;
            case direction.right:
                tutorial.TurnRightPage();
                break;

        }
    }

    
}
