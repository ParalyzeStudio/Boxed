using System.Collections.Generic;
using UnityEngine;

public class BrickRenderer : MonoBehaviour
{
    public Brick m_brick { get; set; }

    private Geometry.Edge m_fallRotationEdge;
    private bool m_transformRotationEdgeToLocal;
    private Vector3 m_fallDirection;

    public const float DEFAULT_ANGULAR_SPEED = 90 / 0.3f;

    public GameObject m_cubePartPfb;
    
    public bool m_brickTeleporting { get; set; }

    //fragments
    public BrickFragmenter m_brickFragmenter;

    //FX
    public GlowCube m_glowCubePfb;
    public ParticleSystem m_teleportInFX;

    /**
    * Build a rectangular cuboid that will serve as our main object in the scene.
    * To handle lighting correctly, our cuboid will need 24 vertices (instead of 8) so light is interpolated correctly so one face has one single color.
    * Pass the tile or tiles (max 2) the brick should be upon
    **/
    public void BuildOnTiles(Tile[] tiles)
    {
        m_brick = new Brick();

        //build mesh vertices
        Vector3[] vertices = new Vector3[24];
        
        for (int i = 0; i != m_brick.Faces.Length; i++)
        {
            Brick.BrickFace face = m_brick.Faces[i];
            for (int j = 0; j != face.m_indices.Length; j++)
            {
                vertices[i * 4 + j] = m_brick.Vertices[face.m_indices[j]];
            }
        }

        //Construct mesh triangles and normals
        int[] triangles = new int[36];
        Vector3[] normals = new Vector3[24];
        
        for (int i = 0; i != 6; i++)
        {
            triangles[i * 6] = i * 4;
            triangles[i * 6 + 1] = i * 4 + 2;
            triangles[i * 6 + 2] = i * 4 + 1;
            triangles[i * 6 + 3] = i * 4;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 2;

            for (int p = 0; p != 4; p++)
            {
                normals[i * 4 + p] = m_brick.Faces[i].m_normal;
            }
        }

        //build uv map
        Vector2[] baseUV = new Vector2[7];
        //baseUV[0] = new Vector2(0.219f, 1 - 0.661f);
        //baseUV[1] = new Vector2(0.507f, 1 - 0.847f);
        //baseUV[2] = new Vector2(0.786f, 1 - 0.675f);
        //baseUV[3] = new Vector2(0.219f, 1 - 0.305f);
        //baseUV[4] = new Vector2(0.5f, 1 - 0.505f);
        //baseUV[5] = new Vector2(0.784f, 1 - 0.31f);
        //baseUV[6] = new Vector2(0.505f, 1 - 0.125f);

        baseUV[0] = new Vector2(0.091f, 1 - 0.736f);
        baseUV[1] = new Vector2(0.5f, 1 - 0.969f);
        baseUV[2] = new Vector2(0.906f, 1 - 0.736f);
        baseUV[3] = new Vector2(0.091f, 1 - 0.263f);
        baseUV[4] = new Vector2(0.5f, 1 - 0.505f);
        baseUV[5] = new Vector2(0.906f, 1 - 0.264f);
        baseUV[6] = new Vector2(0.5f, 1 - 0.027f);


        Vector2[] uv = new Vector2[24];
        uv[0] = baseUV[3];
        uv[1] = baseUV[4];
        uv[2] = baseUV[5];
        uv[3] = baseUV[6];
        uv[4] = baseUV[4];
        uv[5] = baseUV[5];
        uv[6] = baseUV[6];
        uv[7] = baseUV[3];

        uv[8] = baseUV[1];
        uv[9] = baseUV[2];
        uv[10] = baseUV[5];
        uv[11] = baseUV[4];

        uv[12] = baseUV[0];
        uv[13] = baseUV[1];
        uv[14] = baseUV[4];
        uv[15] = baseUV[3];

        uv[16] = baseUV[1];
        uv[17] = baseUV[2];
        uv[18] = baseUV[5];
        uv[19] = baseUV[4];

        uv[20] = baseUV[0];
        uv[21] = baseUV[1];
        uv[22] = baseUV[4];
        uv[23] = baseUV[3];

        Mesh brickMesh = new Mesh();
        brickMesh.name = "BrickMesh";
        brickMesh.vertices = vertices;
        brickMesh.triangles = triangles;
        brickMesh.normals = normals;
        brickMesh.uv = uv;

        brickMesh.RecalculateBounds();

        MeshFilter brickMeshFilter = this.GetComponent<MeshFilter>();
        brickMeshFilter.sharedMesh = brickMesh;

        //place the brick upon the tile parameter
        PlaceOnTiles(tiles);
        
        //GetComponent<MeshRenderer>().sharedMaterial.SetInt("_ZWrite", 1);
    }

    /**
    * Adjust the position and the rotation of the brick so it covers the tiles passed as parameters
    * tile2 can be null if there is only one covered tile
    **/
    public void PlaceOnTiles(Tile[] tiles)
    {
        m_brick.PlaceOnTiles(tiles);

        GameObjectAnimator brickAnimator = this.GetComponent<GameObjectAnimator>();
        brickAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f)); //place the pivot point at the center of the brick     

        if (tiles[1] == null)
        {
            brickAnimator.transform.rotation = Quaternion.identity; //null rotation  
            brickAnimator.SetPosition(tiles[0].GetXZWorldPosition() + new Vector3(0, 1, 0));
        }
        else
        {
            brickAnimator.transform.rotation = m_brick.m_rotation;
            brickAnimator.SetPosition(0.5f * (tiles[0].GetWorldPosition() + tiles[1].GetXZWorldPosition()) + new Vector3(0, 0.5f, 0));
        }
    }

    /**
    * Make the brick roll on one of the 4 available directions (left, top, right, bottom)
    **/
    public void Roll(Brick.RollDirection rollDirection)
    {
        Tile[] previousCoveredTiles = m_brick.m_coveredTiles.GetAsTruncatedArray(); //store the previously covered tiles for further use

        Brick.RollResult rollResult;
        Geometry.Edge rotationEdge;
        m_brick.Roll(rollDirection, out rollResult, out rotationEdge);

        if (rollResult == Brick.RollResult.VALID || rollResult == Brick.RollResult.FALL)
        {
            //Get the rotation axis from the rotation edge
            Vector3 rotationAxis = rotationEdge.m_pointB - rotationEdge.m_pointA;

            if (rollResult == Brick.RollResult.VALID)
            {
                //perform the normal rotation of the brick
                BrickAnimator brickAnimator = GetComponent<BrickAnimator>();
                brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(rotationEdge.GetMiddle()));
                brickAnimator.SetRotationAxis(rotationAxis);
                float rotationDuration = 90 / DEFAULT_ANGULAR_SPEED;
                brickAnimator.RotateBy(90, rotationDuration);

                if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME)
                {
                    //increment the actions count
                    GameController.GetInstance().GetComponent<LevelManager>().m_currentLevelData.m_currentActionsCount++;

                    //update the GameGUI
                    GameGUI gameGUI = (GameGUI)GameController.GetInstance().GetComponent<GUIManager>().m_currentGUI;
                    gameGUI.UpdateParScore();
                }
            }
            else
            {
                CallFuncHandler callFuncHandler = GameController.GetInstance().GetComponent<CallFuncHandler>();

                Brick.CoveredTiles coveredTiles = m_brick.m_coveredTiles;
                float normalRotationAngle = 0;
                bool bPrefall = false;

                if (coveredTiles.GetCount() == 2) //2 tiles are covered
                {
                    if (coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.DISABLED && coveredTiles.GetTileAtIndex(1).CurrentState == Tile.State.DISABLED) //brick fell on two disabled tiles
                    {
                        m_fallRotationEdge = rotationEdge;
                        m_fallDirection = Brick.GetVector3DirectionForRollingDirection(rollDirection);
                        m_transformRotationEdgeToLocal = false;
                        normalRotationAngle = 135;
                        bPrefall = false;
                    }
                    else if (coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.NORMAL || coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.START || coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.FINISH) //first tile is normal and second one is disabled
                    {
                        normalRotationAngle = 90;
                        m_fallDirection = coveredTiles.GetTileAtIndex(1).GetWorldPosition() - coveredTiles.GetTileAtIndex(0).GetWorldPosition();
                        m_fallRotationEdge = Floor.GetCommonEdgeForConsecutiveTiles(coveredTiles.GetTileAtIndex(0), coveredTiles.GetTileAtIndex(1));
                        m_transformRotationEdgeToLocal = true;
                        bPrefall = true;
                    }
                    else /*if (coveredTiles[1].CurrentState == Tile.State.NORMAL)*/ //second tile is normal and first one is disabled
                    {
                        normalRotationAngle = 90;
                        m_fallDirection = coveredTiles.GetTileAtIndex(0).GetWorldPosition() - coveredTiles.GetTileAtIndex(1).GetWorldPosition();
                        m_fallRotationEdge = Floor.GetCommonEdgeForConsecutiveTiles(coveredTiles.GetTileAtIndex(1), coveredTiles.GetTileAtIndex(0));
                        m_transformRotationEdgeToLocal = true;
                        bPrefall = true;
                    }
                }
                else
                {
                    normalRotationAngle = 135;
                    m_fallRotationEdge = rotationEdge;
                    m_fallDirection = Brick.GetVector3DirectionForRollingDirection(rollDirection);
                    m_transformRotationEdgeToLocal = false;
                    bPrefall = false;
                }

                //perform the normal rotation of the brick
                BrickAnimator brickAnimator = GetComponent<BrickAnimator>();
                brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(rotationEdge.GetMiddle()));
                brickAnimator.SetRotationAxis(rotationAxis);
                float rotationDuration = normalRotationAngle / DEFAULT_ANGULAR_SPEED;
                brickAnimator.RotateBy(normalRotationAngle, rotationDuration);

                //perform the prefall action if needed
                if (bPrefall)
                {
                    //rotate the brick to get to the point where it is going to fall
                    callFuncHandler.AddCallFuncInstance(PreFall, rotationDuration);
                }

                //finally make it fall
                callFuncHandler.AddCallFuncInstance(Fall, rotationDuration + (bPrefall ? 45 / DEFAULT_ANGULAR_SPEED : 0));
            }

            //if one or more previous covered tiles were of type ICE, perform collapse action on them
            for (int i = 0; i != previousCoveredTiles.Length; i++)
            {
                if (previousCoveredTiles[i].CurrentState == Tile.State.ICE)
                {
                    TileRenderer iceTileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(previousCoveredTiles[i]);
                    iceTileRenderer.LiftUp();
                }
            }
        }
    }

    /**
    * In case the normal rotation of the brick is not enough to make the brick fall, rotate it again by 45 degrees around the floor edge so it reaches the point it will start falling
    **/
    private void PreFall()
    {    
        if (m_transformRotationEdgeToLocal)
        {
            //transform this edge into brick local coordinates
            m_fallRotationEdge.Translate(-this.transform.localPosition);
            m_fallRotationEdge.ApplyRotation(Quaternion.Inverse(m_brick.m_rotation));
        }

        Vector3 rotationAxis = m_fallRotationEdge.m_pointB - m_fallRotationEdge.m_pointA;

        //rotates the brick until the gravity makes it fall
        BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
        brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(m_fallRotationEdge.GetMiddle()));
        brickAnimator.SetRotationAxis(rotationAxis);
        float rotationAngle = 45;
        float rotationDuration = rotationAngle / DEFAULT_ANGULAR_SPEED;
        brickAnimator.RotateBy(rotationAngle, rotationDuration);
    }

    private void Fall()
    {
        Vector3 fallRotationAxis = m_fallRotationEdge.m_pointB - m_fallRotationEdge.m_pointA;

        BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
        brickAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f));

        //make the brick fall
        float fallHeight = 10.0f;
        float fallSpeed = 1.5f  * 0.5f * Mathf.Deg2Rad * DEFAULT_ANGULAR_SPEED;
        float fallDuration = fallHeight / fallSpeed;

        Vector3 brickToPosition = brickAnimator.GetPosition() + new Vector3(0, -fallHeight, 0) + 4.8f * m_fallDirection;
        brickAnimator.TranslateTo(brickToPosition, fallDuration, 0, ValueAnimator.InterpolationType.LINEAR, true);

        brickAnimator.SetRotationAxis(fallRotationAxis);
        brickAnimator.RotateBy(540, fallDuration);
    }

    private void Explode()
    {
        //if (m_brick.m_coveredTiles.GetCount() == 1)
        //{
        //    //destroy the brick
        //    Destroy(this.gameObject);

        //    Vector3 tilePosition = m_brick.m_coveredTiles.GetFirstTile().GetXZWorldPosition();

        //    //make the two cubes explode
        //    SplitCubeAtPosition(tilePosition + new Vector3(0, 0.5f, 0));
        //    SplitCubeAtPosition(tilePosition + new Vector3(0, 1.5f, 0));
        //}
        //else
        //{
        //    if (m_brick.m_coveredTiles.GetFirstTile().CurrentState == Tile.State.TRAP && m_brick.m_coveredTiles.GetSecondTile().CurrentState == Tile.State.TRAP)
        //    {
        //        Vector3 tile0Position = m_brick.m_coveredTiles.GetFirstTile().GetXZWorldPosition();
        //        SplitCubeAtPosition(tile0Position + new Vector3(0, 0.5f, 0));
        //        Vector3 tile1Position = m_brick.m_coveredTiles.GetSecondTile().GetXZWorldPosition();
        //        SplitCubeAtPosition(tile1Position + new Vector3(0, 0.5f, 0));
        //    }
        //    else
        //    {
        //        //destroy the brick
        //        Destroy(this.gameObject);

        //        Tile trapTile = m_brick.m_coveredTiles[m_brick.m_coveredTiles.GetFirstTile().CurrentState == Tile.State.TRAP ? 0 : 1];
        //        Tile normalTile = m_brick.CoveredTiles[m_brick.CoveredTiles[0].CurrentState == Tile.State.TRAP ? 1 : 0];
        //        Vector3 trappedTilePosition = trapTile.GetXZWorldPosition();
        //        Vector3 normalTilePosition = normalTile.GetXZWorldPosition();

        //        Debug.Log(trappedTilePosition);
        //        SplitCubeAtPosition(trappedTilePosition + new Vector3(0, 0.5f, 0));
        //        CreateCubeAtPosition(normalTilePosition + new Vector3(0, 0.5f, 0));
        //    }
        //}
    }

    private void CreateCubeAtPosition(Vector3 position)
    {
        GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeObject.transform.position = position;
    }

    /**
    * Split a cube into numPartsAlongDimension^3 smaller ones with
    **/
    private List<GameObject> SplitCubeAtPosition(Vector3 cubeWorldPosition, int numPartsAlongDimension = 8)
    {
        GameObject cubeExploded = new GameObject("CubeExploded");
        cubeExploded.transform.position = cubeWorldPosition;

        int cubePartsCount = numPartsAlongDimension * numPartsAlongDimension * numPartsAlongDimension;
        List<GameObject> cubeParts = new List<GameObject>(cubePartsCount);
        float cubePartSize = 1 / (float) numPartsAlongDimension;

        for (int i = 0; i != numPartsAlongDimension; i++)
        {
            for (int j = 0; j != numPartsAlongDimension; j++)
            {
                for (int k = 0; k != numPartsAlongDimension; k++)
                {
                    Vector3 cubePartPosition;
                    if (numPartsAlongDimension % 2 == 0)
                        cubePartPosition = new Vector3((i - (numPartsAlongDimension / 2) + 0.5f) * cubePartSize, 
                                                       (j - (numPartsAlongDimension / 2) + 0.5f) * cubePartSize, 
                                                       (k - (numPartsAlongDimension / 2) + 0.5f) * cubePartSize);
                    else
                        cubePartPosition = new Vector3((i - (numPartsAlongDimension / 2)) * cubePartSize,
                                                       (j - (numPartsAlongDimension / 2)) * cubePartSize,
                                                       (k - (numPartsAlongDimension / 2)) * cubePartSize);

                    GameObject cubePart = Instantiate(m_cubePartPfb);
                    cubePart.transform.parent = cubeExploded.transform;
                    cubePart.transform.localPosition = cubePartPosition;
                    cubePart.transform.localScale = 1.0f * new Vector3(cubePartSize, cubePartSize, cubePartSize);

                    cubeParts.Add(cubePart);
                }
            }
        }

        for (int i = 0; i != cubeParts.Count; i++)
        {
            Rigidbody rb = cubeParts[i].GetComponent<Rigidbody>();
            //rb.useGravity = true;
            rb.AddExplosionForce(300.0f, cubeWorldPosition, Mathf.Sqrt(3) / 2);
        }

        return cubeParts;
    }

    /**
    * Get the object normalized coordinated for our pivot point given its local coordinates
    **/
    public Vector3 GetPivotPointFromLocalCoordinates(Vector3 coords)
    {
        return new Vector3(coords.x, 0.5f * coords.y, coords.z);
    }

    /**
    * Called when the brick has finished its rolling animation
    **/
    public void OnFinishRolling()
    {
        m_brick.OnFinishRolling();

        if (GameController.GetInstance().m_gameStatus == GameController.GameStatus.DEFEAT || GameController.GetInstance().m_gameStatus == GameController.GameStatus.IDLE)
            return;

        //Capture bonuses
        if (GameController.GetInstance().m_gameMode == GameController.GameMode.GAME)
        {
            if (m_brick.m_coveredTiles.GetTileAtIndex(0).AttachedBonus != null)
            {
                TileRenderer tileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(m_brick.m_coveredTiles.GetTileAtIndex(0));
                tileRenderer.OnCaptureBonus();
            }

            if (m_brick.m_coveredTiles.GetTileAtIndex(1) != null && m_brick.m_coveredTiles.GetTileAtIndex(1).AttachedBonus != null)
            {
                TileRenderer tileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(m_brick.m_coveredTiles.GetTileAtIndex(1));
                tileRenderer.OnCaptureBonus();
            }
        }

        //if (m_brick.m_coveredTiles.GetCount() == 1)
        //{
        //    if (m_brick.m_coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.TRAP)
        //        Explode();
        //}
        //else
        //{
        //    if (m_brick.m_coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.TRAP || m_brick.m_coveredTiles.GetTileAtIndex(1).CurrentState == Tile.State.TRAP)
        //        Explode();
        //}
    }

    /**
    * Tells if the brick is upon the finish tile and only it
    **/
    public bool IsOnFinishTile()
    {
        return (m_brick.m_coveredTiles.GetCount() == 1) && (m_brick.m_coveredTiles.GetTileAtIndex(0).CurrentState == Tile.State.FINISH);
    }

    /**
    * Tells if the brick is currently falling
    **/ 
    public bool IsFalling()
    {
        return m_brick.m_state == Brick.BrickState.FALLING;
    }








    /************ BRICK TELEPORTATION **************/
    //private class Cube

    public void OnStartTeleportation()
    {
        m_brickTeleporting = true;

        //float dropHeight = 4.0f * Brick.BRICK_BASIS_DIMENSION;
        //Vector3 brickFinalPosition = transform.localPosition;
        //transform.localPosition += new Vector3(0, dropHeight, 0);

        //float dropDuration = 0.5f;

        //BrickAnimator brickAnimator = GetComponent<BrickAnimator>();
        //brickAnimator.UpdatePivotPoint(Vector3.zero);
        //brickAnimator.TranslateTo(brickFinalPosition, dropDuration, 0, ValueAnimator.InterpolationType.HERMITE1, false);

        //Assemble the brick with small glow cubes
        AssembleBrick2();

        //FX_TeleportIN
        //ParticleSystem teleportInFX = Instantiate(m_teleportInFX);
        //teleportInFX.transform.position = transform.localPosition + new Vector3(0.5f * Brick.BRICK_BASIS_DIMENSION, 0, 0.5f * Brick.BRICK_BASIS_DIMENSION);
        //teleportInFX.Play();
    }

    public void OnFinishTeleportation()
    {
        Debug.Log("OnFinishTeleportation");
        m_brickTeleporting = false;

        Tile landedTile = m_brick.m_coveredTiles.GetTileAtIndex(0);
        TileRenderer tileRenderer = GameController.GetInstance().m_floorRenderer.GetRendererForTile(landedTile);
        tileRenderer.DisplayGlowSquareOnBrickLanding();

        //start the actual game
        GameController.GetInstance().m_gameStatus = GameController.GameStatus.RUNNING;
    }

    private void AssembleBrick2()
    {
        m_brickFragmenter.FragmentBrick();
        m_brickFragmenter.transform.localPosition = new Vector3(0.5f * Brick.BRICK_BASIS_DIMENSION, Brick.BRICK_BASIS_DIMENSION, 0.5f * Brick.BRICK_BASIS_DIMENSION);
    }

    private void AssembleBrick()
    {
        int numCubesPerDimension = 4;
        float glowCubeSize = Brick.BRICK_BASIS_DIMENSION / (float) numCubesPerDimension;

        GameObject cubes = new GameObject("GlowCubes");
        cubes.transform.position = this.transform.position + new Vector3(0.5f * Brick.BRICK_BASIS_DIMENSION, Brick.BRICK_BASIS_DIMENSION, 0.5f * Brick.BRICK_BASIS_DIMENSION);

        Vector3 cubeSpawnPosition = cubes.transform.position + new Vector3(0, 2 * Brick.BRICK_BASIS_DIMENSION, 0);

        for (int i = 0; i != numCubesPerDimension; i++)
        {
            for (int j = 0; j != 2 * numCubesPerDimension; j++)
            {
                for (int k = 0; k != numCubesPerDimension; k++)
                {
                    Vector3 cubePosition;
                    if (numCubesPerDimension % 2 == 0)
                    {
                        cubePosition = new Vector3((i - numCubesPerDimension / 2 + 0.5f) * glowCubeSize,
                                                   (j - 2 * numCubesPerDimension / 2 + 0.5f) * glowCubeSize,
                                                   (k - numCubesPerDimension / 2 + 0.5f) * glowCubeSize);
                    }
                    else
                    {
                        cubePosition = new Vector3((i - numCubesPerDimension / 2) * glowCubeSize,
                                                   (j - 2 * numCubesPerDimension / 2) * glowCubeSize,
                                                   (k - numCubesPerDimension / 2) * glowCubeSize);
                    }

                    GlowCube glowCube = Instantiate(m_glowCubePfb);
                    glowCube.transform.parent = cubes.transform;
                    glowCube.transform.localPosition = Vector3.zero;
                    glowCube.transform.localPosition = cubePosition;
                    glowCube.transform.localScale = glowCubeSize * Vector3.one;

                    float delay = j * 0.1f;

                    GameObjectAnimator cubeAnimator = glowCube.GetComponent<GameObjectAnimator>();
                    //cubeAnimator.SetScale(Vector3.zero);
                    //cubeAnimator.ScaleTo(glowCubeSize * Vector3.one, 0.1f, delay);
                    cubeAnimator.SetPosition(cubeSpawnPosition);
                    //cubeAnimator.TranslateTo(cubePosition, 0.1f, delay);

                }
            }
            //StartCoroutine();
        }
    }
}