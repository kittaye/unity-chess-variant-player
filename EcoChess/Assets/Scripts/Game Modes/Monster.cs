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
    public class Monster : FIDERuleset {
        private bool isWhiteSecondMove = false;

        public Monster() : base() {
        }

        public override string ToString() {
            return "Monster Chess";
        }

        public override void OnTurnComplete() {
            if (isWhiteSecondMove == false && currentTeamTurn == Team.WHITE) {
                isWhiteSecondMove = true;
            } else {
                currentTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
                opposingTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
                ChessPiece temp = currentRoyalPiece;
                currentRoyalPiece = opposingRoyalPiece;
                opposingRoyalPiece = temp;
                isWhiteSecondMove = false;
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

        protected override bool IsPieceInCheck(ChessPiece pieceToCheck) {
            if (checkingForCheck) return false;

            checkingForCheck = true;
            if (pieceToCheck.GetTeam() == Team.BLACK) {
                foreach (ChessPiece piece in GetPieces(Team.WHITE)) {
                    BoardCoord[] immediateMoves = CalculateAvailableMoves(piece).ToArray();
                    for (int i = 0; i < immediateMoves.Length; i++) {
                        // Temporarily simulate the move actually happening
                        ChessPiece originalOccupier = board.GetCoordInfo(immediateMoves[i]).occupier;
                        ChessPiece originalLastMover;
                        BoardCoord oldPos = piece.GetBoardPosition();
                        SimulateMove(piece, immediateMoves[i], originalOccupier, out originalLastMover);

                        ChessPiece occupier = null;
                        if (piece is Pawn) {
                            occupier = CheckPawnEnPassantCapture((Pawn)piece);
                        }

                        if (piece.CalculateTemplateMoves().Contains(pieceToCheck.GetBoardPosition())) {
                            checkingForCheck = false;
                            // Revert the temporary move back to normal
                            if (occupier != null) {
                                piece.CaptureCount--;
                                board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                                occupier.IsAlive = true;
                                occupier.gameObject.SetActive(true);
                            }
                            RevertSimulatedMove(piece, immediateMoves[i], originalOccupier, originalLastMover, oldPos);
                            return true;
                        }

                        // Revert the temporary move back to normal
                        if (occupier != null) {
                            piece.CaptureCount--;
                            board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                            occupier.IsAlive = true;
                            occupier.gameObject.SetActive(true);
                        }
                        RevertSimulatedMove(piece, immediateMoves[i], originalOccupier, originalLastMover, oldPos);
                    }
                }
            } else {
                opposingTeamCheckThreats = GetAllPossibleCheckThreats(pieceToCheck);
                foreach (ChessPiece piece in opposingTeamCheckThreats) {
                    if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                        if(isWhiteSecondMove == false) {
                            BoardCoord[] secondaryTemplateMoves = pieceToCheck.CalculateTemplateMoves().ToArray();
                            int validSquares = secondaryTemplateMoves.Length - 1;
                            for (int i = 0; i < secondaryTemplateMoves.Length; i++) {
                                foreach (ChessPiece piece2 in GetPieces(Team.BLACK)) {
                                    if (piece2.CalculateTemplateMoves().Contains(secondaryTemplateMoves[i])) {
                                        validSquares--;
                                        break;
                                    }
                                }
                            }
                            checkingForCheck = false;
                            return (validSquares == 0);
                        }
                        checkingForCheck = false;
                        return true;
                    }
                }
            }
            checkingForCheck = false;
            return false;
        }
    }
}
