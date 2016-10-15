using UnityEngine;
using System.Collections;
using System;
using DigitalRuby.Tween;
using UnityEngine.UI;


public class Card : MonoBehaviour
{

    private CardVo _cardVo;
    public int CardId
    {
        get
        {
            return _cardVo != null ? _cardVo.id : -1;
        }
    }

    [SerializeField]
    private RectTransform rankSmallHolder;

    [SerializeField]
    private RectTransform rankBigHolder;

    [SerializeField]
    private RectTransform suitHolder;

    [SerializeField]
    private RectTransform cardShirt;

    private Image shirtImage;
    private RectTransform _rectTransform;

    [SerializeField]
    private int _cardId;


    void Awake()
    {
        shirtImage = cardShirt.GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform> ();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public RectTransform RectTransform
    {
        get
        {

            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    public void setCard(CardVo data)
    {
        _cardVo = data;
        _cardId = _cardVo.id;
        shirtImage.enabled = _cardVo.shirtVisible;
        UpdateCard();
    }


   

    public void removeShirt(Action OnFinished = null)
    {
        _cardVo.RemoveShirt();
        shirtImage.enabled = _cardVo.shirtVisible;

    }

    public void UpdateCard()
    {
        applyRankSmallSprite();
        applyRankBigSprite();
        applySuitSprite();

        if (shirtImage != null)
        {
            shirtImage.enabled = _cardVo.shirtVisible;
        }
        else
        {
            shirtImage = cardShirt.GetComponent<Image>();
            shirtImage.enabled = _cardVo.shirtVisible;
        }
        
    }

    private void applyRankSmallSprite()
    {
        Sprite sprite = Resources.Load(_cardVo.CardValueSmallAssetName, typeof(Sprite)) as Sprite;

        var image = rankSmallHolder.GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }

    private void applyRankBigSprite()
    {
        Sprite sprite = Resources.Load(_cardVo.CardValueBigAssetName, typeof(Sprite)) as Sprite;

        var image = rankBigHolder.GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }

    private void applySuitSprite()
    {
        Sprite sprite = Resources.Load(_cardVo.CardSuitAssetName, typeof(Sprite)) as Sprite;

        var image = suitHolder.GetComponent<Image>();
        image.sprite = sprite;
        image.SetNativeSize();
    }
}