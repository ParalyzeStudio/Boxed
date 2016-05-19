﻿using UnityEngine;

public class Brick : MonoBehaviour
{
    private Vector3[] m_vertices; //brick's 8 vertices
    public BrickFace[] m_faces; //brick's 6 faces

    public struct BrickEdge
    {
        public Vector3 m_pointA;
        public Vector3 m_pointB;

        public BrickEdge(Vector3 pointA, Vector3 pointB)
        {
            m_pointA = pointA;
            m_pointB = pointB;
        }

        public Vector3 GetMiddle()
        {
            return 0.5f * (m_pointA + m_pointB);
        }
    }

    /**
    * A face of the brick d-c
    *                     | |
    *                     a-b
    **/
    public struct BrickFace
    {
        public Brick m_parentBrick;

        public int m_index; //index of the face inside the whole brick
        public int[] m_indices; //array of 4 indices
        public Vector3 m_normal;

        //indices of the 4 faces that are adjacent to this one. Faces are sorted accordint to the vertices order.
        //That means that the face sharing edge ab is numbered 1, face sharing bc is numbered 2 and so on
        public int[] m_adjacentsFaces;

        public BrickFace(Brick parentBrick, int index, int a, int b, int c, int d)
        {
            m_parentBrick = parentBrick;
            m_index = index;
            m_indices = new int[4];
            m_indices[0] = a;
            m_indices[1] = b;
            m_indices[2] = c;
            m_indices[3] = d;

            Vector3 ab = m_parentBrick.m_vertices[b] - m_parentBrick.m_vertices[a];
            Vector3 bc = m_parentBrick.m_vertices[c] - m_parentBrick.m_vertices[b];
            m_normal = -Vector3.Cross(ab, bc);
            m_normal.Normalize();

            //set adjacent faces
            if (index == 0)
                m_adjacentsFaces = new int[4] { 4, 3, 2, 5 };
            else if (index == 1)
                m_adjacentsFaces = new int[4] { 2, 3, 4, 5 };
            else if (index == 2)
                m_adjacentsFaces = new int[4] { 0, 3, 1, 5 };
            else if (index == 3)
                m_adjacentsFaces = new int[4] { 0, 4, 1, 2 };
            else if (index == 4)
                m_adjacentsFaces = new int[4] { 0, 5, 1, 3 };
            else
                m_adjacentsFaces = new int[4] { 0, 2, 1, 4 };
        }

        /**
        * Return the edge shared by both this brick face and the adjacent face defined by its index
        **/
        public BrickEdge GetAdjacentFaceSharedEdge(int adjacentFaceIdx)
        {            
            return new BrickEdge(m_parentBrick.m_vertices[m_indices[adjacentFaceIdx]],
                                 m_parentBrick.m_vertices[m_indices[(adjacentFaceIdx == 3) ? 0 : adjacentFaceIdx + 1]]);
        }
    }

    public int m_downFaceIndex; //the index of the face currently touching down the floor

    //current animation state of the brick
    private enum BrickState
    {
        IDLE = 1,
        ROLLING
    }

    private BrickState m_state;

    /**
    * Build a rectangular cuboid that will serve as our main object in the scene.
    * To handle lighting correctly, our cuboid will need 24 vertices (instead of 8) so light is interpolated correctly so one face has one single color.
    **/
    public void Build()
    {
        m_vertices = new Vector3[8];
        m_vertices[0] = new Vector3(0, 0, 0);
        m_vertices[1] = new Vector3(1, 0, 0);
        m_vertices[2] = new Vector3(1, 0, 1);
        m_vertices[3] = new Vector3(0, 0, 1);
        m_vertices[4] = new Vector3(0, 2, 0);
        m_vertices[5] = new Vector3(1, 2, 0);
        m_vertices[6] = new Vector3(1, 2, 1);
        m_vertices[7] = new Vector3(0, 2, 1);

        m_faces = new BrickFace[6];
        //build square faces of the brick
        m_faces[0] = new BrickFace(this, 0, 3, 2, 1, 0);
        m_faces[1] = new BrickFace(this, 1, 4, 5, 6, 7);

        //build rectangle faces of the brick
        for (int i = 2; i != 6; i++)
        {
            int startIdx = i - 2;
            m_faces[i] = new BrickFace(this, i , startIdx, (i == 5) ? 0 : startIdx + 1, (i == 5) ? 4 : startIdx + 5, startIdx + 4);
        }
        
        //build mesh vertices
        Vector3[] vertices = new Vector3[24];
        
        for (int i = 0; i != m_faces.Length; i++)
        {
            BrickFace face = m_faces[i];
            for (int j = 0; j != face.m_indices.Length; j++)
            {
                vertices[i * 4 + j] = m_vertices[face.m_indices[j]];
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
                normals[i * 4 + p] = m_faces[i].m_normal;
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

        BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();        
        //brickAnimator.UpdatePivotPoint(new Vector3(0.5f, 0.5f, 0.5f));

        //at start the first face is touching the ground and idle
        m_downFaceIndex = 0;
        m_state = BrickState.IDLE;
    }

    /***DEBUG FUNCTIONS***/
    //private void TranslateBy(Vector3 translation)
    //{
    //    BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
    //    brickAnimator.TranslateTo(brickAnimator.GetPosition() + translation, 0.5f);
    //}

    //private void ScaleBy(Vector3 scale)
    //{
    //    BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
    //    brickAnimator.ScaleTo(this.transform.localScale + scale, 0.5f);
    //}

    public enum RollDirection
    {
        LEFT = 1,
        TOP,
        RIGHT,
        BOTTOM
    }

    /**
    * Make the brick roll on one of the 4 available directions (left, top, right, bottom)
    **/
    public void Roll(RollDirection rollDirection)
    {
        if (m_state == BrickState.ROLLING) //brick is already rolling do nothing
            return;        

        m_state = BrickState.ROLLING;
        //Associate one vector to every direction
        Vector3 direction;
        if (rollDirection == RollDirection.LEFT)
            direction = new Vector3(-1, 0, 0);
        else if (rollDirection == RollDirection.TOP)
            direction = new Vector3(0, 0, 1);
        else if (rollDirection == RollDirection.RIGHT)
            direction = new Vector3(1, 0, 0);
        else
            direction = new Vector3(0, 0, -1);

        //Find the face that has the same normal vector as the roll direction
        float maxDotProduct = float.MinValue;
        int rollToFaceIdx = -1;
        for (int i = 0; i != 6; i++)
        {
            Vector3 faceNormal = this.transform.rotation * m_faces[i].m_normal;
            float dotProduct = MathUtils.DotProduct(direction, faceNormal);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                rollToFaceIdx = i;
            }
        }
        
        BrickFace rollToFace = m_faces[rollToFaceIdx];

        //Determine which of the 4 adjacent faces the rollToFace is equal to
        BrickFace currentFace = m_faces[m_downFaceIndex];
        int adjacentFaceIdx = -1;
        for (int i = 0; i != currentFace.m_adjacentsFaces.Length; i++)
        {
            BrickFace adjacentFace = m_faces[currentFace.m_adjacentsFaces[i]];
            if (adjacentFace.Equals(rollToFace))
            {
                adjacentFaceIdx = i;
                continue;
            }
        }

        //Get the rotation axis (i.e the edge joining the two adjacent faces)
        BrickEdge rotationEdge = currentFace.GetAdjacentFaceSharedEdge(adjacentFaceIdx);
        Vector3 rotationAxis = rotationEdge.m_pointB - rotationEdge.m_pointA;

        //update the pivot point position to the middle of the edge serving as rotation axis
        BrickAnimator brickAnimator = this.GetComponent<BrickAnimator>();
        brickAnimator.UpdatePivotPoint(GetPivotPointFromLocalCoordinates(rotationEdge.GetMiddle()));
        brickAnimator.SetRotationAxis(rotationAxis);
        brickAnimator.RotateBy(90, 0.3f);
        
        //set the new index for the face touching the floor
        m_downFaceIndex = rollToFace.m_index;
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
        m_state = BrickState.IDLE;
    }
}
