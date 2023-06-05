using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MFarm.Transition
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if(sceneToGo == SceneManager.GetActiveScene().name)
                {
                    UIManager.Instance.GoTo(5);
                }else
                    EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}