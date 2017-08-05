﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// SovereignChess.cs is a chess variant involving multi-coloured armies to control on a 16x16 board.
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

        private List<BoardCoord> promotionSquares;

        private readonly static Color primaryBoardColour = new Color(1, 219f / 255f, 153f / 255f);
        private readonly static Color secondaryBoardColour = new Color(1, 237f / 255f, 204f / 255f);

        private readonly Color Color_orange = new Color(0.9f, 0.58f, 0);
        private readonly Color Color_lightblue = new Color(0.447f, 0.77f, 0.98f);
        private readonly Color Color_silver = new Color(0.85f, 0.85f, 0.85f);
        private readonly Color Color_pink = new Color(1, 0.45f, 0.71f);
        private readonly Color Color_purple = new Color(0.6f, 0, 0.6f);

        private Color whiteCurrentOwnedColour = Color.white;
        private Color blackCurrentOwnedColour = Color.black;

        private HashSet<Color> whiteControlledColours = new HashSet<Color>();
        private HashSet<Color> blackControlledColours = new HashSet<Color>();

        private Dictionary<Color, BoardCoord[]> ColourControlSquares = new Dictionary<Color, BoardCoord[]>(24);

        public SovereignChess() : base(BOARD_WIDTH, BOARD_HEIGHT, primaryBoardColour, secondaryBoardColour) {
            pawnPromotionOptions = new Piece[5] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.King };

            whiteControlledColours.Add(Color.white);
            blackControlledColours.Add(Color.black);

            //Colour control squares
            AddColourControlSquares("e12", "l5", Color.red);
            AddColourControlSquares("e5", "l12", Color.blue);
            AddColourControlSquares("f11","k6", Color.yellow);
            AddColourControlSquares("f6", "k11", Color.green);
            AddColourControlSquares("h11", "i6", Color_pink);
            AddColourControlSquares("i11", "h6", Color_purple);
            AddColourControlSquares("g10", "j7", Color.grey);
            AddColourControlSquares("g7", "j10", Color_silver);
            AddColourControlSquares("f9", "k8", Color_orange);
            AddColourControlSquares("f8", "k9", Color_lightblue);
            AddColourControlSquares("h9", "i8", Color.white);
            AddColourControlSquares("i9", "h8", Color.black);

            //Promotion squares
            promotionSquares = new List<BoardCoord>(16);
            AddPromotionSquare("g7");
            AddPromotionSquare("g8");
            AddPromotionSquare("g9");
            AddPromotionSquare("g10");
            AddPromotionSquare("h7");
            AddPromotionSquare("h8");
            AddPromotionSquare("h9");
            AddPromotionSquare("h10");
            AddPromotionSquare("i7");
            AddPromotionSquare("i8");
            AddPromotionSquare("i9");
            AddPromotionSquare("i10");
            AddPromotionSquare("j7");
            AddPromotionSquare("j8");
            AddPromotionSquare("j9");
            AddPromotionSquare("j10");
        }

        public override string ToString() {
            return "Sovereign Chess";
        }

        private void AddPromotionSquare(string algebraicKeyPosition) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKeyPosition, out coord)) {
                promotionSquares.Add(coord);
            }
        }

        protected override bool CanPromote(Pawn mover, BoardCoord[] availableMoves) {
            for (int i = 0; i < availableMoves.Length; i++) {
                if (promotionSquares.Contains(availableMoves[i])) {
                    return true;
                }
            }
            return false;
        }

        protected override ChessPiece CheckPawnPromotion(Pawn mover) {
            if (promotionSquares.Contains(mover.GetBoardPosition())) {
                RemovePieceFromBoard(mover);
                RemovePieceFromActiveTeam(mover);
                return AddSovereignChessPiece(selectedPawnPromotion, mover.GetBoardPosition(), GetChessPieceColour(mover));
            }
            return null;
        }

        public override void SetPawnPromotionTo(Piece piece) {
            base.SetPawnPromotionTo(piece);
            MouseController.Instance.CalculateLastOccupierAvailableMoves();
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

        private ChessPiece AddSovereignChessPiece(Piece piece, string algebraicKey, Color color) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKey, out coord)) {
                ChessPiece sovereignPiece;
                if (color == Color.black) {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.BLACK, coord));
                } else {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.WHITE, coord));
                }
                if (sovereignPiece != null) {
                    if (color != Color.black) {
                        sovereignPiece.gameObject.GetComponent<SpriteRenderer>().material.color = color;
                    }
                    return sovereignPiece;
                }
            }
            return null;
        }

        private ChessPiece AddSovereignChessPiece(Piece piece, BoardCoord coord, Color color) {
            if (board.ContainsCoord(coord)) {
                ChessPiece sovereignPiece;
                if (color == Color.black) {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.BLACK, coord));
                } else {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.WHITE, coord));
                }
                if (sovereignPiece != null) {
                    if(color != Color.black) {
                        sovereignPiece.gameObject.GetComponent<SpriteRenderer>().material.color = color;
                    }
                    return sovereignPiece;
                }
            }
            return null;
        }

        private SovereignPawn AddSovereignPawn(string algebraicKey, Color color, SovereignPawn.Quadrant quadrant) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKey, out coord)) {
                SovereignPawn sovereignPawn;
                if (color == Color.black) {
                    sovereignPawn = (SovereignPawn)AddPieceToBoard(new SovereignPawn(Team.BLACK, coord, quadrant));
                } else {
                    sovereignPawn = (SovereignPawn)AddPieceToBoard(new SovereignPawn(Team.WHITE, coord, quadrant));
                }
                if (sovereignPawn != null) {
                    if (color != Color.black) {
                        sovereignPawn.gameObject.GetComponent<SpriteRenderer>().material.color = color;
                    }
                    return sovereignPawn;
                }
            }
            return null;
        }

        public override bool IsMoversTurn(ChessPiece mover) {
            if(currentTeamTurn == Team.WHITE) {
                return whiteControlledColours.Contains(GetChessPieceColour(mover));
            } else {
                return blackControlledColours.Contains(GetChessPieceColour(mover));
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

                if (mover is SovereignPawn && IsThreat(mover, templateMoves[i]) == false) {
                    if (QuadrantBoundariesExceeded((SovereignPawn)mover, templateMoves[i])) {
                        continue;
                    }
                }

                if (i > 0 && (Mathf.Abs(templateMoves[i].x - templateMoves[i - 1].x) > 1 || Mathf.Abs(templateMoves[i].y - templateMoves[i - 1].y) > 1)) {
                    cancelDirectionalSlide = false;
                }

                if (cancelDirectionalSlide == false) {
                    if (selectedPawnPromotion == Piece.King && mover is SovereignPawn && promotionSquares.Contains(templateMoves[i]) 
                        && IsInCheckAfterPromotion((SovereignPawn)mover, templateMoves[i])) {
                        continue;
                    } else if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
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

        private bool IsInCheckAfterPromotion(SovereignPawn mover, BoardCoord templateMove) {
            if (AssertContainsCoord(templateMove)) {
                if (checkingForCheck) return false;
                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = board.GetCoordInfo(templateMove).occupier;
                ChessPiece originalLastMover;
                BoardCoord oldPos = mover.GetBoardPosition();
                SimulateMove(mover, templateMove, originalOccupier, out originalLastMover);

                // Check whether the piece is in check after this temporary move
                bool isInCheck = IsPieceInCheck(mover);

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, templateMove, originalOccupier, originalLastMover, oldPos);

                return isInCheck;
            }
            return false;
        }

        protected override bool IsThreat(ChessPiece mover, BoardCoord coord) {
            if (AssertContainsCoord(coord)) {
                ChessPiece occupier = board.GetCoordInfo(coord).occupier;
                if (occupier != null) {
                    if(whiteControlledColours.Contains(GetChessPieceColour(mover))) {
                        return blackControlledColours.Contains(GetChessPieceColour(occupier));
                    } else if (blackControlledColours.Contains(GetChessPieceColour(mover))) {
                        return whiteControlledColours.Contains(GetChessPieceColour(occupier));
                    }
                } else {
                    return false;
                }
            }
            return false;
        }

        protected override bool IsAlly(ChessPiece mover, BoardCoord coord) {
            if (AssertContainsCoord(coord)) {
                ChessPiece occupier = board.GetCoordInfo(coord).occupier;
                if (occupier != null) {
                    if (whiteControlledColours.Contains(GetChessPieceColour(mover))) {
                        return blackControlledColours.Contains(GetChessPieceColour(occupier)) == false;
                    } else if (blackControlledColours.Contains(GetChessPieceColour(mover))) {
                        return whiteControlledColours.Contains(GetChessPieceColour(occupier)) == false;
                    } else {
                        return true;
                    }
                } else {
                    return false;
                }
            }
            return false;
        }

        public override bool CheckWinState() {
            if (numConsecutiveCapturelessMoves == 100) {
                UIManager.Instance.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            foreach (ChessPiece piece in GetPieces()) {
                if(currentTeamTurn == Team.WHITE) {
                    if (whiteControlledColours.Contains(GetChessPieceColour(piece))) {
                        if (CalculateAvailableMoves(piece).Count > 0) return false;
                    }
                } else {
                    if (blackControlledColours.Contains(GetChessPieceColour(piece))) {
                        if (CalculateAvailableMoves(piece).Count > 0) return false;
                    }
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord[] positions = new BoardCoord[2];
            Color movedFromColour = board.GetCoordInfo(mover.GetBoardPosition()).boardChunk.GetComponent<MeshRenderer>().material.color;
            // Try make the move
            if (MakeMove(mover, destination)) {
                // Check castling moves
                if (mover is King && mover.MoveCount == 1) {
                    TryPerformCastlingRookMoves(mover);
                } else if (mover is Pawn) {
                    if(mover is SovereignPawn) UpdatePawnQuadrant((SovereignPawn)mover);
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        mover = promotedPiece;
                        if(mover is King) {
                            RemovePieceFromBoard(currentRoyalPiece);
                            if (currentTeamTurn == Team.WHITE) {
                                whiteControlledColours.Remove(GetChessPieceColour(currentRoyalPiece));
                                whiteCurrentOwnedColour = GetChessPieceColour(mover);
                            } else {
                                blackControlledColours.Remove(GetChessPieceColour(currentRoyalPiece));
                                blackCurrentOwnedColour = GetChessPieceColour(mover);
                            }
                            currentRoyalPiece = mover;
                        }
                    }
                }

                if (movedFromColour != whiteCurrentOwnedColour && movedFromColour != blackCurrentOwnedColour) {
                    if (ColourControlSquares.TryGetValue(movedFromColour, out positions)) {
                        if (whiteControlledColours.Contains(GetChessPieceColour(mover))) {
                            whiteControlledColours.Remove(movedFromColour);
                        } else {
                            blackControlledColours.Remove(movedFromColour);
                        }
                    }
                }

                Color destinationColour = board.GetCoordInfo(destination).boardChunk.GetComponent<MeshRenderer>().material.color;
                if (destinationColour != whiteCurrentOwnedColour && destinationColour != blackCurrentOwnedColour) {
                    if (ColourControlSquares.TryGetValue(destinationColour, out positions)) {
                        if (whiteControlledColours.Contains(GetChessPieceColour(mover))) {
                            blackControlledColours.Remove(destinationColour);
                            whiteControlledColours.Add(destinationColour);
                        } else {
                            whiteControlledColours.Remove(destinationColour);
                            blackControlledColours.Add(destinationColour);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private void UpdatePawnQuadrant(SovereignPawn pawn) {
            const int X_RIGHTSIDELIMIT = 8;
            const int X_LEFTSIDELIMIT = 7;
            const int Y_ABOVESIDELIMIT = 8;
            const int Y_BOTTOMSIDELIMIT = 7;

            switch (pawn.pieceQuadrant) {
                case SovereignPawn.Quadrant.BottomLeft:
                    if (pawn.GetBoardPosition().x > X_LEFTSIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.BottomRight);
                    else if (pawn.GetBoardPosition().y > Y_BOTTOMSIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.TopLeft);
                    break;
                case SovereignPawn.Quadrant.BottomRight:
                    if (pawn.GetBoardPosition().x < X_RIGHTSIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.BottomLeft);
                    else if (pawn.GetBoardPosition().y > Y_BOTTOMSIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.TopRight);
                    break;
                case SovereignPawn.Quadrant.TopLeft:
                    if (pawn.GetBoardPosition().x > X_LEFTSIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.TopRight);
                    else if (pawn.GetBoardPosition().y < Y_ABOVESIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.BottomLeft);
                    break;
                case SovereignPawn.Quadrant.TopRight:
                    if (pawn.GetBoardPosition().x < X_RIGHTSIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.TopLeft);
                    else if (pawn.GetBoardPosition().y < Y_ABOVESIDELIMIT) pawn.ChangePieceQuadrant(SovereignPawn.Quadrant.BottomRight);
                    break;
                default:
                    break;
            }
        }

        private bool QuadrantBoundariesExceeded(SovereignPawn pawn, BoardCoord move) {
            const int X_RIGHTSIDELIMIT = 8;
            const int X_LEFTSIDELIMIT = 7;
            const int Y_ABOVESIDELIMIT = 8;
            const int Y_BOTTOMSIDELIMIT = 7;

            switch (pawn.pieceQuadrant) {
                case SovereignPawn.Quadrant.BottomLeft:
                    if (move.x > X_LEFTSIDELIMIT || move.y > Y_BOTTOMSIDELIMIT) return true;
                    break;
                case SovereignPawn.Quadrant.BottomRight:
                    if (move.x < X_RIGHTSIDELIMIT || move.y > Y_BOTTOMSIDELIMIT) return true;
                    break;
                case SovereignPawn.Quadrant.TopLeft:
                    if (move.x > X_LEFTSIDELIMIT || move.y < Y_ABOVESIDELIMIT) return true;
                    break;
                case SovereignPawn.Quadrant.TopRight:
                    if (move.x < X_RIGHTSIDELIMIT || move.y < Y_ABOVESIDELIMIT) return true;
                    break;
                default:
                    break;
            }
            return false;
        }

        private Color GetChessPieceColour(ChessPiece piece) {
            if (piece.GetTeam() == Team.BLACK) return Color.black;
            return piece.gameObject.GetComponent<SpriteRenderer>().material.color;
        }

        private BoardCoord TryGetValidMove(ChessPiece mover, BoardCoord templateMove, out bool cancelDirectionalSlide) {
            BoardCoord[] positions = new BoardCoord[2];
            Color movedToColour = board.GetCoordInfo(templateMove).boardChunk.GetComponent<MeshRenderer>().material.color;

            if (ColourControlSquares.TryGetValue(movedToColour, out positions)) {
                bool controlledByAlly = false;
                ChessPiece firstOccupier = board.GetCoordInfo(positions[0]).occupier;
                ChessPiece secondOccupier = board.GetCoordInfo(positions[1]).occupier;

                if ((firstOccupier == null && secondOccupier == null) || IsThreat(mover, templateMove)) {
                    cancelDirectionalSlide = false;
                    if (whiteControlledColours.Contains(GetChessPieceColour(mover)) && movedToColour == whiteCurrentOwnedColour
                        || blackControlledColours.Contains(GetChessPieceColour(mover)) && movedToColour == blackCurrentOwnedColour) {
                        return BoardCoord.NULL;
                    }
                    return templateMove;
                } else if (mover.GetBoardPosition() == positions[0] || mover.GetBoardPosition() == positions[1]) {
                    cancelDirectionalSlide = false;
                    return templateMove;
                } else if (IsAlly(mover, positions[0]) || IsAlly(mover, positions[1])) {
                    controlledByAlly = true;
                }

                cancelDirectionalSlide = (controlledByAlly) ? false : true;
                return BoardCoord.NULL;
            } else {
                cancelDirectionalSlide = false;
                return templateMove;
            }
        }

        public override void PopulateBoard() {
#region WHITE+BLACK Teams
            currentRoyalPiece = (King)AddSovereignChessPiece(Piece.King, "i1", Color.white);
            opposingRoyalPiece = (King)AddSovereignChessPiece(Piece.King, "i16", Color.black);

            aSideWhiteRook = (Rook)AddSovereignChessPiece(Piece.Rook, "e1", Color.white);
            hSideWhiteRook = (Rook)AddSovereignChessPiece(Piece.Rook, "l1", Color.white);
            aSideBlackRook = (Rook)AddSovereignChessPiece(Piece.Rook, "e16", Color.black);
            hSideBlackRook = (Rook)AddSovereignChessPiece(Piece.Rook, "l16", Color.black);

            AddSovereignChessPiece(Piece.Queen, "h1", Color.white);
            AddSovereignChessPiece(Piece.Queen, "h16", Color.black);

            AddSovereignPawn("e2", Color.white, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("f2", Color.white, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("g2", Color.white, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("h2", Color.white, SovereignPawn.Quadrant.BottomLeft);

            AddSovereignPawn("i2", Color.white, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("j2", Color.white, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("k2", Color.white, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("l2", Color.white, SovereignPawn.Quadrant.BottomRight);

            AddSovereignPawn("e15", Color.black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("f15", Color.black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("g15", Color.black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("h15", Color.black, SovereignPawn.Quadrant.TopLeft);

            AddSovereignPawn("i15", Color.black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("j15", Color.black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("k15", Color.black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("l15", Color.black, SovereignPawn.Quadrant.TopRight);

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x == 5 || x == 10) {
                    AddSovereignChessPiece(Piece.Knight, new BoardCoord(x, WHITE_BACKROW), Color.white);
                    AddSovereignChessPiece(Piece.Knight, new BoardCoord(x, BLACK_BACKROW), Color.black);
                } else if (x == 6 || x == 9) {
                    AddSovereignChessPiece(Piece.Bishop, new BoardCoord(x, WHITE_BACKROW), Color.white);
                    AddSovereignChessPiece(Piece.Bishop, new BoardCoord(x, BLACK_BACKROW), Color.black);
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

            currentQuadrant = SovereignPawn.Quadrant.TopLeft;
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

            currentQuadrant = SovereignPawn.Quadrant.TopRight;
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
