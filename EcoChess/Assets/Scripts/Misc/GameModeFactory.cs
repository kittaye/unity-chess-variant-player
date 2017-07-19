using UnityEngine;

namespace ChessGameModes {
    public enum GameMode {
        AtomicChess, ChargeOfTheLightBrigade, Chess960, DoubleChess, FIDE, Horde, KingoftheHill, Knightmate, LosingChess
            , Microchess, PawnEndgame, PeasantsRevolt, RacingKings, Silverman4x5, ThreeCheck, UpsideDown, AlmostChess
            , CapablancaChess, GrandChess, MonsterChess, BalboChess, AndernachChess, BerolinaChess, JanusChess, ChigorinChess
            , EmbassyChess
    }

    public static class GameModeFactory {
        public static Chess Create(GameMode mode) {
            switch (mode) {
                case GameMode.AtomicChess:
                    return new Atomic();
                case GameMode.AlmostChess:
                    return new AlmostChess();
                case GameMode.AndernachChess:
                    return new Andernach();
                case GameMode.BalboChess:
                    return new Balbo();
                case GameMode.BerolinaChess:
                    return new Berolina();
                case GameMode.CapablancaChess:
                    return new Capablanca();
                case GameMode.ChargeOfTheLightBrigade:
                    return new ChargeOfTheLightBrigade();
                case GameMode.Chess960:
                    return new Chess960();
                case GameMode.ChigorinChess:
                    return new Chigorin();
                case GameMode.DoubleChess:
                    return new DoubleChess();
                case GameMode.EmbassyChess:
                    return new Embassy();
                case GameMode.FIDE:
                    return new FIDERuleset();
                case GameMode.GrandChess:
                    return new GrandChess();
                case GameMode.Horde:
                    return new Horde();
                case GameMode.JanusChess:
                    return new Janus();
                case GameMode.KingoftheHill:
                    return new KingOfTheHill();
                case GameMode.Knightmate:
                    return new Knightmate();
                case GameMode.LosingChess:
                    return new LosingChess();
                case GameMode.Microchess:
                    return new Microchess();
                case GameMode.MonsterChess:
                    return new Monster();
                case GameMode.PawnEndgame:
                    return new PawnEndgame();
                case GameMode.PeasantsRevolt:
                    return new PeasantsRevolt();
                case GameMode.RacingKings:
                    return new RacingKings();
                case GameMode.Silverman4x5:
                    return new Silverman4x5();
                case GameMode.ThreeCheck:
                    return new ThreeCheck();
                case GameMode.UpsideDown:
                    return new UpsideDown();
                default:
                    Debug.LogError("Game mode: " + mode.ToString() + ", is not supported!");
                    return null;
            }
        }
    }
}
