using System.Collections.Generic;
using UnityEngine;

public class ElectricNoiseGenerator : MonoBehaviour
{
    public GameObject[] m_ElectricLines;
    private Noise3D m_NoiseTexture = new Noise3D();

    private void Start()
    {
        m_NoiseTexture.Create(64, 2);
        for (int i = 0; i < m_ElectricLines.Length; i++)
        {
            MeshRenderer rd = m_ElectricLines[i].GetComponent<MeshRenderer>();
            rd.material.SetTexture("_NoiseTex", m_NoiseTexture.Get());
        }
    }
}