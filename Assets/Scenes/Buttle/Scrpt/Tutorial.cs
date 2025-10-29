using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject pagePoint;
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] Color nowPage;
    [SerializeField] Color unNowPage;

    //ページ関連
    private int pageIndex = 0;      //現在表示しているページ
    private int prevPageIndex = 0;  //前に表示していたページ
    public int PageIndex
    {
        get { return pageIndex; } 
        set { pageIndex = value; }
    }

    //ポジション関連
    float y = -435.0f;              //ページ番号を表す〇のy座標
    float dx = 100.0f;              //ページ番号を表す〇と〇の距離の差
    float pageW = 0.0f;             //ページの横幅

    //アニメーション関連
    float pageMoveSpeed = 0.5f;     //ページがめくられる速さ
    float showSpeed = 0.7f;         //ページが表示されるまでの速さ
    Vector3 firstPos;               //ページが表示される座標
    Vector3 leftPos;                //ページがめくられた際に移動する座標
    Vector3 rightPos;               //ページがめくられる前の座標


    private List<GameObject> pageObj;
    private List<RectTransform> pageRectTransform;
    private List<GameObject> pagePointObj;
    private List<Image> pagePointImg;


    /*
     * スタート関数
     *　・ページ取得
     *　・ページを表す〇の生成
     *　・初期表示
     */
    private void Start()
    {
        pageObj = new List<GameObject>();
        pageRectTransform = new List<RectTransform>();
        pagePointObj = new List<GameObject>();
        pagePointImg = new List<Image>();


        Debug.Log($"CHILD COUNT : {transform.childCount}");
        Transform pages = transform.Find("AllPages");
        pageW = pages.GetComponent<RectTransform>().rect.width;
        for(int i = 0;i < pages.childCount; ++i)
        {
            GameObject page = pages.GetChild(i).gameObject;
            
            if (page.name.Contains("Page"))
            {

                pageObj.Add(page);
                pageRectTransform.Add(page.GetComponent<RectTransform>());
            }
        }

        firstPos = pageObj[0].transform.localPosition;
        leftPos = new Vector3(-pageW, firstPos.y);
        rightPos = new Vector3(pageW, firstPos.y);

        for(int i = 0; i < pageObj.Count; ++i)
        {
            pagePointObj.Add(Instantiate(pagePoint, this.gameObject.transform));
            pagePointImg.Add(pagePointObj[i].GetComponent<Image>());
            
        }
        Init();

    }


    /*
     * 初期化
     *  ページの移動など
     */
    private void Init()
    {
        SetPagePointPos();
        SetPagePos();
        

        leftButton.SetActive(false);
        for(int i = 0;i < pagePointObj.Count; ++i)
        {
            if (i == 0) pagePointImg[i].color = nowPage;
            else pagePointImg[i].color = unNowPage;
        }
        if(pagePointObj.Count == 1)
        {
            rightButton.SetActive(false);
        }
    }


    /*
     * アクティブになった際のアニメーション（大きくなる）
     */
    private void OnEnable()
    {
        this.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), showSpeed);
    }


    /*
     * ページを表す〇の表示
     */
    private void SetPagePointPos()
    {
        bool even = pagePointObj.Count % 2 == 0;
        int middle = pagePointObj.Count / 2;
        for(int i = 0;i < pagePointObj.Count; ++i)
        {
            if(even)
            {
                pagePointObj[i].transform.localPosition = new Vector3(dx * i - (middle -  0.5f) * dx, y, 0.0f);
            }
            else
            {
                pagePointObj[i].transform.localPosition = new Vector3(dx * (i-middle), y, 0.0f);
            }
        }
    }

    /*
     * 各ページの座標を初期値に動かす
     */
    private void SetPagePos()
    {
        for(int i = 0;i < pageObj.Count; ++i)
        {
            if(i == 0)
            {
                pageRectTransform[i].localPosition = firstPos;
            }
            else
            {
                Vector3 v = firstPos;
                v.x = pageW;
                pageRectTransform[i].localPosition = v;
            }
        }
    }

    /*
     * 今表示しているページの〇の色を変える
     */
    private void SetPagePointColor()
    {
        pagePointImg[prevPageIndex].color = unNowPage;
        pagePointImg[pageIndex].color = nowPage;
        
    }

    /*
     * 右矢印が押された際の処理
     */
    public void TurnRightPage()
    {
        prevPageIndex = pageIndex;
        pageIndex++;
        SetPagePointColor();

        for (int i = 0; i < pageRectTransform.Count; ++i) Debug.Log(pageRectTransform[i]);

        DOTween.Sequence()
            .Append(pageRectTransform[pageIndex].DOAnchorPos(firstPos, pageMoveSpeed))
            .Join(pageRectTransform[prevPageIndex].DOAnchorPos(leftPos, pageMoveSpeed));

        Debug.Log($"pageInd = {PageIndex}, prevPageInd = {prevPageIndex}");

        if(pageIndex == pagePointObj.Count-1)
        {
            rightButton.SetActive(false);
        }
        if(prevPageIndex == 0)
        {
            leftButton.SetActive(true);
        }
    }


    /*
     * 左矢印が押された際の処理
     */
    public void TurnLeftPage()
    {


        prevPageIndex = pageIndex;
        pageIndex--;
        SetPagePointColor();

        DOTween.Sequence()
            .Append(pageRectTransform[pageIndex].DOLocalMove(firstPos, pageMoveSpeed))
            .Join(pageRectTransform[prevPageIndex].DOLocalMove(rightPos, pageMoveSpeed));

        Debug.Log($"pageInd = {PageIndex}, prevPageInd = {prevPageIndex}");


        if (pageIndex == 0)
        {
            leftButton.SetActive(false);
        }
        if(prevPageIndex == pagePointObj.Count-1)
        {
            rightButton.SetActive(true);
        }

    }

    


}
