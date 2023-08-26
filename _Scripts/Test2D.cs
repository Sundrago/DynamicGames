using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2D : MonoBehaviour
{
    public GameObject square, p;

    public float player_height;

    void Update()
    {
        SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();

        Vector2 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector2 topLeft = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y, 0));
        Vector2 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
        Vector2 botRight = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y, 0));
    
        Vector2 A = topRight;
        Vector2 B = topLeft;
        Vector2 P = p.transform.position;

        //Get DeltaHeight
        Vector2 midOfLine = Vector2.Lerp(A, B, 0.5f);
        Vector2 midOfSquare = square.transform.position;

        float x = midOfSquare.x - midOfLine.x;
        float y = midOfSquare.y - midOfLine.y;
        float d = Vector2.Distance(midOfLine, midOfSquare);
        Vector2 delta_height = new Vector2(x*player_height/d, y*player_height/d);

        //Get closest point
        Vector2 closetPoint = GetClosestPointOnLineSegment(A, B, P);
        Debug.DrawLine(P, closetPoint, Color.red);

        float normalPoint = Vector2.Distance(A, closetPoint) / Vector2.Distance(A,B);
        float noramlHeight = player_height / Vector2.Distance(A, B) * 1.5f;
        
        Vector2 checkpointA = Vector2.Lerp(A,B,normalPoint-noramlHeight) - delta_height;
        Vector2 checkpointB = Vector2.Lerp(A,B,normalPoint+noramlHeight) - delta_height;
        Debug.DrawLine(checkpointA, checkpointB, Color.yellow);


    }

    public static Vector2 GetClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;       //Vector from A to P   
        Vector2 AB = B - A;       //Vector from A to B  

        float magnitudeAB = AB.sqrMagnitude;    //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        if (distance < 0) return A;
        else if (distance > 1) return B;
        else return A + AB * distance;
    }
}
