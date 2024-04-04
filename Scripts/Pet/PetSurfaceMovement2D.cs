using System.Collections.Generic;
using System.Linq;
using DynamicGames.MainPage;
using DynamicGames.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Responsible for handling the movement of a PetObject on a surface.
    /// </summary>
    [RequireComponent(typeof(PetObject))]
    public class PetSurfaceMovement2D : MonoBehaviour
    {
        public enum CurrentLocationType
        {
            Block,
            Title,
            Island
        }

        private const float HeightMin = 0.15f;
        private const float Tolerance = 0.05f;
        private const float Height = 0.1f;

        [SerializeField] private GameObject headObject;
        [SerializeField] private GameObject island;
        [SerializeField] private float moveVelocity = 0.2f;
        [SerializeField] private float shortTransition = 2.5f;
        [SerializeField] private float longTransition = 2f;
        [SerializeField] private float hitCheckInterval = 0.25f;
        [SerializeField] private int searchComplexity = 5;

        public CurrentLocationType currentPlace = CurrentLocationType.Block;
        private readonly CurrentLocation previousLocation = new();
        private List<AvailableLocation> availableLocations;
        public CurrentLocation currentLocation = new();
        private float headHitCheckTime;

        private bool isOnIsland, isOnTransition, isPaused;
        private float moveSpeed = 0.3f;

        private PetObject petObject;
        private List<SquareElement> squareBlockElements;
        private GameObject[] squares;
        private Transition transition;

        private float transitionSpeed = 1f;

        private void Awake()
        {
            petObject = GetComponent<PetObject>();
        }

        private void Update()
        {
            if (isOnTransition) Transition();
            else Move();
        }

        private void OnEnable()
        {
            FindNearestCorner();
        }

        public void StartMovement()
        {
            LoadSquares();
            GetAvailableLocations(searchComplexity);

            if (isOnIsland) return;

            currentLocation.obj = availableLocations[0].squareElement.obj;
            currentLocation.cornerIdx = availableLocations[0].cornerIdx;
            currentLocation.normal = 0;

            previousLocation.obj = currentLocation.obj;
            previousLocation.cornerIdx = currentLocation.cornerIdx;
            InitiateCurrentCorner(currentLocation);
        }

        private void Transition()
        {
            if (currentLocation.obj == null)
            {
                FindNearestCorner();
                return;
            }

            petObject.JumpUpdate(transition.normal);
            if (transition.normal > 0.95f)
            {
                FinishTransition();
                return;
            }

            UpdateEndPoint();
            UpdateEndRotation();
            UpdateTransitionNormal();
            UpdateTransform();
        }

        private void FinishTransition()
        {
            isOnTransition = false;
            headHitCheckTime = Time.time + hitCheckInterval;
        }

        private void UpdateEndPoint()
        {
            currentLocation.cornerPoints = GetCornerPoint(currentLocation.obj);
            currentLocation.pointA = currentLocation.cornerPoints[currentLocation.cornerIdx];
            currentLocation.pointB =
                currentLocation.cornerPoints[currentLocation.cornerIdx == 3 ? 0 : currentLocation.cornerIdx + 1];
            transition.endPoint = Vector2.Lerp(currentLocation.pointA, currentLocation.pointB, currentLocation.normal);
        }

        private void UpdateEndRotation()
        {
            Vector3 targ = currentLocation.direction ? currentLocation.pointB : currentLocation.pointA;
            targ.z = 0f;
            Vector3 objectPos = transition.endPoint;
            targ.x = targ.x - objectPos.x;
            targ.y = targ.y - objectPos.y;
            var angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
            transition.endRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void UpdateTransitionNormal()
        {
            transition.normal += transitionSpeed * Time.deltaTime;
        }

        private void UpdateTransform()
        {
            gameObject.transform.position = Bezier(transition.normal, transition.startPoint, transition.controlPoint,
                transition.endPoint);
            gameObject.transform.rotation =
                Quaternion.Lerp(transition.startRotation, transition.endRotation, transition.normal);
        }

        private Vector2 Bezier(float t, Vector2 a, Vector2 b, Vector2 c)
        {
            var ab = Vector2.Lerp(a, b, t);
            var bc = Vector2.Lerp(b, c, t);
            return Vector2.Lerp(ab, bc, t);
        }

        private void Move()
        {
            if (currentLocation.obj == null)
            {
                FindNearestCorner();
                return;
            }

            if (Time.frameCount % 10 == 0) UpdateCurrentLocationType();

            currentLocation.cornerPoints = GetCornerPoint(currentLocation.obj);
            currentLocation.pointA = currentLocation.cornerPoints[currentLocation.cornerIdx];
            currentLocation.pointB =
                currentLocation.cornerPoints[currentLocation.cornerIdx == 3 ? 0 : currentLocation.cornerIdx + 1];
            MoveAndRotate();
            PerformHitCheck();
            if (ShouldSearchForNewCorner())
                FindNearestCorner();
        }

        private void UpdateCurrentLocationType()
        {
            if (currentLocation.obj.name == "TITLE") SetCurrentLocationType(CurrentLocationType.Title);
            else if (currentLocation.obj.name == "ISLAND") SetCurrentLocationType(CurrentLocationType.Island);
        }

        private void MoveAndRotate()
        {
            // Move along position data
            if (!isPaused)
            {
                var displacement = moveSpeed * Time.deltaTime;
                currentLocation.normal += currentLocation.direction ? displacement : -displacement;
            }

            gameObject.transform.position =
                Vector2.Lerp(currentLocation.pointA, currentLocation.pointB, currentLocation.normal);

            // Rotate along degree
            Vector3 target = currentLocation.direction ? currentLocation.pointB : currentLocation.pointA;
            target.z = 0f;
            var objectPos = transform.position;
            target -= objectPos;
            var angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        private void PerformHitCheck()
        {
            if (Time.time > headHitCheckTime)
            {
                headHitCheckTime = Time.time + hitCheckInterval;
                if (CheckIfHeadHit())
                {
                    isOnIsland = false;
                    FindNearestCorner();
                }
            }
        }

        private bool ShouldSearchForNewCorner()
        {
            return (currentLocation.normal > 0.95f && currentLocation.direction) ||
                   (currentLocation.normal < 0.05f && !currentLocation.direction);
        }

        private bool CheckIfHeadHit()
        {
            for (var i = 0; i < squareBlockElements.Count; i++)
            {
                if (squareBlockElements[i].obj == null || squareBlockElements[i].obj == currentLocation.obj) continue;

                Vector2 position = headObject.transform.position;
                var closest = squareBlockElements[i].obj.GetComponent<BoxCollider2D>().ClosestPoint(position);

                if (closest == position)
                {
                    var rb2D = squareBlockElements[i].obj.GetComponent<Rigidbody2D>();
                    if (rb2D != null)
                    {
                        var velocity = Mathf.Abs(rb2D.velocity.x) + Mathf.Abs(rb2D.velocity.y);
                        if (velocity >= 0.01f) petObject.OnHit();
                    }

                    return true;
                }
                // else Debug.DrawLine(position, squares[i].transform.position, new Color(1,1,1,0.2f), 0.25f);
            }

            return false;
        }

        private void InitiateCurrentCorner(CurrentLocation currentLocation)
        {
            currentLocation.cornerPoints = GetCornerPoint(currentLocation.obj);
            currentLocation.pointA = currentLocation.cornerPoints[currentLocation.cornerIdx];
            currentLocation.pointB =
                currentLocation.cornerPoints[currentLocation.cornerIdx == 3 ? 0 : currentLocation.cornerIdx + 1];
        }

        private void FindNearestCorner()
        {
            LoadSquares();
            if (isOnIsland)
            {
                currentLocation.obj = island;
                currentLocation.cornerIdx = 1;
            }
            else
            {
                availableLocations = new List<AvailableLocation>();
                GetAvailableLocations(searchComplexity);
                if (availableLocations.Count == 0)
                {
                    Debug.LogWarning("there is no where : " + availableLocations.Count);
                    return;
                }

                availableLocations = availableLocations.OrderBy(x => x.dist).ToList();

                var nextIdx = 0;
                if (availableLocations.Count > 1)
                {
                    while (availableLocations[nextIdx].squareElement.obj == currentLocation.obj &&
                           availableLocations[nextIdx].cornerIdx == currentLocation.cornerIdx &&
                           nextIdx - 1 < availableLocations.Count &&
                           !isOnIsland)
                        nextIdx += 1;

                    if (availableLocations[nextIdx].squareElement.obj == previousLocation.obj &&
                        availableLocations[nextIdx].cornerIdx == previousLocation.cornerIdx &&
                        nextIdx - 1 < availableLocations.Count)
                        nextIdx += 1;
                }


                if (nextIdx >= availableLocations.Count) nextIdx = 0;

                if (Random.Range(0f, 1f) < 0.1f) nextIdx = Random.Range(0, nextIdx + 1);
                if (availableLocations[nextIdx].dist > 0.5f) nextIdx = 0;

                previousLocation.obj = currentLocation.obj;
                previousLocation.cornerIdx = currentLocation.cornerIdx;

                currentLocation.obj = availableLocations[nextIdx].squareElement.obj;
                currentLocation.cornerIdx = availableLocations[nextIdx].cornerIdx;
            }

            //Setup Next Move
            currentLocation.cornerPoints = GetCornerPoint(currentLocation.obj);
            currentLocation.pointA = currentLocation.cornerPoints[currentLocation.cornerIdx];
            currentLocation.pointB =
                currentLocation.cornerPoints[currentLocation.cornerIdx == 3 ? 0 : currentLocation.cornerIdx + 1];

            var closest =
                FindClosestPointsOnLine(gameObject.transform.position, currentLocation.pointA, currentLocation.pointB);
            var dist = Vector2.Distance(currentLocation.pointA, currentLocation.pointB);
            currentLocation.normal = Vector2.Distance(currentLocation.pointA, closest) / dist;
            currentLocation.normal = Mathf.Clamp(currentLocation.normal, 0.05f, 0.95f);
            moveSpeed = moveVelocity / dist * Random.Range(0.8f, 1.2f);


            //Get DeltaHeight
            var midOfLine = Vector2.Lerp(currentLocation.pointA, currentLocation.pointB, 0.5f);
            Vector2 midOfSquare = currentLocation.obj.transform.position;

            var x = midOfSquare.x - midOfLine.x;
            var y = midOfSquare.y - midOfLine.y;
            var d = Vector2.Distance(midOfLine, midOfSquare);
            var delta_height = new Vector2(x * Height / d, y * Height / d);

            var noramlHeight = Height / dist * 1.2f;
            var leftAvailalble = CheckPositionAvailability(currentLocation.pointA, currentLocation.pointB,
                currentLocation.normal - noramlHeight, delta_height);
            var rightAvailable = CheckPositionAvailability(currentLocation.pointA, currentLocation.pointB,
                currentLocation.normal + noramlHeight, delta_height);

            //Set Direction
            if (leftAvailalble && !rightAvailable)
            {
                currentLocation.direction = false;
            }
            else if (!leftAvailalble && rightAvailable)
            {
                currentLocation.direction = true;
            }
            else if (leftAvailalble && rightAvailable)
            {
                currentLocation.direction = Random.value > 0.5f;
            }
            else
            {
                currentLocation.obj = island;
                currentLocation.cornerIdx = 1;
            }

            if (!currentLocation.direction)
                gameObject.transform.localScale = new Vector3(-Mathf.Abs(gameObject.transform.localScale.x),
                    -Mathf.Abs(gameObject.transform.localScale.y), Mathf.Abs(gameObject.transform.localScale.z));
            else
                gameObject.transform.localScale = new Vector3(-Mathf.Abs(gameObject.transform.localScale.x),
                    Mathf.Abs(gameObject.transform.localScale.y), Mathf.Abs(gameObject.transform.localScale.z));

            //Setup Transition
            var transitionDist = Mathf.Abs(Vector2.Distance(gameObject.transform.position, closest));

            transition.startPoint = gameObject.transform.position;
            transition.endPoint = closest;
            if (transitionDist < Height)
            {
                transitionSpeed = shortTransition;
                transition.controlPoint = gameObject.transform.position +
                                          (headObject.transform.position - gameObject.transform.position) * 0.2f;
            }
            else
            {
                transitionSpeed = longTransition;
                transition.controlPoint = gameObject.transform.position +
                                          (headObject.transform.position - gameObject.transform.position) * 2;
            }

            transition.startRotation = gameObject.transform.rotation;
            petObject.JumpStart();
            transition.normal = 0;
            isOnTransition = true;
        }

        private void GetAvailableLocations(int target)
        {
            availableLocations = new List<AvailableLocation>();

            if (isOnIsland)
            {
                currentLocation.obj = island;
                currentLocation.cornerIdx = 1;
                currentLocation.normal = 0;

                InitiateCurrentCorner(currentLocation);
                return;
            }

            var count = 0;
            while (availableLocations.Count < target && count < squares.Length)
            {
                RetrieveSquareCornerData(count);
                count += 1;
            }
        }

        private void SortSquaresByDistance()
        {
            squareBlockElements = new List<SquareElement>();
            Vector2 playerPos = gameObject.transform.position;

            foreach (var square in squares)
            {
                var elem = new SquareElement();

                var closest = square.GetComponent<BoxCollider2D>().ClosestPoint(playerPos);
                elem.dist = Vector2.Distance(playerPos, closest);
                elem.obj = square;
                squareBlockElements.Add(elem);
            }

            squareBlockElements = squareBlockElements.OrderBy(x => x.dist).ToList();
        }

        private Vector2[] GetCornerPoint(GameObject obj)
        {
            var sqr = obj.GetComponent<SquareBlockCtrl>();
            if (sqr != null)
                return new Vector2[]
                {
                    sqr.tl.transform.position,
                    sqr.tr.transform.position,
                    sqr.br.transform.position,
                    sqr.bl.transform.position
                };

            var renderer = obj.GetComponent<SpriteRenderer>();
            return new[]
            {
                TransformToVector2(renderer.transform, renderer.sprite.bounds.max),
                TransformToVector2(renderer.transform,
                    new Vector3(renderer.sprite.bounds.max.x, renderer.sprite.bounds.min.y)),
                TransformToVector2(renderer.transform, renderer.sprite.bounds.min),
                TransformToVector2(renderer.transform,
                    new Vector3(renderer.sprite.bounds.min.x, renderer.sprite.bounds.max.y))
            };
        }

        private Vector2 TransformToVector2(Transform transform, Vector3 vec)
        {
            return transform.TransformPoint(vec);
        }

        private float GetDistFromPointToLine(Vector2 P0, Vector2 P1, Vector2 P2)
        {
            return (Mathf.Abs(Vector2.Distance(P0, P2)) + Mathf.Abs(Vector2.Distance(P0, P1)) +
                    Mathf.Abs(Vector2.Distance(P0, P1 + P2 / 2f))) / 3f;
        }

        private void RetrieveSquareCornerData(int idx)
        {
            if (squareBlockElements[idx].obj == null) return;

            squareBlockElements[idx].cornerPoints = GetCornerPoint(squareBlockElements[idx].obj);

            squareBlockElements[idx].closestPointsOnLine = new Vector2[4];
            squareBlockElements[idx].closestPointsOnLine[0] = FindClosestPointsOnLine(gameObject.transform.position,
                squareBlockElements[idx].cornerPoints[0], squareBlockElements[idx].cornerPoints[1]);
            squareBlockElements[idx].closestPointsOnLine[1] = FindClosestPointsOnLine(gameObject.transform.position,
                squareBlockElements[idx].cornerPoints[1], squareBlockElements[idx].cornerPoints[2]);
            squareBlockElements[idx].closestPointsOnLine[2] = FindClosestPointsOnLine(gameObject.transform.position,
                squareBlockElements[idx].cornerPoints[2], squareBlockElements[idx].cornerPoints[3]);
            squareBlockElements[idx].closestPointsOnLine[3] = FindClosestPointsOnLine(gameObject.transform.position,
                squareBlockElements[idx].cornerPoints[3], squareBlockElements[idx].cornerPoints[0]);

            bool[] constraints = { false, false, false, false };
            if (squareBlockElements[idx].obj.GetComponent<FootstepConstraints>() != null)
                constraints = squareBlockElements[idx].obj.GetComponent<FootstepConstraints>().constraints;
            squareBlockElements[idx].constraints = constraints;


            squareBlockElements[idx].dists = new float[4];
            for (var i = 0; i < 4; i++)
                squareBlockElements[idx].dists[i] = constraints[i]
                    ? float.MaxValue
                    : Vector2.Distance(gameObject.transform.position, squareBlockElements[idx].closestPointsOnLine[i]);

            for (var i = 0; i < 4; i++)
            {
                if (constraints[i]) continue;
                var A = squareBlockElements[idx].cornerPoints[i];
                var B = squareBlockElements[idx].cornerPoints[i + 1 > 3 ? 0 : i + 1];
                Vector2 P = gameObject.transform.position;

                //Get DeltaHeight
                var midOfLine = Vector2.Lerp(A, B, 0.5f);
                Vector2 midOfSquare = squareBlockElements[idx].obj.transform.position;

                var x = midOfSquare.x - midOfLine.x;
                var y = midOfSquare.y - midOfLine.y;
                var d = Vector2.Distance(midOfLine, midOfSquare);
                var delta_height = new Vector2(x * Height / d, y * Height / d);

                //Get closest point
                var closetPoint = squareBlockElements[idx].closestPointsOnLine[i];

                //Check if target points are available
                var normalPoint = Vector2.Distance(A, closetPoint) / Vector2.Distance(A, B);
                var noramlHeight = Height / Vector2.Distance(A, B) * 1.5f;
                var leftAvailalble = CheckPositionAvailability(A, B, normalPoint - noramlHeight, delta_height);
                var rightAvailable = CheckPositionAvailability(A, B, normalPoint + noramlHeight, delta_height);

                var isAvailable = leftAvailalble || rightAvailable;

                // DEBUG!
                // var checkpointA = Vector2.Lerp(A, B, normalPoint - noramlHeight) - delta_height;
                // var checkpointB = Vector2.Lerp(A, B, normalPoint + noramlHeight) - delta_height;
                // if (isAvailable) Debug.DrawLine(checkpointA, checkpointB, Color.red, 1f);
                // else Debug.DrawLine(checkpointA, checkpointB, Color.yellow, 1f);
                // Debug.DrawLine(closetPoint, checkpointA, leftAvailalble ? Color.red : Color.yellow, 1f);
                // Debug.DrawLine(closetPoint, checkpointB, rightAvailable ? Color.red : Color.yellow, 1f);

                if (isAvailable)
                {
                    var available = new AvailableLocation();
                    available.squareElement = squareBlockElements[idx];
                    available.cornerIdx = i;
                    available.dist = squareBlockElements[idx].dists[i];

                    available.leftAvailalble = leftAvailalble;
                    available.rightAvailable = rightAvailable;
                    available.normal = normalPoint;

                    availableLocations.Add(available);
                }
            }
        }

        private static Vector2 FindClosestPointsOnLine(Vector2 P, Vector2 A, Vector2 B)
        {
            var AP = P - A; //Vector from A to P   
            var AB = B - A; //Vector from A to B  

            var magnitudeAB = AB.sqrMagnitude; //Magnitude of AB vector (it's length squared)     
            var ABAPproduct = Vector2.Dot(AP, AB); //The DOT product of a_to_p and a_to_b     
            var distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0) return A;
            if (distance > 1) return B;
            return A + AB * distance;
        }

        private bool CheckPositionAvailability(Vector2 A, Vector2 B, float normal, Vector2 delta_height)
        {
            if (normal < 0f || normal > 1f) return false;

            var point = Vector2.Lerp(A, B, normal) - delta_height;

            foreach (var obj in squares)
            {
                var closest = obj.GetComponent<BoxCollider2D>().ClosestPoint(point);
                if (closest == point)
                {
                    Debug.DrawLine(obj.transform.position, point, Color.magenta, 1f);
                    return false;
                }
            }

            return true;
        }

        private void LoadSquares()
        {
            squares = GameObject.FindGameObjectsWithTag("square");
            if (squares.Length == 0)
            {
                isOnIsland = true;
                return;
            }

            isOnIsland = false;
            SortSquaresByDistance();
        }

        public void PauseMovement()
        {
            isPaused = true;
        }

        public void ContinueMovement(float speed)
        {
            moveVelocity = speed;
            isPaused = false;
            var dist = Vector2.Distance(currentLocation.pointA, currentLocation.pointB);
            moveSpeed = moveVelocity / dist * Random.Range(0.8f, 1.2f);
        }

        private void SetCurrentLocationType(CurrentLocationType place)
        {
            if (currentPlace == place) return;
            currentPlace = place;

            switch (currentPlace)
            {
                case CurrentLocationType.Title:
                    petObject.OnTitle();
                    break;
                case CurrentLocationType.Island:
                    petObject.OnIsland();
                    break;
            }
        }

        public void ForceLandOnSquare(GameObject targetSquare, float holdDuration)
        {
            currentLocation.obj = targetSquare;
            currentLocation.cornerIdx = 3;
            currentLocation.normal = Random.Range(0.4f, 0.6f);
            isOnTransition = false;

            headHitCheckTime = Time.time + holdDuration;
        }
    }


    public class SquareElement
    {
        public Vector2[] closestPointsOnLine;
        public bool[] constraints;
        public Vector2[] cornerPoints;
        public float dist;
        public float[] dists;
        public GameObject obj;
        public int[] shortCornerIdx = null;
    }

    public struct AvailableLocation
    {
        public SquareElement squareElement;
        public int cornerIdx;
        public float dist;
        public bool leftAvailalble, rightAvailable;
        public float normal;
    }

    public class CurrentLocation
    {
        public int cornerIdx;
        public Vector2[] cornerPoints;
        public bool direction;
        public float normal;
        public GameObject obj;
        public Vector2 pointA, pointB;
    }

    public struct Transition
    {
        public Vector2 startPoint;
        public Vector2 controlPoint;
        public Vector2 endPoint;

        public Quaternion startRotation;
        public Quaternion endRotation;

        public float normal;
    }
}