using UnityEngine;

namespace ChessGameModes {
    public enum GameMode {
        ActiveChess, AdvanceChess, AlmostChess, AmazonChess, AndernachChess, AtomicChess, BalboChess, BerolinaChess, CapablancaChess,
        ChargeOfTheLightBrigadeChess, ChecklessChess, CheshireCatChess, Chess960, ChigorinChess, DoubleChess, EmbassyChess, ExtinctionChess,
        FianchettoChess, FIDE, GothicChess, GrandChess, GrasshopperChess, HalfChess, Horde, JanusChess, JesonMor, KingOfTheHill, Knightmate,
        LadderChess, LeganChess, LosAlamosChess, LosingChess, MaharajahChess, Microchess, MongredienChess, MonsterChess, NightriderChess,
        OmegaChess, PawnEndgameChess, PeasantsRevolt, PerfectChess, RacingKings, ReversedRoyals, ShiftedChess, Silverman4x5, SovereignChess,
        ThreeCheck, UpsidedownChess, Weak
    }

    public static class GameModeFactory {
        public static Chess Create(GameMode mode) {
            switch (mode) {
                case GameMode.ActiveChess:
                    return new ActiveChess();
                case GameMode.AdvanceChess:
                    return new Advance();
                case GameMode.AlmostChess:
                    return new AlmostChess();
                case GameMode.AmazonChess:
                    return new AmazonChess();
                case GameMode.AndernachChess:
                    return new Andernach();
                case GameMode.AtomicChess:
                    return new Atomic();
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
                case GameMode.CheshireCatChess:
                    return new CheshireCat();
                case GameMode.Chess960:
                    return new Chess960();
                case GameMode.ChigorinChess:
                    return new Chigorin();
                case GameMode.DoubleChess:
                    return new DoubleChess();
                case GameMode.EmbassyChess:
                    return new Embassy();
                case GameMode.ExtinctionChess:
                    return new Extinction();
                case GameMode.FianchettoChess:
                    return new Fianchetto();
                case GameMode.FIDE:
                    return new FIDERuleset();
                case GameMode.GothicChess:
                    return new GothicChess();
                case GameMode.GrandChess:
                    return new GrandChess();
                case GameMode.GrasshopperChess:
                    return new GrasshopperChess();
                case GameMode.HalfChess:
                    return new HalfChess();
                case GameMode.Horde:
                    return new Horde();
                case GameMode.JanusChess:
                    return new Janus();
                case GameMode.JesonMor:
                    return new JesonMor();
                case GameMode.KingOfTheHill:
                    return new KingOfTheHill();
                case GameMode.Knightmate:
                    return new Knightmate();
                case GameMode.LadderChess:
                    return new LadderChess();
                case GameMode.LeganChess:
                    return new Legan();
                case GameMode.LosAlamosChess:
                    return new LosAlamos();
                case GameMode.LosingChess:
                    return new LosingChess();
                case GameMode.MaharajahChess:
                    return new Maharajah();
                case GameMode.Microchess:
                    return new Microchess();
                case GameMode.MongredienChess:
                    return new Mongredien();
                case GameMode.MonsterChess:
                    return new Monster();
                case GameMode.NightriderChess:
                    return new NightriderChess();
                case GameMode.OmegaChess:
                    return new OmegaChess();
                case GameMode.PawnEndgameChess:
                    return new PawnEndgame();
                case GameMode.PerfectChess:
                    return new PerfectChess();
                case GameMode.PeasantsRevolt:
                    return new PeasantsRevolt();
                case GameMode.RacingKings:
                    return new RacingKings();
                case GameMode.ReversedRoyals:
                    return new ReversedRoyals();
                case GameMode.ShiftedChess:
                    return new ShiftedChess();
                case GameMode.Silverman4x5:
                    return new Silverman4x5();
                case GameMode.SovereignChess:
                    return new SovereignChess();
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
