using UnityEngine;

public class BrickRenderer : MonoBehaviour
{
    private Brick m_brick;

    /**
    * Build a rectangular cuboid that will serve as our main object in the scene.
    * To handle lighting correctly, our cuboid will need 24 vertices (instead of 8) so light is interpolated correctly so one face has one single color.
    * Pass the tile or tiles (max 2) the brick should be upon
    **/
    public void BuildOnTile(Tile tile)
    {
        m_brick = new Brick(tile);

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

        Mesh brickMesh = new Mesh();
        brickMesh.name = "BrickMesh";
        brickMesh.vertices = vertices;
        brickMesh.triangles = triangles;
        brickMesh.normals = normals;

        brickMesh.RecalculateBounds();

        MeshFilter brickMeshFilter = this.GetComponent<MeshFilter>();
        brickMeshFilter.sharedMesh = brickMesh;

        //place the brick upon the tile parameter
        PlaceOnTiles(tile, null);
    }

    /**
    * Adjust the position and the rotation of the brick so it covers the tiles passed as parameters
    * tile2 can be null if there is only one covered tile
    **/
    public void PlaceOnTiles(Tile tile1, Tile tile2)
    {
        GameObjectAnimator brickAnimator = this.GetComponent<GameObjectAnimator>();
        brickAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f)); //place the pivot point at the center of the brick

        Vector3 tile1WorldPosition = tile1.GetWorldPosition();        

        if (tile2 == null)
        {
            brickAnimator.transform.rotation = Quaternion.Euler(0, 0, 0); //null rotation
            brickAnimator.SetPosition(tile1WorldPosition + new Vector3(0, 1 + 0.5f * TileRenderer.TILE_HEIGHT, 0));
        }
        else
        {
            Vector3 tile2WorldPosition = tile2.GetWorldPosition();
            Vector3 diff = tile2WorldPosition - tile1WorldPosition;
            Vector3 rotationAxis = Vector3.Cross(diff, Vector3.up);

            brickAnimator.SetRotationAxis(rotationAxis);
            brickAnimator.ApplyRotationAngle(90);

            brickAnimator.SetPosition(0.5f * (tile1WorldPosition + tile2WorldPosition) + new Vector3(0, 0.5f + 0.5f * TileRenderer.TILE_HEIGHT, 0));
        }
    }

    /**
    * Make the brick roll on one of the 4 available directions (left, top, right, bottom)
    **/
    public void Roll(Brick.RollDirection rollDirection)
    {
        Brick.RollResult rollResult;
        Brick.BrickEdge rotationEdge;
        m_brick.Roll(rollDirection, out rollResult, out rotationEdge);

        if (rollResult == Brick.RollResult.VALID || rollResult == Brick.RollResult.FALL)
        {
            //Get the rotation axis from the rotation edge
            Vector3 rotationAxis = rotationEdge.m_pointB - rotationEdge.m_pointA;

            //update the pivot point position to the middle of the edge serving as rotation axis
            BrickAnimator brickAnimator = GetComponent<BrickAnimator>();
            brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(rotationEdge.GetMiddle()));
            brickAnimator.SetRotationAxis(rotationAxis);
            brickAnimator.RotateBy(90, 0.3f);
        }                  
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
    }
}