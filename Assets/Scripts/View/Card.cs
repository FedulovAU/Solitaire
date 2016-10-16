using UnityEngine;
using System.Collections;
using System;
using DigitalRuby.Tween;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class Card : MonoBehaviour
{
    public event Action<CardVo> OnCardClick;

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

    [SerializeField]
    private Image _purpleGlow;

    [SerializeField]
    private Image _greenGlow;

    private EventTrigger _eventTrigger;


    void Awake()
    {
        _purpleGlow.enabled = false;
        _greenGlow.enabled = false;

        shirtImage = cardShirt.GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform> ();

        _eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener( (data) =>
        {
            Debug.Log("tap on card id: " + _cardVo.id);
            if(OnCardClick!= null)
            {
                OnCardClick.Invoke(_cardVo);
            }
        });

        _eventTrigger.triggers.Add(entry);

    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableGreenGlow(bool isEnabled)
    {
        _greenGlow.enabled = isEnabled;
    }

    public void EnablePurpleGlow(bool isEnabled)
    {
        _purpleGlow.enabled = isEnabled;
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