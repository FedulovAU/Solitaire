using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;


public class GameScreen : MonoBehaviour {
    //normal
    /*
    private const float PRE_DEAL_DELAY = 1f;
    private const float FLIP_DURATION = 0.2f;
    private const float MOVE_DURATION = 0.05f;
    private const float PRE_MOVE_DELAY = 0.01f;
    private const float DEAL_DURATION = 0.2f;
    private const float SHIFT_DECK_DURATION = 0.1f;
    */

    //fast
    private const float PRE_DEAL_DELAY = 0.1f;
    private const float FLIP_DURATION = 0.01f;
    private const float MOVE_DURATION = 0.01f;
    private const float PRE_MOVE_DELAY = 0.01f;
    private const float DEAL_DURATION = 0.01f;
    private const float SHIFT_DECK_DURATION = 0.01f;

    [SerializeField]
    private RectTransform _cardGenerator;

    [SerializeField]
    private RectTransform _cardsSpreadContainer;

    [SerializeField]
    private RectTransform _playerDeckContainer;

    [SerializeField]
    private RectTransform _opponentDeckContainer;

    [SerializeField]
    private RectTransform _commonDeckContainer;

    [SerializeField]
    private GameController _controller;


    private List<Card> _playerCards;
    private List<Card> _opponentCards;
    private List<Card> _spreadCards;
    private List<Card> _commonDeckCards;

    private List<CardHolder> _playerCardHolders;
    private List<CardHolder> _opponentCardHolders;
    private List<CardHolder> _spreadCardHolders;
    private List<CardHolder> _commonDeckCardHolders;

    private Queue<Action> _commandsSequence;
    private Action _currentCommand;

    private GameObject _cubePrefab;
    private GameObject _cardPrefab;
    private GameObject _cardHolderPrefab;

    private RectTransform _canvasRectTransform;

    private bool _isStarted;
    public bool IsStarted
    {
        get
        {
            return _isStarted;
        }
    }

    public event Action Started;


    void Awake()
    {
        _cardPrefab = Resources.Load("Prefabs\\Card", typeof(GameObject)) as GameObject;
        _cardHolderPrefab = Resources.Load("Prefabs\\CardHolder", typeof(GameObject)) as GameObject;
        _canvasRectTransform = GetComponent<RectTransform>();

        _cubePrefab = Resources.Load("Prefabs\\Cube", typeof(GameObject)) as GameObject;
        _commandsSequence = new Queue<Action>();

    }

    // Use this for initialization
    void Start () {
        _isStarted = true;
        if (Started != null)
        {
            Started.Invoke();
        }
    }


    private void NextCommand()
    {
        if(_commandsSequence.Count > 0)
        {
            _currentCommand = _commandsSequence.Dequeue();
            _currentCommand();
        }
    }


    private void PrescaleTransform(RectTransform rectTransform)
    {
        var adjustedPosition = rectTransform.position;
        adjustedPosition.x /= _canvasRectTransform.localScale.x;
        adjustedPosition.y /= _canvasRectTransform.localScale.x;
        adjustedPosition.z /= _canvasRectTransform.localScale.x;

        var adjustedLocalScale = rectTransform.localScale;
        adjustedLocalScale.x /= rectTransform.localScale.x;
        adjustedLocalScale.y /= rectTransform.localScale.y;
        adjustedLocalScale.z /= rectTransform.localScale.z;

        rectTransform.position = adjustedPosition;
        rectTransform.localScale = adjustedLocalScale;
    }

    private int _holdersReady = 0;
    private void CheckHolderReady(CardHolder holder)
    {
        _holdersReady++;
        holder.OnReady -= CheckHolderReady;
        if (_holdersReady == GameConfig.SPREAD_SIZE + GameConfig.COMMON_DECK_SIZE 
            + GameConfig.PLAYER_DECK_SIZE * 2)
        {
            NextCommand();
        }
    }

  
    private void CreateCardHolders()
    {
        int i;
        _spreadCardHolders = new List<CardHolder>();
        _commonDeckCardHolders = new List<CardHolder>();
        _playerCardHolders = new List<CardHolder>();
        _opponentCardHolders = new List<CardHolder>();



        for (i=0; i< GameConfig.SPREAD_SIZE; i++)
        {
            var instance = GameObject.Instantiate(_cardHolderPrefab);
            CardHolder holder = instance.GetComponent<CardHolder>();

            holder.RectTransform.SetParent( _cardsSpreadContainer);
            holder.RectTransform.localScale = Vector3.one;

            holder.Visible = true;
            holder.OnReady += CheckHolderReady;
            _spreadCardHolders.Add(holder);
        }

        for (i = 0; i < GameConfig.COMMON_DECK_SIZE; i++)
        {
            var instance = GameObject.Instantiate(_cardHolderPrefab);
            CardHolder holder = instance.GetComponent<CardHolder>();

            holder.RectTransform.SetParent( _commonDeckContainer);
            holder.RectTransform.localScale = Vector3.one;

            holder.Visible = true;
            holder.OnReady += CheckHolderReady;
            _commonDeckCardHolders.Add(holder);
        }

        for (i = 0; i < GameConfig.PLAYER_DECK_SIZE; i++)
        {
            var instance = GameObject.Instantiate(_cardHolderPrefab);
            CardHolder holder = instance.GetComponent<CardHolder>();

            holder.RectTransform.SetParent( _playerDeckContainer);
            holder.RectTransform.localScale = Vector3.one;

            holder.Visible = true;
            holder.OnReady += CheckHolderReady;
            _playerCardHolders.Add(holder);
        }

        for (i = 0; i < GameConfig.PLAYER_DECK_SIZE; i++)
        {
            var instance = GameObject.Instantiate(_cardHolderPrefab);
            CardHolder holder = instance.GetComponent<CardHolder>();

            holder.RectTransform.SetParent( _opponentDeckContainer);
            holder.RectTransform.localScale = Vector3.one;

            holder.Visible = true;
            holder.OnReady += CheckHolderReady;
            _opponentCardHolders.Add(holder);
        }
    }

    public void FillSpreadDeck(List<CardVo> cards)
    {
        CreateCardHolders();

        _playerCards = new List<Card>();
        _spreadCards = new List<Card>();
        _opponentCards = new List<Card>();
        _commonDeckCards = new List<Card>();

        _commandsSequence.Enqueue(() =>
        {
            
            Sequence dealingSeq = DOTween.Sequence();
            dealingSeq.OnComplete(() => {
                NextCommand();
            });
            

            for (int i = 0; i < cards.Count; i++)
            {
                var cardInstance = GameObject.Instantiate(_cardPrefab);
                Card card = cardInstance.GetComponent<Card>();

                card.RectTransform.SetParent( _canvasRectTransform);
                card.RectTransform.position = _cardGenerator.position;
                card.RectTransform.localScale = new Vector3(-1, 1, 1);
                card.setCard(cards[i]);
                
                _spreadCards.Add(card);
            }

            for (int i = _spreadCards.Count - 1; i >= 0; i--) 
            {
                var card = _spreadCards[i];
                var holder = GetEmptyHolder(_spreadCardHolders, true);

                if(holder != null)
                {
                    holder.AttachCard(card.CardId);

                    
                    var flipShirt = card.RectTransform.DOScaleX(0, FLIP_DURATION / 2).OnComplete(() =>
                    {
                        card.removeShirt();
                    });

                    var flipNoShirt = card.RectTransform.DOScaleX(1, FLIP_DURATION / 2);
                    var moveCard = card.RectTransform.
                    DOMove(holder.RectTransform.position, MOVE_DURATION * (i+1)).
                    SetDelay(PRE_MOVE_DELAY);

                    dealingSeq.Append(flipShirt);
                    dealingSeq.Append(flipNoShirt);
                    dealingSeq.Append(moveCard);
                    

                }
                
            }
        });
        
    }


    public void FillCommonDeck(List<CardVo> cards)
    {
        _commandsSequence.Enqueue(() =>
        {
            Sequence dealingSeq = DOTween.Sequence();
            dealingSeq.OnComplete(() => {
                NextCommand();
            });

            for (int i = 0; i < cards.Count; i++)
            {
                var cardInstance = GameObject.Instantiate(_cardPrefab);
                Card card = cardInstance.GetComponent<Card>();

                card.RectTransform.SetParent(_canvasRectTransform);
                card.RectTransform.position = _cardGenerator.position;
                card.RectTransform.localScale = new Vector3(-1, 1, 1);
                card.setCard(cards[i]);

                _commonDeckCards.Add(card);
            }

            for (int i = _commonDeckCards.Count - 1; i >= 0; i--)
            {
                var card = _commonDeckCards[i];
                var holder = GetEmptyHolder(_commonDeckCardHolders, true);

                if (holder != null)
                {
                    holder.AttachCard(card.CardId);

                    var flipShirt = card.RectTransform.DOScaleX(0, FLIP_DURATION / 2).OnComplete(() =>
                    {
                        card.removeShirt();
                    });

                    var flipNoShirt = card.RectTransform.DOScaleX(1, FLIP_DURATION / 2);
                    var moveCard = card.RectTransform.
                    DOMove(holder.RectTransform.position, MOVE_DURATION * (i + 1)).
                    SetDelay(PRE_MOVE_DELAY);

                    dealingSeq.Append(flipShirt);
                    dealingSeq.Append(flipNoShirt);
                    dealingSeq.Append(moveCard);

                }

            }
        });
    }

    public void UpdateSpreadDeck(CardVo cardVo)
    {
        _commandsSequence.Enqueue(() =>
        {
            var delay = 0f;
            for (int i = _spreadCards.Count-1; i >= 0; i--)
            {
                var holderToMove = GetEmptyHolder(_spreadCardHolders);
                var holderFromMove = FindCardHolderByCardId(_spreadCardHolders, _spreadCards[i].CardId);

                if (holderToMove != null)
                {
                    Card cardToMove = _spreadCards[i];
                    var movingTween = cardToMove.RectTransform.DOMove(holderToMove.RectTransform.position, SHIFT_DECK_DURATION);
                    movingTween.SetDelay(delay);
                    delay += 0.05f;

                    holderToMove.AttachCard(cardToMove.CardId);

                    if (i == 0)
                    {
                        movingTween.OnComplete(() =>
                        {
                            OnUpdateDeckShiftCardsComplete(cardVo);
                        });
                    }

                    if (holderFromMove != null)
                    {
                        holderFromMove.DetachCard();
                    } else
                    {
                        Debug.LogError("UpdateSpreadDeck: holderFromMove is null for card Id: " + _spreadCards[i].CardId);
                    }


                }
                else
                {
                    Debug.LogError("UpdateSpreadDeck: holderToMove is null");
                }

            }
        });
    }

    private void OnUpdateDeckShiftCardsComplete(CardVo cardVo)
    {
        var cardInstance = GameObject.Instantiate(_cardPrefab);
        Card card = cardInstance.GetComponent<Card>();

        card.RectTransform.SetParent(_canvasRectTransform);
        card.RectTransform.position = _cardGenerator.position;
        card.RectTransform.localScale = new Vector3(-1, 1, 1);
        card.setCard(cardVo);
        _spreadCards.Insert(0, card);
        var holderToMove = GetEmptyHolder(_spreadCardHolders);

        Sequence inserDeckSequence = DOTween.Sequence();
        inserDeckSequence.OnComplete(() => {
            NextCommand();
        });

        holderToMove.AttachCard(card.CardId);

        var flipShirt = card.RectTransform.DOScaleX(0, FLIP_DURATION / 2).OnComplete(() =>
        {
            card.removeShirt();
        });

        var flipNoShirt = card.RectTransform.DOScaleX(1, FLIP_DURATION / 2);
        var moveCard = card.RectTransform.
        DOMove(holderToMove.RectTransform.position, MOVE_DURATION).
        SetDelay(PRE_MOVE_DELAY);

        inserDeckSequence.Append(flipShirt);
        inserDeckSequence.Append(flipNoShirt);
        inserDeckSequence.Append(moveCard);
    }


    private void OnUpdateCommonDeckShiftCardsComplete(CardVo cardVo)
    {
        var cardInstance = GameObject.Instantiate(_cardPrefab);
        Card card = cardInstance.GetComponent<Card>();

        card.RectTransform.SetParent(_canvasRectTransform);
        card.RectTransform.position = _cardGenerator.position;
        card.RectTransform.localScale = new Vector3(-1, 1, 1);
        card.setCard(cardVo);
        _commonDeckCards.Insert(0, card);
        var holderToMove = GetEmptyHolder(_commonDeckCardHolders);

        Sequence inserDeckSequence = DOTween.Sequence();
        inserDeckSequence.OnComplete(() => {
            NextCommand();
        });

        holderToMove.AttachCard(card.CardId);

        var flipShirt = card.RectTransform.DOScaleX(0, FLIP_DURATION / 2).OnComplete(() =>
        {
            card.removeShirt();
        });

        var flipNoShirt = card.RectTransform.DOScaleX(1, FLIP_DURATION / 2);
        var moveCard = card.RectTransform.
        DOMove(holderToMove.RectTransform.position, MOVE_DURATION).
        SetDelay(PRE_MOVE_DELAY);

        inserDeckSequence.Append(flipShirt);
        inserDeckSequence.Append(flipNoShirt);
        inserDeckSequence.Append(moveCard);
    }

    public void UpdateCommonDeck(CardVo cardVo)
    {
        _commandsSequence.Enqueue(() =>
        {
            var delay = 0f;
            for (int i = _commonDeckCards.Count - 1; i >= 0; i--)
            {
                var holderToMove = GetEmptyHolder(_commonDeckCardHolders);
                var holderFromMove = FindCardHolderByCardId(_commonDeckCardHolders, _commonDeckCards[i].CardId);

                if (holderToMove != null)
                {
                    Card cardToMove = _commonDeckCards[i];
                    var movingTween = cardToMove.RectTransform.DOMove(holderToMove.RectTransform.position, 0.1f);
                    movingTween.SetDelay(delay);
                    delay += 0.05f;

                    holderToMove.AttachCard(cardToMove.CardId);

                    if (i == 0)
                    {
                        movingTween.OnComplete(() =>
                        {
                            OnUpdateCommonDeckShiftCardsComplete(cardVo);
                        });
                    }

                    if (holderFromMove != null)
                    {
                        holderFromMove.DetachCard();
                    }
                    else
                    {
                        Debug.LogError("UpdateSpreadDeck: holderFromMove is null for card Id: " + _commonDeckCards[i].CardId);
                    }


                }
                else
                {
                    Debug.LogError("UpdateSpreadDeck: holderToMove is null");
                }

            }
        });
    }

    public void DealCardToPlayer(int spreadCardId, int playerCardId, GameModel.Turn turn)
    {
        _commandsSequence.Enqueue(() =>
        {
            Debug.Log("DealCardToPlayer animation targetPlayer " + turn);

            var cardToMove = FindCardById(_spreadCards, spreadCardId);
            var holderMoveFrom = FindCardHolderByCardId(_spreadCardHolders, spreadCardId);
            if(holderMoveFrom != null)
            {
                holderMoveFrom.DetachCard();
            } else
            {
                Debug.LogError("cardHolder is null for spread card id: "+ spreadCardId);
            }
            

            CardHolder holderMoveTo;
            var currenPlayerCardHolders = turn == GameModel.Turn.PLAYER ? _playerCardHolders : _opponentCardHolders;
            var currenPlayerCards = turn == GameModel.Turn.PLAYER ? _playerCards : _opponentCards;

            if (playerCardId == -1)
            {
                holderMoveTo =  GetEmptyHolder(currenPlayerCardHolders);
            } else
            {
                holderMoveTo = FindCardHolderByCardId(currenPlayerCardHolders, playerCardId);
            }

            if(holderMoveTo != null && cardToMove != null)
            {
                holderMoveTo.AttachCard(spreadCardId);
                currenPlayerCards.Add(cardToMove);
                _spreadCards.Remove(cardToMove);

                cardToMove.RectTransform.DOMove(holderMoveTo.RectTransform.position, DEAL_DURATION).
                OnComplete(()=> {
                    NextCommand();
                });

            } else 
            {
                if(holderMoveTo == null)
                {
                    Debug.LogError("holderToMove is null for player card id: " + playerCardId);
                }

                if (cardToMove == null)
                {
                    Debug.LogError("cardToMove is null for spread card id: " + spreadCardId);
                }
            }

        });
    }


    public void MoveCardFromPlayerToCommonDeck(int playerCardId, int commonDeckCardId)
    {
        _commandsSequence.Enqueue(() =>
        {

        });
    }

    public void MoveCardFromOpponentToCommonDeck(int playerCardId, int commonDeckCardId)
    {
        _commandsSequence.Enqueue(() =>
        {

        });
    }

    public void MatchedPlayerCards(Dictionary<GameConfig.MatchType, List<CardVo>> cardsByMatchingType, GameModel.Turn turn)
    {
        _commandsSequence.Enqueue(() =>
        {
            Sequence sequence = DOTween.Sequence();

            var currenPlayerCardHolders = turn == GameModel.Turn.PLAYER ? _playerCardHolders : _opponentCardHolders;
            var currenPlayerCards = turn == GameModel.Turn.PLAYER ? _playerCards : _opponentCards;
            List<Card> cardsToRemove = new List<Card>();

            foreach (GameConfig.MatchType matchingType in cardsByMatchingType.Keys)
            {
                var cardsList = cardsByMatchingType[matchingType];

                Sequence scaleUpSequence = DOTween.Sequence();
                Sequence scaleDownSequence = DOTween.Sequence();

                foreach (CardVo cardVo in cardsList)
                {
                    var cardToAnimate = FindCardById(currenPlayerCards, cardVo.id);

                    if (cardToAnimate)
                    {
                        if (cardsToRemove.IndexOf(cardToAnimate) == -1)
                        {
                            cardsToRemove.Add(cardToAnimate);
                        }

                        scaleUpSequence.Join(cardToAnimate.RectTransform.DOScale(new Vector3(1.1f, 1.5f, 1), 0.3f));
                        scaleDownSequence.Join(cardToAnimate.RectTransform.DOScale(Vector3.one, 0.2f));
                    } else
                    {
                        Debug.LogError("cand find card for id " + cardVo.id + "  for target " + turn);
                    }

                }

                sequence.Append(scaleUpSequence);
                sequence.Append(scaleDownSequence);
            }

            Sequence scaleToZeroSequence = DOTween.Sequence();
            foreach (Card card in cardsToRemove)
            {
                scaleToZeroSequence.Join(card.RectTransform.DOScale(new Vector3(0f, 0f, 1), 0.2f).OnComplete(()=>
                {
                    GameObject.Destroy(card);
                }));   
            }

            sequence.Append(scaleToZeroSequence);
            sequence.OnComplete(() =>
            {
                NextCommand();
            });

        });
    }


    public void ChangeTurn(GameModel.Turn turn)
    {
        _commandsSequence.Enqueue(() =>
        {

        });
    }


    private CardHolder GetEmptyHolder(List<CardHolder> holders, bool isNearest = false)
    {
        if (isNearest)
        {
            for (int i = 0; i < holders.Count; i++)
            {
                if(holders[i].AttachedCard == -1)
                {
                    return holders[i];
                }
            }
        } else
        {
            for (int i = holders.Count - 1; i >= 0 ; i--)
            {
                if (holders[i].AttachedCard == -1)
                {
                    return holders[i];
                }
            }
        }

        return null;
        
    }

    private CardHolder FindCardHolderByCardId(List<CardHolder> holders, int cardId)
    {

        for (int i = 0; i < holders.Count; i++)
        {
            if (holders[i].AttachedCard == cardId)
            {
                return holders[i];
            }
        }


        return null;

    }

    private Card FindCardById(List<Card> cards, int cardId)
    {
        
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].CardId == cardId)
            {
                return cards[i];
            }
        }
        

        return null;

    }
}
