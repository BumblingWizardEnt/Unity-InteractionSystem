using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BumblingWizard
{
    public class InteractionManager : MonoBehaviour
    {
        // Static Reference so that interactable objects can register and deregister themselves with OnEnable and OnDisable
        public static InteractionManager Instance;

        [Header("Interaction Config")]
        [Tooltip("Minimum Distance required to interact with an object")]
        [SerializeField] private float minDistanceToInteract = 1;

        [Tooltip("How often the manager update")]
        [SerializeField] private float updateFrequency = 0.25f;

        [Header("Input System")]
        [Tooltip("The input action asset you will be referencing for input")]
        [SerializeField] private InputActionAsset _inputActionsAsset;

        [Tooltip("The input action name")]
        [SerializeField] private string interactInputActionName = "Interact";

        // all interactable objects in the scene
        HashSet<Interactable> interactableObjects = new HashSet<Interactable>();

        // an empty reference to store the nearest (if any) interactable object
        Interactable nearestInteractableObject;
        Interactable previousNearest;

        // reference to get the player (Could just put the player here instead)
        GameManager gameManager;

        // reference to the player transform
        private Transform playerTransform;

        private float timeSinceLastUpdate = 0;

        private void Awake()
        {
            // So we can register and unregister interactions
            Instance = this;

            // a reference to the game manager so we can get reference to the player
            gameManager = GetComponent<GameManager>();
            playerTransform = gameManager.Player.transform;
            timeSinceLastUpdate = Time.time;
        }

        private void Update()
        {
            if (Time.time - timeSinceLastUpdate > updateFrequency)
            {
                timeSinceLastUpdate = Time.time;
                MonitorInteractableObjects();
            }
        }

        private void OnEnable()
        {
            DeinitPlayerInput();
            InitPlayerInput();
        }

        private void OnDisable()
        {
            DeinitPlayerInput();
        }

        // Runs on loop 
        void MonitorInteractableObjects()
        {
            float minDist = float.MaxValue;
            Interactable nearestInteraction = null;
            float sqrDist = 0, maxDist = 0;

            // gather which interactable object is closest to the player 
            foreach (var interactable in interactableObjects)
            {
                if (interactable == null) continue;

                sqrDist = (playerTransform.position - interactable.transform.position).sqrMagnitude;
                maxDist = minDistanceToInteract * minDistanceToInteract;

                if (sqrDist < 0.01f)
                {
                    nearestInteraction = interactable;
                    break;
                }

                if (sqrDist < minDist && sqrDist <= maxDist)
                {
                    minDist = sqrDist;
                    nearestInteraction = interactable;
                }
            }
            nearestInteractableObject = nearestInteraction;
            if (previousNearest != nearestInteraction)
            {
                if (previousNearest != null)
                    previousNearest.UpdateIndicator(false);

                if (nearestInteraction != null)
                    nearestInteraction.UpdateIndicator(true);

                previousNearest = nearestInteraction;
            }
        }

        public void InteractWithClosestInteractiveObject()
        {
            if (nearestInteractableObject == null)
                return;
            nearestInteractableObject.OnInteract();
        }

        public void RegisterInteraction(Interactable interactable) => interactableObjects.Add(interactable);

        public void UnRegisterInteraction(Interactable interactable) => interactableObjects.Remove(interactable);

        ////// -----------------------------------------------------------
        ////// INPUT SYSTEM
        ////// -----------------------------------------------------------

        public InputAction InteractInput { get; set; }

        public InputActionAsset inputActionsAsset => _inputActionsAsset;

        public virtual void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                InteractWithClosestInteractiveObject();
        }

        protected virtual void InitPlayerInput()
        {
            if (inputActionsAsset == null)
                return;

            // Movement input action
            InteractInput = inputActionsAsset.FindAction(interactInputActionName);
            if (InteractInput != null)
            {
                InteractInput.started += OnInteract;
                InteractInput.Enable();
            }

        }

        protected virtual void DeinitPlayerInput()
        {
            // Unsubscribe from input action events and disable input actions
            if (InteractInput != null)
            {
                InteractInput.started -= OnInteract;
                InteractInput.Disable();
                InteractInput = null;
            }
        }

    }
}
