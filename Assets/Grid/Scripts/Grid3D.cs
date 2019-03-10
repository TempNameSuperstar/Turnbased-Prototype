﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GridNamespace
{
    public class Grid3D : MonoBehaviour
    {
        #region Fields
        public static Grid3D grid = null;

        public Vector3Int gridSize;
        public Vector3 cellSize = Vector3.one;


        private GameObject floorQuad;


        public ScriptableObject[,,] GridTiles { get; private set; }


        #endregion

        #region Public methods and accessors
        // Get the local position of a cell
        public Vector3 CellToLocal(Vector3Int cellPosition)
        {
            return cellPosition;
        }

        // Get the world position of a cell
        public Vector3 CellToWorld(Vector3Int cellPosition)
        {
            return cellPosition + transform.position;
        }

        // Get the bounds of a cell
        public Bounds GetBoundsLocal(Vector3Int cellPosition)
        {
            return new Bounds(cellPosition, cellSize);
        }

        // Get a cell from local position
        public Vector3Int LocalToCell(Vector3 localPosition)
        {
            return new Vector3Int(
                Mathf.FloorToInt(localPosition.x * cellSize.x),
                Mathf.FloorToInt(localPosition.y * cellSize.y),
                Mathf.FloorToInt(localPosition.z * cellSize.z)
                );
        }

        // Get world position from local position
        public Vector3 LocalToWorld(Vector3 localPosition)
        {
            return localPosition + transform.position;
        }

        // Get a cell fro world position
        public Vector3Int WorldToCell(Vector3 worldPosition)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPosition.x * cellSize.x),
                Mathf.FloorToInt(worldPosition.y * cellSize.y),
                Mathf.FloorToInt(worldPosition.z * cellSize.z)
                );
        }

        // Get local position from world position
        public Vector3 WorldToLocal(Vector3 worldPosition)
        {
            return worldPosition - transform.position;
        }

        public Vector3 GetCellOffset()
        {
            return cellSize / 2;
        }

        public Vector3 GetGridCenter()
        {
            return (Vector3)gridSize / 2;
        }

        #endregion

        #region Internal Methods

        private void Awake()
        {
            if (grid == null) {
                grid = this;
            }
            else if (grid != this) {
                Debug.LogWarning("Tried to instantiate additional Grid3D");
                Destroy(gameObject);
            }

            InitializeTileList();
        }

        private void Start()
        {
            floorQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            floorQuad.transform.position = Vector3.Scale(GetGridCenter(), new Vector3(1, 0, 1));
            floorQuad.transform.rotation = Quaternion.Euler(90 * Vector3.right);
            floorQuad.transform.localScale = new Vector3(gridSize.x, gridSize.z, 0);
        }


        private void InitializeTileList()
        {
            //Sets a default Grid if non exists
            if (gridSize == null || gridSize == Vector3.zero) {
                gridSize = new Vector3Int(10, 10, 10);
            }

            GridTiles = new GridTile[gridSize.x, gridSize.y, gridSize.z];

            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    for (int z = 0; z < gridSize.z; z++) {
                        GridTile tile = ScriptableObject.CreateInstance(typeof(GridTile)) as GridTile;
                        tile.Initialize(new Vector3(x, y, z));


                        GridTiles[x, y, z] = tile;
                    }
                }
            }
        }


        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            Color gridColor = Color.red;
            gridColor.a = 0.5f;
            Gizmos.color = gridColor;
            
            DrawGrid(gridSize.x, gridSize.z, cellSize.x, cellSize.z);
            DrawCorners();
            DrawCenter();
        }


        private void DrawGrid(int xMax, int zMax, float xCel, float zCel)
        {
            Vector3 transformOffset = Vector3.Scale((transform.position + cellSize), new Vector3(1, 0, 1));
            float heightOffset = 0;
            int i = 0;
            int j = 0;

            for (i = 0; i < xMax + 1; i++) {
                Vector3 iFrom = new Vector3(i - xCel, heightOffset, 0 - zCel) + transformOffset;
                Vector3 iTo = new Vector3(i - xCel, heightOffset, zMax - zCel) + transformOffset;

                Gizmos.DrawLine(iFrom, iTo);
            }
            for (j = 0; j < zMax + 1; j++) {
                Vector3 jFrom = new Vector3(0 - xCel, heightOffset, j - zCel) + transformOffset;
                Vector3 jTo = new Vector3(xMax - xCel, heightOffset, j - zCel) + transformOffset;

                Gizmos.DrawLine(jFrom, jTo);
            }
        }

        private void DrawCorners()
        {
            Vector3[] corners = 
                { Vector3.zero, gridSize,
                new Vector3(0, 0, gridSize.z), new Vector3(0, gridSize.y, 0),new Vector3(gridSize.x, 0, 0),
                new Vector3(0, gridSize.y, gridSize.z), new Vector3(gridSize.x, 0, gridSize.z), new Vector3(gridSize.x, gridSize.y, 0)
            };
            float radius = 0.1f;

            foreach (Vector3 corner in corners) {
                Gizmos.DrawSphere(corner, radius);
            }
        }

        private void DrawCenter()
        {
            Gizmos.DrawSphere(GetGridCenter(), 0.1f);
        }

        #endregion
    }

}