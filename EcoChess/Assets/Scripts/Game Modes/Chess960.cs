using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     ? ? ? ? ? ? ? ?
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     ? ? ? ? ? ? ? ?
    /// </summary>
    public class Chess960 : Chess {
        private Rook castlingRook;

        public Chess960() : base() {
            castlingRook = null;
        }

        public override string ToString() {
            return "Chess960";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Bobby Fischer (1996)",
                this.ToString() + " is a variant that randomises the positions of the back rank pieces identically for both teams.",
                "Checkmate.",
                "- Note: Given the required space, castling is allowed (with the king selected, select the rook to castle with).",
                "https://en.wikipedia.org/wiki/Fischer_random_chess"
            );
        }

        private Dictionary<int, Piece[]> KingsTable = new Dictionary<int, Piece[]> {
            { 0, new Piece[]{ Piece.Queen, Piece.Knight, Piece.Knight, Piece.Rook, Piece.King, Piece.Rook } },
            { 16, new Piece[]{ Piece.Knight, Piece.Queen, Piece.Knight, Piece.Rook, Piece.King, Piece.Rook } },
            { 32, new Piece[]{ Piece.Knight, Piece.Knight, Piece.Queen, Piece.Rook, Piece.King, Piece.Rook } },
            { 48, new Piece[]{ Piece.Knight, Piece.Knight, Piece.Rook, Piece.Queen, Piece.King, Piece.Rook } },
            { 64, new Piece[]{ Piece.Knight, Piece.Knight, Piece.Rook, Piece.King, Piece.Queen, Piece.Rook } },
            { 80, new Piece[]{ Piece.Knight, Piece.Knight, Piece.Rook, Piece.King, Piece.Rook, Piece.Queen } },
            { 96, new Piece[]{ Piece.Queen, Piece.Knight, Piece.Rook, Piece.Knight, Piece.King, Piece.Rook } },
            { 112, new Piece[]{ Piece.Knight, Piece.Queen, Piece.Rook, Piece.Knight, Piece.King, Piece.Rook } },
            { 128, new Piece[]{ Piece.Knight, Piece.Rook, Piece.Queen, Piece.Knight, Piece.King, Piece.Rook } },
            { 144, new Piece[]{ Piece.Knight, Piece.Rook, Piece.Knight, Piece.Queen, Piece.King, Piece.Rook } },
            { 160, new Piece[]{ Piece.Knight, Piece.Rook, Piece.Knight, Piece.King, Piece.Queen, Piece.Rook } },
            { 176, new Piece[]{ Piece.Knight, Piece.Rook, Piece.Knight, Piece.King, Piece.Rook, Piece.Queen } },
            { 192, new Piece[]{ Piece.Queen, Piece.Knight, Piece.Rook, Piece.King, Piece.Knight, Piece.Rook } },
            { 208, new Piece[]{ Piece.Knight, Piece.Queen, Piece.Rook, Piece.King, Piece.Knight, Piece.Rook } },
            { 224, new Piece[]{ Piece.Knight, Piece.Rook, Piece.Queen, Piece.King, Piece.Knight, Piece.Rook } },
            { 240, new Piece[]{ Piece.Knight, Piece.Rook, Piece.King, Piece.Queen, Piece.Knight, Piece.Rook } },
            { 256, new Piece[]{ Piece.Knight, Piece.Rook, Piece.King, Piece.Knight, Piece.Queen, Piece.Rook } },
            { 272, new Piece[]{ Piece.Knight, Piece.Rook, Piece.King, Piece.Knight, Piece.Rook, Piece.Queen } },
            { 288, new Piece[]{ Piece.Queen, Piece.Knight, Piece.Rook, Piece.King, Piece.Rook, Piece.Knight } },
            { 304, new Piece[]{ Piece.Knight, Piece.Queen, Piece.Rook, Piece.King, Piece.Rook, Piece.Knight } },
            { 320, new Piece[]{ Piece.Knight, Piece.Rook, Piece.Queen, Piece.King, Piece.Rook, Piece.Knight } },
            { 336, new Piece[]{ Piece.Knight, Piece.Rook, Piece.King, Piece.Queen, Piece.Rook, Piece.Knight } },
            { 352, new Piece[]{ Piece.Knight, Piece.Rook, Piece.King, Piece.Rook, Piece.Queen, Piece.Knight } },
            { 368, new Piece[]{ Piece.Knight, Piece.Rook, Piece.King, Piece.Rook, Piece.Knight, Piece.Queen } },
            { 384, new Piece[]{ Piece.Queen, Piece.Rook, Piece.Knight, Piece.Knight, Piece.King, Piece.Rook } },
            { 400, new Piece[]{ Piece.Rook, Piece.Queen, Piece.Knight, Piece.Knight, Piece.King, Piece.Rook } },
            { 416, new Piece[]{ Piece.Rook, Piece.Knight, Piece.Queen, Piece.Knight, Piece.King, Piece.Rook } },
            { 432, new Piece[]{ Piece.Rook, Piece.Knight, Piece.Knight, Piece.Queen, Piece.King, Piece.Rook } },
            { 448, new Piece[]{ Piece.Rook, Piece.Knight, Piece.Knight, Piece.King, Piece.Queen, Piece.Rook } },
            { 464, new Piece[]{ Piece.Rook, Piece.Knight, Piece.Knight, Piece.King, Piece.Rook, Piece.Queen } },
            { 480, new Piece[]{ Piece.Queen, Piece.Rook, Piece.Knight, Piece.King, Piece.Knight, Piece.Rook } },
            { 496, new Piece[]{ Piece.Rook, Piece.Queen, Piece.Knight, Piece.King, Piece.Knight, Piece.Rook } },
            { 512, new Piece[]{ Piece.Rook, Piece.Knight, Piece.Queen, Piece.King, Piece.Knight, Piece.Rook } },
            { 528, new Piece[]{ Piece.Rook, Piece.Knight, Piece.King, Piece.Queen, Piece.Knight, Piece.Rook } },
            { 544, new Piece[]{ Piece.Rook, Piece.Knight, Piece.King, Piece.Knight, Piece.Queen, Piece.Rook } },
            { 560, new Piece[]{ Piece.Rook, Piece.Knight, Piece.King, Piece.Knight, Piece.Rook, Piece.Queen } },
            { 576, new Piece[]{ Piece.Queen, Piece.Rook, Piece.Knight, Piece.King, Piece.Rook, Piece.Knight } },
            { 592, new Piece[]{ Piece.Rook, Piece.Queen, Piece.Knight, Piece.King, Piece.Rook, Piece.Knight } },
            { 608, new Piece[]{ Piece.Rook, Piece.Knight, Piece.Queen, Piece.King, Piece.Rook, Piece.Knight } },
            { 624, new Piece[]{ Piece.Rook, Piece.Knight, Piece.King, Piece.Queen, Piece.Rook, Piece.Knight } },
            { 640, new Piece[]{ Piece.Rook, Piece.Knight, Piece.King, Piece.Rook, Piece.Queen, Piece.Knight } },
            { 656, new Piece[]{ Piece.Rook, Piece.Knight, Piece.King, Piece.Rook, Piece.Knight, Piece.Queen } },
            { 672, new Piece[]{ Piece.Queen, Piece.Rook, Piece.King, Piece.Knight, Piece.Knight, Piece.Rook } },
            { 688, new Piece[]{ Piece.Rook, Piece.Queen, Piece.King, Piece.Knight, Piece.Knight, Piece.Rook } },
            { 704, new Piece[]{ Piece.Rook, Piece.King, Piece.Queen, Piece.Knight, Piece.Knight, Piece.Rook } },
            { 720, new Piece[]{ Piece.Rook, Piece.King, Piece.Knight, Piece.Queen, Piece.Knight, Piece.Rook } },
            { 736, new Piece[]{ Piece.Rook, Piece.King, Piece.Knight, Piece.Knight, Piece.Queen, Piece.Rook } },
            { 752, new Piece[]{ Piece.Rook, Piece.King, Piece.Knight, Piece.Knight, Piece.Rook, Piece.Queen } },
            { 768, new Piece[]{ Piece.Queen, Piece.Rook, Piece.King, Piece.Knight, Piece.Rook, Piece.Knight } },
            { 784, new Piece[]{ Piece.Rook, Piece.Queen, Piece.King, Piece.Knight, Piece.Rook, Piece.Knight } },
            { 800, new Piece[]{ Piece.Rook, Piece.King, Piece.Queen, Piece.Knight, Piece.Rook, Piece.Knight } },
            { 816, new Piece[]{ Piece.Rook, Piece.King, Piece.Knight, Piece.Queen, Piece.Rook, Piece.Knight } },
            { 832, new Piece[]{ Piece.Rook, Piece.King, Piece.Knight, Piece.Rook, Piece.Queen, Piece.Knight } },
            { 848, new Piece[]{ Piece.Rook, Piece.King, Piece.Knight, Piece.Rook, Piece.Knight, Piece.Queen } },
            { 864, new Piece[]{ Piece.Queen, Piece.Rook, Piece.King, Piece.Rook, Piece.Knight, Piece.Knight } },
            { 880, new Piece[]{ Piece.Rook, Piece.Queen, Piece.King, Piece.Rook, Piece.Knight, Piece.Knight } },
            { 896, new Piece[]{ Piece.Rook, Piece.King, Piece.Queen, Piece.Rook, Piece.Knight, Piece.Knight } },
            { 912, new Piece[]{ Piece.Rook, Piece.King, Piece.Rook, Piece.Queen, Piece.Knight, Piece.Knight } },
            { 928, new Piece[]{ Piece.Rook, Piece.King, Piece.Rook, Piece.Knight, Piece.Queen, Piece.Knight } },
            { 944, new Piece[]{ Piece.Rook, Piece.King, Piece.Rook, Piece.Knight, Piece.Knight, Piece.Queen } },
            { 960, new Piece[]{ Piece.Queen, Piece.Knight, Piece.Knight, Piece.Rook, Piece.King, Piece.Rook } },
        };

        private List<BoardCoord> BishopsTable = new List<BoardCoord> {
            new BoardCoord(0,1), new BoardCoord(0,3), new BoardCoord(0,5), new BoardCoord(0,7), new BoardCoord(1,2),
            new BoardCoord(2,3), new BoardCoord(2,5), new BoardCoord(2,7), new BoardCoord(1,4), new BoardCoord(3,4),
            new BoardCoord(4,5), new BoardCoord(4,7), new BoardCoord(1,6), new BoardCoord(3,6), new BoardCoord(5,6),
            new BoardCoord(6,7),
        };

        private Piece[] GetStartingPosition(int SPseed) {
            SPseed %= 959;
            int remainder = SPseed % 16;
            int factorof16 = SPseed - remainder;

            BoardCoord bishopPositions = BishopsTable[remainder];
            Piece[] piecePositions = KingsTable[factorof16];

            Piece[] pieceOrder = new Piece[8];
            int cnt = 0;
            for (int i = 0; i < pieceOrder.Length; i++) {
                if(i == bishopPositions.x || i == bishopPositions.y) {
                    pieceOrder[i] = Piece.Bishop;
                } else {
                    pieceOrder[i] = piecePositions[cnt];
                    cnt++;
                }
            }
            return pieceOrder;
        }

        public override void PopulateBoard() {
            int SPseed = new System.Random().Next(0, 960);

            RaiseEventOnLogInfoMessage(SPseed.ToString());

            Piece[] randomPieceOrder = GetStartingPosition(SPseed);

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (randomPieceOrder[x] == Piece.King) {
                    currentRoyalPiece = (King)AddNewPieceToBoard(randomPieceOrder[x], Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    opposingRoyalPiece = (King)AddNewPieceToBoard(randomPieceOrder[x], Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else {
                    AddNewPieceToBoard(randomPieceOrder[x], Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(randomPieceOrder[x], Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            bool kingCastlingThisMove = false;

            ChessPiece destinationOccupier = Board.GetCoordInfo(destination).GetAliveOccupier();
            // If the selected destination has a friendly rook occupying it, the move is a castling move.
            if (mover == currentRoyalPiece && mover.MoveCount == 0 && destinationOccupier is Rook) {
                // Switch the destination from the rook's position to the king's final castle position.
                kingCastlingThisMove = true;
                castlingRook = (Rook)destinationOccupier;

                if (destination.x < mover.GetBoardPosition().x) {
                    destination = new BoardCoord(2, destination.y);
                } else {
                    destination = new BoardCoord(6, destination.y);
                }
            }

            // Try make the move
            if (MakeBaseMove(mover, destination)) {
                if (kingCastlingThisMove) {
                    TryPerformCastlingMove((King)mover);
                } else if (mover is Pawn) {
                    TryPerformPawnEnPassantCapture((Pawn)mover);
                    TryPerformPawnPromotion((Pawn)mover);
                }
                return true;
            }
            return false;
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 2) {
                    UpdatePiecePositionAndOccupance(castlingRook, new BoardCoord(3, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 6) {
                    UpdatePiecePositionAndOccupance(castlingRook, new BoardCoord(5, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }

        protected override BoardCoord[] TryAddAvailableCastleMoves(ChessPiece king, Piece[] castlerOptions, int castlingDistance, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            // If king is not in check and hasn't moved, it can try castle moves.
            if (!IsPieceInCheck(king)) {
                List<BoardCoord> castleMoves = new List<BoardCoord>(2);

                BoardCoord CASTLE_ROOKPOS;
                BoardCoord CASTLE_KINGPOS;

                for (int i = LEFT; i <= RIGHT; i+=2) {
                    if (i == LEFT) {
                        if (!canCastleLeftward) continue;
                        CASTLE_KINGPOS = new BoardCoord(2, king.GetBoardPosition().y);
                        CASTLE_ROOKPOS = new BoardCoord(3, king.GetBoardPosition().y);
                    } else {
                        if (!canCastleRightward) break;
                        CASTLE_KINGPOS = new BoardCoord(6, king.GetBoardPosition().y);
                        CASTLE_ROOKPOS = new BoardCoord(5, king.GetBoardPosition().y);
                    }

                    // If the king would be in check after moving to the end square, it is not valid.
                    if (IsPieceInCheckAfterThisMove(king, king, CASTLE_KINGPOS)) {
                        continue;
                    }

                    int x = king.GetBoardPosition().x + i;
                    int y = king.GetBoardPosition().y;
                    BoardCoord coord = new BoardCoord(x, y);
                    Rook castlingRook = null;
                    uint obstructingPieces = 0;
                    bool validCastle = true;

                    // Check every square left and right of the king
                    while (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).GetAliveOccupier();
                        if (piece != null) {
                            // Count the pieces inbetween the king's starting position to its ending position
                            if((i == LEFT && coord.x >= CASTLE_KINGPOS.x) || (i == RIGHT && coord.x <= CASTLE_KINGPOS.x)) {
                                obstructingPieces++;
                                if(obstructingPieces > 1) {
                                    validCastle = false;
                                    break;
                                }
                            }
                            // If a friendly rook is found, it is the castling rook
                            if (piece is Rook && piece.MoveCount == 0 && king.IsAllyTowards(piece)) {
                                castlingRook = (Rook)piece;
                                if (castlingRook.GetBoardPosition() == CASTLE_KINGPOS) break;
                            } else {
                                break;
                            }
                        }
                        // If the king would be in check at any point inbetween the castling maneuver, then it is not valid.
                        if (IsPieceInCheckAfterThisMove(king, king, coord)
                            && ((i == LEFT && coord.x >= CASTLE_KINGPOS.x) || (i == RIGHT && coord.x <= CASTLE_KINGPOS.x))) {
                            validCastle = false;
                            break;
                        }

                        x += i;
                        coord = new BoardCoord(x, y);
                    }

                    // There must be only 1 piece obstructor or less and a castling rook found to continue
                    if(castlingRook != null && validCastle) {
                        ChessPiece currentRookPosOccupier = Board.GetCoordInfo(CASTLE_ROOKPOS).GetAliveOccupier();
                        ChessPiece currentKingPosOccupier = Board.GetCoordInfo(CASTLE_KINGPOS).GetAliveOccupier();

                        // If the final rook and king positions are either null, or occupied by a friendly king or castling rook, the castle is valid.
                        if (currentRookPosOccupier.IsAllyTowards(CASTLE_ROOKPOS) && currentRookPosOccupier != castlingRook && currentRookPosOccupier != king) {
                            continue;
                        } else if (currentKingPosOccupier == null || currentKingPosOccupier == castlingRook || currentKingPosOccupier == king) {
                            castleMoves.Add(castlingRook.GetBoardPosition());
                        }
                    }
                }
                return castleMoves.ToArray();
            }
            return new BoardCoord[0];
        }
    }
}
