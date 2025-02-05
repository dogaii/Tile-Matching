using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int rows = 10;    // Number of rows in the grid
    public int columns = 10; // Number of columns in the grid
    public float cellSize = 2.56f; // Size of each block in the grid
    private Camera mainCamera;
    public static CameraManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep CameraManager alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        mainCamera = Camera.main;
        CenterCamera();
    }

    private void CenterCamera()
    {
        float x = (columns - 1) * cellSize / 2f;
        float y = (rows - 1) * cellSize / 2f;

        transform.position = new Vector3(x, -y, -10f);

        float verticalSize = (rows * cellSize) / 2f;
        float horizontalSize = (columns * cellSize) / (2f * mainCamera.aspect);

        mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}