using UnityEngine;
using System.Collections;
using System;

public class CardVo {

    private const string ASSET_PREFIX = "Cards\\";

    public enum CardSuit { dimonds, spades, hearts, clubs };
    public enum CardRank
    {
        two = 0, three = 1, four = 2, five = 3, six = 4, seven = 5,
        eight = 6, ningh = 7, ten = 8, jack = 9, queen = 10, king = 11, ace = 12
    };

    private CardSuit _suit = CardSuit.clubs;
    public CardSuit suit
    {
        get { return _suit; }
    }

    private CardRank _rank = CardRank.jack;
    public CardRank rank
    {
        get { return _rank; }
    }

    private int _id;
    public int id
    {
        get { return _id; }
    }


    private bool _shirtVisible = false;
    public bool shirtVisible
    {
        get { return _shirtVisible; }
    }




    public CardVo(CardSuit suit, CardRank rank, bool shirtVisible, int cardId)
    {
        _id = cardId;
        _suit = suit;
        _rank = rank;
        _shirtVisible = shirtVisible;
    }

    public static CardSuit RandomSuit
    {
        get
        {
            Array suits = Enum.GetValues(typeof(CardSuit));
            var index = UnityEngine.Random.Range(0, suits.Length - 1);

            var randomSuit = (CardSuit)suits.GetValue(index);
            return randomSuit;
        }
    }

    public static CardRank RandomRank
    {
        get
        {
            Array ranks = Enum.GetValues(typeof(CardRank));
            var index = UnityEngine.Random.Range(0, ranks.Length - 1);

            var randomRank = (CardRank)ranks.GetValue(index);
            return randomRank;
        }
    }

    public static CardVo RandomCard(bool shirtVisible, int cardId)
    {
       
        CardVo card = new CardVo(RandomSuit, RandomRank, shirtVisible, cardId);

        return card;
    }

    public CardVo Clone()
    {
        CardVo cloneCard = new CardVo(suit, rank, shirtVisible, id);
        return cloneCard;
    }

    public void RemoveShirt()
    {
        _shirtVisible = false;
    }

    public string CardColor
    {
        get
        {
            if (suit == CardSuit.hearts || _suit == CardSuit.dimonds)
            {
                return "red";
            }
            else
            {
                return "black";
            }
        }
    }

    public string CardValueSmallAssetName
    {
        get
        {
            string assetName = ASSET_PREFIX + "card-value-small-" + CardColor + "-" + (int)_rank;
            return assetName;
        }
    }

    public string CardValueBigAssetName
    {
        get
        {
            string assetName = ASSET_PREFIX + "card-value-big-" + CardColor + "-" + (int)_rank;
            return assetName;
        }
    }

    public string CardSuitAssetName
    {
        get
        {
            string assetName = ASSET_PREFIX + "suit-symbol-" + _suit.ToString();
            return assetName;
        }
    }





}
