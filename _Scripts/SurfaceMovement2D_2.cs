using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SquareElement
{
    public GameObject obj;
    public float dist;
    public Vector2[] cornerPoints = null;
    public Vector2[] closestPointOnLine;
    public float[] dists = null;
    public int[] shortCornerIdx = null;
    public bool[] constraints;
}

public struct AvailableCorner
{
    public SquareElement sqrElm;
    public int cornerIdx;
    public float dist;
    public bool leftAvailalble, rightAvailable;
    public float normal;
}

public class CurrentCorner
{
    public GameObject obj = null;
    public Vector2[] cornerPoints = null;
    public int cornerIdx;
    public float normal;
    public bool direction;
    public Vector2 pointA, pointB;
}

public struct Transition
{
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;

    public Quaternion r1;
    public Quaternion r2;

    public float normal;
}

public class SurfaceMovement2D_2 : MonoBehaviour
{
    [SerializeField] GameObject headObj, island, cat;
    [SerializeField] float moveVelocity = 0.3f;
    [SerializeField] float shortTransition = 0.3f;
    [SerializeField] float longTransition = 0.3f;
    [SerializeField] float hitCheckInterval = 0.5f;
    [SerializeField] int searchComplexity = 5;

    private List<AvailableCorner> availables;
    private List<SquareElement> SquareElements;

    private GameObject[] squares;
    private CurrentCorner currentCorner = new CurrentCorner();
    private CurrentCorner oldCorner = new CurrentCorner();
    private Transition transition = new Transition();

    private const float height_min = 0.15f;
    private const float tolerance = 0.05f;
    private const float height = 0.1f;

    private float transitionSpeed = 1f;
    private float headHitCheckTime;
    private float moveSpeed = 0.3f;

    private bool onTransition = false;
    private bool onIsland = false;



    // Start is called before the first frame update
    void Start()
    {
        cat.GetComponent<Player_anim>().Walk();
        LoadSquare();
        GetAvailable(searchComplexity);

        if (onIsland) return;
        
        currentCorner.obj = availables[0].sqrElm.obj;
        currentCorner.cornerIdx = availables[0].cornerIdx;
        currentCorner.normal = 0;

        oldCorner.obj = currentCorner.obj;
        oldCorner.cornerIdx = currentCorner.cornerIdx;
        InitializeCurrentCorner(currentCorner);
    }

    //Update is called once per frame
    void Update()
    {
        if (onTransition) Transition();
        else Move();
    }

    void Transition()
    {
        if (currentCorner.obj == null)
        {
            //print("obj destroyed");
            LoadSquare();
            FindNearCorner();
            return;
        }

        if (transition.normal > 0.95f)
        {
            //print("Transition Finsihed");
            cat.GetComponent<Player_anim>().Walk();
            onTransition = false;
            headHitCheckTime = Time.time + hitCheckInterval;
            return;
        }

        //Update P3
        currentCorner.cornerPoints = GetCornerPoint(currentCorner.obj.GetComponent<SpriteRenderer>());
        currentCorner.pointA = currentCorner.cornerPoints[currentCorner.cornerIdx];
        currentCorner.pointB = currentCorner.cornerPoints[currentCorner.cornerIdx == 3 ? 0 : currentCorner.cornerIdx + 1];
        transition.p3 = Vector2.Lerp(currentCorner.pointA, currentCorner.pointB, currentCorner.normal);
    
        //Update R2
        Vector3 targ = currentCorner.direction ? currentCorner.pointB : currentCorner.pointA;
        targ.z = 0f;

        Vector3 objectPos = transition.p3;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transition.r2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //GetNormal
        transition.normal += transitionSpeed * Time.deltaTime;

        //Update Positon & rotation
        gameObject.transform.position = Bezier(transition.normal, transition.p1, transition.p2, transition.p3);
        gameObject.transform.rotation = Quaternion.Lerp(transition.r1, transition.r2, transition.normal);

        Debug.DrawLine(transition.p1, transition.p2, Color.magenta, 0.1f);
        Debug.DrawLine(transition.p2, transition.p3, Color.magenta, 0.1f);
    }

    public Vector2 Bezier(float t, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 ab = Vector2.Lerp(a, b, t);
        Vector2 bc = Vector2.Lerp(b, c, t);
        Debug.DrawLine(ab, bc, Color.magenta, 0.1f);
        return Vector2.Lerp(ab, bc, t);
    }

    void Move()
    {
        if(currentCorner.obj == null)
        {
            //print("obj destroyed");
            LoadSquare();
            FindNearCorner();
            return;
        }
        currentCorner.cornerPoints = GetCornerPoint(currentCorner.obj.GetComponent<SpriteRenderer>());
        currentCorner.pointA = currentCorner.cornerPoints[currentCorner.cornerIdx];
        currentCorner.pointB = currentCorner.cornerPoints[currentCorner.cornerIdx == 3 ? 0 : currentCorner.cornerIdx + 1];

        //Move along position data
        if(currentCorner.direction) currentCorner.normal += moveSpeed * Time.deltaTime;
        else currentCorner.normal -= moveSpeed * Time.deltaTime;

        gameObject.transform.position = Vector2.Lerp(currentCorner.pointA, currentCorner.pointB, currentCorner.normal);

        //Rotate along degree
        Vector3 targ = currentCorner.direction ? currentCorner.pointB : currentCorner.pointA;
        targ.z = 0f;

        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        //Get Height Hit Check info
        if(Time.time > headHitCheckTime)
        {
            headHitCheckTime = Time.time + hitCheckInterval;
            if (CheckIfHeadHit())
            {
                onIsland = false;
                //print("headHit");
                FindNearCorner();
                return;
            }
        }
        if ((currentCorner.normal > 0.95f && currentCorner.direction) || (currentCorner.normal < 0.05f && !currentCorner.direction))
        {
            //print("walkCycleDone");
            FindNearCorner();
            return;
        }
    }

    bool CheckIfHeadHit()
    {
        for(int i = 0; i<SquareElements.Count; i++)
        {
            if (SquareElements[i].obj == null) continue;
            if (SquareElements[i].obj == currentCorner.obj) continue;
            if (SquareElements[i].dist > 0.5f) continue;

            Vector2 position = headObj.transform.position;
            Vector2 closest = SquareElements[i].obj.GetComponent<BoxCollider2D>().ClosestPoint(position);

            if (closest == position) {
                Debug.DrawLine(position, squares[i].transform.position, Color.red, 1f);
                return true;
            } else {
                Debug.DrawLine(position, squares[i].transform.position, Color.cyan, 0.5f);
            }
        }
        return false;
    }

    void InitializeCurrentCorner(CurrentCorner currentCorner)
    {
        //currentCorner.normal = 0;
        currentCorner.cornerPoints = GetCornerPoint(currentCorner.obj.GetComponent<SpriteRenderer>());
        currentCorner.pointA = currentCorner.cornerPoints[currentCorner.cornerIdx];
        currentCorner.pointB = currentCorner.cornerPoints[currentCorner.cornerIdx == 3 ? 0 : currentCorner.cornerIdx + 1];
    }

    void FindNearCorner()
    {
        //print("findnearCornter");
        LoadSquare();
        if (onIsland)
        {
            currentCorner.obj = island;
            currentCorner.cornerIdx = 1;
        } else { 
            availables = new List<AvailableCorner>();
            GetAvailable(searchComplexity);
            availables = availables.OrderBy(x => x.dist).ToList();
            //sort available list by

            int nextIdx = 0;
            if(availables.Count > 1) { 
                while (availables[nextIdx].sqrElm.obj == currentCorner.obj && availables[nextIdx].cornerIdx == currentCorner.cornerIdx && nextIdx - 1 < availables.Count && !onIsland)
                {
                    nextIdx += 1;
                }

                if(availables[nextIdx].sqrElm.obj == oldCorner.obj && availables[nextIdx].cornerIdx == oldCorner.cornerIdx && nextIdx - 1 < availables.Count)
                    nextIdx += 1;
            }
            
            if(Random.Range(0f, 1f) < 0.1f) nextIdx = Random.Range(0, nextIdx + 1);
            if(availables[nextIdx].dist > 0.5f) nextIdx = 0;

            oldCorner.obj = currentCorner.obj;
            oldCorner.cornerIdx = currentCorner.cornerIdx;

            if(nextIdx >= availables.Count) {
                Debug.LogWarning("there is no where : " + availables.Count);
                return; 
            } else {
                currentCorner.obj = availables[nextIdx].sqrElm.obj;
                currentCorner.cornerIdx = availables[nextIdx].cornerIdx;
                //currentCorner.normal = availables[nextIdx].normal;
            }
        }

        //Setup Next Move
        currentCorner.cornerPoints = GetCornerPoint(currentCorner.obj.GetComponent<SpriteRenderer>());
        currentCorner.pointA = currentCorner.cornerPoints[currentCorner.cornerIdx];
        currentCorner.pointB = currentCorner.cornerPoints[currentCorner.cornerIdx == 3 ? 0 : currentCorner.cornerIdx + 1];

        Vector2 closest = GetClosestPointOnLineSegment(gameObject.transform.position, currentCorner.pointA, currentCorner.pointB);
        float dist = Vector2.Distance(currentCorner.pointA, currentCorner.pointB);
        currentCorner.normal = Vector2.Distance(currentCorner.pointA, closest) / dist;
        currentCorner.normal = Mathf.Clamp(currentCorner.normal, 0.05f,0.95f);
        moveSpeed = moveVelocity / dist * Random.Range(0.8f,1.2f);           


        //Get DeltaHeight
        Vector2 midOfLine = Vector2.Lerp(currentCorner.pointA, currentCorner.pointB, 0.5f);
        Vector2 midOfSquare = currentCorner.obj.transform.position;

        float x = midOfSquare.x - midOfLine.x;
        float y = midOfSquare.y - midOfLine.y;
        float d = Vector2.Distance(midOfLine, midOfSquare);
        Vector2 delta_height = new Vector2(x*height/d, y*height/d);

        float noramlHeight = height / dist * 1.2f;
        bool leftAvailalble = CheckAvailablity(currentCorner.pointA, currentCorner.pointB, currentCorner.normal - noramlHeight, delta_height);
        bool rightAvailable = CheckAvailablity(currentCorner.pointA, currentCorner.pointB, currentCorner.normal + noramlHeight, delta_height);

        //print(leftAvailalble + ", " + rightAvailable);

        //Set Direction
        if (leftAvailalble && !rightAvailable)
            currentCorner.direction = false;
        else if(!leftAvailalble && rightAvailable) 
            currentCorner.direction = true;
        else if(leftAvailalble && rightAvailable)
            currentCorner.direction = (Random.value > 0.5f);
        else {
            currentCorner.obj = island;
            currentCorner.cornerIdx = 1;
        }

        if (!currentCorner.direction)
        {
            gameObject.transform.localScale = new Vector3(50f, -50f, 1f);
        } else gameObject.transform.localScale = new Vector3(50f, 50f, 1f);

        //Setup Transition
        float transitionDist = Mathf.Abs(Vector2.Distance(gameObject.transform.position, closest));

        transition.p1 = gameObject.transform.position;
        transition.p3 = closest;
        if (transitionDist < height) {
            transitionSpeed = shortTransition;
            transition.p2 = gameObject.transform.position + (headObj.transform.position - gameObject.transform.position) * 0.2f;
        }
        else {
            transitionSpeed = longTransition;
            transition.p2 = gameObject.transform.position + (headObj.transform.position - gameObject.transform.position) * 2;
        }
        transition.r1 = gameObject.transform.rotation;
        cat.GetComponent<Player_anim>().Jump();
        transition.normal = 0;
        onTransition = true;

        //print("Start Transition");
        return;
    }

    void GetAvailable(int target)
    {
        availables = new List<AvailableCorner>();

        if(onIsland)
        {
            currentCorner.obj = island;
            currentCorner.cornerIdx = 1;
            currentCorner.normal = 0;

            InitializeCurrentCorner(currentCorner);
            return;
        }

        int count = 0;
        while(availables.Count < target & count < squares.Length)
        {
            GetSquarePointsData(count);
            count += 1;
        }
    }

    void GetSquareDistSort()
    {
        SquareElements = new List<SquareElement>();
        Vector2 playerPos = gameObject.transform.position;

        foreach (GameObject square in squares)
        {
            SquareElement elem = new SquareElement();

            Vector2 closest = square.GetComponent<BoxCollider2D>().ClosestPoint(playerPos);
            elem.dist = Vector2.Distance(playerPos, closest);
            elem.obj = square;
            SquareElements.Add(elem);
        }

        SquareElements = SquareElements.OrderBy(x => x.dist).ToList();
    }

    public Vector2[] GetCornerPoint(SpriteRenderer renderer)
    {
        Vector2 topRight = renderer.transform.TransformPoint(renderer.sprite.bounds.max);
        Vector2 topLeft = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y, 0));
        Vector2 botLeft = renderer.transform.TransformPoint(renderer.sprite.bounds.min);
        Vector2 botRight = renderer.transform.TransformPoint(new Vector3(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y, 0));
        return new Vector2[] { topRight, topLeft, botLeft, botRight };
    }

    public float GetDistFromPointToLine(Vector2 P0, Vector2 P1, Vector2 P2)
    {
        return (Mathf.Abs(Vector2.Distance(P0, P2)) + Mathf.Abs(Vector2.Distance(P0, P1)) + Mathf.Abs(Vector2.Distance(P0, P1 + P2 / 2f))) / 3f;
    }

    void GetSquarePointsData(int idx)
    {
        Debug.DrawLine(gameObject.transform.position, SquareElements[idx].obj.transform.position, Color.blue, 1f);
        if (SquareElements[idx].obj == null) return;

        SquareElements[idx].cornerPoints = GetCornerPoint(SquareElements[idx].obj.GetComponent<SpriteRenderer>());

        SquareElements[idx].closestPointOnLine = new Vector2[4];
        SquareElements[idx].closestPointOnLine[0] = GetClosestPointOnLineSegment(gameObject.transform.position, SquareElements[idx].cornerPoints[0], SquareElements[idx].cornerPoints[1]);
        SquareElements[idx].closestPointOnLine[1] = GetClosestPointOnLineSegment(gameObject.transform.position, SquareElements[idx].cornerPoints[1], SquareElements[idx].cornerPoints[2]);
        SquareElements[idx].closestPointOnLine[2] = GetClosestPointOnLineSegment(gameObject.transform.position, SquareElements[idx].cornerPoints[2], SquareElements[idx].cornerPoints[3]);
        SquareElements[idx].closestPointOnLine[3] = GetClosestPointOnLineSegment(gameObject.transform.position, SquareElements[idx].cornerPoints[3], SquareElements[idx].cornerPoints[0]);

        bool[] constraints = {false, false, false, false};
        if(SquareElements[idx].obj.GetComponent<FootstepConstraints>() != null) constraints = SquareElements[idx].obj.GetComponent<FootstepConstraints>().constraints;
        SquareElements[idx].constraints = constraints;


        SquareElements[idx].dists = new float[4];
        for(int i = 0; i<4; i++) {
            SquareElements[idx].dists[i] = constraints[i] ? float.MaxValue : Vector2.Distance(gameObject.transform.position, SquareElements[idx].closestPointOnLine[i]);
        }
        for(int i = 0; i<4; i++) {
            if(SquareElements[idx].constraints[i]) continue;
            Vector2 A = SquareElements[idx].cornerPoints[i];
            Vector2 B = SquareElements[idx].cornerPoints[i + 1 > 3 ? 0 : i + 1];
            Vector2 P = gameObject.transform.position;

            //Get DeltaHeight
            Vector2 midOfLine = Vector2.Lerp(A, B, 0.5f);
            Vector2 midOfSquare = SquareElements[idx].obj.transform.position;

            float x = midOfSquare.x - midOfLine.x;
            float y = midOfSquare.y - midOfLine.y;
            float d = Vector2.Distance(midOfLine, midOfSquare);
            Vector2 delta_height = new Vector2(x*height/d, y*height/d);

            //Get closest point
            Vector2 closetPoint = SquareElements[idx].closestPointOnLine[i];

            //Check if target points are available
            float normalPoint = Vector2.Distance(A, closetPoint) / Vector2.Distance(A,B);
            float noramlHeight = height / Vector2.Distance(A, B) * 1.5f;
            bool leftAvailalble = CheckAvailablity(A, B, normalPoint - noramlHeight, delta_height);
            bool rightAvailable = CheckAvailablity(A, B, normalPoint + noramlHeight, delta_height);

            bool isAvailable = (leftAvailalble || rightAvailable);
            
            //DEBUG!
            Vector2 checkpointA = Vector2.Lerp(A,B,normalPoint-noramlHeight) - delta_height;
            Vector2 checkpointB = Vector2.Lerp(A,B,normalPoint+noramlHeight) - delta_height;
            
            if(isAvailable) Debug.DrawLine(checkpointA, checkpointB, Color.red, 1f);
            else Debug.DrawLine(checkpointA, checkpointB, Color.yellow, 1f);
            Debug.DrawLine(closetPoint, checkpointA, leftAvailalble ? Color.red : Color.yellow, 1f);
            Debug.DrawLine(closetPoint, checkpointB, rightAvailable ? Color.red : Color.yellow, 1f);

            if (isAvailable) {

                AvailableCorner available = new AvailableCorner();
                available.sqrElm = SquareElements[idx];
                available.cornerIdx = i;
                available.dist = SquareElements[idx].dists[i];

                available.leftAvailalble = leftAvailalble;
                available.rightAvailable = rightAvailable;
                available.normal = normalPoint;

                availables.Add(available);
            }
        }
    }

    public static Vector2 GetClosestPointOnLineSegment(Vector2 P, Vector2 A, Vector2 B)
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

    bool CheckAvailablity(Vector2 A, Vector2 B, float normal, Vector2 delta_height)
    {
        if(normal < 0f || normal > 1f) return false;

        Vector2 point = Vector2.Lerp(A,B,normal) - delta_height;

        foreach(GameObject obj in squares)
        {
            Vector2 closest = obj.GetComponent<BoxCollider2D>().ClosestPoint(point);
            if (closest == point) return false;

            closest = island.GetComponent<BoxCollider2D>().ClosestPoint(point);
            if (closest == point) return false;
        }
        return true;
    }

    public void LoadSquare()
    {
        squares = GameObject.FindGameObjectsWithTag("square");
        //print("SQRS.COUNT : " + squares.Length);
        if(squares.Length == 0)
        {
            onIsland = true;
            return;
        } else onIsland = false;
        //onIsland = false;
        GetSquareDistSort();
    }
}
