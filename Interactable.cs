using UnityEngine;

namespace BumblingWizard
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] GameObject indicator;
        public GameObject Indicator { get { return indicator; } }


        protected virtual void OnEnable()
        {
            InteractionManager.Instance.RegisterInteraction(this);
            UpdateIndicator(false);
        }

        protected virtual void OnDisable()
        {
            InteractionManager.Instance.UnRegisterInteraction(this);
        }

        public virtual void OnInteract()
        {
            // virtual method for on interact
        }

        public void UpdateIndicator(bool setActive)
        {
            if (indicator == null)
                return;

            // turn it off if its on
            if (indicator.activeSelf)
                indicator.SetActive(false);
            // if this is the one were activating then activate it
            if (setActive)
                indicator.SetActive(setActive);
        }
    }
}