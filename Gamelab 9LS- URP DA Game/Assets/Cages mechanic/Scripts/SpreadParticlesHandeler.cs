using Quarantine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpreadParticlesHandeler : MonoBehaviour
{

    private GameObject ps_spread, arrow;

    private CageBehaviour CB;
    private List<CageBehaviour> adjCages = new(); 
    private List<GameObject> adjArrows = new();

    public List<PSRef> psRefList = new();

    private float edgeDistance = .35f;

    void Start()
    {
        CB = GetComponent<CageBehaviour>();

        ps_spread = CB.spPrefab;
        arrow = CB.spreadArrow; 



        adjCages = CB.AdjCages;
        if (adjCages != null)
        {
            InitializeSystems();
        }
    }


    private void InitializeSystems()
    {
        foreach (var cage in adjCages)
        {
            Vector3 pos = cage.transform.position;
            Vector3 m_pos = transform.position;

            Vector3 middle = (pos + m_pos) /2.0f  ;

            Vector3 dir = (pos - m_pos).normalized;

            Vector3 edgePos = m_pos + dir * edgeDistance;
            
            GameObject spawnedObject = Instantiate(arrow, edgePos, Quaternion.identity, transform);

            //GameObject spawnedObject = Instantiate(arrow, middle, Quaternion.identity, transform);
            adjArrows.Add(spawnedObject);


            //ParticleSystem ps = spawnedObject.GetComponentInChildren<ParticleSystem>();
            //psRefList.Add(new PSRef { cage = cage, ps = ps, arrow = spawnedObject });

            
            spawnedObject.transform.LookAt(transform.position);
            spawnedObject.SetActive(false);
        }
    }


    private void Update()
    {
        for (int i = 0; i < adjCages.Count; i ++)
        {
            if(adjCages[i].IsContagious(CB.myAnimal))
            {
                adjArrows[i].SetActive(true);
            }
            else adjArrows[i].SetActive(false);
        }
        
    }




    public void ToggleParticlesForCage(CageBehaviour targetCage, bool state)
    {
        foreach (PSRef psRef in psRefList)
        {
            if (psRef.cage == targetCage)
            {
                if (psRef.ps != null)
                {
                    if (state)
                    {
                        psRef.ps.Play();

                    }
                    else
                    {
                        psRef.ps.Stop();

                    }
                }
                break;
            }
        }
    }



    [System.Serializable]
    public class PSRef
    {
        public CageBehaviour cage;
        public GameObject arrow;
        public ParticleSystem ps; 
    }

}
