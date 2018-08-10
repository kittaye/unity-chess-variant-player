using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Extinction.cs is a chess variant with a custom winstate.
    /// 
    /// Winstate: Elimination of all of a type of chess piece.
    /// Piece types: Orthodox.
    /// Piece rules: King isn't royal -- can castle unrestricted by checks.
    /// Board layout: Orthodox.
    /// </summary>
    public class Extinction : Chess {
        private readonly Piece[] pieces;
        private readonly Dictionary<Piece, int> whitePieceCounts;
        private readonly Dictionary<Piece, int> blackPieceCounts;

        public Extinction() : base() {
            pieces = new Piece[6] { Piece.King, Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.Pawn };

            whitePieceCounts = new Dictionary<Piece, int>(6) {
                { Piece.King, 1 }, { Piece.Queen, 1 }, { Piece.Rook, 2 }, { Piece.Bishop, 2 }, { Piece.Knight, 2 }, { Piece.Pawn, 8 }
            };
            blackPieceCounts = new Dictionary<Piece, int>(6) {
                { Piece.King, 1 }, { Piece.Queen, 1 }, { Piece.Rook, 2 }, { Piece.Bishop, 2 }, { Piece.Knight, 2 }, { Piece.Pawn, 8 }
            };
        }

        public override string ToString() {
            return "Extinction Chess";
        }

        public override bool CheckWinState() {
            if (CapturelessMovesLimit()) {
                return true;
            }

            if (GetCurrentTeamTurn() == Team.WHITE) {
                for (int i = 0; i < 6; i++) {
                    if(whitePieceCounts[pieces[i]] == 0) {
                        UIManager.Instance.Log("Team WHITE's " + pieces[i].ToString().ToLower() + "s have all been eliminated -- Team BLACK wins!");
                        return true;
                    }
                }
            } else {
                for (int i = 0; i < 6; i++) {
                    if (blackPieceCounts[pieces[i]] == 0) {
                        UIManager.Instance.Log("Team BLACK's " + pieces[i].ToString().ToLower() + "s have all been eliminated -- Team WHITE wins!");
                        return true;
                    }
                }
            }

            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) return false;
                }
            }

            UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            return true;
        }

        public override void PopulateBoard() {
            AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                availableMoves.Add(templateMoves[i]);
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();
            ChessPiece capturedPiece = Board.GetCoordInfo(destination).occupier;

            // Try make the move
            if (MakeMove(mover, destination)) {
                // Check castling moves
                if (mover is King && mover.MoveCount == 1) {
                    TryPerformCastlingRookMoves(mover);
                } else if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -1) != oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        if (GetCurrentTeamTurn() == Team.WHITE) {
                            whitePieceCounts[Piece.Pawn]--;
                            whitePieceCounts[promotedPiece.GetPieceType()]++;
                        } else {
                            blackPieceCounts[Piece.Pawn]--;
                            blackPieceCounts[capturedPiece.GetPieceType()]++;
                        }

                        mover = promotedPiece;
                    }
                }

                if(capturedPiece != null) {
                    if(GetCurrentTeamTurn() == Team.WHITE) {
                        blackPieceCounts[capturedPiece.GetPieceType()]--;
                    } else {
                        whitePieceCounts[capturedPiece.GetPieceType()]--;
                    }
                }

                return true;
            }
            return false;
        }

        protected override BoardCoord[] TryAddAvailableCastleMoves(ChessPiece king, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            List<BoardCoord> castleMoves = new List<BoardCoord>(2);

            for (int i = LEFT; i <= RIGHT; i += 2) {
                if (!canCastleLeftward && i == LEFT) continue;
                if (!canCastleRightward && i == RIGHT) break;

                int x = king.GetBoardPosition().x + i;
                int y = king.GetBoardPosition().y;
                BoardCoord coord = new BoardCoord(x, y);

                while (Board.ContainsCoord(coord)) {
                    ChessPiece occupier = Board.GetCoordInfo(coord).occupier;
                    if (occupier != null) {
                        if (occupier is Rook && occupier.MoveCount == 0) {
                            if (IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i, 0)) == false
                                && IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)) == false) {
                                castleMoves.Add(TryGetSpecificMove(king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)));
                            }
                        }
                        break;
                    }
                    x += i;
                    coord = new BoardCoord(x, y);
                }
            }
            return castleMoves.ToArray();
        }
    }
}