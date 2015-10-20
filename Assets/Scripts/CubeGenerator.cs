using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeGenerator : MonoBehaviour
{
    public GameObject cubeletPrefab;
    public GameObject rotatorPrefab;
    public Material[] textureArray = new Material[6];

    public int xSize;
    public int ySize;
    public int zSize;

    Vector3 startingPosition;
    Vector3 buildingPosition;

    public List<GameObject> cubelets = new List<GameObject>();
    public List<GameObject> rotators = new List<GameObject>();

    int offset;

    // Use this for initialization
    void Start()
    {
        Init();
        BuildCube();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Initialization variables
    void Init()
    {
        offset = (int)cubeletPrefab.transform.localScale.x;
        startingPosition = transform.position;
        buildingPosition = new Vector3(xSize / 2.0f, ySize / 2.0f, zSize / 2.0f);
    }

    // Basic cube construction
    void BuildCube()
    {
        // Move the cube into a good building position
        this.transform.position = buildingPosition;
        // Create the objects that we will use to facilitate rotation
        CreateRotationMatrix();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    if (((x == 0 || x == xSize - 1) || (y == 0 || y == ySize - 1)) || (z == 0 || z == zSize - 1))
                    {
                        if ((x == 0 && y == 0 && z == 0) || (x == 0 && y == 0 && z == zSize - 1) || (x == 0 && y == ySize - 1 && z == 0) || (x == 0 && y == ySize - 1 && z == zSize - 1) || (x == xSize - 1 && y == 0 && z == 0) || (x == xSize - 1 && y == 0 && z == zSize - 1) || (x == xSize - 1 && y == ySize - 1 && z == 0) || (x == xSize - 1 && y == ySize - 1 && z == zSize - 1))
                        {
                            cubeletPrefab = (GameObject)Instantiate(cubeletPrefab, new Vector3((x + (offset / 2.0f) * offset), (y + (offset / 2.0f) * offset), (z + (offset / 2.0f) * offset)), Quaternion.identity);
                            cubeletPrefab.transform.SetParent(transform);
                            cubeletPrefab.name = "Corner";
                            cubelets.Add(cubeletPrefab);
                        }
                        if ((x >= 1 && x <= xSize - 2 && y >= 1 && y <= ySize - 2) || (y >= 1 && y <= ySize - 2 && z >= 1 && z <= zSize - 2) || (x >= 1 && x <= xSize - 2 && z >= 1 && z <= zSize - 2))
                        {
                            cubeletPrefab = (GameObject)Instantiate(cubeletPrefab, new Vector3((x + (offset / 2.0f) * offset), (y + (offset / 2.0f) * offset), (z + (offset / 2.0f) * offset)), Quaternion.identity);
                            cubeletPrefab.transform.SetParent(transform);
                            cubeletPrefab.name = "Center";
                            cubelets.Add(cubeletPrefab);
                        }
                        if (((x > 0 && x < xSize - 1) && (y == 0 || y == ySize - 1) && (z == 0 || z == zSize - 1)) || ((x == 0 || x == xSize - 1) && (y > 0 && y < ySize - 1) && (z == 0 || z == zSize - 1)) || ((x == 0 || x == xSize - 1) && (y == 0 || y == ySize - 1) && (z > 0 && z < zSize - 1)))
                        {
                            cubeletPrefab = (GameObject)Instantiate(cubeletPrefab, new Vector3((x + (offset / 2.0f) * offset), (y + (offset / 2.0f) * offset), (z + (offset / 2.0f) * offset)), Quaternion.identity);
                            cubeletPrefab.transform.SetParent(transform);
                            cubeletPrefab.name = "Edge";
                            cubelets.Add(cubeletPrefab);
                        }
                    }
                }
            }
        }
        AssignFaceColor();
        // Move the cube back to its original position
        this.transform.position = startingPosition;
    }

    // Assign the colors or textures to each face of the cube
    void AssignFaceColor()
    {
        foreach (GameObject cubelet in cubelets)
        {
            if ((cubelet.transform.position.x - offset / 2.0f) == 0)
            {
                cubelet.transform.GetChild(5).GetComponent<MeshRenderer>().material = textureArray[4];
            }
            if ((cubelet.transform.position.x - offset / 2.0f) == xSize - 1)
            {
                cubelet.transform.GetChild(6).GetComponent<MeshRenderer>().material = textureArray[5];
            }
            if ((cubelet.transform.position.y - offset / 2.0f) == 0)
            {
                cubelet.transform.GetChild(2).GetComponent<MeshRenderer>().material = textureArray[1];
            }
            if ((cubelet.transform.position.y - offset / 2.0f) == ySize - 1)
            {
                cubelet.transform.GetChild(1).GetComponent<MeshRenderer>().material = textureArray[0];
            }
            if ((cubelet.transform.position.z - offset / 2.0f) == 0)
            {
                cubelet.transform.GetChild(3).GetComponent<MeshRenderer>().material = textureArray[2];
            }
            if ((cubelet.transform.position.z - offset / 2.0f) == zSize - 1)
            {
                cubelet.transform.GetChild(4).GetComponent<MeshRenderer>().material = textureArray[3];
            }
        }
    }

    void CreateRotationMatrix()
    {
        GameObject rotParent = (GameObject)Instantiate(rotatorPrefab, new Vector3((xSize / 2.0f), (ySize / 2.0f), (zSize / 2.0f)), Quaternion.identity);
        rotParent.name = "Rotation Core";
        rotators.Add(rotParent);
        rotParent.transform.SetParent(transform);

        for (int i = 0; i < xSize; i++)
        {
            Vector3 temp = new Vector3((i + (offset / 2.0f) * offset), (ySize / 2.0f), (zSize / 2.0f));
            if (temp != transform.position)
            {
                rotatorPrefab = (GameObject)Instantiate(rotatorPrefab, temp, Quaternion.identity);
                //facePrefab.transform.localScale = new Vector3(offset, offset, offset);
                rotatorPrefab.name = "XFace " + rotatorPrefab.transform.position;
                rotatorPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                rotators.Add(rotatorPrefab);
                rotatorPrefab.transform.SetParent(rotParent.transform);
            }
        }
        for (int i = 0; i < ySize; i++)
        {
            Vector3 temp = new Vector3((xSize / 2.0f), (i + (offset / 2.0f) * offset), (zSize / 2.0f));
            if (temp != transform.position)
            {
                rotatorPrefab = (GameObject)Instantiate(rotatorPrefab, temp, Quaternion.identity);
                //facePrefab.transform.localScale = new Vector3(offset, offset, offset);
                rotatorPrefab.name = "YFace " + rotatorPrefab.transform.position;
                rotatorPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                rotators.Add(rotatorPrefab);
                rotatorPrefab.transform.SetParent(rotParent.transform);
            }
        }
        for (int i = 0; i < zSize; i++)
        {
            Vector3 temp = new Vector3((xSize / 2.0f), (ySize / 2.0f), (i + (offset / 2.0f) * offset));
            if (temp != transform.position)
            {
                rotatorPrefab = (GameObject)Instantiate(rotatorPrefab, temp, Quaternion.identity);
                //facePrefab.transform.localScale = new Vector3(offset, offset, offset);
                rotatorPrefab.name = "ZFace " + rotatorPrefab.transform.position;
                rotatorPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                rotators.Add(rotatorPrefab);
                rotatorPrefab.transform.SetParent(rotParent.transform);
            }
        }
    }

    public int Offset
    {
        get { return offset; }
        set { offset = value; }
    }
}
