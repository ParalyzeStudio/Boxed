using UnityEngine;
using System.Collections.Generic;

public class GameObjectAnimator : ValueAnimator
{
    protected Vector3 m_pivotPoint; //the pivot point of this game object, if the pivot point position (which is m_poisiotn) 
                                    //is at the transform.localposition of the object then the pivot point is Vector3.zero
    private Vector3 m_objectSize; //the size of the object that will serve to position correctly our object in conjunction with the pivot point

    public override void Awake()
    {
        m_pivotPoint = Vector3.zero;
        m_objectSize = Vector3.zero;

        ////set the correct value for pivot point
        ////First find the bounding box of the object and its center
        //Bounds bbox = GetGameObjectBoundingBox();
        //Vector3 objectCenter = bbox.center;
        //Vector3 objectCenterToObjectPosition = this.transform.position - objectCenter;

        ////scale this value by the game object size
        //m_pivotPoint = new Vector3(objectCenterToObjectPosition.x / bbox.size.x,
        //                           objectCenterToObjectPosition.y / bbox.size.y,
        //                           objectCenterToObjectPosition.z / bbox.size.z);        
    }

    public virtual Vector3 GetGameObjectSize()
    {
        if (m_objectSize != Vector3.zero) //we already calculate the object size
            return m_objectSize;

        m_objectSize = GetGameObjectBoundingBox().size;
        return m_objectSize;
    }

    public virtual Bounds GetGameObjectBoundingBox()
    {
        MeshRenderer[] childRenderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();

        if (childRenderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds bounds = childRenderers[0].bounds;

        //encapsulate further bounds
        if (childRenderers.Length > 1)
        {
            for (int i = 1; i != childRenderers.Length; i++)
            {
                bounds.Encapsulate(childRenderers[i].bounds);
            }
        }

        return bounds;
    }

    /**
     * Update the pivot point without changing the object position
     * With this operation only the pivot point and its position are updated
     * **/
    public void UpdatePivotPoint(Vector3 pivotPoint)
    {
        m_pivotPoint = pivotPoint;

        //the local vector joining the object local position and the pivot point local position
        Vector3 objectLocalPositionToPivotPointPosition = pivotPoint;
        objectLocalPositionToPivotPointPosition.Scale(GetGameObjectSize());
        //we need to apply object transformation to this vector
        objectLocalPositionToPivotPointPosition = this.transform.rotation * objectLocalPositionToPivotPointPosition;
        //we finally obtain the pivot point local position
        m_position = this.transform.localPosition + objectLocalPositionToPivotPointPosition;
    }

    /**
     * Update the pivot point position without changing the object position
     * With this operation only the pivot point and its position are updated
     * **/
    public void UpdatePivotPointPosition(Vector3 pivotPointPosition)
    {
        m_position = pivotPointPosition;

        Vector3 objectLocalPositionToPivotPointPosition = m_position - this.transform.localPosition;
        Vector3 objectSize = GetGameObjectSize();
        Vector3 invObjectSize = new Vector3(1 / objectSize.x, 1 / objectSize.y, 1 / objectSize.z);
        objectLocalPositionToPivotPointPosition.Scale(invObjectSize);
        //apply game object inverse transformation
        m_pivotPoint = Quaternion.Inverse(this.transform.rotation) * objectLocalPositionToPivotPointPosition;
        Debug.Log("m_pivotPoint:" + m_pivotPoint);
    }

    public override void SetPosition(Vector3 position)
    {
        Vector3 pivotPointPositionToObjectPosition = this.transform.localPosition - m_position;
        this.transform.localPosition = position + pivotPointPositionToObjectPosition;
        base.SetPosition(position);
    }

    public override void SetScale(Vector3 scale)
    {
        //update the size of the game object
        Vector3 oldScale = this.transform.localScale;
        this.transform.localScale = scale;

        //scale the vector going from pivot point position to object local position by the ratio newScale/oldScale
        Vector3 pivotPointPositionToObjectPosition = m_position - transform.localPosition;
        Vector3 invScale = new Vector3(1 / oldScale.x, 1 / oldScale.y, 1 / oldScale.z);
        Vector3 deltaScale = scale; 
        deltaScale.Scale(invScale); //newScale / oldScale        
        pivotPointPositionToObjectPosition.Scale(deltaScale);

        //recalculate the new position of the object
        this.transform.localPosition = m_position - pivotPointPositionToObjectPosition;

        //recalculate the object size
        m_objectSize.Scale(deltaScale);

        //replace old scale value by new one
        base.SetScale(scale);
    }

    public override void ApplyRotationAngle(float angle)
    {
        //rotate the object itself
        Quaternion rotation = Quaternion.AngleAxis(angle, m_rotationAxis);
        this.gameObject.transform.rotation *= rotation;

        Vector3 objectPositionToPivotPointPosition = m_pivotPoint;
        objectPositionToPivotPointPosition.Scale(m_objectSize);
        objectPositionToPivotPointPosition = this.gameObject.transform.rotation * objectPositionToPivotPointPosition; //rotate this vector

        this.gameObject.transform.localPosition = m_position - objectPositionToPivotPointPosition;

        base.ApplyRotationAngle(angle);
    }
}