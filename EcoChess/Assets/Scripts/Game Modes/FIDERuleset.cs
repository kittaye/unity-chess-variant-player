using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// FIDERuleset.cs is the fully standardised ruleset for traditional chess.
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
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class FIDERuleset : Chess {
        protected const int BOARD_WIDTH = 8;
        protected const int BOARD_HEIGHT = 8;
        protected const int WHITE_BACKROW = 0;
        protected const int WHITE_PAWNROW = 1;
        protected int BLACK_BACKROW;
        protected int BLACK_PAWNROW;

        protected bool checkingForCheck;

        protected ChessPiece currentRoyalPiece;
        protected ChessPiece opposingRoyalPiece;

        protected Rook aSideWhiteRook;
        protected Rook hSideWhiteRook;
        protected Rook aSideBlackRook;
        protected Rook hSideBlackRook;

        protected List<ChessPiece> opposingTeamCheckThreats;

        public FIDERuleset(uint boardWidth, uint boardHeight) : base(boardWidth, boardHeight) {
            BLACK_BACKROW = board.GetHeight() - 1;
            BLACK_PAWNROW = board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
        }

        public FIDERuleset() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            BLACK_BACKROW = board.GetHeight() - 1;
            BLACK_PAWNROW = board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
        }

        public override string ToString() {
            return "FIDE";
        }

        public override void OnTurnComplete() {
            base.OnTurnComplete();

            ChessPiece temp = currentRoyalPiece;
            currentRoyalPiece = opposingRoyalPiece;
            opposingRoyalPiece = temp;
        }

        public override bool CheckWinState() {
            if(numConsecutiveCapturelessMoves == 100) {
                Debug.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) return false;
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                Debug.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                Debug.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

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
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if(mover is King) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove(mover);
                if(enPassantMove != BoardCoord.NULL) {
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
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if(promotedPiece != null) {
                        mover = promotedPiece;
                    }
                }
                return true;
            }
            return false;
        }

        protected void TryPerformCastlingRookMoves(ChessPiece mover, int castlerLeftx = 2, int castlerRightx = 6, int rookLeftx = 3, int rookRightx = 5) {
            if (mover.GetBoardPosition().x == castlerLeftx) {
                if (mover.GetTeam() == Team.WHITE) {
                    aSideWhiteRook = PerformCastle(aSideWhiteRook, new BoardCoord(rookLeftx, mover.GetBoardPosition().y));
                } else {
                    aSideBlackRook = PerformCastle(aSideBlackRook, new BoardCoord(rookLeftx, mover.GetBoardPosition().y));
                }
            } else if (mover.GetBoardPosition().x == castlerRightx) {
                if (mover.GetTeam() == Team.WHITE) {
                    hSideWhiteRook = PerformCastle(hSideWhiteRook, new BoardCoord(rookRightx, mover.GetBoardPosition().y));
                } else {
                    hSideBlackRook = PerformCastle(hSideBlackRook, new BoardCoord(rookRightx, mover.GetBoardPosition().y));
                }
            }
        }

        protected virtual Pawn CheckPawnEnPassantCapture(Pawn mover) {
            if (board.ContainsCoord(mover.GetRelativeBoardCoord(0, -1)) && IsThreat(mover, mover.GetRelativeBoardCoord(0, -1))) {
                ChessPiece occupier = board.GetCoordInfo(mover.GetRelativeBoardCoord(0, -1)).occupier;
                if (occupier != null && occupier is Pawn && ((Pawn)occupier).validEnPassant) {
                    mover.CaptureCount++;
                    RemovePieceFromBoard(occupier);
                    return (Pawn)occupier;
                }
            }
            return null;
        }

        protected virtual ChessPiece CheckPawnPromotion(Pawn mover) {
            if (mover.GetRelativeBoardCoord(0, 1).y < WHITE_BACKROW || mover.GetRelativeBoardCoord(0, 1).y > BLACK_BACKROW) {
                RemovePieceFromBoard(mover);
                RemovePieceFromActiveTeam(mover);
                return AddPieceToBoard(ChessPieceFactory.Create(Piece.Queen, mover.GetTeam(), mover.GetBoardPosition()));
            }
            return null;
        }

        protected Rook PerformCastle(Rook castlingRook, BoardCoord castlingRookNewPos) {
            if (AssertContainsCoord(castlingRookNewPos)) {
                if (castlingRook != null) {
                    RemovePieceFromBoard(castlingRook);
                    RemovePieceFromActiveTeam(castlingRook);
                    return (Rook)AddPieceToBoard(new Rook((castlingRook.GetTeam()), castlingRookNewPos));
                } else {
                    Debug.LogError("Reference to the castling rook should not be null! Ensure rook references were made.");
                }
            }
            return null;
        }

        protected virtual bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            if (AssertContainsCoord(dest)) {
                if (checkingForCheck) return false;

                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = board.GetCoordInfo(dest).occupier;
                ChessPiece originalLastMover;
                BoardCoord oldPos = mover.GetBoardPosition();
                SimulateMove(mover, dest, originalOccupier, out originalLastMover);

                ChessPiece occupier = null;
                if (mover is Pawn) {
                    occupier = CheckPawnEnPassantCapture((Pawn)mover);
                }

                // Check whether the piece is in check after this temporary move
                bool isInCheck = IsPieceInCheck(pieceToCheck);

                if (occupier != null) {
                    board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                    occupier.IsAlive = true;
                    occupier.gameObject.SetActive(true);
                }

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, dest, originalOccupier, originalLastMover, oldPos);

                return isInCheck;
            }
            return false;
        }

        protected virtual bool IsPieceInCheck(ChessPiece pieceToCheck) {
            if (checkingForCheck) return false;

            opposingTeamCheckThreats = GetAllPossibleCheckThreats(pieceToCheck);

            checkingForCheck = true;
            foreach (ChessPiece piece in opposingTeamCheckThreats) {
                if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                    checkingForCheck = false;
                    return true;
                }
            }
            checkingForCheck = false;
            return false;
        }

        protected virtual List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = new List<ChessPiece>();

            for (int i = (int)MoveDirection.Up; i <= (int)MoveDirection.DownRight; i++) {
                int xModifier, yModifier;
                GetMoveDirectionModifiers(pieceToCheck, (MoveDirection)i, out xModifier, out yModifier);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + new BoardCoord(xModifier, yModifier);

                while (board.ContainsCoord(coord)) {
                    if (IsThreat(pieceToCheck, coord)) {
                        possibleCheckThreats.Add(board.GetCoordInfo(coord).occupier);
                    }
                    coord.x += xModifier;
                    coord.y += yModifier;
                }
            }

            GetPiecesOfType<Knight>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected virtual BoardCoord TryAddAvailableEnPassantMove(ChessPiece mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover is Pawn && ((Pawn)mover).canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (board.ContainsCoord(coord)) {
                        ChessPiece piece = board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == lastMovedPiece && ((Pawn)piece).validEnPassant) {
                            ((Pawn)mover).enPassantTargets.Add((Pawn)piece);
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                            }
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }

        protected virtual BoardCoord[] TryAddAvailableCastleMoves(ChessPiece king, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (IsPieceInCheck(king) == false) {
                List<BoardCoord> castleMoves = new List<BoardCoord>(2);

                for (int i = LEFT; i <= RIGHT; i += 2) {
                    if (!canCastleLeftward && i == LEFT) continue;
                    if (!canCastleRightward && i == RIGHT) break;

                    int x = king.GetBoardPosition().x + i;
                    int y = king.GetBoardPosition().y;
                    BoardCoord coord = new BoardCoord(x, y);

                    while (board.ContainsCoord(coord)) {
                        ChessPiece occupier = board.GetCoordInfo(coord).occupier;
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
            return new BoardCoord[0];
        }
    }
}
