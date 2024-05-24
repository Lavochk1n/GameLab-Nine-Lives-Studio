using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Quarantine
{

    public class CageBehaviour : Interactable
    {
        [SerializeField] private List<CageBehaviour> AdjCages;
        [SerializeField] private LayerMask wallLayer;
        [SerializeField] private float searchDistance =3f;

        private float spreadSpeed;

        [SerializeField] private CageVisual myCageVisual;

        public Animal myAnimal = new();

        public bool markedForRemoval = false; 

        private void Start()
        {
            InitializeCages();
            spreadSpeed = QuarentineManager.Instance.spreadSpeed;

            spreadSpeed *= Random.Range(.8f, 1.2f) * GameManager.Instance.GetDifficultyRatio();
            if (myAnimal.state == SickState.sick)
            {
                myAnimal.sickProgression = 100f;
            }
            else
            {
                if (TutorialManager.Instance == null)
                {
                    myAnimal.sickProgression = 0f;
                }
            }

            UpdateCage();

            StartCoroutine(UpdateVisuals());
        }

        private void Update()
        {
            if (!QuarentineManager.Instance.PlayerSpawned() || QuarentineManager.Instance.GamePaused() ) return;

            CheckSpread();
        }

        public IEnumerator UpdateVisuals( )
        {
            while (true)
            {
                yield return new WaitForSeconds(.1f);
                myCageVisual.UpdateProgressbar(myAnimal);
                //myCageVisual.UpdateIcon(myAnimal);
            }
        }

        public override void Interact(Interactor interactor)
        {
            PlayerBehaviour playerBehaviour = interactor.GetComponent<PlayerBehaviour>();

            Animal heldAnimal = playerBehaviour.heldAnimal;

            if (heldAnimal.type == AnimalTypes.Empty || myAnimal.type == AnimalTypes.Empty)
            {
                if (heldAnimal.type == AnimalTypes.Empty)
                {
                    if(!playerBehaviour.GetComponent<GloveManager>().HasGloves())
                    {
                        return;
                    }
                    playerBehaviour.GetComponent<GloveManager>().RemoveGlove();
                }
                else
                {
                    PlayerBehaviour player1 = QuarentineManager.Instance.player.GetComponent<PlayerBehaviour>();
                    PlayerBehaviour player2 = QuarentineManager.Instance.player2.GetComponent<PlayerBehaviour>();

                    
                    if (playerBehaviour ==  player1)
                    {
                        if(player2.mostRecentCage == this)
                        {
                            player2.mostRecentCage = player1.mostRecentCage;
                        }
                    }
                    if (playerBehaviour == player2)
                    {
                        if (player1.mostRecentCage == this)
                        {
                            player1.mostRecentCage = player2.mostRecentCage;
                        }
                    }

                    //if (markedForRemoval)
                    //{
                    //    markedForRemoval = false;
                    //    playerBehaviour.mostRecentCage.markedForRemoval = true;

                    //}
                }

                playerBehaviour.heldAnimal = myAnimal;

                playerBehaviour.mostRecentCage = this; 

                myAnimal = heldAnimal;

                UpdateCage();
                playerBehaviour.UpdateHeldAnimal(); 
            }
        }

        public override void Interact_Secondairy(Interactor interactor)
        {
            if (myAnimal.type == AnimalTypes.Empty) { return; }


            if(interactor != null)
            {

                PlayerBehaviour pb = interactor.GetComponent<PlayerBehaviour>();
                if(markedForRemoval)
                {
                    pb.flagAmount++;
                    markedForRemoval = !markedForRemoval;
                }
                else
                {
                    if (pb.flagAmount > 0 )
                    {
                        pb.flagAmount--;
                        markedForRemoval = !markedForRemoval;
                    }
                }
            }
            else
            {
                markedForRemoval = !markedForRemoval;
            }
            myCageVisual.UpdateFlag(markedForRemoval);
        }

        public override string GetDescription()
        {
            //myCageVisual.IsLookedAt(); 
            return null; 
        }


        private void InitializeCages()
        {
            AdjCages = new List<CageBehaviour>();

            Collider[] colliders = Physics.OverlapSphere(transform.position, searchDistance);

            foreach (Collider col in colliders)
            {
                if (col.TryGetComponent<CageBehaviour>(out var cage))
                {
                    //Vector3 direction = transform.position - col.transform.position; 

                    //RaycastHit hit;
                    //if(Physics.Raycast(transform.position, direction.normalized, out hit, searchDistance/*, wallLayer*/))
                    //{

                    //}
                    //else
                    {
                        AdjCages.Add(cage);
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, searchDistance);
            
            foreach(CageBehaviour cageBehaviour in AdjCages)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawLine(transform.position, cageBehaviour.transform.position);

            }
        }

        public void ChangeOccupation(AnimalTypes animal)
        {
            myAnimal.type = animal;
        }
        public void ChangeSickstate(SickState sick)
        {
            myAnimal.state = sick;
        }

        public void UpdateCage()
        {
            myCageVisual.UpdateIcon(myAnimal);
        }


        private void CheckSpread()
        {
            if (myAnimal.state == SickState.sick) return;

            if (AdjDisease())
            {
                if (myAnimal.sickProgression >= 100)
                {
                    myAnimal.state = SickState.sick;
                    myAnimal.sickProgression = 100;
                    UpdateCage();
                }
                else 
                {
                    myAnimal.sickProgression += spreadSpeed * Time.deltaTime; 
                }
            } 
            else if(myAnimal.sickProgression > 0)
            {
                myAnimal.sickProgression -= spreadSpeed * Time.deltaTime;

                if (myAnimal.sickProgression < 0) myAnimal.sickProgression = 0; 
            }
        }

        public bool AdjDisease()
        {
            foreach(CageBehaviour cage in AdjCages)
            {
                if (cage.IsContagious(myAnimal.type))
                {
                    return true;
                } 
            }

            return false;
        }

        private bool IsContagious(AnimalTypes type)
        {

            if (type == AnimalTypes.Bunny)
            {
                if (myAnimal.type == AnimalTypes.Bunny && myAnimal.state == SickState.sick)
                {
                    return true;
                }
            }

            if (type == AnimalTypes.parrot)
            {
                if (myAnimal.type == AnimalTypes.parrot && myAnimal.state == SickState.sick)
                {
                    return true;
                }
            }

            if (type == AnimalTypes.crow)
            {
                if(myAnimal.type == AnimalTypes.parrot || (myAnimal.type == AnimalTypes.crow && myAnimal.state == SickState.sick))
                {
                    return true;
                }
            }
            return false; 
        }
    }
}