using UnityEngine;
using System.Collections.Generic;

public class GameConfig  {

    public enum MatchType { BY_SUIT, BY_COLOR, BY_RANK };

    public const int CARDS_AMOUNT_TO_MATCH = 3;

    public const int PLAYER_DECK_SIZE = 4;
    public const int SPREAD_SIZE = 8;
    public const int COMMON_DECK_SIZE = 6;

    public int GetRewardForMatchType(MatchType matchType)
    {
        switch(matchType)
        {
            case MatchType.BY_COLOR: return 10;
            case MatchType.BY_SUIT: return 20;
            case MatchType.BY_RANK: return 30;
            default: return 0;
        }
    }

}
