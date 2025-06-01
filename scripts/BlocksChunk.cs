using Godot;
using Godot.Collections;
using System;

public enum BlockType
{
    BT_EMPTY,
    BT_FILLED,
    BT_NONE
}

public partial class BlocksChunk : Node
{
    const int BlockSideSize = 32; // according to block sprite 
    const int GridSideSize = 24;
    Array<BlockType> Grid = new Array<BlockType>();

    private void FillGrid()
    {
        for (int i = 0; i < GridSideSize; i++)
        {
            for (int j = 0; j < GridSideSize; j++)
            {
                Grid.Add(BlockType.BT_FILLED);
            }
        }
    }
    private void GenerateLevel()
    {
        //int currentRowIdx = 0;
        //int currentColumnIdx = 0;

        for (int i = 0; i < Grid.Count; i++)
        {
            if (Grid[i] == BlockType.BT_FILLED)
            {
                int currentX = (i % GridSideSize) * BlockSideSize;
                int currentY = (i / GridSideSize) * BlockSideSize;
                Vector2 blockPosition = new Vector2(currentX, -currentY);
                SpawnBlock(blockPosition);
            }
        }
    }
    private void SpawnBlock(Vector2 position)
    {
        PackedScene blockScene = GD.Load<PackedScene>("res://scenes/environment/Block01.tscn");
        Node2D blockInstance = (Node2D)blockScene.Instantiate();
        blockInstance.Position = position;
        AddChild(blockInstance);
    }
    public override void _Ready()
    {
        FillGrid();
        GenerateLevel();
    }

}
