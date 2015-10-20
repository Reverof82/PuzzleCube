using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotationManager : MonoBehaviour
{
    enum Swipe { None, Up, Down, Left, Right };
    enum Face { None, Up, Down, Left, Right, Front, Back };
    Face selectedFace;
    Swipe swipeDirection;
    GameObject cube;
    Transform cameraTarget;
    GameObject cubelet;
    CubeGenerator gc;
    List<GameObject> groupToRotate = new List<GameObject>();
    bool animating;
    bool dragging = false;
    bool mousePressed;
    Vector2 mouseStartPosition;
    Vector2 mouseEndPosition;
    Vector2 currentSwipe;
    float cubeWidth = 0.0f;
    float distance = 5.0f;

    float xSpeed = 180.0f;
    float ySpeed = 90.0f;

    float yMinLimit = -80.0f;
    float yMaxLimit = 80.0f;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        cube = GameObject.Find("CubeCore");
        cameraTarget = cube.transform;
        gc = cube.GetComponent<CubeGenerator>();
        cubeWidth = gc.xSize * gc.Offset;
        distance = cubeWidth + 3.0f;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        PositionCamera();
    }

    void LateUpdate()
    {
        if (!Animating && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                cubelet = hit.transform.gameObject;
                selectedFace = GethitFace(hit);
                mousePressed = true;
                mouseStartPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mousePressed)
            {
                mouseEndPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                currentSwipe = new Vector2(mouseEndPosition.x - mouseStartPosition.x, mouseEndPosition.y - mouseStartPosition.y);
                currentSwipe.Normalize();
                if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    swipeDirection = Swipe.Up;
                }
                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    swipeDirection = Swipe.Down;
                }
                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    swipeDirection = Swipe.Right;
                }
                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    swipeDirection = Swipe.Left;
                }
                if (swipeDirection != Swipe.None)
                {
                    print("Swiped " + swipeDirection + " on the " + selectedFace + " face.");
                }
                print("Cube location: " + cubelet.transform.position.x + ", " + cubelet.transform.position.y + ", " + cubelet.transform.position.z);
                AssignCubeletsToRotator(cubelet, swipeDirection, selectedFace);
                mousePressed = false;
                cubelet = null;
                swipeDirection = Swipe.None;
            }
        }

        if (animating)
        {
            transform.LookAt(cameraTarget);
            x += xSpeed * 0.005f;
            PositionCamera();
        }

        if (cameraTarget && Input.GetMouseButton(0) && cubelet == null)
        {
            dragging = true;
            x += Input.GetAxis("Mouse X") * xSpeed * 0.05f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.05f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            PositionCamera();
        }
        else
        {
            dragging = false;
        }
    }

    void PositionCamera()
    {
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + cameraTarget.position;
        transform.rotation = rotation;
        transform.position = position;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    Face GethitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;
        if (incomingVec == new Vector3(0, -1, -1))
        {
            return Face.Front;
        }
        if (incomingVec == new Vector3(0, -1, 1))
        {
            return Face.Back;
        }
        if (incomingVec == new Vector3(0, 0, 0))
        {
            return Face.Up;
        }
        if (incomingVec == new Vector3(0, -2, 0))
        {
            return Face.Down;
        }
        if (incomingVec == new Vector3(-1, -1, 0))
        {
            return Face.Left;
        }
        if (incomingVec == new Vector3(1, -1, 0))
        {
            return Face.Right;
        }
        return Face.None;
    }

    void AssignCubeletsToRotator(GameObject cubeletClicked, Swipe swipeDirection, Face faceSelected)
    {
        Vector3 axis = Vector3.up;
        float angle = 90.0f;
        groupToRotate.Clear();
        
        if (swipeDirection == Swipe.Up || swipeDirection == Swipe.Down)
        {
            if (swipeDirection == Swipe.Down)
            {
                angle = -angle;
            }
            if (faceSelected == Face.Back || faceSelected == Face.Left || faceSelected == Face.Up)
            {
                angle = -angle;
            }
            foreach (GameObject cubelet in gc.cubelets)
            {
                if (faceSelected == Face.Front || faceSelected == Face.Back)
                {
                    axis = Vector3.right;
                    if (cubelet.transform.position.x == cubeletClicked.transform.position.x)
                    {
                        groupToRotate.Add(cubelet);
                    }
                }
                if(faceSelected == Face.Left || faceSelected == Face.Right)
                {
                    axis = Vector3.forward;
                    if (cubelet.transform.position.z == cubeletClicked.transform.position.z)
                    {
                        groupToRotate.Add(cubelet);
                    }
                }
                if (faceSelected == Face.Up || faceSelected == Face.Down)
                {
                    axis = Vector3.up;
                    if (cubelet.transform.position.y == cubeletClicked.transform.position.y)
                    {
                        groupToRotate.Add(cubelet);
                    }
                }
            }
        }
        if (swipeDirection == Swipe.Left || swipeDirection == Swipe.Right)
        {
            if (swipeDirection == Swipe.Right)
            {
                angle = -angle;
            }
            foreach (GameObject cubelet in gc.cubelets)
            {
                if (faceSelected == Face.Front || faceSelected == Face.Back)
                {
                    if (cubelet.transform.position.y == cubeletClicked.transform.position.y)
                    {
                        groupToRotate.Add(cubelet);
                    }
                }
                if (faceSelected == Face.Left || faceSelected == Face.Right)
                {
                    if (cubelet.transform.position.y == cubeletClicked.transform.position.y)
                    {
                        groupToRotate.Add(cubelet);
                    }
                }
                if (faceSelected == Face.Up || faceSelected == Face.Down)
                {
                    if (cubelet.transform.position.x == cubeletClicked.transform.position.x)
                    {
                        groupToRotate.Add(cubelet);
                    }
                }
            }
        }
        print(groupToRotate.Count);
        foreach (GameObject test in groupToRotate)
        {
            test.transform.RotateAround(cube.transform.position, axis, angle);
            test.transform.position = new Vector3(Mathf.Round(test.transform.position.x), Mathf.Round(test.transform.position.y), Mathf.Round(test.transform.position.z));
        }
    }

    public bool Animating
    {
        get { return animating; }
        set { animating = value; }
    }

    public bool Dragging
    {
        get { return dragging; }
        set { dragging = value; }
    }
}
