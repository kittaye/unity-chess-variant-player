using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class Berolina : Chess {
        public Berolina() : base() {
        }

        public override string ToString() {
            return "Berolina Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Edmund Nebermann (1926)",
                this.ToString() + " is a variant where pawns are replaced with berolina pawns.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://en.wikipedia.org/wiki/Berolina_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new BerolinaPawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new BerolinaPawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).GetAliveOccupier();
                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(0, 1)) == false) {
                                enpassantMoves.Add(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(0, 1)));
                            }
                        }
                    }
                }
            }
            return enpassantMoves.ToArray();
        }

        protected override Pawn TryPerformPawnEnPassantCapture(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            BoardCoord oldPos = mover.MoveStateHistory[GameMoveNotations.Count - 1].position;
            BoardCoord newPos = mover.GetBoardPosition();

            for (int i = LEFT; i <= RIGHT; i += 2) {
                if (Board.ContainsCoord(mover.GetRelativeBoardCoord(i, -1)) && IsThreat(mover, mover.GetRelativeBoardCoord(i, -1))) {
                    ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(i, -1)).GetAliveOccupier();
                    if (occupier != null && occupier is Pawn && CheckEnPassantVulnerability((Pawn)occupier)) {
                        mover.CaptureCount++;
                        KillPiece(occupier);

                        SetLastMoveNotationToEnPassant(oldPos, newPos);
                        return (Pawn)occupier;
                    }
                }
            }
            return null;
        }
    }
}
