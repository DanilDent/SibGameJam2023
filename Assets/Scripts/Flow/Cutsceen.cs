using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Enemy;

namespace GameFlow
{
    public class Cutsceen : MonoBehaviour
    {
        [SerializeField] private List<Image> _imageSequance;
        [SerializeField] private float _delayBetweenFrames;

        public IEnumerator StartCutsceen(EnemySpawner spawnerToDeactivate)
        {
            spawnerToDeactivate.CanSpawn = false;
            _imageSequance[0].gameObject.SetActive(true);
            yield return StartCoroutine(SequanceCoroutine());
            _imageSequance[_imageSequance.Count - 1].gameObject.SetActive(false);
            spawnerToDeactivate.CanSpawn = true;
        }

        private IEnumerator SequanceCoroutine()
        {
            Image lastImage = _imageSequance[0];

            for (int i = 1; i < _imageSequance.Count; i++)
            {
                yield return new WaitForSeconds(_delayBetweenFrames);
                lastImage.gameObject.SetActive(false);
                Image currentImage = _imageSequance[i];
                currentImage.gameObject.SetActive(true);
                lastImage = currentImage;
            }

            yield return new WaitForSeconds(_delayBetweenFrames);
        }
    }
}