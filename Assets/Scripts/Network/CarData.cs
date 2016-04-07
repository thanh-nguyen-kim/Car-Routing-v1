using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Network
{
    public class CarData : MonoBehaviour
    {
        public int velocity = 20;
        public Text velTextField = null;
        public GameObject car;
        void OnEnable()
        {
            velTextField = transform.GetChild(3).GetComponent<Text>();
            velTextField.text = this.velocity.ToString();
            if (car == null)
                car = GameController.Instance.SpawnCar();
        }

        public void OnDisable()
        {

        }

        public void DeSpawnCar()
        {
            GameController.Instance.DespawnCar(this.car);
            car = null;
        }

        public void IncreaseVel()
        {
            this.velocity += 5;
            velTextField.text = this.velocity.ToString();
        }

        public void DecreaseVel()
        {
            this.velocity -= 5;
            velTextField.text = this.velocity.ToString();
        }
    }
}
