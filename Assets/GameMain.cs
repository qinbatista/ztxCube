using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMain : MonoBehaviour
{
    [SerializeField] private GameObject _cubePrefab;
    private int _cubeCount;

    // Start is called before the first frame update
    void Start()
    {
        Reset(3);
    }
    void Reset(int num)
    {
        _cubeCount = num;
        for (int y = 0; y < _cubeCount; y++)
        {
            for (int x = 0; x < _cubeCount; x++)
            {
                for (int z = 0; z < _cubeCount; z++)
                {
                    float gap = (_cubeCount - 1) * 0.1f;
                    GameObject one = Instantiate(_cubePrefab);
                    one.transform.SetParent(transform);
                    one.transform.position = new Vector3(-1.2f + x + gap, -1.2f + y + gap, -1.2f + z + gap);
                    one.transform.rotation = Quaternion.Euler(90 * Random.Range(0, 4), 90 * Random.Range(0, 4), 90 * Random.Range(0, 4));
                }
            }
        }
    }
}
