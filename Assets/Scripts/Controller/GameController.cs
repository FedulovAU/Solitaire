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
        
    }

    public void DealCardToPlayer(int spreadCardId, int playerCardId, GameModel.Turn turn)
    {
        _gameScreen.DealCardToPlayer(spreadCardId, playerCardId, turn);
    }


    public void MoveCardFromPlayerToCommonDeck(int playerCardId, int commonDeckCardId, GameModel.Turn turn)
    {
        
    }


    public void MatchedPlayerCards(Dictionary<GameConfig.MatchType, List<CardVo>> cards, GameModel.Turn turn)
    {
        _gameScreen.MatchedPlayerCards(cards, turn);
    }

    public void ChangeTurn(GameModel.Turn turn)
    {
        
    }


}
