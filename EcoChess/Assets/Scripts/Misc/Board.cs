using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

public class Board {
    public GameObject gameBoardObj { get; private set; }
    public bool allowFlipping;
    public bool isFlipped;

    private readonly uint boardWidth;
    private readonly uint boardHeight;
    private readonly Dictionary<BoardCoord, CoordInfo> coordinates;
    private List<BoardCoord> highlightedCoords;
    private char[] boardLetters;
    private string[] boardNumbers;
    private const int MAX_DIM = 26;

    public readonly Color primaryBoardColour;
    public readonly Color secondaryBoardColour;

    public Board(uint width, uint height, Color primaryBoardColour, Color secondaryBoardColour) {
        highlightedCoords = new List<BoardCoord>();
        coordinates = new Dictionary<BoardCoord, CoordInfo>();
        boardWidth = width;
        boardHeight = height;
        allowFlipping = false;
        isFlipped = false;
        this.primaryBoardColour = primaryBoardColour;
        this.secondaryBoardColour = secondaryBoardColour;

        GenerateBoardCoordinateValues();
        gameBoardObj = InstantiateGameBoard();
    }

    private GameObject InstantiateGameBoard() {
        if (this.GetWidth() > MAX_DIM || this.GetHeight() > MAX_DIM) {
            Debug.LogError(string.Format("Board dimensions greater than {0} are not allowed.", MAX_DIM));
        }

        GameObject gameBoardObj = new GameObject("Board");
        GameObject boardChunkObj = GameManager.Instance.boardChunkPrefab;

        //Create the board from the bottom up, row by row.
        for (int y = 0; y < this.GetHeight(); y++) {
            for (int x = 0; x < this.GetWidth(); x++) {
                //Instantiate board piece at (x,y) and parent it to the board.
                GameObject go = MonoBehaviour.Instantiate
                    (boardChunkObj, new Vector3(x, y), boardChunkObj.transform.rotation, gameBoardObj.transform);

                //Rename piece to match the board coordinate its on.
                coordinates.Add(new BoardCoord(x, y), new CoordInfo(boardLetters[x] + boardNumbers[y], go));
                go.name = this.GetCoordInfo(new BoardCoord(x, y)).algebraicKey;

                //Alternate piece colour with each instantiation.
                Material mat = go.GetComponent<Renderer>().material;
                mat.color = ((y + x) % 2 != 0) ? this.primaryBoardColour : this.secondaryBoardColour;
            }
        }
        return gameBoardObj;
    }

    private void GenerateBoardCoordinateValues() {
        StringBuilder letters = new StringBuilder(MAX_DIM);
        StringBuilder numbers = new StringBuilder(MAX_DIM);

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

    /// <summary>
    /// Retrieves the details of a coordinate on the board.
    /// </summary>
    /// <param name="algebraicKey">Algebraic key value to search for.</param>
    /// <returns></returns>
    public CoordInfo GetCoordInfo(string algebraicKey) {
        if (TryGetCoordWithKey(algebraicKey, out BoardCoord coord)) {
            return coordinates[coord];
        }

        Debug.LogErrorFormat("ERROR: The GameBoard does not contain a CoordInfo for coordinate key: {0}", algebraicKey);
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

    public bool ContainsCoord(string coordKey) {
        return TryGetCoordWithKey(coordKey, out BoardCoord coord);
    }

    public void RemoveAndDestroyBoardCoordinates(string[] coordKeys) {
        for (int i = 0; i < coordKeys.Length; i++) {
            RemoveAndDestroyBoardCoordinate(coordKeys[i]);
        }
    }

    public bool RemoveAndDestroyBoardCoordinate(string coordKey) {
        if (TryGetCoordWithKey(coordKey, out BoardCoord coord)) {
            MonoBehaviour.Destroy(GetCoordInfo(coord).boardChunk);
            coordinates.Remove(coord);
            return true;
        }
        return false;
    }

    public void RemoveAndDestroyBoardCoordinates(BoardCoord[] coords) {
        for (int i = 0; i < coords.Length; i++) {
            RemoveAndDestroyBoardCoordinate(coords[i]);
        }
    }

    public bool RemoveAndDestroyBoardCoordinate(BoardCoord coord) {
        if (ContainsCoord(coord)) {
            MonoBehaviour.Destroy(GetCoordInfo(coord).boardChunk);
            coordinates.Remove(coord);
            return true;
        }
        return false;
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

    public bool SetCustomBoardAlgebraicKey(string coordToChange, string newAlgebraicKey) {
        if (TryGetCoordWithKey(coordToChange, out BoardCoord coord)) {
            return SetCustomBoardAlgebraicKey(coord, newAlgebraicKey);
        }
        return false;
    }

    public bool SetCustomBoardAlgebraicKey(BoardCoord coordToChange, string newAlgebraicKey) {
        if (ContainsCoord(coordToChange)) {
            coordinates[coordToChange].algebraicKey = newAlgebraicKey;
            coordinates[coordToChange].boardChunk.name = newAlgebraicKey;
            return true;
        }
        return false;
    }

    public void SetCustomBoardAlgebraicKeys(string coordKeyToStartFrom, int stopAfterXPos, int stopAfterYPos) {
        if (TryGetCoordWithKey(coordKeyToStartFrom, out BoardCoord coordToStartFrom)) {
            SetCustomBoardAlgebraicKeys(coordToStartFrom, stopAfterXPos, stopAfterYPos);
        }
    }

    public void SetCustomBoardAlgebraicKeys(BoardCoord coordToStartFrom, int stopAfterXPos, int stopAfterYPos) {
        if (stopAfterXPos >= MAX_DIM || stopAfterYPos >= MAX_DIM || coordToStartFrom.x >= MAX_DIM || coordToStartFrom.y >= MAX_DIM) {
            Debug.LogError(string.Format("Board dimensions greater than {0} are not allowed.", MAX_DIM));
        } else if (coordToStartFrom.x < 0 || coordToStartFrom.y < 0) {
            Debug.LogError("Starting boardcoord values are undefined! Do not use negative numbers.");
        }

        for (int y = coordToStartFrom.y; y <= stopAfterYPos; y++) {
            for (int x = coordToStartFrom.x; x <= stopAfterXPos; x++) {
                SetCustomBoardAlgebraicKey(new BoardCoord(x, y), boardLetters[x - coordToStartFrom.x] + boardNumbers[y - coordToStartFrom.y]);
            }
        }
    }
}
