using UnityEngine;

namespace ChessGameModes {
    public enum GameMode {
        AlmostChess, AndernachChess, AtomicChess, BalboChess, BerolinaChess, CapablancaChess, ChargeOfTheLightBrigadeChess,
        ChecklessChess, Chess960, ChigorinChess, DoubleChess, EmbassyChess, FIDE, GrandChess, HalfChess, Horde, JanusChess,
        KingOfTheHill, Knightmate, LosingChess, Microchess, MonsterChess, OmegaChess, PawnEndgameChess, PeasantsRevolt,
        RacingKings, ShiftedChess, Silverman4x5, ThreeCheck, UpsidedownChess, Weak
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
                case GameMode.ChargeOfTheLightBrigadeChess:
                    return new ChargeOfTheLightBrigade();
                case GameMode.ChecklessChess:
                    return new Checkless();
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
                case GameMode.HalfChess:
                    return new HalfChess();
                case GameMode.Horde:
                    return new Horde();
                case GameMode.JanusChess:
                    return new Janus();
                case GameMode.KingOfTheHill:
                    return new KingOfTheHill();
                case GameMode.Knightmate:
                    return new Knightmate();
                case GameMode.LosingChess:
                    return new LosingChess();
                case GameMode.Microchess:
                    return new Microchess();
                case GameMode.MonsterChess:
                    return new Monster();
                case GameMode.OmegaChess:
                    return new OmegaChess();
                case GameMode.PawnEndgameChess:
                    return new PawnEndgame();
                case GameMode.PeasantsRevolt:
                    return new PeasantsRevolt();
                case GameMode.RacingKings:
                    return new RacingKings();
                case GameMode.ShiftedChess:
                    return new ShiftedChess();
                case GameMode.Silverman4x5:
                    return new Silverman4x5();
                //case GameMode.SovereignChess:
                //    return new SovereignChess();
                case GameMode.ThreeCheck:
                    return new ThreeCheck();
                case GameMode.UpsidedownChess:
                    return new UpsideDown();
                case GameMode.Weak:
                    return new Weak();
                default:
                    Debug.LogError("Game mode: " + mode.ToString() + ", is not supported!");
                    return null;
            }
        }
    }
}
