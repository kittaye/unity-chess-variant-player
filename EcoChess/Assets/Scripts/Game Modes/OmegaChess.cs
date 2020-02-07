using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     w                     w
    ///       c r n b q k b n r c
    ///       p p p p p p p p p p 
    ///       . . . . . . . . . .     Cc = Champions
    ///       . . . . . . . . . .     Ww = Wizards
    ///       . . . . . . . . . . 
    ///       . . . . . . . . . . 
    ///       p p p p p p p p p p 
    ///       C R N B Q K B N R C 
    ///     W                     W
    /// </summary>
    public class OmegaChess : Chess {
        private new const int BOARD_WIDTH = 12;
        private new const int BOARD_HEIGHT = 12;
        private new const int WHITE_BACKROW = 1;
        private new const int WHITE_PAWNROW = 2;

        public OmegaChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            Board.RemoveBoardCoordinates(new string[] { "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "a10", "a11" });
            Board.RemoveBoardCoordinates(new string[] { "l2", "l3", "l4", "l5", "l6", "l7", "l8", "l9", "l10", "l11" });
            Board.RemoveBoardCoordinates(new string[] { "b1", "c1", "d1", "e1", "f1", "g1", "h1", "i1", "j1", "k1" });
            Board.RemoveBoardCoordinates(new string[] { "b12", "c12", "d12", "e12", "f12", "g12", "h12", "i12", "j12", "k12" });

            Board.SetCustomBoardCoordinateKey("a1", "W1");
            Board.SetCustomBoardCoordinateKey("l1", "W2");
            Board.SetCustomBoardCoordinateKey("a12", "W3");
            Board.SetCustomBoardCoordinateKey("l12", "W4");

            Board.ResetAlgebraicKeys("b2", 10, 10);

            BLACK_BACKROW = Board.GetHeight() - 2;
            BLACK_PAWNROW = Board.GetHeight() - 3;

            PawnPromotionOptions = new Piece[6] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.Wizard, Piece.Champion };
        }

        public override string ToString() {
            return "Omega Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Daniel C. Macdonald (1999)",
                this.ToString() + " is a variant involving wizard and champion fairy pieces on a 10x10 board, with an additional 4 corner squares.",
                "Checkmate.",
                "- Pawns may move up to three squares on the initial move.\n" +
                "- En passant rules also apply to pawns moving three squares\n" +
                "  (in which case the opposing pawn on the same rank or one rank behind the initial mover may perform en passant).\n" +
                "- Pawns may also promote to a wizard or champion.",
                "https://www.chessvariants.com/large.dir/omega/rules.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "f1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "f10"));

            AddPieceToBoard(new Queen(Team.WHITE, "e1"));
            AddPieceToBoard(new Queen(Team.BLACK, "e10"));

            AddPieceToBoard(new Rook(Team.WHITE, "b1"));
            AddPieceToBoard(new Rook(Team.WHITE, "i1"));
            AddPieceToBoard(new Rook(Team.BLACK, "b10"));
            AddPieceToBoard(new Rook(Team.BLACK, "i10"));

            AddPieceToBoard(new Wizard(Team.WHITE, "W1"));
            AddPieceToBoard(new Wizard(Team.WHITE, "W2"));
            AddPieceToBoard(new Wizard(Team.BLACK, "W3"));
            AddPieceToBoard(new Wizard(Team.BLACK, "W4"));

            for (int x = 1; x < BOARD_WIDTH - 1; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), initialMoveLimit: 3));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), initialMoveLimit: 3));

                if(x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Champion(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Champion(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 3 || x == BOARD_WIDTH - 4) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 4 || x == BOARD_WIDTH - 5) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 4) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(2, mover.GetBoardPosition().y)).GetOccupier();
                    MakeDirectMove(castlingPiece, new BoardCoord(5, mover.GetBoardPosition().y), false);
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 8) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(9, mover.GetBoardPosition().y)).GetOccupier();
                    MakeDirectMove(castlingPiece, new BoardCoord(7, mover.GetBoardPosition().y), false);
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    int y = 0;
                    while (Board.ContainsCoord(mover.GetRelativeBoardCoord(i, y))) {
                        BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, y));
                        if (Board.ContainsCoord(coord)) {
                            ChessPiece piece = Board.GetCoordInfo(coord).GetOccupier();
                            if (piece != null) {
                                if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                                    if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                        enpassantMoves.Add(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1)));
                                    }
                                } else {
                                    break;
                                }
                            }
                        }
                        y--;
                    }
                }
            }
            return enpassantMoves.ToArray();
        }

        protected override Pawn TryPerformPawnEnPassantCapture(Pawn mover) {
            BoardCoord oldPos = mover.MoveStateHistory[GameMoveNotations.Count - 1].position;
            BoardCoord newPos = mover.GetBoardPosition();
            int y = -1;

            while (Board.ContainsCoord(mover.GetRelativeBoardCoord(0, y))) {
                ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(0, y)).GetOccupier();

                if (occupier != null) {
                    if (IsThreat(mover, occupier.GetBoardPosition())) {
                        if (occupier is Pawn && CheckEnPassantVulnerability((Pawn)occupier)) {
                            mover.CaptureCount++;
                            KillPiece(occupier);

                            SetLastMoveNotationToEnPassant(oldPos, newPos);
                            return (Pawn)occupier;
                        } else {
                            return null;
                        }
                    } else {
                        return null;
                    }
                }
                y--;
            }
            return null;
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Wizard>(pieceToCheck.GetOpposingTeam()));
            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Champion>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }
    }
}