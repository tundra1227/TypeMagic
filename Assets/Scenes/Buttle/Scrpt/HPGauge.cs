using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HPGauge : MonoBehaviour
{
    // Start is called before the first frame update

    [ExecuteAlways]
    [SerializeField, Tooltip("ダメージゲージ")]
    RectTransform damagedRect, HPGaugeRect;

    float gauge = 1.0f;

    Vector3 gaugeSize;

    public float Gauge
    {
        get { return gauge; }
        set { gauge = value; }

    }
    float interval = 1.0f;
    const float DAMAGEANIMDELAY = 1.5f;


    void Start()
    {
        gaugeSize = damagedRect.transform.localScale;
    }

    /*
     * ダメージゲージアップデート
     *  hp : 現在のhp
     */
    public void GaugeUpdate(float hp, float MAXHP)
    {
        Gauge = hp / MAXHP;

        DamageGaugeAnimation();
    }

    /*
     * 回復
     *  hp : 現在のhp
     */
    public void HealUpdate(float hp, float MAXHP)
    {
        DOTween.Kill(HPGaugeRect.transform);
        DOTween.Kill(damagedRect.transform);
        HPGaugeRect.DOScaleX(gauge, interval);
        damagedRect.DOScaleX(gauge, interval);

    }

    /*
     * ダメージゲージのアニメーションを再生する
     */
    private void DamageGaugeAnimation()
    {
        DOTween.Kill(HPGaugeRect.transform);
        DOTween.Kill(damagedRect.transform);
        HPGaugeRect.DOScaleX(gauge, interval);
        damagedRect.DOScaleX(gauge, interval).SetDelay(DAMAGEANIMDELAY);
    }

    /*
     * 初期化を行う
     */
    public void Init(float hp, float MAXHP)
    {
        float value = hp / MAXHP;
        gaugeSize = new Vector3 (value ,1.0f, 1.0f);
        damagedRect.localScale = gaugeSize;
        HPGaugeRect.localScale = gaugeSize;
    }

    /*
     * アニメーションを削除
     */
    public void OnDestroy()
    {
        DOTween.Kill(HPGaugeRect.transform);
        DOTween.Kill(damagedRect.transform);
    }
}
