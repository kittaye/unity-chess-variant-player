using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// RacingKings.cs is a chess variant where the king must reach the eighth rank to win.
    /// 
    /// Winstate: King reaches 8th rank.
    /// Piece types: Orthodox.
    /// Piece rules: Neither team can check or be checked.
    /// Board layout:
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     k r b n N B R K
    ///     q r b n N B R Q
    /// </summary>
    public class RacingKings : Chess {
        public RacingKings() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            AllowCastling = false;
            AllowEnpassantCapture = false;
            AllowPawnPromotion = false;
        }

        public override string ToString() {
            return "Racing Kings";
        }

        public override void PopulateBoard() {
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(0, 1)));
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(7, 1)));

            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(0, 0)));
            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(7, 0)));

            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(1, 0)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(1, 1)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(6, 0)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(6, 1)));

            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(2, 0)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(2, 1)));
            AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(5, 0)));
            AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(5, 1)));

            AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(3, 0)));
            AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(3, 1)));
            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(4, 0)));
            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(4, 1)));
        }

        public override bool CheckWinState() {
            if(opposingRoyalPiece.GetBoardPosition().y == BOARD_HEIGHT - 1) {
                BoardCoord stalemateSquare1 = new BoardCoord(currentRoyalPiece.GetBoardPosition().x, BOARD_HEIGHT - 1);
                BoardCoord stalemateSquare2 = new BoardCoord(currentRoyalPiece.GetBoardPosition().x + 1, BOARD_HEIGHT - 1);
                BoardCoord stalemateSquare3 = new BoardCoord(currentRoyalPiece.GetBoardPosition().x - 1, BOARD_HEIGHT - 1);

                List<BoardCoord> kingMoves = CalculateAvailableMoves(currentRoyalPiece);
                if (kingMoves.Contains(stalemateSquare1) || kingMoves.Contains(stalemateSquare2) || kingMoves.Contains(stalemateSquare3)) {
                    UIManager.Instance.LogCustom("Stalemate after team " + GetOpposingTeamTurn().ToString() + "'s move! (Team " + GetCurrentTeamTurn().ToString()
                        + " is able to reach the eighth rank on this turn)  -- Draw!");
                    return true;
                } else {
                    UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + "'s king has reached the eighth rank -- Team "
                        + GetOpposingTeamTurn().ToString() + "wins!");
                    return true;
                }
            }

            if(!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            string moveNotation = MakeDirectMove(mover, destination);
            if(moveNotation != null) {
                GetMoveNotations.Push(moveNotation);
                return true;
            }
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
               if (IsAKingInCheckAfterThisMove(mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
               }
            }
            return availableMoves;
        }

        private bool IsAKingInCheckAfterThisMove(ChessPiece mover, BoardCoord dest) {
            if (AssertContainsCoord(dest)) {
                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = Board.GetCoordInfo(dest).occupier;
                ChessPiece originalLastMover;
                BoardCoord oldPos = mover.GetBoardPosition();
                SimulateMove(mover, dest, originalOccupier, out originalLastMover);

                // Check if either king is in check after this temporary move
                bool kingChecked = IsPieceInCheck(currentRoyalPiece) || IsPieceInCheck(opposingRoyalPiece);

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, dest, originalOccupier, originalLastMover, oldPos);

                return kingChecked;
            }
            return false;
        }
    }
}
