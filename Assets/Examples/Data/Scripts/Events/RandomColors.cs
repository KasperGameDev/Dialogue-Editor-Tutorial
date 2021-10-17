using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.Dialogue.Example
{
    public class RandomColors : MonoBehaviour
    {
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

        public void DoRandomColorModel()
        {

                foreach (Material material in materials)
                {
                    material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
            
        }


    }
}