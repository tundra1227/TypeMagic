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
     * 初期化
     */
    private void OnEnable()
    {
        if (img == null) return;
        Color c = img.color;
        c.a = 1.0f;
        img.color = c;
    }

    /*
     * カーソルが乗ると少し透過する
     */
    public void OnPointerEnter(PointerEventData eventData)
    {
        img.DOFade(fadeValue, fadeTime);
        
    }


    /*
     * カーソルが離れると色が元に戻る
     */
    public void OnPointerExit(PointerEventData eventData)
    {
        img.DOFade(1.0f, fadeTime);
    }

    /*
     * クリックした際の処理
     *  dirがleftならページを戻す
     *  dirがrightならページを進める
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
