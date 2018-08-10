using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Monster.cs is a chess variant where white has only 4 pawns and a king, but can move twice per turn.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     r n b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . p p p p . .
    ///     . . . . K . . .
    /// </summary>
    public class Monster : Chess {
        private bool isWhiteSecondMove = false;

        public Monster() : base() {
        }

        public override string ToString() {
            return "Monster Chess";
        }

        public override void OnTurnComplete() {
            if (isWhiteSecondMove == false && GetCurrentTeamTurn() == Team.WHITE) {
                isWhiteSecondMove = true;
            } else {
                base.OnTurnComplete();
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x > 1 && x < 6) {
                    AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                }
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override bool CheckWinState() {
            if (CapturelessMovesLimit()) {
                return true;
            }

            if (isWhiteSecondMove) {
                if (IsPieceInCheck(opposingRoyalPiece)) {
                    UIManager.Instance.Log("Team " + GetOpposingTeamTurn().ToString() + " has been checkmated -- Team " + GetCurrentTeamTurn().ToString() + " wins!");
                    return true;
                }
            }

            if(isWhiteSecondMove || GetCurrentTeamTurn() == Team.BLACK) {
                foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                    if (piece.IsAlive) {
                        if (CalculateAvailableMoves(piece).Count > 0) return false;
                    }
                }

                if (IsPieceInCheck(currentRoyalPiece)) {
                    UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                } else {
                    UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                }
                return true;
            }

            return false;
        }

        protected override bool IsPieceInCheck(ChessPiece pieceToCheck) {
            if (checkingForCheck) return false;

            checkingForCheck = true;
            if (pieceToCheck.GetTeam() == Team.BLACK) {
                foreach (ChessPiece piece in GetPieces(Team.WHITE)) {
                    if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                        checkingForCheck = false;
                        return true;
                    }
                }
            } else {
                if (isWhiteSecondMove) {
                    opposingTeamCheckThreats = GetAllPossibleCheckThreats(pieceToCheck);
                    foreach (ChessPiece piece in opposingTeamCheckThreats) {
                        if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                            checkingForCheck = false;
                            return true;
                        }
                    }
                }
            }
            checkingForCheck = false;
            return false;
        }
    }
}
