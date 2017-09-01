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
    ///              Controlled pawns may promote to a king, which removes the previous king from the board and re-sets the owned colour.
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
        public static event Action<bool> _DisplayDefectionUI;
        public static event Action<Color[]> _SetDefectionOptions;
        public List<Color> defectionOptions { get; protected set; }
        public Color selectedDefection { get; protected set; }

        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 16;

        private List<BoardCoord> promotionSquares;

        private readonly static Color primaryBoardColour = new Color(1, 219f / 255f, 153f / 255f);
        private readonly static Color secondaryBoardColour = new Color(1, 237f / 255f, 204f / 255f);

        private bool kingHasDoubleMoveDefection = false;
        private Color whiteCurrentOwnedColour = Color.white;
        private Color blackCurrentOwnedColour = Color.black;
        private HashSet<Color> whiteControlledColours = new HashSet<Color>();
        private HashSet<Color> blackControlledColours = new HashSet<Color>();

        private readonly Dictionary<Color, BoardCoord[]> ColourControlSquares = new Dictionary<Color, BoardCoord[]>(24);

        public SovereignChess() : base(BOARD_WIDTH, BOARD_HEIGHT, primaryBoardColour, secondaryBoardColour) {
            pawnPromotionOptions = new Piece[5] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.King };
            defectionOptions = new List<Color>();
            selectedDefection = whiteCurrentOwnedColour;

            whiteControlledColours.Add(Color.white);
            blackControlledColours.Add(Color.black);

            //Colour control squares
            AddColourControlSquares("e12", "l5", ColourName.Red);
            AddColourControlSquares("e5", "l12", ColourName.Blue);
            AddColourControlSquares("f11","k6", ColourName.Yellow);
            AddColourControlSquares("f6", "k11", ColourName.Green);
            AddColourControlSquares("h11", "i6", ColourName.Pink);
            AddColourControlSquares("i11", "h6", ColourName.Purple);
            AddColourControlSquares("g10", "j7", ColourName.Grey);
            AddColourControlSquares("g7", "j10", ColourName.Silver);
            AddColourControlSquares("f9", "k8", ColourName.Orange);
            AddColourControlSquares("f8", "k9", ColourName.Lightblue);
            AddColourControlSquares("h9", "i8", ColourName.White);
            AddColourControlSquares("i9", "h8", ColourName.Black);

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

        public override void OnTurnComplete() {
            if (kingHasDoubleMoveDefection == false) {
                base.OnTurnComplete();
            }
            selectedDefection = (currentTeamTurn == Team.WHITE) ? whiteCurrentOwnedColour : blackCurrentOwnedColour;
        }

        protected override ChessPiece CheckPawnPromotion(Pawn mover) {
            if (promotionSquares.Contains(mover.GetBoardPosition())) {
                RemovePieceFromBoard(mover);
                RemovePieceFromActiveTeam(mover);
                return AddSovereignChessPiece(selectedPawnPromotion, mover.GetBoardPosition(), mover.gameObject.GetComponent<SovereignColour>().colour);
            }
            return null;
        }

        public override void SetPawnPromotionTo(Piece piece) {
            base.SetPawnPromotionTo(piece);
            MouseController.Instance.CalculateLastOccupierAvailableMoves();
        }

        private void AddColourControlSquares(string algebraicKey, string algebraicKey2, ColourName color) {
            BoardCoord coord1;
            if (board.TryGetCoordWithKey(algebraicKey, out coord1)) {
                board.GetCoordInfo(coord1).boardChunk.GetComponent<MeshRenderer>().material.color = SovereignExtensions.GetColour(color);
            }
            BoardCoord coord2;
            if (board.TryGetCoordWithKey(algebraicKey2, out coord2)) {
                board.GetCoordInfo(coord2).boardChunk.GetComponent<MeshRenderer>().material.color = SovereignExtensions.GetColour(color);
            }
            ColourControlSquares.Add(SovereignExtensions.GetColour(color), new BoardCoord[2] { coord1, coord2 });
        }

        private ChessPiece AddSovereignChessPiece(Piece piece, string algebraicKey, ColourName color) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKey, out coord)) {
                ChessPiece sovereignPiece;
                if (color == ColourName.Black) {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.BLACK, coord));
                } else {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.WHITE, coord));
                }
                if (sovereignPiece != null) {
                    if (color != ColourName.Black) {
                        sovereignPiece.gameObject.GetComponent<SpriteRenderer>().material.color = SovereignExtensions.GetColour(color);
                    }
                    sovereignPiece.gameObject.AddComponent<SovereignColour>().colour = color;
                    return sovereignPiece;
                }
            }
            return null;
        }

        private ChessPiece AddSovereignChessPiece(Piece piece, BoardCoord coord, ColourName color) {
            if (board.ContainsCoord(coord)) {
                ChessPiece sovereignPiece;
                if (color == ColourName.Black) {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.BLACK, coord));
                } else {
                    sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, Team.WHITE, coord));
                }
                if (sovereignPiece != null) {
                    if(color != ColourName.Black) {
                        sovereignPiece.gameObject.GetComponent<SpriteRenderer>().material.color = SovereignExtensions.GetColour(color);
                    }
                    sovereignPiece.gameObject.AddComponent<SovereignColour>().colour = color;
                    return sovereignPiece;
                }
            }
            return null;
        }

        private SovereignPawn AddSovereignPawn(string algebraicKey, ColourName color, SovereignPawn.Quadrant quadrant) {
            BoardCoord coord;
            if (board.TryGetCoordWithKey(algebraicKey, out coord)) {
                SovereignPawn sovereignPawn;
                if (color == ColourName.Black) {
                    sovereignPawn = (SovereignPawn)AddPieceToBoard(new SovereignPawn(Team.BLACK, coord, quadrant));
                } else {
                    sovereignPawn = (SovereignPawn)AddPieceToBoard(new SovereignPawn(Team.WHITE, coord, quadrant));
                }
                if (sovereignPawn != null) {
                    if (color != ColourName.Black) {
                        sovereignPawn.gameObject.GetComponent<SpriteRenderer>().material.color = SovereignExtensions.GetColour(color);
                    }
                    sovereignPawn.gameObject.AddComponent<SovereignColour>().colour = color;
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
            if (kingHasDoubleMoveDefection && (mover is King) == false) {
                return availableMoves;
            }

            bool cancelDirectionalSlide = false;
            for (int i = 0; i < templateMoves.Length; i++) {
                if(Mathf.Abs(mover.GetBoardPosition().x - templateMoves[i].x) > 8 || Mathf.Abs(mover.GetBoardPosition().y - templateMoves[i].y) > 8) {
                    continue;
                }
                
                if (mover is SovereignPawn) {
                    if (selectedPawnPromotion == Piece.King && promotionSquares.Contains(templateMoves[i]) && IsPieceInCheckAfterThisMove(mover, mover, templateMoves[i])) {
                        continue;
                    } else if (IsThreat(mover, templateMoves[i]) == false && QuadrantBoundariesExceeded((SovereignPawn)mover, templateMoves[i])) {
                        continue;
                    }
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

            if (mover is King) {
                if (checkingForCheck == false) {
                    HashSet<Color> controlledColours = GetControlledColours(mover);
                    if (controlledColours.Count > 1) {
                        Color[] clrs = new Color[controlledColours.Count];
                        controlledColours.CopyTo(clrs);
                        defectionOptions = new List<Color>(clrs);
                        defectionOptions.Remove(GetTeamOwnedColour(mover));
                        if (selectedDefection == GetTeamOwnedColour(mover)) {
                            selectedDefection = defectionOptions[0];
                        }

                        BoardCoord move = TryAddDefectionMove(mover);
                        if (move != BoardCoord.NULL) {
                            availableMoves.Add(move);
                            if (_SetDefectionOptions != null) {
                                _SetDefectionOptions.Invoke(defectionOptions.ToArray());
                                if (_DisplayDefectionUI != null) _DisplayDefectionUI.Invoke(true);
                            }
                        }
                    }
                }
            } else if (mover is Pawn) {
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        private BoardCoord TryAddDefectionMove(ChessPiece mover) {
            BoardCoord[] positions = new BoardCoord[2];
            Color positionColour = board.GetCoordInfo(mover.GetBoardPosition()).boardChunk.GetComponent<MeshRenderer>().material.color;
            Color ownedColour = GetTeamOwnedColour(mover);

            if (ColourControlSquares.TryGetValue(ownedColour, out positions)) {
                bool doubleMoveDefection = false;
                BoardCoord[] secondaryMoves = mover.CalculateTemplateMoves().ToArray();
                if (positionColour == selectedDefection) {
                    doubleMoveDefection = true;
                }

                if (IsThreat(mover, positions[0]) || IsThreat(mover, positions[1])) {
                    HashSet<Color> controlledColours = GetControlledColours(mover);
                    HashSet<Color> opposingControlledColours = GetOpposingControlledColours(mover);
                    HashSet<Color> tempOriginalControlledColours = new HashSet<Color>(controlledColours);
                    HashSet<Color> tempOriginalEnemyColours = new HashSet<Color>(opposingControlledColours);

                    // Temporarily add controlled colours to enemy team to check for check after defection.
                    foreach (Color color in tempOriginalControlledColours) {
                        if(color != selectedDefection) {
                            opposingControlledColours.Add(color);
                        }
                    }

                    if (doubleMoveDefection) {
                        int validMoves = secondaryMoves.Length;
                        for (int i = 0; i < secondaryMoves.Length; i++) {
                            if(IsPieceInCheckAfterThisMove(mover, mover, secondaryMoves[i])) {
                                validMoves--;
                            }
                        }
                        if(validMoves == 0) {
                            return BoardCoord.NULL;
                        } else {
                            return mover.GetBoardPosition();
                        }
                    }

                    // Checks for check after adding appropriate colours to opposing team
                    foreach (ChessPiece piece in GetPieces()) {
                        if (piece == mover) continue;
                        // If the piece is a temporary enemy...
                        if (opposingControlledColours.Contains(GetChessPieceColour(piece))) {
                            // If the piece can move to the king's position...
                            if (piece.CalculateTemplateMoves().Contains(mover.GetBoardPosition())) {
                                // Now remove the attacking piece's colour from the mover's team (needed before for the king's own colour to attack itself).
                                controlledColours.Remove(GetChessPieceColour(piece));

                                // If the piece is the enemy's owned colour, then king is in check and defection is illegal.
                                if (GetChessPieceColour(piece) == GetOpposingTeamOwnedColour(mover)) {
                                    // Revert controlled colours back to original
                                    if (ownedColour == whiteCurrentOwnedColour) {
                                        whiteControlledColours = tempOriginalControlledColours;
                                        blackControlledColours = tempOriginalEnemyColours;
                                    } else {
                                        blackControlledColours = tempOriginalControlledColours;
                                        whiteControlledColours = tempOriginalEnemyColours;
                                    }
                                    return BoardCoord.NULL;
                                }

                                bool inCheck = false;
                                Color pieceClr = GetChessPieceColour(piece);
                                // Loop through all colour control parents of this piece
                                while (ColourControlSquares.TryGetValue(pieceClr, out positions)) {
                                    // Get colour control parent of this piece
                                    ChessPiece parentColourControlOccupier = null;
                                    for (int i = 0; i < 2; i++) {
                                        if (IsThreat(mover, positions[i])) {
                                            parentColourControlOccupier = board.GetCoordInfo(positions[i]).occupier;
                                        }
                                    }

                                    // If there is no parent, break out
                                    if (parentColourControlOccupier == null) {
                                        inCheck = false;
                                        break;
                                    }

                                    // If the original enemy team controls the parent's colour...
                                    if (tempOriginalEnemyColours.Contains(GetChessPieceColour(parentColourControlOccupier))) {
                                        inCheck = true;
                                        break;
                                    }

                                    // Otherwise, set piece colour to the parent's colour and repeat
                                    pieceClr = GetChessPieceColour(parentColourControlOccupier);
                                }

                                if (inCheck) {
                                    // Revert controlled colours back to original
                                    if (ownedColour == whiteCurrentOwnedColour) {
                                        whiteControlledColours = tempOriginalControlledColours;
                                        blackControlledColours = tempOriginalEnemyColours;
                                    } else {
                                        blackControlledColours = tempOriginalControlledColours;
                                        whiteControlledColours = tempOriginalEnemyColours;
                                    }
                                    return BoardCoord.NULL;
                                }
                            }
                            controlledColours.Add(GetChessPieceColour(piece));
                        }
                    }

                    // Revert controlled colours back to original
                    if (ownedColour == whiteCurrentOwnedColour) {
                        whiteControlledColours = tempOriginalControlledColours;
                        blackControlledColours = tempOriginalEnemyColours;
                    } else {
                        blackControlledColours = tempOriginalControlledColours;
                        whiteControlledColours = tempOriginalEnemyColours;
                    }
                } else {
                    if (doubleMoveDefection) {
                        int validMoves = secondaryMoves.Length;
                        for (int i = 0; i < secondaryMoves.Length; i++) {
                            if (IsPieceInCheckAfterThisMove(mover, mover, secondaryMoves[i])) {
                                validMoves--;
                            }
                        }
                        if (validMoves == 0) {
                            return BoardCoord.NULL;
                        } else {
                            return mover.GetBoardPosition();
                        }
                    }
                }
                return mover.GetBoardPosition();
            }
            Debug.LogError("Error! ownedColour is not present in the colour list.");
            return BoardCoord.NULL;
        }

        public void SetDefectOptionTo(Color clr) {
            this.selectedDefection = clr;
            MouseController.Instance.CalculateLastOccupierAvailableMoves();
        }

        private Color GetTeamOwnedColour(ChessPiece piece) {
            if (whiteControlledColours.Contains(GetChessPieceColour(piece))) {
                return whiteCurrentOwnedColour;
            } else if (blackControlledColours.Contains(GetChessPieceColour(piece))) {
                return blackCurrentOwnedColour;
            }
            Debug.LogError("Piece has no control!");
            return new Color(0, 0, 0, 0);
        }

        private Color GetOpposingTeamOwnedColour(ChessPiece piece) {
            if (whiteControlledColours.Contains(GetChessPieceColour(piece))) {
                return blackCurrentOwnedColour;
            } else if (blackControlledColours.Contains(GetChessPieceColour(piece))) {
                return whiteCurrentOwnedColour;
            }
            Debug.LogError("Piece has no control!");
            return new Color(0, 0, 0, 0);
        }

        private HashSet<Color> GetControlledColours(ChessPiece piece) {
            if (whiteControlledColours.Contains(GetChessPieceColour(piece))){
                return whiteControlledColours;
            } else if (blackControlledColours.Contains(GetChessPieceColour(piece))) {
                return blackControlledColours;
            }
            Debug.LogError("Piece has no control!");
            return null;
        }

        private HashSet<Color> GetOpposingControlledColours(ChessPiece piece) {
            if (whiteControlledColours.Contains(GetChessPieceColour(piece))) {
                return blackControlledColours;
            } else if (blackControlledColours.Contains(GetChessPieceColour(piece))) {
                return whiteControlledColours;
            }
            Debug.LogError("Piece has no control!");
            return null;
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
                if (currentTeamTurn == Team.WHITE) {
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
            BoardCoord oldPos = mover.GetBoardPosition();
            Color movedFromColour = board.GetCoordInfo(oldPos).boardChunk.GetComponent<MeshRenderer>().material.color;

            // Try make the move
            if (MakeMove(mover, destination)) {
                if (mover is King) {
                    if (kingHasDoubleMoveDefection) kingHasDoubleMoveDefection = false;
                    // Try perform castling move.
                    if (mover.MoveCount == 1) {
                        //TryPerformCastlingRookMoves(mover);
                    }
                    // Try perform defection move.
                    if (oldPos == destination) {
                        PerformDefectionMove(mover);
                        if(GetChessPieceColour(mover) == board.GetCoordInfo(mover.GetBoardPosition()).boardChunk.GetComponent<MeshRenderer>().material.color) {
                            kingHasDoubleMoveDefection = true;
                        }
                        return true;
                    }
                } else if (mover is Pawn) {
                    if (mover is SovereignPawn) UpdatePawnQuadrant((SovereignPawn)mover);
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        mover = promotedPiece;
                        if(mover is King) {
                            RemovePieceFromBoard(currentRoyalPiece);
                            SwitchOwnedArmy(currentTeamTurn, GetChessPieceColour(currentRoyalPiece), GetChessPieceColour(mover));
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

        private void PerformDefectionMove(ChessPiece mover) {
            Color prevMoverClr = GetChessPieceColour(mover);
            UpdateSovereignColour(mover, SovereignExtensions.GetColourName(selectedDefection));
            SwitchOwnedArmy(currentTeamTurn, prevMoverClr, selectedDefection);

            if (prevMoverClr == Color.black) {
                mover.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WHITE_King");
            } else if (selectedDefection == Color.black) {
                mover.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BLACK_King");
            }

            // Re-check opposing team controls.
            BoardCoord[] positions = new BoardCoord[2];
            if (ColourControlSquares.TryGetValue(prevMoverClr, out positions)) {
                if (IsThreat(mover, positions[0]) || IsThreat(mover, positions[1])) {
                    if (currentTeamTurn == Team.WHITE) {
                        blackControlledColours.Add(prevMoverClr);
                    } else {
                        whiteControlledColours.Add(prevMoverClr);
                    }

                    // Add all colours that the mover's team previously controlled except its newly owned colour, to the opposing team.
                    foreach (ChessPiece piece in GetPieces()) {
                        Color pieceClr = GetChessPieceColour(piece);
                        if (GetControlledColours(mover).Contains(pieceClr)) {
                            // Loop through all colour control parents of this piece
                            while (ColourControlSquares.TryGetValue(pieceClr, out positions)) {
                                // If the piece colour is the owned colour, break out
                                if (pieceClr == GetTeamOwnedColour(mover)) {
                                    break;
                                }

                                // If the piece colour is the previously owned colour, remove it from current team and add it
                                // to the opposing team and break out
                                if (pieceClr == prevMoverClr) {
                                    if (currentTeamTurn == Team.WHITE) {
                                        whiteControlledColours.Remove(prevMoverClr);
                                        blackControlledColours.Add(prevMoverClr);
                                    } else {
                                        blackControlledColours.Remove(prevMoverClr);
                                        whiteControlledColours.Add(prevMoverClr);
                                    }
                                    break;
                                }

                                // Get colour control parent of this piece
                                ChessPiece parentColourControlOccupier = null;
                                for (int i = 0; i < 2; i++) {
                                    if (IsAlly(mover, positions[i])) {
                                        parentColourControlOccupier = board.GetCoordInfo(positions[i]).occupier;
                                    }
                                }

                                // If there is no parent, break out
                                if(parentColourControlOccupier == null) {
                                    break;
                                }
 
                                // Otherwise, set piece colour to the parent's colour and repeat
                                pieceClr = GetChessPieceColour(parentColourControlOccupier);
                            }
                        }
                    }
                }
            }
        }

        private void SwitchOwnedArmy(Team team, Color previousColour, Color newColour) {
            if (team == Team.WHITE) {
                whiteControlledColours.Remove(previousColour);
                whiteControlledColours.Add(newColour);
                whiteCurrentOwnedColour = newColour;
            } else {
                blackControlledColours.Remove(previousColour);
                blackControlledColours.Add(newColour);
                blackCurrentOwnedColour = newColour;
            }
        }

        private void UpdateSovereignColour(ChessPiece mover, ColourName colour) {
            mover.gameObject.GetComponent<SovereignColour>().colour = colour;
            mover.gameObject.GetComponent<SpriteRenderer>().material.color = SovereignExtensions.GetColour(colour);
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
            return SovereignExtensions.GetColour(piece.gameObject.GetComponent<SovereignColour>().colour);
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
            currentRoyalPiece = (King)AddSovereignChessPiece(Piece.King, "i1", ColourName.White);
            opposingRoyalPiece = (King)AddSovereignChessPiece(Piece.King, "i16", ColourName.Black);

            aSideWhiteRook = (Rook)AddSovereignChessPiece(Piece.Rook, "e1", ColourName.White);
            hSideWhiteRook = (Rook)AddSovereignChessPiece(Piece.Rook, "l1", ColourName.White);
            aSideBlackRook = (Rook)AddSovereignChessPiece(Piece.Rook, "e16", ColourName.Black);
            hSideBlackRook = (Rook)AddSovereignChessPiece(Piece.Rook, "l16", ColourName.Black);

            AddSovereignChessPiece(Piece.Queen, "h1", ColourName.White);
            AddSovereignChessPiece(Piece.Queen, "h16", ColourName.Black);

            AddSovereignPawn("e2", ColourName.White, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("f2", ColourName.White, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("g2", ColourName.White, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("h2", ColourName.White, SovereignPawn.Quadrant.BottomLeft);

            AddSovereignPawn("i2", ColourName.White, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("j2", ColourName.White, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("k2", ColourName.White, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("l2", ColourName.White, SovereignPawn.Quadrant.BottomRight);

            AddSovereignPawn("e15", ColourName.Black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("f15", ColourName.Black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("g15", ColourName.Black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("h15", ColourName.Black, SovereignPawn.Quadrant.TopLeft);

            AddSovereignPawn("i15", ColourName.Black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("j15", ColourName.Black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("k15", ColourName.Black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("l15", ColourName.Black, SovereignPawn.Quadrant.TopRight);

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x == 5 || x == 10) {
                    AddSovereignChessPiece(Piece.Knight, new BoardCoord(x, WHITE_BACKROW), ColourName.White);
                    AddSovereignChessPiece(Piece.Knight, new BoardCoord(x, BLACK_BACKROW), ColourName.Black);
                } else if (x == 6 || x == 9) {
                    AddSovereignChessPiece(Piece.Bishop, new BoardCoord(x, WHITE_BACKROW), ColourName.White);
                    AddSovereignChessPiece(Piece.Bishop, new BoardCoord(x, BLACK_BACKROW), ColourName.Black);
                }
            }
            #endregion

            SovereignPawn.Quadrant currentQuadrant = SovereignPawn.Quadrant.BottomLeft;
            AddSovereignPawn("c2", ColourName.Pink, currentQuadrant);
            AddSovereignPawn("d2", ColourName.Pink, currentQuadrant);
            AddSovereignPawn("b3", ColourName.Red, currentQuadrant);
            AddSovereignPawn("b4", ColourName.Red, currentQuadrant);
            AddSovereignPawn("b5", ColourName.Orange, currentQuadrant);
            AddSovereignPawn("b6", ColourName.Orange, currentQuadrant);
            AddSovereignPawn("b7", ColourName.Yellow, currentQuadrant);
            AddSovereignPawn("b8", ColourName.Yellow, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.TopLeft;
            AddSovereignPawn("b9", ColourName.Green, currentQuadrant);
            AddSovereignPawn("b10", ColourName.Green, currentQuadrant);
            AddSovereignPawn("b11", ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("b12", ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("b13", ColourName.Blue, currentQuadrant);
            AddSovereignPawn("b14", ColourName.Blue, currentQuadrant);
            AddSovereignPawn("c15", ColourName.Purple, currentQuadrant);
            AddSovereignPawn("d15", ColourName.Purple, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.BottomRight;
            AddSovereignPawn("m2", ColourName.Green, currentQuadrant);
            AddSovereignPawn("n2", ColourName.Green, currentQuadrant);
            AddSovereignPawn("o3", ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("o4", ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("o5", ColourName.Blue, currentQuadrant);
            AddSovereignPawn("o6", ColourName.Blue, currentQuadrant);
            AddSovereignPawn("o7", ColourName.Purple, currentQuadrant);
            AddSovereignPawn("o8", ColourName.Purple, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.TopRight;
            AddSovereignPawn("o9", ColourName.Pink, currentQuadrant);
            AddSovereignPawn("o10", ColourName.Pink, currentQuadrant);
            AddSovereignPawn("o11", ColourName.Red, currentQuadrant);
            AddSovereignPawn("o12", ColourName.Red, currentQuadrant);
            AddSovereignPawn("o13", ColourName.Orange, currentQuadrant);
            AddSovereignPawn("o14", ColourName.Orange, currentQuadrant);
            AddSovereignPawn("m15", ColourName.Yellow, currentQuadrant);
            AddSovereignPawn("n15", ColourName.Yellow, currentQuadrant);

            //-------------------------------

            AddSovereignChessPiece(Piece.Queen, "a1", ColourName.Grey);
            AddSovereignChessPiece(Piece.Rook, "a2", ColourName.Grey);
            AddSovereignChessPiece(Piece.Bishop, "a3", ColourName.Red);
            AddSovereignChessPiece(Piece.Queen, "a4", ColourName.Red);
            AddSovereignChessPiece(Piece.Rook, "a5", ColourName.Orange);
            AddSovereignChessPiece(Piece.Knight, "a6", ColourName.Orange);
            AddSovereignChessPiece(Piece.Bishop, "a7", ColourName.Yellow);
            AddSovereignChessPiece(Piece.Queen, "a8", ColourName.Yellow);
            AddSovereignChessPiece(Piece.Queen, "a9", ColourName.Green);
            AddSovereignChessPiece(Piece.Bishop, "a10", ColourName.Green);
            AddSovereignChessPiece(Piece.Knight, "a11", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Rook, "a12", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Queen, "a13", ColourName.Blue);
            AddSovereignChessPiece(Piece.Bishop, "a14", ColourName.Blue);
            AddSovereignChessPiece(Piece.Rook, "a15", ColourName.Silver);
            AddSovereignChessPiece(Piece.Queen, "a16", ColourName.Silver);

            AddSovereignChessPiece(Piece.Bishop, "b1", ColourName.Grey);
            AddSovereignChessPiece(Piece.Knight, "b2", ColourName.Grey);
            AddSovereignChessPiece(Piece.Knight, "b15", ColourName.Silver);
            AddSovereignChessPiece(Piece.Bishop, "b16", ColourName.Silver);

            AddSovereignChessPiece(Piece.Rook, "c1", ColourName.Pink);
            AddSovereignChessPiece(Piece.Knight, "d1", ColourName.Pink);
            AddSovereignChessPiece(Piece.Rook, "c16", ColourName.Purple);
            AddSovereignChessPiece(Piece.Knight, "d16", ColourName.Purple);

            //--------------------------

            AddSovereignChessPiece(Piece.Queen, "p1", ColourName.Silver);
            AddSovereignChessPiece(Piece.Rook, "p2", ColourName.Silver);
            AddSovereignChessPiece(Piece.Bishop, "p3", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Queen, "p4", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Rook, "p5", ColourName.Blue);
            AddSovereignChessPiece(Piece.Knight, "p6", ColourName.Blue);
            AddSovereignChessPiece(Piece.Bishop, "p7", ColourName.Purple);
            AddSovereignChessPiece(Piece.Queen, "p8", ColourName.Purple);
            AddSovereignChessPiece(Piece.Queen, "p9", ColourName.Pink);
            AddSovereignChessPiece(Piece.Bishop, "p10", ColourName.Pink);
            AddSovereignChessPiece(Piece.Knight, "p11", ColourName.Red);
            AddSovereignChessPiece(Piece.Rook, "p12", ColourName.Red);
            AddSovereignChessPiece(Piece.Queen, "p13", ColourName.Orange);
            AddSovereignChessPiece(Piece.Bishop, "p14", ColourName.Orange);
            AddSovereignChessPiece(Piece.Rook, "p15", ColourName.Grey);
            AddSovereignChessPiece(Piece.Queen, "p16", ColourName.Grey);

            AddSovereignChessPiece(Piece.Bishop, "o1", ColourName.Silver);
            AddSovereignChessPiece(Piece.Knight, "o2", ColourName.Silver);
            AddSovereignChessPiece(Piece.Knight, "o15", ColourName.Grey);
            AddSovereignChessPiece(Piece.Bishop, "o16", ColourName.Grey);

            AddSovereignChessPiece(Piece.Knight, "m1", ColourName.Green);
            AddSovereignChessPiece(Piece.Rook, "n1", ColourName.Green);
            AddSovereignChessPiece(Piece.Knight, "m16", ColourName.Yellow);
            AddSovereignChessPiece(Piece.Rook, "n16", ColourName.Yellow);
        }
    }
}
