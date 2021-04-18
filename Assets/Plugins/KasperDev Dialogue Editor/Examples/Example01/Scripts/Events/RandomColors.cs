using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.DialogueEditor
{
    public class RandomColors : MonoBehaviour
    {
        [SerializeField] private int myNumber;
        private List<Material> materials = new List<Material>();

        private void Awake()
        {
            SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer smr in skinnedMeshRenderers)
            {
                foreach (Material mat in smr.materials)
                {
                    materials.Add(mat);
                }
            }
        }

        private void Start()
        {
            GameEvents.Instance.RandomColorModel += DoRandomColorModel;
        }

        private void OnDestroy()
        {
            GameEvents.Instance.RandomColorModel -= DoRandomColorModel;
        }

        private void DoRandomColorModel(int _number)
        {
            if (myNumber == _number)
            {
                foreach (Material material in materials)
                {
                    material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
            }
        }


    }
}