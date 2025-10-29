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

    //�y�[�W�֘A
    private int pageIndex = 0;      //���ݕ\�����Ă���y�[�W
    private int prevPageIndex = 0;  //�O�ɕ\�����Ă����y�[�W
    public int PageIndex
    {
        get { return pageIndex; } 
        set { pageIndex = value; }
    }

    //�|�W�V�����֘A
    float y = -435.0f;              //�y�[�W�ԍ���\���Z��y���W
    float dx = 100.0f;              //�y�[�W�ԍ���\���Z�ƁZ�̋����̍�
    float pageW = 0.0f;             //�y�[�W�̉���

    //�A�j���[�V�����֘A
    float pageMoveSpeed = 0.5f;     //�y�[�W���߂����鑬��
    float showSpeed = 0.7f;         //�y�[�W���\�������܂ł̑���
    Vector3 firstPos;               //�y�[�W���\���������W
    Vector3 leftPos;                //�y�[�W���߂���ꂽ�ۂɈړ�������W
    Vector3 rightPos;               //�y�[�W���߂�����O�̍��W


    private List<GameObject> pageObj;
    private List<RectTransform> pageRectTransform;
    private List<GameObject> pagePointObj;
    private List<Image> pagePointImg;


    /*
     * �X�^�[�g�֐�
     *�@�E�y�[�W�擾
     *�@�E�y�[�W��\���Z�̐���
     *�@�E�����\��
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
     * ������
     *  �y�[�W�̈ړ��Ȃ�
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
     * �A�N�e�B�u�ɂȂ����ۂ̃A�j���[�V�����i�傫���Ȃ�j
     */
    private void OnEnable()
    {
        this.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), showSpeed);
    }


    /*
     * �y�[�W��\���Z�̕\��
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
     * �e�y�[�W�̍��W�������l�ɓ�����
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
     * ���\�����Ă���y�[�W�́Z�̐F��ς���
     */
    private void SetPagePointColor()
    {
        pagePointImg[prevPageIndex].color = unNowPage;
        pagePointImg[pageIndex].color = nowPage;
        
    }

    /*
     * �E��󂪉����ꂽ�ۂ̏���
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
     * ����󂪉����ꂽ�ۂ̏���
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
