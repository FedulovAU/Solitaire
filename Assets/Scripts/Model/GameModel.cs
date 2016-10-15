using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameModel {
    
    public enum Turn { PLAYER, OPPONENT};
    

    public event Action<List<CardVo>> FillSpreadDeck;
    public event Action<List<CardVo>> FillCommonDeck;

    public event Action<CardVo> UpdateSpreadDeck;
    public event Action<CardVo> UpdateCommonDeck;

    public event Action<int, int, Turn> DealCardToPlayer;
    public event Action<int, int, Turn> MoveCardFromPlayerToCommonDeck;

    public event Action<Dictionary<GameConfig.MatchType, List<CardVo>>, Turn> MatchedPlayerCards;

    public event Action<Turn> ChangeTurn;



    private List<CardVo> _playerDeck;
    private List<CardVo> _opponentDeck;
    private List<CardVo> _spreadDeck;
    private List<CardVo> _commonDeck;

    private bool _isGameStarted = false;
    private Turn _currentTurn;

    private static int lastCardId = 0;

    public void StartGame()
    {
        Debug.Log("start game");

        if (_isGameStarted)
        {
            Debug.LogWarning("game already started");
            return;
        }

        _isGameStarted = true;
        _opponentDeck = new List<CardVo>();
        _playerDeck = new List<CardVo>();

        CreateSpread();
        CreateCommonDeck();
        DealCardsToPlayer(Turn.PLAYER);
        DealCardsToPlayer(Turn.OPPONENT);

        PerformMatchingAndDealingSequence(Turn.PLAYER);

        SwitchTurn();
    }

    private void SwitchTurn()
    {
        if(_currentTurn == Turn.PLAYER)
        {
            _currentTurn = Turn.OPPONENT;
        } else
        {
            _currentTurn = Turn.PLAYER;
        }

        ChangeTurn.Invoke(_currentTurn);
    }


    private void CreateSpread()
    {
        Debug.Log("create spread");

        _spreadDeck = new List<CardVo>();

        for (int i = 0; i < GameConfig.SPREAD_SIZE; i++)
        {
            var card = CreateCard();
            _spreadDeck.Add(card);
        }

        /*
        CardVo test;

        test = new CardVo(CardVo.CardSuit.clubs, CardVo.CardRank.queen, true, lastCardId++);
        _spreadDeck.Add(test);

        test = new CardVo(CardVo.CardSuit.clubs, CardVo.CardRank.ace, true, lastCardId++);
        _spreadDeck.Add(test);

        test = new CardVo(CardVo.CardSuit.clubs, CardVo.CardRank.ace, true, lastCardId++);
        _spreadDeck.Add(test);

        test = new CardVo(CardVo.CardSuit.spades, CardVo.CardRank.ace, true, lastCardId++);
        _spreadDeck.Add(test);
        */
        

        FillSpreadDeck.Invoke(CloneList(_spreadDeck));
    }

    private void CreateCommonDeck()
    {
        Debug.Log("create common deck");

        _commonDeck = new List<CardVo>();
        for (int i = 0; i < GameConfig.COMMON_DECK_SIZE; i++)
        {
            var card = CreateCard();
            _commonDeck.Add(card);
        }

        FillCommonDeck.Invoke(CloneList(_commonDeck));
    }


    private void DealCardsToPlayer(Turn target)
    {
        Debug.Log("deal cards to target player + " + target);

        List<CardVo> currentPlayerDeck = target == Turn.PLAYER ? _playerDeck : _opponentDeck;

        for (int i = 0; i < GameConfig.PLAYER_DECK_SIZE; i++)
        {
            var cardToDeal = _spreadDeck[_spreadDeck.Count - 1];
            _spreadDeck.RemoveAt(_spreadDeck.Count - 1);
            currentPlayerDeck.Add(cardToDeal);

            Debug.Log("DealCardToPlayer: card to deal id: " + cardToDeal.id + "  target card id: " + -1);
            DealCardToPlayer.Invoke(cardToDeal.id, -1, target);

            AddCardToSpread();
        }
    }

    private void UpdatePlayerCards(Turn target, List<CardVo> cardsToDelete)
    {
        Debug.Log("deal cards to target player + " + target);

        List<CardVo> currentPlayerDeck = target == Turn.PLAYER ? _playerDeck : _opponentDeck;

        for (int i = 0; i < currentPlayerDeck.Count; i++)
        {
            if(cardsToDelete.IndexOf(currentPlayerDeck[i]) != -1)
            {
                var cardToDeal = _spreadDeck[_spreadDeck.Count - 1];
                _spreadDeck.RemoveAt(_spreadDeck.Count - 1);

                var cardToReplace = currentPlayerDeck[i];
                currentPlayerDeck[i] = cardToDeal;

                Debug.Log("UpdatePlayerCards: card to deal id: " + cardToDeal.id +
                    "  target card to replace id: " + cardToReplace.id);

                DealCardToPlayer.Invoke(cardToDeal.id, cardToReplace.id, target);

                AddCardToSpread();
            }
        }
    }

    private void AddCardToSpread()
    {
        var card = CreateCard();
        _spreadDeck.Insert(0, card);

        Debug.Log("add card to spread, new cardId: " + card.id);

        UpdateSpreadDeck.Invoke(card.Clone());
    }

    private void AddCardToCommonDeck()
    {
        Debug.Log("add card to common deck");

        var card = CreateCard();
        _spreadDeck.Insert(0, card);

        UpdateCommonDeck.Invoke(card.Clone());
    }

    private void PerformMatchingAndDealingSequence(Turn targetTurn)
    {
        List<CardVo> cardsToDelete = CheckForPlayerMatchingCards(targetTurn);

        var safeCounter = 100;
        while(cardsToDelete.Count > 0 && safeCounter > 0)
        {
            safeCounter--;
          UpdatePlayerCards(targetTurn, cardsToDelete);
            cardsToDelete = CheckForPlayerMatchingCards(targetTurn);
        }
    }

    private List<CardVo> CheckForPlayerMatchingCards(Turn targetTurn)
    {
        Debug.Log("CheckForPlayerMatchingCards target player "+ targetTurn);

        List<CardVo> currentPlayerDeck = targetTurn == Turn.PLAYER ? _playerDeck : _opponentDeck;

        List<CardVo> cardsToRemove = new List<CardVo>();
        List<CardVo> matchedCards = new List<CardVo>();
        List<CardVo> allMatchedCards = new List<CardVo>();
        Dictionary<GameConfig.MatchType, List<CardVo>> matchedCardsByMatchType =
            new Dictionary<GameConfig.MatchType, List<CardVo>>();


        matchedCards = MatchCardsByRank(currentPlayerDeck);

        if (matchedCards.Count >= GameConfig.CARDS_AMOUNT_TO_MATCH)
        {
            Debug.Log("Cards matched by rank amount: " + matchedCards.Count);
            allMatchedCards.AddRange(matchedCards);
            matchedCardsByMatchType.Add(GameConfig.MatchType.BY_RANK, CloneList(matchedCards));
        }

        matchedCards = MatchCardsByColor(currentPlayerDeck);
        if (matchedCards.Count >= GameConfig.CARDS_AMOUNT_TO_MATCH)
        {
            Debug.Log("Cards matched by color amount: " + matchedCards.Count);
            allMatchedCards.AddRange(matchedCards);
            matchedCardsByMatchType.Add(GameConfig.MatchType.BY_COLOR, CloneList(matchedCards));
        }

        matchedCards = MatchCardsBySuit(currentPlayerDeck);
        if (matchedCards.Count >= GameConfig.CARDS_AMOUNT_TO_MATCH)
        {
            Debug.Log("Cards matched by suit amount: " + matchedCards.Count);
            allMatchedCards.AddRange(matchedCards);
            matchedCardsByMatchType.Add(GameConfig.MatchType.BY_SUIT, CloneList(matchedCards));
        }

       
        for (int i=0; i< allMatchedCards.Count; i++)
        {
            var index = cardsToRemove.IndexOf(allMatchedCards[i]);
            if (index == -1 )
            {
                cardsToRemove.Add(allMatchedCards[i]);
            }
        }

        if (matchedCardsByMatchType.Count > 0)
        {
            Debug.Log("dispatch matched cards ");
            MatchedPlayerCards.Invoke(matchedCardsByMatchType, targetTurn);
        }

        return cardsToRemove;

    }

    private List<CardVo> MatchCardsByRank(List<CardVo> cards)
    {
        List<CardVo> matchedCards = new List<CardVo>();
        matchedCards.Add(cards[0]);

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].rank != matchedCards[matchedCards.Count - 1].rank)
            {
                if (matchedCards.Count >= GameConfig.CARDS_AMOUNT_TO_MATCH)
                {
                    return matchedCards;
                }
                matchedCards.Clear();
                matchedCards.Add(cards[i]);
            }
            else
            {
                matchedCards.Add(cards[i]);
            }
        }

        return matchedCards;
    }

    private List<CardVo> MatchCardsBySuit(List<CardVo> cards)
    {
        List<CardVo> matchedCards = new List<CardVo>();
        matchedCards.Add(cards[0]);

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].suit != matchedCards[matchedCards.Count-1].suit)
            {
                if (matchedCards.Count >= GameConfig.CARDS_AMOUNT_TO_MATCH)
                {
                    return matchedCards;
                }
                matchedCards.Clear();
                matchedCards.Add(cards[i]);
            }
            else
            {
                matchedCards.Add(cards[i]);
            }
        }

        return matchedCards;
    }

    private List<CardVo> MatchCardsByColor(List<CardVo> cards)
    {
        List<CardVo> matchedCards = new List<CardVo>();
        matchedCards.Add(cards[0]);

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].CardColor != matchedCards[matchedCards.Count-1].CardColor)
            {
                if(matchedCards.Count >= GameConfig.CARDS_AMOUNT_TO_MATCH)
                {
                    return matchedCards;
                }
                matchedCards.Clear();
                matchedCards.Add(cards[i]);
            }
            else
            {
                matchedCards.Add(cards[i]);
            }
        }

        return matchedCards;
    }


    private List<CardVo> CloneList(List<CardVo> list)
    {
        List<CardVo> clonnedList = new List<CardVo>();

        for (int i = 0; i < list.Count; i++)
        {
            clonnedList.Add(list[i].Clone());
        }

        return clonnedList;
    }

    private CardVo CreateCard()
    {
        return CardVo.RandomCard(true, lastCardId++);
    }




}
