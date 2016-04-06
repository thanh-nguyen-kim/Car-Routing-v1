using UnityEngine;
namespace Assets.Scripts.ShortScript
{
    public class SpawnCarBtn:MonoBehaviour
    {
        public void AddCar()
        {
            for(int i = 0; i < transform.parent.childCount; i++)
            {
                if (!transform.parent.GetChild(i).gameObject.activeInHierarchy)
                {
                    transform.parent.GetChild(i).gameObject.SetActive(true);
                    return;
                }
            }
        }

        public void SpawnCar()
        {

        }
    }
}
