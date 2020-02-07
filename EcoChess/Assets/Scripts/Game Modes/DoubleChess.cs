using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b q k b n r r n b q k b n r
    ///     p p p p p p p p p p p p p p p p
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     p p p p p p p p p p p p p p p p
    ///     R N B Q K B N R R N B Q K B N R
    /// </summary>
    public class DoubleChess : Chess {
        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 12;
        private King secondCurrentKing;
        private King secondOpposingKing;

        public DoubleChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            secondCurrentKing = secondOpposingKing = null;
        }

        public override string ToString() {
            return "Double-Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Julian Hayward (1916)",
                this.ToString() + " is a variant on a 12x16 board with an additional king for each team.",
                "Checkmate.",
                "- a-side king castles a-side, p-side king castles p-side.\n" +
                "- Pawns may move up to 4 squares on the initial move.",
                "https://en.wikipedia.org/wiki/Double_chess"
            );
        }

        protected override void SwapCurrentAndOpposingRoyaltyPieces() {
            base.SwapCurrentAndOpposingRoyaltyPieces();

            King temp = secondCurrentKing;
            secondCurrentKing = secondOpposingKing;
            secondOpposingKing = temp;
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            secondCurrentKing = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(12, WHITE_BACKROW)));
            secondOpposingKing = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(12, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(15, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(15, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), initialMoveLimit: 4));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), initialMoveLimit: 4));

                if (x == 1 || x == 6 || x == 9 || x == 14) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == 5 || x == 10 || x == 13) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 3 || x == 11) {
                    AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 7 || x == 8) {
                    AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetOccupier();
                    MakeDirectMove(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y), false);
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 14) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetOccupier();
                    MakeDirectMove(castlingPiece, new BoardCoord(13, mover.GetBoardPosition().y), false);
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false
                    && IsPieceInCheckAfterThisMove(secondCurrentKing, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (IsRoyal(mover)) {
                if (mover == currentRoyalPiece) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleRightward: false));
                } else if (mover == secondCurrentKing) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleLeftward: false));
                }
            } else if (mover is Pawn) {
                availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
            }
            return availableMoves;
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    int y = 0;
                    while(Board.ContainsCoord(mover.GetRelativeBoardCoord(i, y))) {
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

            while(Board.ContainsCoord(mover.GetRelativeBoardCoord(0, y))) {
                ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(0, y)).GetOccupier();

                if(occupier != null) {
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
    }
}
