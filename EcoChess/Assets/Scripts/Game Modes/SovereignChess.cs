using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// SovereignChess.cs is a chess variant involving multi-coloured armies to control.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: All sliding pieces may only move up to 8 squares at a time. 
    ///              Pawn enpassant is not allowed.
    ///              Pawns must move and capture towards the center, either horizontally or vertically.
    ///              Owned pawns may promote to a king, which removes the previous king from the board.
    ///              Pieces may not move to or over a square controlled by the opposing team unless to capture and gain control.
    ///              Pieces may not move to a square of its own colour.
    ///              Pieces may capture pieces to steal control from the opposing team.
    ///              Kings may switch teams to a controlled colour.
    /// Board layout:
    ///     Q B R N ? ? ? ? ? ? ? ? N R N Q
    ///     R P P P ? ? ? ? ? ? ? ? P P P R
    ///     B P . . . . . . . . . . . . P B
    ///     Q P . . . . . . . . . . . . P Q
    ///     R P . . r . . . . . . b . . P R     Capitals: neutral pieces
    ///     N P . . . y . p P . g . . . P N     ? top: FIDE black pieces
    ///     B P . . . . G . . s . . . . P B     ? bottom: FIDE white pieces
    ///     Q P . . . o . W B . a . . . P Q
    ///     Q P . . . a . B W . o . . . P Q     
    ///     B P . . . . s . . G . . . . P B
    ///     N P . . . g . P p . y . . . P N
    ///     R P . . b . . . . . . r . . P R
    ///     Q P . . . . . . . . . . . . P Q
    ///     B P . . . . . . . . . . . . P B
    ///     R P P P ? ? ? ? ? ? ? ? P P P R
    ///     Q N R N ? ? ? ? ? ? ? ? N R B Q
    ///   
    ///     r = red, y = yellow, g = green, b = blue, a = aqua, o = orange, P = Purple, p = pink, G = gray, s = silver, W = White, B = Black
    /// </summary>
    public class SovereignChess : FIDERuleset {
        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 16;

        private readonly static Color primaryBoardColour = new Color(1, 219f / 255f, 153f / 255f);
        private readonly static Color secondaryBoardColour = new Color(1, 237f / 255f, 204f / 255f);

        private readonly static Color Color_orange = new Color(0.9f, 0.58f, 0);
        private readonly static Color Color_lightblue = new Color(0.447f, 0.77f, 0.98f);
        private readonly static Color Color_silver = new Color(0.85f, 0.85f, 0.85f);
        private readonly static Color Color_pink = new Color(1, 0.45f, 0.71f);
        private readonly static Color Color_purple = new Color(0.6f, 0, 0.6f);

        private Dictionary<Color, BoardCoord[]> ColourControlSquares = new Dictionary<Color, BoardCoord[]>(24);

        public SovereignChess() : base(BOARD_WIDTH, BOARD_HEIGHT, primaryBoardColour, secondaryBoardColour) {
            //Red
            AddColourControlSquares("e12", "l5", Color.red);

            //Blue
            AddColourControlSquares("e5", "l12", Color.blue);

            //Yellow
            AddColourControlSquares("f11","k6", Color.yellow);

            //Green
            AddColourControlSquares("f6", "k11", Color.green);

            //Pink
            AddColourControlSquares("h11", "i6", Color_pink);

            //Purple
            AddColourControlSquares("i11", "h6", Color_purple);

            //Grey
            AddColourControlSquares("g10", "j7", Color.grey);
            
            //Silver
            AddColourControlSquares("g7", "j10", Color_silver);

            //Orange
            AddColourControlSquares("f9", "k8", Color_orange);

            //Light blue
            AddColourControlSquares("f8", "k9", Color_lightblue);

            //White
            AddColourControlSquares("h9", "i8", Color.white);

            //Black
            AddColourControlSquares("i9", "h8", Color.black);
        }

        public override string ToString() {
            return "Sovereign Chess";
        }

        private void AddColourControlSquares(string algebraicKey, string algebraicKey2, Color color) {
            BoardCoord coord1;
            if (board.TryGetCoordWithKey(algebraicKey, out coord1)) {
                board.GetCoordInfo(coord1).boardChunk.GetComponent<MeshRenderer>().material.color = color;
            }
            BoardCoord coord2;
            if (board.TryGetCoordWithKey(algebraicKey2, out coord2)) {
                board.GetCoordInfo(coord2).boardChunk.GetComponent<MeshRenderer>().material.color = color;
            }
            ColourControlSquares.Add(color, new BoardCoord[2] { coord1, coord2 });
        }

        private void AddSovereignChessPiece(Piece piece, string algebraicKey, Color color) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKey, out coord)) {
                ChessPiece sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.WHITE, coord), false);
                if (sovereignPiece != null) {
                    sovereignPiece.gameObject.GetComponent<SpriteRenderer>().material.color = color;
                }
            }
        }

        private void AddSovereignPawn(string algebraicKey, Color color, SovereignPawn.Quadrant quadrant) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKey, out coord)) {
                ChessPiece sovereignPawn = AddPieceToBoard(new SovereignPawn(Team.WHITE, coord, quadrant), false);
                if (sovereignPawn != null) {
                    sovereignPawn.gameObject.GetComponent<SpriteRenderer>().material.color = color;
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            bool cancelDirectionalSlide = false;
            for (int i = 0; i < templateMoves.Length; i++) {
                if(Mathf.Abs(mover.GetBoardPosition().x - templateMoves[i].x) > 8 || Mathf.Abs(mover.GetBoardPosition().y - templateMoves[i].y) > 8) {
                    continue;
                }

                if (i > 0 && (Mathf.Abs(templateMoves[i].x - templateMoves[i - 1].x) > 1 || Mathf.Abs(templateMoves[i].y - templateMoves[i - 1].y) > 1)) {
                    cancelDirectionalSlide = false;
                }

                if (cancelDirectionalSlide == false) {
                    if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                        BoardCoord move = TryGetValidMove(mover, templateMoves[i], out cancelDirectionalSlide);
                        if (move != BoardCoord.NULL) {
                            availableMoves.Add(move);
                        } else if ((mover is Queen || mover is Rook || mover is Bishop) == false) {
                            cancelDirectionalSlide = false;
                        }
                    }
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        private BoardCoord TryGetValidMove(ChessPiece mover, BoardCoord templateMove, out bool cancelDirectionalSlide) {
            BoardCoord[] positions = new BoardCoord[2];
            Color movedToColour = board.GetCoordInfo(templateMove).boardChunk.GetComponent<MeshRenderer>().material.color;
            if (ColourControlSquares.TryGetValue(movedToColour, out positions)) {
                bool controlledByMover = false;
                bool controlledByAlly = false;
                bool controlNeutral = false;
                ChessPiece firstOccupier = board.GetCoordInfo(positions[0]).occupier;
                ChessPiece secondOccupier = board.GetCoordInfo(positions[1]).occupier;
                if (firstOccupier == null && secondOccupier == null) {
                    controlNeutral = true;
                } else if (mover.GetBoardPosition() == positions[0] || mover.GetBoardPosition() == positions[1]) {
                    controlledByMover = true;
                } else if (IsAlly(mover, positions[0]) || IsAlly(mover, positions[1])) {
                    controlledByAlly = true;
                }

                if (controlledByMover || IsThreat(mover, templateMove) || controlNeutral) {
                    cancelDirectionalSlide = false;
                    return templateMove;
                } else {
                    cancelDirectionalSlide = (controlledByAlly) ? false : true;
                    return BoardCoord.NULL;
                }
            }
            cancelDirectionalSlide = false;
            return templateMove;
        }

        public override void PopulateBoard() {
#region WHITE+BLACK Teams
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(8, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(8, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(11, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(11, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x > 3 && x < 12) {
                    AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                    AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));
                }

                if (x == 5 || x == 10) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 6 || x == 9) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
            #endregion

            SovereignPawn.Quadrant currentQuadrant = SovereignPawn.Quadrant.BottomLeft;
            AddSovereignPawn("c2", Color_pink, currentQuadrant);
            AddSovereignPawn("d2", Color_pink, currentQuadrant);
            AddSovereignPawn("b3", Color.red, currentQuadrant);
            AddSovereignPawn("b4", Color.red, currentQuadrant);
            AddSovereignPawn("b5", Color_orange, currentQuadrant);
            AddSovereignPawn("b6", Color_orange, currentQuadrant);
            AddSovereignPawn("b7", Color.yellow, currentQuadrant);
            AddSovereignPawn("b8", Color.yellow, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.UpLeft;
            AddSovereignPawn("b9", Color.green, currentQuadrant);
            AddSovereignPawn("b10", Color.green, currentQuadrant);
            AddSovereignPawn("b11", Color_lightblue, currentQuadrant);
            AddSovereignPawn("b12", Color_lightblue, currentQuadrant);
            AddSovereignPawn("b13", Color.blue, currentQuadrant);
            AddSovereignPawn("b14", Color.blue, currentQuadrant);
            AddSovereignPawn("c15", Color_purple, currentQuadrant);
            AddSovereignPawn("d15", Color_purple, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.BottomRight;
            AddSovereignPawn("m2", Color.green, currentQuadrant);
            AddSovereignPawn("n2", Color.green, currentQuadrant);
            AddSovereignPawn("o3", Color_lightblue, currentQuadrant);
            AddSovereignPawn("o4", Color_lightblue, currentQuadrant);
            AddSovereignPawn("o5", Color.blue, currentQuadrant);
            AddSovereignPawn("o6", Color.blue, currentQuadrant);
            AddSovereignPawn("o7", Color_purple, currentQuadrant);
            AddSovereignPawn("o8", Color_purple, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.UpRight;
            AddSovereignPawn("o9", Color_pink, currentQuadrant);
            AddSovereignPawn("o10", Color_pink, currentQuadrant);
            AddSovereignPawn("o11", Color.red, currentQuadrant);
            AddSovereignPawn("o12", Color.red, currentQuadrant);
            AddSovereignPawn("o13", Color_orange, currentQuadrant);
            AddSovereignPawn("o14", Color_orange, currentQuadrant);
            AddSovereignPawn("m15", Color.yellow, currentQuadrant);
            AddSovereignPawn("n15", Color.yellow, currentQuadrant);

            //-------------------------------

            AddSovereignChessPiece(Piece.Queen, "a1", Color.grey);
            AddSovereignChessPiece(Piece.Rook, "a2", Color.grey);
            AddSovereignChessPiece(Piece.Bishop, "a3", Color.red);
            AddSovereignChessPiece(Piece.Queen, "a4", Color.red);
            AddSovereignChessPiece(Piece.Rook, "a5", Color_orange);
            AddSovereignChessPiece(Piece.Knight, "a6", Color_orange);
            AddSovereignChessPiece(Piece.Bishop, "a7", Color.yellow);
            AddSovereignChessPiece(Piece.Queen, "a8", Color.yellow);
            AddSovereignChessPiece(Piece.Queen, "a9", Color.green);
            AddSovereignChessPiece(Piece.Bishop, "a10", Color.green);
            AddSovereignChessPiece(Piece.Knight, "a11", Color_lightblue);
            AddSovereignChessPiece(Piece.Rook, "a12", Color_lightblue);
            AddSovereignChessPiece(Piece.Queen, "a13", Color.blue);
            AddSovereignChessPiece(Piece.Bishop, "a14", Color.blue);
            AddSovereignChessPiece(Piece.Rook, "a15", Color_silver);
            AddSovereignChessPiece(Piece.Queen, "a16", Color_silver);

            AddSovereignChessPiece(Piece.Bishop, "b1", Color.grey);
            AddSovereignChessPiece(Piece.Knight, "b2", Color.grey);
            AddSovereignChessPiece(Piece.Knight, "b15", Color_silver);
            AddSovereignChessPiece(Piece.Bishop, "b16", Color_silver);

            AddSovereignChessPiece(Piece.Rook, "c1", Color_pink);
            AddSovereignChessPiece(Piece.Knight, "d1", Color_pink);
            AddSovereignChessPiece(Piece.Rook, "c16", Color_purple);
            AddSovereignChessPiece(Piece.Knight, "d16", Color_purple);

            //--------------------------

            AddSovereignChessPiece(Piece.Queen, "p1", Color_silver);
            AddSovereignChessPiece(Piece.Rook, "p2", Color_silver);
            AddSovereignChessPiece(Piece.Bishop, "p3", Color_lightblue);
            AddSovereignChessPiece(Piece.Queen, "p4", Color_lightblue);
            AddSovereignChessPiece(Piece.Rook, "p5", Color.blue);
            AddSovereignChessPiece(Piece.Knight, "p6", Color.blue);
            AddSovereignChessPiece(Piece.Bishop, "p7", Color_purple);
            AddSovereignChessPiece(Piece.Queen, "p8", Color_purple);
            AddSovereignChessPiece(Piece.Queen, "p9", Color_pink);
            AddSovereignChessPiece(Piece.Bishop, "p10", Color_pink);
            AddSovereignChessPiece(Piece.Knight, "p11", Color.red);
            AddSovereignChessPiece(Piece.Rook, "p12", Color.red);
            AddSovereignChessPiece(Piece.Queen, "p13", Color_orange);
            AddSovereignChessPiece(Piece.Bishop, "p14", Color_orange);
            AddSovereignChessPiece(Piece.Rook, "p15", Color.grey);
            AddSovereignChessPiece(Piece.Queen, "p16", Color.grey);

            AddSovereignChessPiece(Piece.Bishop, "o1", Color_silver);
            AddSovereignChessPiece(Piece.Knight, "o2", Color_silver);
            AddSovereignChessPiece(Piece.Knight, "o15", Color.grey);
            AddSovereignChessPiece(Piece.Bishop, "o16", Color.grey);

            AddSovereignChessPiece(Piece.Knight, "m1", Color.green);
            AddSovereignChessPiece(Piece.Rook, "n1", Color.green);
            AddSovereignChessPiece(Piece.Knight, "m16", Color.yellow);
            AddSovereignChessPiece(Piece.Rook, "n16", Color.yellow);
        }
    }
}
