using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("Aspect Ratio to use for game.  If Vector2.zero, the default aspect ratio will be used.")] [SerializeField]
    private Vector2 aspectRatio = Vector2.zero;

    [Tooltip("Whether or not full screen will be used")] [SerializeField]
    private bool fullScreen = false;

    private void Awake()
    {
        if (aspectRatio != Vector2.zero)
        {
            var x = Screen.height * (aspectRatio.x / aspectRatio.y);
            var y = Screen.height;
            Screen.SetResolution((int) x, y, fullScreen);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("Tetris");
    }
}