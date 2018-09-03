using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// PerfectChess.cs is a chess variant where all rook+bishop+knight combinations occur exactly once.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Amazons, Empresses, Princesses.
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

            GetPiecesOfType<Amazon>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected override void TryPerformCastlingRookMoves(ChessPiece mover) {
            if (mover.GetBoardPosition().x == 2) {
                ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).occupier;
                MakeDirectMove(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y), false);
            } else if (mover.GetBoardPosition().x == 6) {
                ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).occupier;
                MakeDirectMove(castlingPiece, new BoardCoord(5, mover.GetBoardPosition().y), false);
            }
        }
    }
}
