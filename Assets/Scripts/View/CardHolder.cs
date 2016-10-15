using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardHolder : MonoBehaviour {

    public event Action<CardHolder> OnReady;

    [SerializeField]
    private int _attachedCardId = -1;
    private RectTransform _rectTransform;

    [SerializeField]
    private bool hideOnStart = true;


    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    // Use this for initialization
    void Start () {
        Visible = !hideOnStart;
	}

    // Update is called once per frame
    private bool _dispatched = false;
	void Update () {
        if (!_dispatched)
        {
            if (OnReady != null && (transform.position.x != 0) && (transform.position.y != 0))
            {
                OnReady.Invoke(this);
                _dispatched = true;
            }
        }
	}

    public void DetachCard()
    {
        _attachedCardId = -1;
    }

    public void AttachCard(int cardId)
    {
        _attachedCardId = cardId;
    }

    public int AttachedCard
    {
        get
        {
            return _attachedCardId;
        }
         
    }

    public RectTransform RectTransform
    {
        get
        {
            return _rectTransform;
        }
    }


    public bool Visible
    {
        set
        {
            hideOnStart = !value;
            if (value)
            {
                var prevColor = (GetComponent<Image>() as Image).color;
                (GetComponent<Image>() as Image).color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.5f);
            } else
            {
                var prevColor = (GetComponent<Image>() as Image).color;
                (GetComponent<Image>() as Image).color = new Color(prevColor.r, prevColor.g, prevColor.b, 0);
            }
        }
    }
}
