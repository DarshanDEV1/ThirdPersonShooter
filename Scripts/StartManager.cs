using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField] Button _startBTN;

    private void Start()
    {
        _startBTN.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GameScene");
        });
    }
}
