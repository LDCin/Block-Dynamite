using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverPanel : MonoBehaviour
    {
        public void Start()
        {
            gameObject.SetActive(false);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}