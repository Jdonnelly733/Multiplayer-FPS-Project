using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NJGame.Singleplayer.Other
{

    public class AutoDestroy : MonoBehaviour
    {

        [SerializeField] private float timeTilDestruction;

        public GameObject obj;
        public bool fromStart;

        private void Start()
        {
            if (fromStart && obj != null)
            {
                DestroyObject(obj, timeTilDestruction);
            }
        }

        public void DestroyComponent(Component componentToDestroy, float timeToDestroy)
        {
            timeTilDestruction = timeToDestroy;

            StartCoroutine(destroyComponent(componentToDestroy));
        }

        private IEnumerator destroyComponent(Component destroyObj)
        {
            yield return new WaitForSeconds(timeTilDestruction);
            Destroy(destroyObj);
            Destroy(this.GetComponent<AutoDestroy>());
        }

        public void DestroyObject(GameObject objectToDestroy, float timeToDestroy)
        {
            timeTilDestruction = timeToDestroy;

            StartCoroutine(destroyObject(objectToDestroy));
        }

        private IEnumerator destroyObject(GameObject destroyObj)
        {
            yield return new WaitForSeconds(timeTilDestruction);
            Destroy(destroyObj);
            Destroy(this.GetComponent<AutoDestroy>());
        }

    }

}