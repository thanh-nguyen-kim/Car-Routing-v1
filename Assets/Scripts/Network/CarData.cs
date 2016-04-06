using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.Network
{
    public class CarData : MonoBehaviour
    {
        public int velocity = 20;
        public Text velTextField=null;

        void OnEnable()
        {
            velTextField = transform.GetChild(3).GetComponent<Text>();
            velTextField.text = this.velocity.ToString();
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
