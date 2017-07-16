using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

public class Board {
    public GameObject gameBoardObj { get; private set; }

    private uint boardWidth;
    private uint boardHeight;
    private Dictionary<BoardCoord, CoordInfo> coordinates;
    private List<BoardCoord> highlightedCoords;
    private char[] boardLetters;
    private string[] boardNumbers;

    public Board(uint width, uint height) {
        highlightedCoords = new List<BoardCoord>();
        coordinates = new Dictionary<BoardCoord, CoordInfo>();
        boardWidth = width;
        boardHeight = height;
        GenerateBoardCoordinateValues();
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                coordinates.Add(new BoardCoord(x, y), new CoordInfo(boardLetters[x] + boardNumbers[y]));
            }
        }
        gameBoardObj = ChessObjectSpawner.Instance.InstantiateGameBoard(this);
    }

    private void GenerateBoardCoordinateValues() {
        StringBuilder letters = new StringBuilder((int)boardWidth);
        StringBuilder numbers = new StringBuilder((int)boardWidth);

        int nextLetter = 'a';
        for (int i = 0; i < letters.Capacity; i++) {
            letters.Append((char)nextLetter++);
            numbers.Append((i + 1) + ",");
        }
        boardLetters = letters.ToString().ToCharArray();
        boardNumbers = numbers.ToString().Split(new char[] { ',' });
    }

    /// <summary>
    /// Retrieves a boardcoord value on the board using it's algebraic key.
    /// </summary>
    /// <param name="algebraicKey"></param>
    /// <param name="coord"></param>
    /// <returns></returns>
    public bool TryGetCoordWithKey(string algebraicKey, out BoardCoord coord) {
        coord = new BoardCoord();
        if (coordinates.Count != 0) {
            foreach (KeyValuePair<BoardCoord, CoordInfo> pair in coordinates) {
                if (pair.Value.algebraicKey.Equals(algebraicKey)) {
                    coord = pair.Key;
                    return true;
                }
            }
        } else {
            Debug.LogError("Couldn't retrieve a BoardCoord because the GameBoard hasn't been created yet!");
            return false;
        }
        Debug.LogError("Couldn't retrieve a BoardCoord because the GameBoard doesn't contain key: " + algebraicKey + "!");
        return false;
    }

    /// <summary>
    /// Retrieves the details of a coordinate on the board.
    /// </summary>
    /// <param name="coord">Boardcoord value to search for.</param>
    /// <returns></returns>
    public CoordInfo GetCoordInfo(BoardCoord coord) {
        if (ContainsCoord(coord)) {
            return coordinates[coord];
        }

        Debug.LogErrorFormat("ERROR: The GameBoard does not contain a CoordInfo for coordinate position: {0}", coord.ToString());
        return null;
    }

    public int GetHeight() {
        return (int)boardHeight;
    }

    public int GetWidth() {
        return (int)boardWidth;
    }

    public bool ContainsCoord(BoardCoord coord) {
        return coordinates.ContainsKey(coord);
    }

    public bool RemoveBoardCoordinates(string coordKey) {
        BoardCoord coord;
        if (TryGetCoordWithKey(coordKey, out coord)) {
            GetCoordInfo(coord).boardChunk.SetActive(false);
            coordinates.Remove(coord);
            return true;
        }
        return false;
    }

    public void RemoveBoardCoordinates(string[] coordKeys) {
        for (int i = 0; i < coordKeys.Length; i++) {
            BoardCoord coord;
            if (TryGetCoordWithKey(coordKeys[i], out coord)) {
                GetCoordInfo(coord).boardChunk.SetActive(false);
                coordinates.Remove(coord);
            }
        }
    }

    public bool RemoveBoardCoordinates(BoardCoord coord) {
        if (ContainsCoord(coord)) {
            GetCoordInfo(coord).boardChunk.SetActive(false);
            coordinates.Remove(coord);
            return true;
        }
        return false;
    }

    public void RemoveBoardCoordinates(BoardCoord[] coords) {
        for (int i = 0; i < coords.Length; i++) {
            if (ContainsCoord(coords[i])) {
                GetCoordInfo(coords[i]).boardChunk.SetActive(false);
                coordinates.Remove(coords[i]);
            }
        }
    }

    public bool AddCustomBoardCoordinate(BoardCoord coord, string algebraicCode) {
        if (ContainsCoord(coord)) {
            Debug.LogErrorFormat("Board already contains coord: {0}. Cannot add to board.", coord.ToString());
            return false;
        }
        foreach (KeyValuePair<BoardCoord, CoordInfo> item in coordinates) {
            if (item.Value.algebraicKey.Equals(algebraicCode)) {
                Debug.LogErrorFormat("Board already contains algebraic code: {0}. Cannot add to board.", algebraicCode);
            }
            return false;
        }
        coordinates.Add(coord, new CoordInfo(algebraicCode));
        return true;
    }


    public void HighlightCoordinates(BoardCoord[] coords) {
        RemoveHighlightedCoordinates();

        highlightedCoords.AddRange(coords);
        for (int i = 0; i < coords.Length; i++) {
            if (ContainsCoord(coords[i])) {
                coordinates[coords[i]].boardChunk.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
        }
    }

    public void RemoveHighlightedCoordinates() {
        for (int i = 0; i < highlightedCoords.Count; i++) {
            if (ContainsCoord(highlightedCoords[i])) {
                coordinates[highlightedCoords[i]].boardChunk.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }
        highlightedCoords.Clear();
    }
}
