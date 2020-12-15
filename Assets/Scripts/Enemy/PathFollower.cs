using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public Transform[] nodes;

    public Vector3 GetNodePos(int id)
    {
        return nodes[id].position;
    }

    public float reachDistance = 1f;
    public bool drawGizmos = false;
    public float speed = 5f;
    public float rotSpeed = 10f;
    private int currentNodeID = 0;

    void Update()
    {
        Vector3 dest = GetNodePos(currentNodeID);
        Vector3 offset = dest - transform.position;
        if (offset.sqrMagnitude > reachDistance)
        {
            offset = offset.normalized;
            transform.Translate(offset * speed * Time.deltaTime, Space.World);

            Quaternion lookRot = Quaternion.LookRotation(offset);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotSpeed * Time.deltaTime);
        }
        else
        {
            ChangeDestNode();
        }
    }

    void ChangeDestNode()
    {
        currentNodeID++;
        if (currentNodeID >= nodes.Length)
        {
            currentNodeID = 0;
        }
    }

}
