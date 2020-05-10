﻿using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     r n b $ q k ^ b n r
    ///     p p p p p p p p p p
    ///     . . . . . . . . . .
    ///     . . . . . . . . . .     $ = Empress
    ///     . . . . . . . . . .     ^ = Princess
    ///     . . . . . . . . . .
    ///     p p p p p p p p p p
    ///     R N B $ Q K ^ B N R
    /// </summary>
    public class Capablanca : Chess {
        private new const int BOARD_WIDTH = 10;
        private new const int BOARD_HEIGHT = 8;

        public Capablanca() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[6]{ Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            castlingDistance = 3;
        }

        public override string ToString() {
            return "Capablanca's Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by José Raúl Capablanca (1920s)",
                this.ToString() + " is a variant on a 10x8 board adding empresses and princesses.",
                "Checkmate.",
                "- King moves three squares when castling.\n" +
                "- Pawns may also promote to an empress or princess.",
                "https://en.wikipedia.org/wiki/Capablanca_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(5, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(5, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(9, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(9, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Empress, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Empress, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Princess, Team.WHITE, new BoardCoord(6, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Princess, Team.BLACK, new BoardCoord(6, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()));
            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 8) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(7, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }
    }
}