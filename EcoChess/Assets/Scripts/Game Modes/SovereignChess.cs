using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     Q B R N ? ? ? ? ? ? ? ? N R N Q
    ///     R P P P ? ? ? ? ? ? ? ? P P P R
    ///     B P . . . . . . . . . . . . P B
    ///     Q P . . . . . . . . . . . . P Q
    ///     R P . . r . . . . . . b . . P R     Capitals: neutral pieces
    ///     N P . . . y . p P . g . . . P N     ? top: FIDE black pieces
    ///     B P . . . . G . . s . . . . P B     ? bottom: FIDE white pieces
    ///     Q P . . . o . w B . a . . . P Q
    ///     Q P . . . a . B w . o . . . P Q     
    ///     B P . . . . s . . G . . . . P B
    ///     N P . . . g . P p . y . . . P N
    ///     R P . . b . . . . . . r . . P R
    ///     Q P . . . . . . . . . . . . P Q
    ///     B P . . . . . . . . . . . . P B
    ///     R P P P ? ? ? ? ? ? ? ? P P P R
    ///     Q N R N ? ? ? ? ? ? ? ? N R B Q
    ///   
    ///     r = red, y = yellow, g = green, b = blue, a = aqua, o = orange, P = Purple, p = pink, G = gray, s = silver, w = white, B = Black
    /// </summary>
    public class SovereignChess : Chess {
        public static event Action<bool> _DisplayDefectionUI;
        public static event Action<Color[]> _SetDefectionOptions;
        public List<Color> defectionOptions { get; protected set; }
        public Color selectedDefection { get; protected set; }

        private Rook aSideWhiteRook;
        private Rook aSideBlackRook;
        private Rook hSideWhiteRook;
        private Rook hSideBlackRook;

        public class SovereignStateSnapshot {
            public readonly Dictionary<ColourName, Team> colouredArmies;

            public SovereignStateSnapshot(Dictionary<ColourName, Team> colouredArmiesState) {
                this.colouredArmies = new Dictionary<ColourName, Team>(colouredArmiesState);
            }
        }

        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 16;

        private List<BoardCoord> promotionSquares;

        private readonly static Color primaryBoardColour = new Color(1, 219f / 255f, 153f / 255f);
        private readonly static Color secondaryBoardColour = new Color(1, 237f / 255f, 204f / 255f);

        private bool kingHasDoubleMoveDefection = false;
        private Color whiteCurrentOwnedColour = Color.white;
        private Color blackCurrentOwnedColour = Color.black;

        private Dictionary<ColourName, Team> colouredArmies;
        private Stack<SovereignStateSnapshot> colouredArmiesHistory;

        private readonly Dictionary<Color, BoardCoord[]> ColourControlSquares = new Dictionary<Color, BoardCoord[]>(24);

        public SovereignChess() : base(BOARD_WIDTH, BOARD_HEIGHT, primaryBoardColour, secondaryBoardColour) {
            PawnPromotionOptions = new Piece[5] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.King };
            defectionOptions = new List<Color>();
            selectedDefection = whiteCurrentOwnedColour;

            colouredArmies = new Dictionary<ColourName, Team>(12) {
                { ColourName.White, Team.WHITE      },
                { ColourName.Black, Team.BLACK      },
                { ColourName.Red, Team.NONE      },
                { ColourName.Blue, Team.NONE    },
                { ColourName.Yellow, Team.NONE   },
                { ColourName.Green, Team.NONE    },
                { ColourName.Pink, Team.NONE    },
                { ColourName.Purple, Team.NONE   },
                { ColourName.Grey, Team.NONE     },
                { ColourName.Silver, Team.NONE   },
                { ColourName.Orange, Team.NONE   },
                { ColourName.Lightblue, Team.NONE }
            };

            colouredArmiesHistory = new Stack<SovereignStateSnapshot>();

            aSideWhiteRook = aSideBlackRook = null;
            hSideWhiteRook = hSideBlackRook = null;

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

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Created by Mark Bates (2012)",
                this.ToString() + " is a variant involving multi-coloured armies to control on a 16x16 board.",
                "Checkmate.",
                "For all 16 rules of Sovereign Chess, visit <i>http://www.sovereignchess.com/rules</i>",
                "https://en.wikipedia.org/wiki/Sovereign_Chess"
            );
        }

        public override void OnMoveComplete() {
            if (kingHasDoubleMoveDefection == false) {
                base.OnMoveComplete();
            }
            selectedDefection = (GetCurrentTeamTurn() == Team.WHITE) ? whiteCurrentOwnedColour : blackCurrentOwnedColour;
        }

        public override void IncrementGameAndPieceStateHistory() {
            base.IncrementGameAndPieceStateHistory();

            colouredArmiesHistory.Push(new SovereignStateSnapshot(colouredArmies));
        }

        public override bool CheckWinState() {
            bool hasAnyMoves = false;
            foreach (ChessPiece piece in GetAlivePiecesOfType<ChessPiece>(GetCurrentTeamTurn())) {
                if (CalculateAvailableMoves(piece).Count > 0) {
                    hasAnyMoves = true;
                    break;
                }
            }

            if (!hasAnyMoves) {
                if (IsPieceInCheck(currentRoyalPiece)) {
                    UIManager.Instance.LogCheckmate(SovereignExtensions.GetColourName(GetChessPieceColour(opposingRoyalPiece)).ToString(),
                        SovereignExtensions.GetColourName(GetChessPieceColour(currentRoyalPiece)).ToString());
                } else {
                    UIManager.Instance.LogStalemate(SovereignExtensions.GetColourName(GetChessPieceColour(currentRoyalPiece)).ToString());
                }
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override string GetCurrentTurnLabel() {
            if(GetCurrentTeamTurn() == Team.WHITE) {
                return SovereignExtensions.GetColourName(whiteCurrentOwnedColour).ToString() + "'s move";
            } else {
                return SovereignExtensions.GetColourName(blackCurrentOwnedColour).ToString() + "'s move";
            }
        }

        public void SetDefectOptionTo(Color clr) {
            this.selectedDefection = clr;
            MouseController.Instance.CalculateLastOccupierAvailableMoves();
        }

        public override void SetPawnPromotionTo(Piece piece) {
            base.SetPawnPromotionTo(piece);
            MouseController.Instance.CalculateLastOccupierAvailableMoves();
        }

        protected override bool IsAPromotionMove(BoardCoord move) {
            return promotionSquares.Contains(move);
        }

        protected override bool PerformedAPromotionMove(Pawn mover) {
            return promotionSquares.Contains(mover.GetBoardPosition());
        }

        protected override ChessPiece TryPerformPawnPromotion(Pawn mover) {
            if (PerformedAPromotionMove(mover)) {
                KillPiece(mover);

                ChessPiece newPromotedPiece = AddSovereignChessPiece(
                    SelectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition(), mover.gameObject.GetComponent<SovereignColour>().colour);

                AddPromotionToLastMoveNotation(newPromotedPiece.GetLetterNotation());
                
                return newPromotedPiece;
            }

            return null;
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
                    if (SelectedPawnPromotion == Piece.King && promotionSquares.Contains(templateMoves[i]) && IsPieceInCheckAfterThisMove(mover, mover, templateMoves[i])) {
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
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, castlingDistance));

                if (checkingForCheck == false) {
                    List<Color> controlledColours = GetControlledColours(mover.GetTeam());
                    if (controlledColours.Count > 1) {
                        defectionOptions = new List<Color>(controlledColours);
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
            }

            return availableMoves;
        }

        private BoardCoord TryGetValidMove(ChessPiece mover, BoardCoord templateMove, out bool cancelDirectionalSlide) {
            cancelDirectionalSlide = false;
            BoardCoord[] colourPositions = new BoardCoord[2];
            Color movedToColour = Board.GetCoordInfo(templateMove).boardChunk.GetComponent<MeshRenderer>().material.color;

            // Moved to square is a coloured square...
            if (ColourControlSquares.TryGetValue(movedToColour, out colourPositions)) {
                // Check if moved to square is occupied AND not a threat... (must be an ally or neutral piece).
                if (Board.GetCoordInfo(templateMove).GetAliveOccupier() != null && IsThreat(mover, templateMove) == false) {
                    cancelDirectionalSlide = true;
                    return BoardCoord.NULL;
                }
                
                // Get occupiers of the squares of that colour
                ChessPiece firstOccupier = Board.GetCoordInfo(colourPositions[0]).GetAliveOccupier();
                ChessPiece secondOccupier = Board.GetCoordInfo(colourPositions[1]).GetAliveOccupier();

                // Check if no occupiers OR a threat...
                if ((firstOccupier == null && secondOccupier == null) || IsThreat(mover, templateMove)) {
                    if (CurrentTeamControlsArmy(GetChessPieceColour(mover)) && movedToColour == whiteCurrentOwnedColour
                        || OpposingTeamControlsColouredArmy(GetChessPieceColour(mover)) && movedToColour == blackCurrentOwnedColour) {
                        return BoardCoord.NULL;
                    }
                    return templateMove;

                    // Check if the mover is occuping one of the squares...
                } else if (mover.GetBoardPosition() == colourPositions[0] || mover.GetBoardPosition() == colourPositions[1]) {
                    return templateMove;
                }

                return BoardCoord.NULL;

                // Else moved to square is not a coloured square...
            } else {
                // Check if moved to square is occupied AND not a threat... (must be an ally or neutral piece).
                if (Board.GetCoordInfo(templateMove).GetAliveOccupier() != null && IsThreat(mover, templateMove) == false) {
                    cancelDirectionalSlide = true;
                    return BoardCoord.NULL;

                    // Else a threat was found.
                } else {
                    cancelDirectionalSlide = false;
                    return templateMove;
                }
            }
        }

        private BoardCoord TryAddDefectionMove(ChessPiece mover) {
            BoardCoord[] positions = new BoardCoord[2];
            Color positionColour = Board.GetCoordInfo(mover.GetBoardPosition()).boardChunk.GetComponent<MeshRenderer>().material.color;
            Color ownedColour = GetTeamOwnedColour(mover);

            if (ColourControlSquares.TryGetValue(ownedColour, out positions)) {
                bool doubleMoveDefection = false;
                bool doubleMoveLegal = true;

                BoardCoord[] secondaryMoves = mover.CalculateTemplateMoves().ToArray();
                if (positionColour == selectedDefection) {
                    doubleMoveDefection = true;
                }

                // If the defection involves a second move of the king, determine whether a legal move exists.
                if (doubleMoveDefection) {
                    int validMoves = secondaryMoves.Length;
                    for (int i = 0; i < secondaryMoves.Length; i++) {
                        if (IsPieceInCheckAfterThisMove(mover, mover, secondaryMoves[i])) {
                            validMoves--;
                        }
                    }
                    if (validMoves == 0) {
                        doubleMoveLegal = false;
                    }
                } else if (IsPieceInCheck(mover)) return BoardCoord.NULL;

                if (IsThreat(mover, positions[0]) || IsThreat(mover, positions[1])) {
                    List<Color> controlledColours = GetControlledColours(mover.GetTeam());
                    List<Color> opposingControlledColours = GetOpposingControlledColours(mover.GetTeam());
                    List<Color> originalOpposingControlledColours = new List<Color>(opposingControlledColours);

                    // Temporarily add controlled colours to enemy team to check for check after defection.
                    foreach (Color color in controlledColours) {
                        if(color != selectedDefection) {
                            opposingControlledColours.Add(color);
                        }
                    }

                    // Checks for check after adding appropriate colours to opposing team
                    foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                        if (piece == mover) continue;
                        // If the piece is a temporary enemy...
                        if (opposingControlledColours.Contains(GetChessPieceColour(piece))) {
                            // If the piece can move to the king's position...
                            if (piece.CalculateTemplateMoves().Contains(mover.GetBoardPosition())) {
                                // Now remove the attacking piece's colour from the mover's team (needed before for the king's own colour to attack itself).
                                controlledColours.Remove(GetChessPieceColour(piece));

                                // If the piece is the enemy's owned colour, then king is in check and defection is illegal.
                                if (GetChessPieceColour(piece) == GetOpposingTeamOwnedColour(mover)) {
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
                                            parentColourControlOccupier = Board.GetCoordInfo(positions[i]).GetAliveOccupier();
                                        }
                                    }

                                    // If there is no parent, break out
                                    if (parentColourControlOccupier == null) {
                                        inCheck = false;
                                        break;
                                    }

                                    // If the original enemy team controls the parent's colour...
                                    if (originalOpposingControlledColours.Contains(GetChessPieceColour(parentColourControlOccupier))) {
                                        inCheck = true;
                                        break;
                                    }

                                    // Otherwise, set piece colour to the parent's colour and repeat
                                    pieceClr = GetChessPieceColour(parentColourControlOccupier);
                                }

                                if (inCheck) {
                                    return BoardCoord.NULL;
                                }
                            }
                            controlledColours.Add(GetChessPieceColour(piece));
                        }
                    }
                } else if (doubleMoveDefection && doubleMoveLegal == false) {
                    return BoardCoord.NULL;
                }
                return mover.GetBoardPosition();
            }
            Debug.LogError("Error! ownedColour is not present in the colour list.");
            return BoardCoord.NULL;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord[] positions = new BoardCoord[2];
            BoardCoord oldPos = mover.GetBoardPosition();
            Color movedFromColour = Board.GetCoordInfo(oldPos).boardChunk.GetComponent<MeshRenderer>().material.color;

            // Try make the move
            if (MakeBaseMove(mover, destination)) {
                if (mover is King) {
                    if (kingHasDoubleMoveDefection) kingHasDoubleMoveDefection = false;

                    // Try perform defection move.
                    if (oldPos == destination) {
                        PerformDefectionMove(mover);
                        if (GetChessPieceColour(mover) == Board.GetCoordInfo(mover.GetBoardPosition()).boardChunk.GetComponent<MeshRenderer>().material.color) {
                            kingHasDoubleMoveDefection = true;
                        }
                        return true;

                        // Else try perform castling move.
                    } else {
                        TryPerformCastlingMove(mover);
                    } 
                } else if (mover is Pawn) {
                    if (mover is SovereignPawn) UpdatePawnQuadrant((SovereignPawn)mover);
                    ChessPiece promotedPiece = TryPerformPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        mover = promotedPiece;
                        if(mover is King) {
                            KillPiece(currentRoyalPiece);
                            SwitchOwnedArmy(mover, GetChessPieceColour(currentRoyalPiece), GetChessPieceColour(mover));
                            currentRoyalPiece = mover;
                        }
                    }
                }

                // Moving off of a coloured square.
                if (movedFromColour != whiteCurrentOwnedColour && movedFromColour != blackCurrentOwnedColour) {
                    if (ColourControlSquares.TryGetValue(movedFromColour, out positions)) {
                        SetColouredArmyToTeam(movedFromColour, Team.NONE);
                    }
                }

                // Moving onto a coloured square.
                Color destinationColour = Board.GetCoordInfo(destination).boardChunk.GetComponent<MeshRenderer>().material.color;
                if (destinationColour != whiteCurrentOwnedColour && destinationColour != blackCurrentOwnedColour) {
                    if (ColourControlSquares.TryGetValue(destinationColour, out positions)) {
                        SetColouredArmyToTeam(destinationColour, GetCurrentTeamTurn());
                    }
                }
                return true;
            }
            return false;
        }

        private void PerformDefectionMove(ChessPiece mover) {
            Color prevOwnedClr = GetChessPieceColour(mover);
            UpdateSovereignColour(mover, SovereignExtensions.GetColourName(selectedDefection));
            SwitchOwnedArmy(mover, prevOwnedClr, selectedDefection);

            if (prevOwnedClr == Color.black) {
                mover.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WHITE_King");
            } else if (selectedDefection == Color.black) {
                mover.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("BLACK_King");
            }

            // Re-check opposing team controls.
            BoardCoord[] positions = new BoardCoord[2];
            if (ColourControlSquares.TryGetValue(prevOwnedClr, out positions)) {
                if (IsThreat(mover, positions[0]) || IsThreat(mover, positions[1])) {
                    SetColouredArmyToTeam(prevOwnedClr, GetOpposingTeamTurn());

                    // Add all colours that the mover's team previously controlled except its newly owned colour, to the opposing team.
                    foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                        Color pieceClr = GetChessPieceColour(piece);
                        if (TeamControlsColouredArmy(mover.GetTeam(), pieceClr)) {
                            // Loop through all colour control parents of this piece
                            while (ColourControlSquares.TryGetValue(pieceClr, out positions)) {
                                // If the piece colour is the owned colour, break out
                                if (pieceClr == GetTeamOwnedColour(mover)) {
                                    break;
                                }

                                // Get colour control parent of this piece
                                ChessPiece parentColourControlOccupier = null;
                                for (int i = 0; i < 2; i++) {
                                    if (Board.GetCoordInfo(positions[i]).GetAliveOccupier() != null) {
                                        parentColourControlOccupier = Board.GetCoordInfo(positions[i]).GetAliveOccupier();
                                    }
                                }

                                // If there is no parent, break out
                                if(parentColourControlOccupier == null) {
                                    break;
                                    // Else, if the opposing team controls this parent's colour, 
                                    // then add the child's colour to their team, and remove from defecting team.
                                } else if (OpposingTeamControlsColouredArmy(GetChessPieceColourName(parentColourControlOccupier))) {
                                    SetColouredArmyToTeam(pieceClr, GetOpposingTeamTurn());
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

        private bool TeamControlsColouredArmy(Team team, ColourName colour) {
            return colouredArmies[colour] == team;
        }

        private bool TeamControlsColouredArmy(Team team, Color colour) {
            return colouredArmies[SovereignExtensions.GetColourName(colour)] == team;
        }

        private bool OpposingTeamControlsColouredArmy(ColourName colour) {
            return colouredArmies[colour] == GetOpposingTeamTurn();
        }

        private bool OpposingTeamControlsColouredArmy(Color colour) {
            return colouredArmies[SovereignExtensions.GetColourName(colour)] == GetOpposingTeamTurn();
        }

        private bool CurrentTeamControlsColouredArmy(ColourName colour) {
            return colouredArmies[colour] == GetCurrentTeamTurn();
        }

        private bool CurrentTeamControlsArmy(Color colour) {
            return colouredArmies[SovereignExtensions.GetColourName(colour)] == GetCurrentTeamTurn();
        }

        private void SetColouredArmyToTeam(ColourName color, Team team) {
            colouredArmies[color] = team;
        }

        private void SetTeamOfColouredArmy(ColourName colour, Team team) {
            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                if (GetChessPieceColourName(piece) == colour) {
                    piece.SetTeam(team);
                }
            }
        }

        private void SetColouredArmyToTeam(Color color, Team team) {
            colouredArmies[SovereignExtensions.GetColourName(color)] = team;
            SetTeamOfColouredArmy(SovereignExtensions.GetColourName(color), team);
        }

        private List<Color> GetControlledColours(Team team) {
            List<Color> controlledColours = new List<Color>();

            foreach (var pair in colouredArmies) {
                if (pair.Value == team) {
                    controlledColours.Add(SovereignExtensions.GetColour(pair.Key));
                }
            }

            return controlledColours;
        }

        private List<Color> GetOpposingControlledColours(Team team) {
            List<Color> controlledColours = new List<Color>();

            Team oppositeTeam;
            if (team == Team.WHITE) {
                oppositeTeam = Team.BLACK;
            } else {
                oppositeTeam = Team.WHITE;
            }

            foreach (var pair in colouredArmies) {
                if (pair.Value == oppositeTeam) {
                    controlledColours.Add(SovereignExtensions.GetColour(pair.Key));
                }
            }

            return controlledColours;
        }

        protected override BoardCoord[] TryAddAvailableCastleMoves(ChessPiece king, Piece[] castlerOptions, int castlingDistance, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (IsPieceInCheck(king) == false) {
                List<BoardCoord> castleMoves = new List<BoardCoord>(2);

                for (int i = LEFT; i <= RIGHT; i += 2) {
                    if (!canCastleLeftward && i == LEFT) continue;
                    if (!canCastleRightward && i == RIGHT) break;

                    int x = king.GetBoardPosition().x + (i * 2);
                    int y = king.GetBoardPosition().y;
                    BoardCoord coord = new BoardCoord(x, y);

                    // Check the square immediately next to the king.
                    if(IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i, 0))) {
                        continue;
                    }

                    // Check subesquent squares.
                    while (Board.ContainsCoord(coord)) {
                        if (IsPieceInCheckAfterThisMove(king, king, coord)) {
                            break;
                        }

                        ChessPiece occupier = Board.GetCoordInfo(coord).GetAliveOccupier();
                        if (occupier != null) {
                            if (occupier is Rook && occupier.MoveCount == 0 && IsAlly(king, coord)) {
                                ChessPiece occupierStop = null;
                                coord = new BoardCoord(king.GetBoardPosition().x + i * 2, king.GetBoardPosition().y);
                                if(GetTeamOwnedColour(king) == whiteCurrentOwnedColour) {
                                    if(i == LEFT) {
                                        aSideWhiteRook = (Rook)occupier;
                                    } else {
                                        hSideWhiteRook = (Rook)occupier;
                                    }
                                } else {
                                    if (i == LEFT) {
                                        aSideBlackRook = (Rook)occupier;
                                    } else {
                                        hSideBlackRook = (Rook)occupier;
                                    }
                                }
                                while(occupierStop != occupier) {
                                    if (Board.ContainsCoord(coord) && IsAlly(king, coord) == false) {
                                        castleMoves.Add(coord);
                                    }
                                    coord.x += i;
                                    occupierStop = Board.GetCoordInfo(coord).GetAliveOccupier();
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

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            ChessPiece castlingPiece = null;

            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x < 7) {
                    if (GetTeamOwnedColour(mover) == whiteCurrentOwnedColour) {
                        castlingPiece = aSideWhiteRook;
                    } else {
                        castlingPiece = aSideBlackRook;
                    }
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(mover.GetBoardPosition().x + 1, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x > 9) {
                    if (GetTeamOwnedColour(mover) == whiteCurrentOwnedColour) {
                        castlingPiece = hSideWhiteRook;
                    } else {
                        castlingPiece = hSideBlackRook;
                    }
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(mover.GetBoardPosition().x - 1, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }

        private void RewindColouredArmyStateToPreviousMove() {
            if (colouredArmiesHistory.Count > 1) {
                colouredArmiesHistory.Pop();

                SovereignStateSnapshot colouredArmiesStateToRestore = colouredArmiesHistory.Peek();

                colouredArmies = new Dictionary<ColourName, Team>(colouredArmiesStateToRestore.colouredArmies);

                foreach (var item in colouredArmies) {
                    SetTeamOfColouredArmy(item.Key, item.Value);
                }
            }
        }

        public override bool UndoLastMove() {
            if (base.UndoLastMove()) {
                RewindColouredArmyStateToPreviousMove();
                return true;
            }
            return false;
        }

        #region Helper Functions
        private void AddColourControlSquares(string algebraicKey, string algebraicKey2, ColourName color) {
            if (Board.TryGetCoordWithKey(algebraicKey, out BoardCoord firstSquare)) {
                Board.GetCoordInfo(firstSquare).boardChunk.GetComponent<MeshRenderer>().material.color = SovereignExtensions.GetColour(color);
            }

            if (Board.TryGetCoordWithKey(algebraicKey2, out BoardCoord secondSquare)) {
                Board.GetCoordInfo(secondSquare).boardChunk.GetComponent<MeshRenderer>().material.color = SovereignExtensions.GetColour(color);
            }

            ColourControlSquares.Add(SovereignExtensions.GetColour(color), new BoardCoord[2] { firstSquare, secondSquare });
        }

        private void AddPromotionSquare(string algebraicKeyPosition) {
            if (Board.TryGetCoordWithKey(algebraicKeyPosition, out BoardCoord coord)) {
                promotionSquares.Add(coord);
            }
        }

        private ChessPiece AddSovereignChessPiece(Piece piece, Team team, string algebraicKey, ColourName colour) {
            if (Board.TryGetCoordWithKey(algebraicKey, out BoardCoord coord)) {
                ChessPiece sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, team, coord));

                SetPieceColour(sovereignPiece, colour);

                return sovereignPiece;
            }
            return null;
        }

        private ChessPiece AddSovereignChessPiece(Piece piece, Team team, BoardCoord coord, ColourName colour) {
            if (Board.ContainsCoord(coord)) {
                ChessPiece sovereignPiece = AddPieceToBoard(ChessPieceFactory.Create(piece, team, coord));

                SetPieceColour(sovereignPiece, colour);

                return sovereignPiece;
            }
            return null;
        }

        private SovereignPawn AddSovereignPawn(string algebraicKey, Team team, ColourName colour, SovereignPawn.Quadrant quadrant) {
            if (Board.TryGetCoordWithKey(algebraicKey, out BoardCoord coord)) {
                SovereignPawn sovereignPawn = (SovereignPawn)AddPieceToBoard(new SovereignPawn(team, coord, quadrant));
                    
                SetPieceColour(sovereignPawn, colour);

                return sovereignPawn;
            }
            return null;
        }

        private void SetPieceColour(ChessPiece piece, ColourName colour) {
            if (piece != null) {
                if (colour != ColourName.Black) {
                    piece.gameObject.GetComponent<SpriteRenderer>().material.color = SovereignExtensions.GetColour(colour);
                }
                piece.gameObject.AddComponent<SovereignColour>().colour = colour;
            }
        }

        private Color GetTeamOwnedColour(ChessPiece piece) {
            if (piece.GetTeam() == Team.WHITE) {
                return whiteCurrentOwnedColour;
            } else if (piece.GetTeam() == Team.BLACK) {
                return blackCurrentOwnedColour;
            }

            Debug.LogError("Piece has no control!");
            return new Color(0, 0, 0, 0);
        }

        private Color GetOpposingTeamOwnedColour(ChessPiece piece) {
            if (piece.GetTeam() == Team.WHITE) {
                return blackCurrentOwnedColour;
            } else if (piece.GetTeam() == Team.BLACK) {
                return whiteCurrentOwnedColour;
            }
            Debug.LogError("Piece has no control!");
            return new Color(0, 0, 0, 0);
        }

        private ColourName GetChessPieceColourName(ChessPiece piece) {
            return piece.gameObject.GetComponent<SovereignColour>().colour;
        }


        private Color GetChessPieceColour(ChessPiece piece) {
            return SovereignExtensions.GetColour(piece.gameObject.GetComponent<SovereignColour>().colour);
        }

        private void SwitchOwnedArmy(ChessPiece mover, Color previousColour, Color newColour) {
            Color ownedColour = GetTeamOwnedColour(mover);

            SetColouredArmyToTeam(previousColour, Team.NONE);
            SetColouredArmyToTeam(newColour, mover.GetTeam());

            if (ownedColour == whiteCurrentOwnedColour) {
                whiteCurrentOwnedColour = newColour;
            } else {
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
        #endregion

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddSovereignChessPiece(Piece.King, Team.WHITE, "i1", ColourName.White);
            opposingRoyalPiece = (King)AddSovereignChessPiece(Piece.King, Team.BLACK, "i16", ColourName.Black);

            aSideWhiteRook = (Rook)AddSovereignChessPiece(Piece.Rook, Team.WHITE, "e1", ColourName.White);
            hSideWhiteRook = (Rook)AddSovereignChessPiece(Piece.Rook, Team.WHITE, "l1", ColourName.White);
            aSideBlackRook = (Rook)AddSovereignChessPiece(Piece.Rook, Team.BLACK, "e16", ColourName.Black);
            hSideBlackRook = (Rook)AddSovereignChessPiece(Piece.Rook, Team.BLACK, "l16", ColourName.Black);

            AddSovereignChessPiece(Piece.Queen, Team.WHITE, "h1", ColourName.White);
            AddSovereignChessPiece(Piece.Queen, Team.BLACK, "h16", ColourName.Black);

            AddSovereignPawn("e2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("f2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("g2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomLeft);
            AddSovereignPawn("h2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomLeft);

            AddSovereignPawn("i2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("j2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("k2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomRight);
            AddSovereignPawn("l2", Team.WHITE, ColourName.White, SovereignPawn.Quadrant.BottomRight);

            AddSovereignPawn("e15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("f15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("g15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopLeft);
            AddSovereignPawn("h15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopLeft);

            AddSovereignPawn("i15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("j15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("k15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopRight);
            AddSovereignPawn("l15", Team.BLACK, ColourName.Black, SovereignPawn.Quadrant.TopRight);

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x == 5 || x == 10) {
                    AddSovereignChessPiece(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW), ColourName.White);
                    AddSovereignChessPiece(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW), ColourName.Black);
                } else if (x == 6 || x == 9) {
                    AddSovereignChessPiece(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW), ColourName.White);
                    AddSovereignChessPiece(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW), ColourName.Black);
                }
            }

            //--------------------------------

            SovereignPawn.Quadrant currentQuadrant = SovereignPawn.Quadrant.BottomLeft;
            AddSovereignPawn("c2", Team.NONE, ColourName.Pink, currentQuadrant);
            AddSovereignPawn("d2", Team.NONE, ColourName.Pink, currentQuadrant);
            AddSovereignPawn("b3", Team.NONE, ColourName.Red, currentQuadrant);
            AddSovereignPawn("b4", Team.NONE, ColourName.Red, currentQuadrant);
            AddSovereignPawn("b5", Team.NONE, ColourName.Orange, currentQuadrant);
            AddSovereignPawn("b6", Team.NONE, ColourName.Orange, currentQuadrant);
            AddSovereignPawn("b7", Team.NONE, ColourName.Yellow, currentQuadrant);
            AddSovereignPawn("b8", Team.NONE, ColourName.Yellow, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.TopLeft;
            AddSovereignPawn("b9",  Team.NONE, ColourName.Green, currentQuadrant);
            AddSovereignPawn("b10", Team.NONE, ColourName.Green, currentQuadrant);
            AddSovereignPawn("b11", Team.NONE, ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("b12", Team.NONE, ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("b13", Team.NONE, ColourName.Blue, currentQuadrant);
            AddSovereignPawn("b14", Team.NONE, ColourName.Blue, currentQuadrant);
            AddSovereignPawn("c15", Team.NONE, ColourName.Purple, currentQuadrant);
            AddSovereignPawn("d15", Team.NONE, ColourName.Purple, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.BottomRight;
            AddSovereignPawn("m2", Team.NONE, ColourName.Green, currentQuadrant);
            AddSovereignPawn("n2", Team.NONE, ColourName.Green, currentQuadrant);
            AddSovereignPawn("o3", Team.NONE, ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("o4", Team.NONE, ColourName.Lightblue, currentQuadrant);
            AddSovereignPawn("o5", Team.NONE, ColourName.Blue, currentQuadrant);
            AddSovereignPawn("o6", Team.NONE, ColourName.Blue, currentQuadrant);
            AddSovereignPawn("o7", Team.NONE, ColourName.Purple, currentQuadrant);
            AddSovereignPawn("o8", Team.NONE, ColourName.Purple, currentQuadrant);

            currentQuadrant = SovereignPawn.Quadrant.TopRight;
            AddSovereignPawn("o9",  Team.NONE, ColourName.Pink, currentQuadrant);
            AddSovereignPawn("o10", Team.NONE, ColourName.Pink, currentQuadrant);
            AddSovereignPawn("o11", Team.NONE, ColourName.Red, currentQuadrant);
            AddSovereignPawn("o12", Team.NONE, ColourName.Red, currentQuadrant);
            AddSovereignPawn("o13", Team.NONE, ColourName.Orange, currentQuadrant);
            AddSovereignPawn("o14", Team.NONE, ColourName.Orange, currentQuadrant);
            AddSovereignPawn("m15", Team.NONE, ColourName.Yellow, currentQuadrant);
            AddSovereignPawn("n15", Team.NONE, ColourName.Yellow, currentQuadrant);

            //-------------------------------

            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "a1", ColourName.Grey);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "a2", ColourName.Grey);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "a3", ColourName.Red);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "a4", ColourName.Red);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "a5", ColourName.Orange);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "a6", ColourName.Orange);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "a7", ColourName.Yellow);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "a8", ColourName.Yellow);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "a9", ColourName.Green);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "a10", ColourName.Green);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "a11", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "a12", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "a13", ColourName.Blue);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "a14", ColourName.Blue);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "a15", ColourName.Silver);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "a16", ColourName.Silver);

            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "b1", ColourName.Grey);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "b2", ColourName.Grey);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "b15", ColourName.Silver);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "b16", ColourName.Silver);

            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "c1", ColourName.Pink);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "d1", ColourName.Pink);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "c16", ColourName.Purple);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "d16", ColourName.Purple);

            //-------------------------------

            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "p1", ColourName.Silver);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "p2", ColourName.Silver);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "p3", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "p4", ColourName.Lightblue);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "p5", ColourName.Blue);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "p6", ColourName.Blue);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "p7", ColourName.Purple);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "p8", ColourName.Purple);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "p9", ColourName.Pink);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "p10", ColourName.Pink);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "p11", ColourName.Red);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "p12", ColourName.Red);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "p13", ColourName.Orange);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "p14", ColourName.Orange);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "p15", ColourName.Grey);
            AddSovereignChessPiece(Piece.Queen,  Team.NONE, "p16", ColourName.Grey);

            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "o1", ColourName.Silver);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "o2", ColourName.Silver);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "o15", ColourName.Grey);
            AddSovereignChessPiece(Piece.Bishop, Team.NONE, "o16", ColourName.Grey);

            AddSovereignChessPiece(Piece.Knight, Team.NONE, "m1", ColourName.Green);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "n1", ColourName.Green);
            AddSovereignChessPiece(Piece.Knight, Team.NONE, "m16", ColourName.Yellow);
            AddSovereignChessPiece(Piece.Rook,   Team.NONE, "n16", ColourName.Yellow);
        }
    }
}
