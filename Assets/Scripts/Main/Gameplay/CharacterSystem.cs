using UnityEngine;


namespace Main.Gameplay
{
    public class CharacterSystem : MonoBehaviour
    {
        public CharacterMovement CharacterMovement { get; private set; }
        public CharacterItem CharacterItem { get; private set; }
        public ICharacterInput InputHandler { get; private set; }

        public GameObject graphics;

        protected virtual void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
            CharacterItem = GetComponent<CharacterItem>();
            InputHandler = GetComponent<ICharacterInput>();
        }

        protected virtual void Update()
        {

        }
    }
}