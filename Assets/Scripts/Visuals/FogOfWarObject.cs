using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace AgeOfHeroes
{
    public enum FogOfWarCellType
    {
        Clear = 0,
        Low = 1,
        Deep = 2
    }

    public class FogOfWarObject
    {
        private byte[,] _fogMap;

        public string name;

        public Vector2Int Size
        {
            get => new Vector2Int(sizeX, sizeY);
        }

        private int sizeX, sizeY;

        public FogOfWarObject(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            _fogMap = new byte[sizeX, sizeY];
            SetEveryCell(FogOfWarCellType.Deep);
        }

        public byte[,] FogMap => _fogMap;

        public void SetEveryCell(FogOfWarCellType fogOfWarCellType)
        {
            for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                _fogMap[i, j] = (byte)fogOfWarCellType;
            }
        }

        public void SetAllCellsInList(List<Vector2Int> cells, FogOfWarCellType fogOfWarCellType)
        {
            foreach (var cell in cells)
            {
                _fogMap[cell.x, cell.y] = (byte)fogOfWarCellType;
            }
        }

        public List<Vector2Int> GetAllCellsOfType(FogOfWarCellType fogOfWarCellType)
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            for (int i = 0; i < sizeX; i++)
            for (int j = 0; j < sizeY; j++)
            {
                FogOfWarCellType cellType = (FogOfWarCellType)_fogMap[i, j];
                if (cellType == fogOfWarCellType)
                    cells.Add(new Vector2Int(i, j));
            }

            return cells;
        }

        public void SetCell(Vector2Int position, FogOfWarCellType fogOfWarCellType)
        {
            // if (position.x >= sizeX || position.y >= sizeY) return;
            _fogMap[position.x, position.y] = (byte)fogOfWarCellType;
        }

        public FogOfWarCellType GetCell(Vector2Int position)
        {
            return (FogOfWarCellType)_fogMap[position.x, position.y];
        }
    }
}