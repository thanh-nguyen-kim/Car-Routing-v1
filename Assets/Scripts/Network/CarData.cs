using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
namespace Assets.Scripts.Network
{
    public class CarData : MonoBehaviour
    {
        public int velocity = 50;
        public int color = 0;
        public Text velTextField = null;
        public GameObject car;
        void OnEnable()
        {
            velTextField = transform.GetChild(3).GetComponent<Text>();
            velTextField.text = this.velocity.ToString();
            SetColor();
            
        }

        public void OnDisable()
        {

        }

        public void SpawnCar() {
            if (car == null&&gameObject.activeSelf)
                car = GameController.Instance.SpawnCar(this.velocity, this.color);
        }

        public void DeSpawnCar()
        {
            if (car != null)
                GameController.Instance.DespawnCar(this.car);
            car = null;
        }

        public void IncreaseVel()
        {
            this.velocity += 5;
            velTextField.text = this.velocity.ToString();
            if(car!=null)
            car.GetComponent<CarController>().MaxSpeed = this.velocity;
        }

        public void DecreaseVel()
        {
            this.velocity -= 5;
            velTextField.text = this.velocity.ToString();
            if (car != null)
                car.GetComponent<CarController>().MaxSpeed = this.velocity;
        }

        private void SetColor() {
            Color tmpC = Color.blue;
            switch (this.color)
            {
                case 0:
                    tmpC = Color.blue;
                    break;
                case 1:
                    tmpC = Color.grey;
                    break;
                case 2:
                    tmpC = Color.red;
                    break;
                case 3:
                    tmpC = Color.yellow;
                    break;
                case 4:
                    tmpC = Color.white;
                    break;
            }
            transform.GetChild(0).GetComponent<Image>().color = tmpC;
        }

        public void ChooseColor()
        {
            this.color = (this.color + 1) % 5;
            Color tmpC = Color.blue;
            switch (this.color)
            {
                case 0:
                    tmpC = Color.blue;
                    break;
                case 1:
                    tmpC = Color.grey;
                    break;
                case 2:
                    tmpC = Color.red;
                    break;
                case 3:
                    tmpC = Color.yellow;
                    break;
                case 4:
                    tmpC = Color.white;
                    break;
            }
            transform.GetChild(0).GetComponent<Image>().color = tmpC;
            DeSpawnCar();
        }
    }
}
