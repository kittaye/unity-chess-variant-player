using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// JesonMor.cs is a chess variant that involves a custom board layout and a custom winstate.
    /// 
    /// Winstate: Eliminate all pieces OR enter and leave the central square (e5).
    /// Piece types: Orthodox.
    /// Board layout: 
    ///     n n n n n n n n n
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     N N N N N N N N N
    /// </summary>
    public class JesonMor : Chess {
        protected new const int BOARD_WIDTH = 9;
        protected new const int BOARD_HEIGHT = 9;

        private static BoardCoord centerSquare = new BoardCoord(4, 4);
        private bool gameFinished = false;

        public JesonMor() : base(BOARD_WIDTH, BOARD_HEIGHT) {
        }

        public override string ToString() {
            return "Jeson Mor";
        }

        public override void PopulateBoard() {
            for(int i = 0; i < BOARD_WIDTH; i++) {
                AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(i, 0)));
                AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(i, BOARD_HEIGHT - 1)));
            }
        }

        public override bool CheckWinState() {
            if (gameFinished) {
                UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + " has left e5 -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                return true;
            } else if (GetPieces(GetCurrentTeamTurn()).TrueForAll((x) => (x.IsAlive == false))) {
                UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + " wins by elimination!");
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        protected override bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            return mover.CalculateTemplateMoves();
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            string moveNotation = MakeDirectMove(mover, destination);
            if (moveNotation != null) {
                if (oldPos == centerSquare) {
                    gameFinished = true;
                }
                GetMoveNotations.Push(moveNotation);
                return true;
            }
            return false;
        }
    }
}