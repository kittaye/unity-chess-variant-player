using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Maharajah.cs is a chess variant that pits the black FIDE army against one amazon.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Amazon.
    /// Piece rules: No pawn promotion.
    /// Board layout: 
    ///     r n b q k b n r 
    ///     p p p p p p p p 
    ///     . . . . . . . . 
    ///     . . . . . . . . 
    ///     . . . . . . . .     A = Amazon
    ///     . . . . . . . . 
    ///     . . . . . . . . 
    ///     . . . . A . . . 
    /// </summary>
    public class Maharajah : Chess {

        public Maharajah() : base() {
        }

        public override string ToString() {
            return "Maharajah Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = AddPieceToBoard(new Amazon(Team.WHITE, "e1"));
            opposingRoyalPiece = AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            // Try make the move
            if (MakeMove(mover, destination)) {
                // Check castling moves
                if (mover is King && mover.MoveCount == 1) {
                    TryPerformCastlingRookMoves(mover);
                } else if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -1) != oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                }
                return true;
            }
            return false;
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Amazon>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }
    }
}