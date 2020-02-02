using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     $ ^ q a k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     $ = Empress
    ///     . . . . . . . .     ^ = Princess
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     $ ^ Q A K B N R
    /// </summary>
    public class PerfectChess : Chess {

        public PerfectChess() : base() {
            SelectedPawnPromotion = Piece.Amazon;
            PawnPromotionOptions = new Piece[7] { Piece.Amazon, Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            CastlerOptions = new Piece[] { Piece.Rook, Piece.Empress };
        }

        public override string ToString() {
            return "Perfect Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Köksal Karakus (2000)",
                this.ToString() + " is a variant involving amazon, empress, and princess fairy pieces on both teams.",
                "Checkmate.",
                "- Kings move three squares when castling.\n" +
                "- Kings may a-side castle with the empress.\n" +
                "- Pawns may also promote to an amazon, empress, or princess.",
                "https://www.chessvariants.com/diffmove.dir/perfectchess.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "e1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "e8"));

            AddPieceToBoard(new Empress(Team.WHITE, "a1"));
            AddPieceToBoard(new Empress(Team.BLACK, "a8"));
            AddPieceToBoard(new Rook(Team.WHITE, "h1"));
            AddPieceToBoard(new Rook(Team.BLACK, "h8"));

            AddPieceToBoard(new Princess(Team.WHITE, "b1"));
            AddPieceToBoard(new Princess(Team.BLACK, "b8"));

            AddPieceToBoard(new Queen(Team.WHITE, "c1"));
            AddPieceToBoard(new Queen(Team.BLACK, "c8"));

            AddPieceToBoard(new Amazon(Team.WHITE, "d1"));
            AddPieceToBoard(new Amazon(Team.BLACK, "d8"));

            AddPieceToBoard(new Bishop(Team.WHITE, "f1"));
            AddPieceToBoard(new Bishop(Team.BLACK, "f8"));

            AddPieceToBoard(new Knight(Team.WHITE, "g1"));
            AddPieceToBoard(new Knight(Team.BLACK, "g8"));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetAllPiecesOfType<Amazon>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetAllPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetAllPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover, ref string moveNotation) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetOccupier();
                    MakeDirectMove(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y), false);
                    moveNotation = "O-O-O";
                    return true;

                } else if (mover.GetBoardPosition().x == 6) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetOccupier();
                    MakeDirectMove(castlingPiece, new BoardCoord(5, mover.GetBoardPosition().y), false);
                    moveNotation = "O-O";
                    return true;
                }
            }
            return false;
        }
    }
}
