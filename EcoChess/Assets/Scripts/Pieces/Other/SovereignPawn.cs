﻿using System.Collections.Generic;

/// <summary>
/// Sovereign Pawns are specific only to the Sovereign Chess variant and should not be used elsewhere.
/// </summary>
public class SovereignPawn : Pawn {
    public enum Quadrant { BottomLeft, BottomRight, TopLeft, TopRight }
    private enum ClosestEdge { Bottom, Left, Right, Top }
    public Quadrant pieceQuadrant;

    public SovereignPawn(Team team, BoardCoord position, Board board) : base(team, position, board) {
        m_pieceType = Piece.SovereignPawn;
    }
    public SovereignPawn(Team team, string algebraicKeyPosition, Board board) : base(team, algebraicKeyPosition, board) {
        m_pieceType = Piece.SovereignPawn;
    }

    public override string GetCanonicalName() {
        return "Pawn";
    }

    public override string GetLetterNotation() {
        return "SP";
    }

    public void ChangePieceQuadrant(Quadrant quadrant) {
        pieceQuadrant = quadrant;
    }

    private ClosestEdge GetClosestEdge() {
        // NOTE: It is impossible for a pawn to end up on any 4x4 corner section of the board.
        BoardCoord pos = GetBoardPosition();

        switch (pieceQuadrant) {
            case Quadrant.BottomLeft:
                if (pos.y > pos.x) return ClosestEdge.Left;
                else return ClosestEdge.Bottom;
            case Quadrant.BottomRight:
                if (pos.y > 1) return ClosestEdge.Right;
                else return ClosestEdge.Bottom;
            case Quadrant.TopLeft:
                if (pos.y < 14) return ClosestEdge.Left;
                else return ClosestEdge.Top;
            case Quadrant.TopRight:
                if (pos.y > pos.x) return ClosestEdge.Top;
                else return ClosestEdge.Right;
            default:
                // Should not happen!
                return ClosestEdge.Bottom;
        }
    }

    public override List<BoardCoord> CalculateTemplateMoves() {
        List<BoardCoord> moves = new List<BoardCoord>();

        BoardCoord pos = GetBoardPosition();
        bool hasInitialMove = (pos.x <= 1 || pos.x >= 14 || pos.y <= 1 || pos.y >= 14) ? true : false;
        ClosestEdge edge;

        switch (pieceQuadrant) {
            case Quadrant.BottomLeft:
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight, moveCap: 1, threatsOnly: true, teamSensitive: false));

                if (hasInitialMove) {
                    edge = GetClosestEdge();

                    if(edge == ClosestEdge.Bottom) {
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    } else { // == ClosestEdge.Left
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    }
                }

                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                break;

            case Quadrant.BottomRight:
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft, moveCap: 1, threatsOnly: true, teamSensitive: false));

                if (hasInitialMove) {
                    edge = GetClosestEdge();

                    if (edge == ClosestEdge.Bottom) {
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    } else { // == ClosestEdge.Right {
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    }
                }

                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Up, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                break;
            case Quadrant.TopLeft:
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpRight, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight, moveCap: 1, threatsOnly: true, teamSensitive: false));

                if (hasInitialMove) {
                    edge = GetClosestEdge();

                    if (edge == ClosestEdge.Top) {
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    } else { // == ClosestEdge.Left
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    }
                }

                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Right, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                break;
            case Quadrant.TopRight:
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.UpLeft, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownLeft, moveCap: 1, threatsOnly: true, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.DownRight, moveCap: 1, threatsOnly: true, teamSensitive: false));

                if (hasInitialMove) {
                    edge = GetClosestEdge();

                    if (edge == ClosestEdge.Top) {
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    } else { // == ClosestEdge.Right
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 2, threatAttackLimit: 0, teamSensitive: false));
                        moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                        break;
                    }
                }

                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Down, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                moves.AddRange(TryGetDirectionalTemplateMoves(MoveDirection.Left, moveCap: 1, threatAttackLimit: 0, teamSensitive: false));
                break;
            default:
                break;
        }
        return moves;
    }
}
