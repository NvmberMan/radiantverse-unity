using UnityEngine;


namespace Main.Gameplay
{
    public class CharacterSystem : MonoBehaviour
    {
        public CharacterMovement CharacterMovement { get; private set; }
        public CharacterItem CharacterItem { get; private set; }
        public ICharacterInput InputHandler { get; private set; }

        protected Transform graphics;

        protected virtual void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
            CharacterItem = GetComponent<CharacterItem>();
            InputHandler = GetComponent<ICharacterInput>();

            graphics = GetComponentInChildren<Animator>()?.transform;

            if(graphics == null)
            {
                Debug.LogError("Graphics (Animator) tidak ditemukan di child!");
            }

        }

        protected virtual void Update()
        {

        }
    }
}