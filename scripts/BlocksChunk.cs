using Godot;
using Godot.Collections;
using System;

public enum BlockType
{
    BT_EMPTY,
    BT_FILLED,
    BT_NONE
}

public partial class BlocksChunk : Node2D
{
    const int BlockSideSize = 32; // according to block sprite 
    const int GridSideSize = 200;
    Array<Array<bool>> Grid = new Array<Array<bool>>();

    public Vector2 PlayerStart;

    

    private void FillGrid()
    {
        
        for (int i = 0; i < GridSideSize; i++)
        {
            Grid.Add(new Array<bool>());
            for (int j = 0; j < GridSideSize; j++)
            {
                RandomNumberGenerator randomGenerator = new RandomNumberGenerator();
                float randomNumber = randomGenerator.RandfRange(0, 1);
                Grid[i].Add(randomNumber < 0.45);
            }
        }
    }

    private void GenerateLevel()
    {
        for (int i = 0; i < 4; i++)
        {
            Array<Array<bool>> bufferGrid = Grid.Duplicate(true);
            for (int x = 0; x < GridSideSize; x++)
            {
                for (int y = 0; y < GridSideSize; y++)
                {
                    int wallCount = CountNeighbouringWalls(x, y);
                    if (Grid[x][y])
                    {
                        bufferGrid[x][y] = wallCount > 3;
                    }
                    else
                    {
                        bufferGrid[x][y] = wallCount > 4;
                    }
                }
            }
            Grid = bufferGrid;
        }
    }

    private int CountNeighbouringWalls(int x, int y)
    {
        int result = 0;
        for (int i = -1; i < 2; i++)
        {
            for (int j = - 1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int newX = x + i;
                int newY = y + j;
                if (CheckGridBounds(newX, newY))
                {
                    result++;
                }
                else if (Grid[newX][newY])
                {
                    result++;
                }
            }
        }
        return result;
    }

    private bool CheckGridBounds(int x, int y)
    {
        return x < 0 || x >= GridSideSize || y < 0 || y >= GridSideSize;
    }

    private void DrawLevel()
    {
        for (int i = 0; i < Grid.Count; i++)
        {
            for (int j = 0; j < Grid[i].Count; j++)
            {
                if (Grid[i][j])
                {
                    int currentX = i * BlockSideSize;
                    int currentY = j * BlockSideSize;
                    Vector2 blockPosition = new Vector2(currentX, currentY);
                    SpawnBlock(blockPosition);
                }
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
    private void FindAndSetPlayerPosition()
    {
        bool done = false;
        RandomNumberGenerator randomGenerator = new RandomNumberGenerator();
        while (!done)
        {
            int randX = randomGenerator.RandiRange(0, GridSideSize);
            int randY = randomGenerator.RandiRange(0, GridSideSize);
            if (!Grid[randX][randY])// && Grid[randX][randY-1] && Grid[randX][randY + 1])
            {
                PlayerStart.X = randX * BlockSideSize + Position.X;
                PlayerStart.Y = randY * BlockSideSize + Position.Y;
                done = true;
            }
        }
    }
    public override void _Ready()
    {
        FillGrid();
        GenerateLevel();
        DrawLevel();
        FindAndSetPlayerPosition();
    }

}
