using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RotationManager : MonoBehaviour
{
    enum Swipe { Up, Down, Left, Right };
    enum Face { Up, Down, Left, Right, Front, Back };
    Face selectedFace;
    Face lastFace;
    Swipe swipeDirection;
    GameObject cube;
    Transform cameraTarget;
    GameObject cubelet;
    CubeGenerator cg;

    bool animating;
    bool rotating;
    bool canDrag = true;
    bool dragging = false;
    bool mousePressed;
    bool locked = false;
    bool spinning = false;

    Vector2 mouseStartPosition;
    Vector2 mouseEndPosition;
    Vector2 currentSwipe;
    Vector3 degrees;

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
        cg = cube.GetComponent<CubeGenerator>();
        cubeWidth = cg.xSize * cg.Offset;
        distance = cubeWidth + 3.0f;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        PositionCamera();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Scramble();
        }
        if (!Animating && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                canDrag = false;
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
                SelectRotator();
                mousePressed = false;
                cubelet = null;
                canDrag = true;
            }
        }

        if (spinning)
        {
            transform.LookAt(cameraTarget);
            x += xSpeed * 0.005f;
            PositionCamera();
        }

        if (cameraTarget && Input.GetMouseButton(0) && canDrag && !locked)
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

    public void LockUnlock()
    {
        GameObject Btn_Lock = GameObject.Find("Btn_Lock");
        if (Locked)
        {
            Locked = false;
            Btn_Lock.GetComponent<Image>().color = Color.white;
        }
        else
        {
            Locked = true;
            Btn_Lock.GetComponent<Image>().color = Color.red;
        }
    }

    void Rewards()
    {
        GameObject Btn_Rewards = GameObject.Find("Btn_Rewards");
    }

    public void Scramble()
    {
        GameObject Btn_Scramble = GameObject.Find("Btn_Scramble");
        if (!Animating)
        {
            Animating = true;
            StartCoroutine(Jumble());
            Btn_Scramble.GetComponent<Image>().color = Color.red;
        }
        else if (Animating)
        {
            Animating = false;
            spinning = false;
            Btn_Scramble.GetComponent<Image>().color = Color.white;
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
        return Face.Front;
    }

    void SelectRotator()
    {
        NormalizeCubelet();
        if (swipeDirection == Swipe.Up || swipeDirection == Swipe.Down)
        {
            if (selectedFace == Face.Front || selectedFace == Face.Back)
            {
                if (cubelet.transform.position.x == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.x;
                }
                foreach (GameObject rotator in cg.rotators)
                {
                    if (cubelet.transform.position.x == rotator.transform.position.x && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.x)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                }
            }
            if (selectedFace == Face.Left || selectedFace == Face.Right)
            {
                if (cubelet.transform.position.z == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.z;
                }
                foreach (GameObject rotator in cg.rotators)
                {
                    if (cubelet.transform.position.z == rotator.transform.position.z && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.z)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                }
            }
            if (selectedFace == Face.Up || selectedFace == Face.Down)
            {
                if(cubelet.transform.position.x == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.x;
                }
                if (cubelet.transform.position.z == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.z;
                }
                foreach (GameObject rotator in cg.rotators)
                {
                    if(cubelet.transform.position.x == rotator.transform.position.x && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.x)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                    if (cubelet.transform.position.z == rotator.transform.position.z && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.z)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                }
            }
        }
        if (swipeDirection == Swipe.Left || swipeDirection == Swipe.Right)
        {
            if (selectedFace == Face.Front || selectedFace == Face.Back)
            {
                if (cubelet.transform.position.y == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.y;
                }
                foreach (GameObject rotator in cg.rotators)
                {
                    if (cubelet.transform.position.y == rotator.transform.position.y && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.y)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                }
            }
            if (selectedFace == Face.Left || selectedFace == Face.Right)
            {
                if (cubelet.transform.position.y == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.y;
                }
                foreach (GameObject rotator in cg.rotators)
                {
                    if (cubelet.transform.position.y == rotator.transform.position.y && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.y)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                }
            }
            if (selectedFace == Face.Up || selectedFace == Face.Down)
            {
                if (cubelet.transform.position.x == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.x;
                }
                if (cubelet.transform.position.z == 0)
                {
                    GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.z;
                }
                foreach (GameObject rotator in cg.rotators)
                {
                    if (cubelet.transform.position.z == rotator.transform.position.z && rotator.GetComponent<RotatorData>().RotationAxis == RotatorData.Rotation.z)
                    {
                        AssignCubelets(rotator);
                        break;
                    }
                }
            }
        }
        GameObject.Find("RotationCore").GetComponent<RotatorData>().RotationAxis = RotatorData.Rotation.n;
    }

    void AssignCubelets(GameObject rotator)
    {
        RotatorData rd = rotator.GetComponent<RotatorData>();
        foreach (GameObject cubelet in cg.cubelets)
        {
            if (rotator != this.transform.gameObject)
            {
                if (rd.RotationAxis == RotatorData.Rotation.n)
                {
                    break;
                }
                if (cubelet.transform.position.x == rotator.transform.position.x && rd.RotationAxis == RotatorData.Rotation.x)
                {
                    cubelet.transform.SetParent(rotator.transform);
                }
                if (cubelet.transform.position.y == rotator.transform.position.y && rd.RotationAxis == RotatorData.Rotation.y)
                {
                    cubelet.transform.SetParent(rotator.transform);
                }
                if (cubelet.transform.position.z == rotator.transform.position.z && rd.RotationAxis == RotatorData.Rotation.z)
                {
                    cubelet.transform.SetParent(rotator.transform);
                }
            }
        }
        AssignRotationVector(rotator);
        if (swipeDirection == Swipe.Down)
        {
            if (selectedFace == Face.Back || selectedFace == Face.Left)
            {
                StartCoroutine(Rotate(true, true));
            }
            else
            {
                StartCoroutine(Rotate(true, false));
            }
        }
        else if (swipeDirection == Swipe.Up)
        {
            if (selectedFace == Face.Back || selectedFace == Face.Left)
            {
                StartCoroutine(Rotate(true, false));
            }
            else
            {
                StartCoroutine(Rotate(true, true));
            }
        }
        else if (swipeDirection == Swipe.Left)
        {
            if (selectedFace == Face.Back || selectedFace == Face.Left)
            {
                StartCoroutine(Rotate(true, true));
            }
            else
            {
                StartCoroutine(Rotate(true, true));
            }
        }
        else if (swipeDirection == Swipe.Right)
        {
            if (selectedFace == Face.Back || selectedFace == Face.Left)
            {
                StartCoroutine(Rotate(true, false));
            }
            else
            {
                StartCoroutine(Rotate(true, false));
            }
        }
    }

    void AssignRotationVector(GameObject rotator)
    {
        RotatorData rd = rotator.GetComponent<RotatorData>();
        RotatorData.Rotation rotationAxis = rd.RotationAxis;
        rd.Active = true;
        switch (rotationAxis)
        {
            case RotatorData.Rotation.x:
                degrees = new Vector3(90.0f, 0.0f, 0.0f);
                break;
            case RotatorData.Rotation.y:
                degrees = new Vector3(0.0f, 90.0f, 0.0f);
                break;
            case RotatorData.Rotation.z:
                degrees = new Vector3(0.0f, 0.0f, 90.0f);
                break;
        }
    }

    public IEnumerator Rotate(bool animate, bool clockwise)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;
        Quaternion direction;
        GameObject rotatorToUse = null;
        if (clockwise)
        {
            direction = Quaternion.Euler(degrees.x, degrees.y, degrees.z);
        }
        else
        {
            direction = Quaternion.Euler(-degrees.x, -degrees.y, -degrees.z);
        }
        foreach (GameObject rotator in cg.rotators)
        {
            if (rotator.GetComponent<RotatorData>().Active)
            {
                rotatorToUse = rotator;
            }
        }
        float i = 0.0f;
        float rate = 5.0f;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            rotatorToUse.transform.rotation = Quaternion.Slerp(rotatorToUse.transform.rotation, direction, Mathf.SmoothStep(0.0f, 1.0f, i));
            if (animate)
            {
                yield return new WaitForSeconds(0.001f);
            }
        }
        rotating = false;
        rotatorToUse.GetComponent<RotatorData>().Active = false;
        NormalizeCubelet();
        NormalizeRotator();
    }

    public IEnumerator Jumble()
    {
        while(Animating)
        {
            if(Dragging)
            {
                spinning = false;
            }
            else
            {
                spinning = true;
            }
            selectedFace = (Face)Random.Range(0, 6);
            swipeDirection = (Swipe)Random.Range(0, 4);
            cubelet = cg.cubelets[Random.Range(0, cg.cubelets.Count)];
            if (lastFace == selectedFace)
            {
                selectedFace = (Face)Random.Range(0, 6);
            }
            
            SelectRotator();
            yield return new WaitForSeconds(0.26f);
            lastFace = selectedFace;
        }
    }

    void NormalizeCubelet()
    {
        foreach (GameObject cubelet in cg.cubelets)
        {
            cubelet.transform.SetParent(GameObject.Find("CubeCore").transform);
            cubelet.transform.position = new Vector3((Mathf.Round(cubelet.transform.position.x * 100) / 100), (Mathf.Round(cubelet.transform.position.y * 100) / 100), (Mathf.Round(cubelet.transform.position.z * 100) / 100));
        }
    }

    void NormalizeRotator()
    {
        foreach (GameObject rotator in cg.rotators)
        {
            rotator.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
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

    public bool Locked
    {
        get { return locked; }
        set { locked = value; }
    }
}
