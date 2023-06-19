using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField]
    List<Neighbour> _neighbours;

    private void Awake()
    {
        for(int i = 0; i < _neighbours.Count; i++)
        {
            if (_neighbours[i].isTwoWay && _neighbours[i].point != null)
            {
                _neighbours[i].point.CreateConnection(this, -_neighbours[i].Direction, _neighbours[i].ConnectionType, _neighbours[i].isTwoWay);
            }
        }
    }

    public void CreateConnection(ClimbPoint point, Vector2 direction, ConnectionType connectionType, bool isTwoWay)
    {
        var neighbour = new Neighbour()
        {
            point = point,
            Direction = direction,
            ConnectionType = connectionType,
            isTwoWay = isTwoWay
        };
        _neighbours.Add(neighbour);
    }

    public Neighbour GetNeighbour(Vector2 direction)
    {
        Neighbour neighbour = null;

        if (direction.y != 0)
            neighbour = _neighbours.FirstOrDefault(n => n.Direction.y == direction.y);

        if (neighbour == null && direction.x != 0)
            neighbour = _neighbours.FirstOrDefault(n => n.Direction.x == direction.x);

        return neighbour;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        for (int i = 0; i < _neighbours.Count; i++)
        {
            if (_neighbours[i].point != null)
            {
                Debug.DrawLine(transform.position, _neighbours[i].point.transform.position, (_neighbours[i].isTwoWay) ? Color.green : Color.red);
            }
        }
    }

}

[System.Serializable]
public class Neighbour
{
    public ClimbPoint point;
    public Vector2 Direction;
    public ConnectionType ConnectionType;
    public bool isTwoWay = true;
}

public enum ConnectionType
{
    Jump,
    Move
}
