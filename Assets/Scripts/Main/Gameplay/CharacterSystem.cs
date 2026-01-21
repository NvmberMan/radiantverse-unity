using Spine.Unity;
using UnityEngine;


namespace Main.Gameplay
{
    public class CharacterSystem : MonoBehaviour
    {
        public CharacterMovement CharacterMovement { get; private set; }
        public ICharacterInput InputHandler { get; private set; }

        protected SkeletonAnimation skeletonAnimation;

        protected virtual void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
            InputHandler = GetComponent<ICharacterInput>();

            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

            if(skeletonAnimation == null)
            {
                Debug.LogError("Graphics (Animator) tidak ditemukan di child!");
            }

        }

        protected virtual void Update()
        {

        }
    }
}