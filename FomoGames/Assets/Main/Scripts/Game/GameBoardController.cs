using System;
using System.Collections.Generic;
using DG.Tweening;
using Main.Scripts.General;
using Main.Scripts.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Main.Scripts.Game
{
    public class GameBoardController
    {
        public bool IsThereAnyBlock => Board.IsThereAnyBlock;
        
        public Board Board { get; private set; }
        
        private GameManager _gameManager;
        private Transform _boardParent;
        private Vector3 _initialPosition;
        private readonly Dictionary<Block, BlockView> _blockPairs = new();
        private readonly Dictionary<Gate, GateView> _gatePairs = new();
        
        public void Init(LevelData levelData)
        {
            Board = new Board(levelData);
            _gameManager = ContextController.Instance.GameManager;
            _boardParent = new GameObject("Board").transform;
            _initialPosition = new Vector3((1 - Board.ColumnCount) / 2f, 0f, -(1 - Board.RowCount) / 2f) * Board.CellWidth;
            
            SpawnBoardGround();
            SpawnBlocks();
            SpawnGates();
        }
        
        private void SpawnBoardGround()
        {
            var boardGroundPrefab = _gameManager.BoardAssets.boardGround;
            var boardGround = Object.Instantiate(boardGroundPrefab, _boardParent);
            boardGround.localScale = new Vector3(Board.ColumnCount, 1f, Board.RowCount) * 0.1f;
            boardGround.position = new Vector3(0, -0.01f, 0f);
            var tiling = new Vector2(Board.ColumnCount, Board.RowCount);
            boardGround.GetComponent<MeshRenderer>().material.mainTextureScale = tiling;
        }
        
        private void SpawnBlocks()
        {
            var blocks = Board.Blocks;
            for (var i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                var position = GetCellPosition(block.PivotI, block.PivotJ);
                var rotation = Quaternion.Euler(0f, 90f * ((block.BlockDirection.ToInt() + 1) % 2), 0f);
                var blockPrefab = _gameManager.BoardAssets.GetBlockPrefab(block.Length);
                var blockView = Object.Instantiate(blockPrefab, position, rotation, _boardParent);
                blockView.Init(block);
                
                _blockPairs.Add(block, blockView);
            }
        }
        
        private void SpawnGates()
        {
            var gates = Board.Gates;
            for (var index = 0; index < gates.Count; index++)
            {
                var gate = gates[index];
                var position = GetCellPosition(gate.PivotI, gate.PivotJ);
                var rotation = Quaternion.Euler(0f, 90f * gate.GateDirection.ToInt(), 0f);
                var gatePrefab = _gameManager.BoardAssets.gatePrefab;
                var gateView = Object.Instantiate(gatePrefab, position, rotation, _boardParent);
                gateView.Init(gate);
                
                _gatePairs.Add(gate, gateView);
            }
        }
        
        public void SelectBlock(int id, out Block block)
        {
            block = GetBlock(id);
            var blockView = _blockPairs[block];
            blockView.Select();
        }
        
        public void DeselectBlock(int id)
        {
            var block = GetBlock(id);
            var blockView = _blockPairs[block];
            blockView.Deselect();
        }
        
        public bool TryMoveBlock(int id, BlockDirection moveDirection, out Sequence sequence)
        {
            var block = GetBlock(id);
            var blockView = _blockPairs[block];
            var pivotI = block.PivotI;
            var pivotJ = block.PivotJ;
            var willExit = Board.GetTargetIndex(block.ID, moveDirection, 
                out var targetI, out var targetJ, 
                out var outsideI, out var outsideJ, 
                out var gate);
            var isMoved = !(targetI == pivotI && targetJ == pivotJ);
            
            var targetPosition = GetCellPosition(targetI, targetJ);
            var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
            var duration = 0.14f * difference;
            
            sequence = DOTween.Sequence().SetLink(blockView.gameObject);
            if (willExit)
            {
                Board.ExitBlock(block.ID);
                _blockPairs.Remove(block);
                
                var outsidePosition = GetCellPosition(outsideI, outsideJ);
                var difference2 = Math.Max(Math.Abs(outsideI - targetI), Math.Abs(outsideJ - targetJ));
                var duration2 = 0.1f * difference2;
                var gateView = _gatePairs[gate];
                blockView.DisableCollider();
                
                sequence.AppendCallback(gateView.Open);
                sequence.Append(blockView.transform.DOMove((targetPosition + outsidePosition) / 2f, duration).SetEase(Ease.OutExpo));
                sequence.Append(blockView.transform.DOMove(outsidePosition, duration2).SetEase(Ease.Linear));
                sequence.AppendInterval(0.1f);
                sequence.AppendCallback(() => Object.Destroy(blockView.gameObject));
            }
            else
            {
                Board.ReplaceBlock(block.ID, targetI, targetJ);
                
                sequence.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutExpo));
            }
            
            return isMoved;
        }
        
        private Block GetBlock(int id)
        {
            return Board.GetBlock(id);
        }
        
        private Vector3 GetCellPosition(float i, float j)
        {
            return _initialPosition + new Vector3(j, 0f, -i) * Board.CellWidth;
        }
        
        public void Clear()
        {
            Object.Destroy(_boardParent?.gameObject);
            _boardParent = null;
            Board = null;
        }
    }
}
