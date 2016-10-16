using UnityEngine;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour {

    private GameModel _model;

    [SerializeField]
    private GameScreen _gameScreen;

    void Awake()
    {
        _model = new GameModel();

        _model.FillSpreadDeck += FillSpreadDeck;
        _model.FillCommonDeck += FillCommonDeck;
        _model.UpdateSpreadDeck += UpdateSpreadDeck;
        _model.UpdateCommonDeck += UpdateCommonDeck;
        _model.DealCardToPlayer += DealCardToPlayer;
        _model.MoveCardFromPlayerToCommonDeck += MoveCardFromPlayerToCommonDeck;
        _model.MatchedPlayerCards += MatchedPlayerCards;
        _model.ChangeTurn += ChangeTurn;
    }


    void Start() {
        if (_gameScreen.IsStarted)
        {
            _model.StartGame();
        } else
        {
            _gameScreen.Started += () =>
            {
                _model.StartGame();
            };
        }
        
    }

    // Update is called once per frame
    void Update() {

    }

    private void UnSubscribe()
    {
        _model.FillSpreadDeck -= FillSpreadDeck;
        _model.FillCommonDeck -= FillCommonDeck;
        _model.UpdateSpreadDeck -= UpdateSpreadDeck;
        _model.UpdateCommonDeck -= UpdateCommonDeck;
        _model.DealCardToPlayer -= DealCardToPlayer;
        _model.MoveCardFromPlayerToCommonDeck -= MoveCardFromPlayerToCommonDeck;
        _model.MatchedPlayerCards -= MatchedPlayerCards;
        _model.ChangeTurn -= ChangeTurn;
    }



    public void FillSpreadDeck(List<CardVo> cards)
    {
        _gameScreen.FillSpreadDeck(cards);
    }
    public void FillCommonDeck(List<CardVo> cards)
    {
        _gameScreen.FillCommonDeck(cards);
    }

    public void UpdateSpreadDeck(CardVo card)
    {
        _gameScreen.UpdateSpreadDeck(card);
    }

    public void UpdateCommonDeck(CardVo card)
    {
        _gameScreen.UpdateCommonDeck(card);
    }

    public void DealCardToPlayer(int spreadCardId, int playerCardId, GameModel.Turn turn)
    {
        _gameScreen.DealCardToPlayer(spreadCardId, playerCardId, turn);
    }


    public void MoveCardFromPlayerToCommonDeck(int playerCardId, int commonDeckCardId, GameModel.Turn turn)
    {
        _gameScreen.MoveCardFromPlayerToCommonDeck(playerCardId, commonDeckCardId, turn);
    }


    public void MatchedPlayerCards(Dictionary<GameConfig.MatchType, List<CardVo>> cards, GameModel.Turn turn)
    {
        _gameScreen.MatchedPlayerCards(cards, turn);
    }

    public void ChangeTurn(GameModel.Turn turn)
    {
        
    }

    public void OnPlayerDeckCardTap(CardVo cardVo)
    {

        var possibleMoves = _model.FindPossibleMoves(cardVo);
        _gameScreen.RemoveHighLightFromCards();
        if (possibleMoves.Count > 0)
        {
            _gameScreen.HighLightPlayerCard(cardVo);
            _gameScreen.HighLightCommonDeck(possibleMoves);
        }
    }

    public void OnCommonDeckCardTap(CardVo cardVo)
    {
        bool sucess = _model.TryToMoveCard(cardVo);
        if (sucess)
        {
            _gameScreen.RemoveHighLightFromCards();
        }
    }


}
