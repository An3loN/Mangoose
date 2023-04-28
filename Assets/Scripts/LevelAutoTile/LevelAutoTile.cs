using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/AutoTile")]
public class LevelAutoTile : TileBase
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private List<Sprite> sprites;

    public static byte[,] schemataMatrixOrder = new byte[,]
    {
        {0, 1, 2},
        {7, 0, 3},
        {6, 5, 4}
    };

    private AutoTileSample[] autoTileSampleArray = new AutoTileSample[]
    {
        new AutoTileSample(new byte[]{255,},new byte[]{6,}),
        new AutoTileSample(new byte[]{170,},new byte[]{32,}),
        new AutoTileSample(new byte[]{85,21,69,5,81,17,65,1,84,20,68,4,80,16,64,0,},new byte[]{31,}),
        new AutoTileSample(new byte[]{221,157,205,141,217,153,201,137,220,156,204,140,216,152,200,136,},new byte[]{16,25,}),
        new AutoTileSample(new byte[]{187,},new byte[]{45,46,}),
        new AutoTileSample(new byte[]{253,249,252,248,},new byte[]{1,7,11,5,}),
        new AutoTileSample(new byte[]{173,169,172,168,},new byte[]{37,39,38,40,}),
        new AutoTileSample(new byte[]{181,165,177,161,180,164,176,160,},new byte[]{4,9,8,3,}),
        new AutoTileSample(new byte[]{213,149,197,133,209,145,193,129,212,148,196,132,208,144,192,128,},new byte[]{17,26,15,24,}),
        new AutoTileSample(new byte[]{245,229,241,225,244,228,240,224,},new byte[]{2,12,10,0,}),
        new AutoTileSample(new byte[]{222,158,206,142,},new byte[]{30,33,28,36,}),
        new AutoTileSample(new byte[]{189,185,188,184,},new byte[]{27,34,29,35,}),
        new AutoTileSample(new byte[]{175,},new byte[]{20,21,22,23,}),
        new AutoTileSample(new byte[]{171,},new byte[]{41,42,43,44,}),
        new AutoTileSample(new byte[]{191,},new byte[]{14,19,18,13,}),

    };
    
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
        RefreshEnvironment(position, tilemap);
    }

    string GetStringFromByte(byte value)
    {
        byte devadingValue = value;
        string reversedResult = "";
        for (int index = 0; index < 8; index++)
        {
            reversedResult += (devadingValue % 2).ToString();
            devadingValue /= 2;
        }

        string result = "";
        for (int index = 7; index > -1; index--)
        {
            result += reversedResult[index];
        }
        return result;
    }

    byte GetShiftedByte(byte source, int shift)
    {
        byte shiftedByte = source;
        for (int iteration = 0; iteration < shift; iteration++)
        {
            shiftedByte = (byte)(shiftedByte/2 + Mathf.Pow(2*(shiftedByte%2),7));
        }
        return shiftedByte;
    }
    
    byte GetByteFromBoolArray(bool[] boolArray)
    {
        byte result = 0;
        for (int index = 0; index < 8; index++)
        {
            if(boolArray[index]) result += (byte)Mathf.Pow(2, index);
        }
        return result;
    }

    void DoForEachNeighbourTile(Vector3Int position, Action<Vector3Int, int, int> doThis)
    {
        for (int line = 0; line < 3; line++)
        {
            for (int row = 0; row < 3; row++)
            {
                if(line == 1 && row == 1) continue;
                doThis(new Vector3Int(position.x - 1 + row, position.y + 1 - line, 0), line, row);
            }
        }
    }

    byte GetEnvironmentSchemataByte(Vector3Int position, ITilemap tilemap)
    {
        bool[] schemata = new bool[8];
        DoForEachNeighbourTile(position, (neighbourTilePosition,line,row) =>
        {
            schemata[schemataMatrixOrder[line, row]] = tilemap.GetTile(neighbourTilePosition);
        });
        return GetByteFromBoolArray(schemata);
    }

    void RefreshEnvironment(Vector3Int position, ITilemap tilemap)
    {
        DoForEachNeighbourTile(position, (neighbourTilePosition, line, row) =>
        {
            tilemap.RefreshTile(neighbourTilePosition);
        });
    }
    
    Sprite CalculateSuitableTile(Vector3Int position, ITilemap tilemap)
    {
        byte schemataByte = GetEnvironmentSchemataByte(position, tilemap);
        Sprite sprite = defaultSprite;
        for(byte rotation = 0; rotation < 4; rotation ++)
        {
            byte schemataByteRotation = GetShiftedByte(schemataByte, rotation * 2);
            foreach (var sample in autoTileSampleArray)
            {
                byte[] byteDataArray = sample.schemataArray;
                for (int index = 0; index < byteDataArray.Length; index++)
                {
                    if (byteDataArray[index] == schemataByteRotation)
                    {
                        sprite = sprites[sample.spriteIDArray[rotation % sample.spriteIDArray.Length]];
                        return sprite;
                    }
                }
            }
        }
        return sprite;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.colliderType = Tile.ColliderType.Sprite;
        tileData.sprite = CalculateSuitableTile(position, tilemap);
    }
}

class AutoTileSample
{
    public byte[] schemataArray;
    public byte[] spriteIDArray;

    public AutoTileSample(byte[] schemataArray, byte[] spriteIDArray)
    {
        this.schemataArray = schemataArray;
        this.spriteIDArray = spriteIDArray;
    }
}