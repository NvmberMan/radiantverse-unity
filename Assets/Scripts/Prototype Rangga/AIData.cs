using Spine.Unity;
using System.Collections.Generic;
using Unity.Barracuda;

namespace Main.Gameplay
{
    [System.Serializable ]
    public class AIData
    {
        public string characterName;
        public int wayIndex;
        public NNModel brain;
        public SkeletonDataAsset sourceAsset;

        [SpineSkin(dataField: "sourceAsset")]
        public List<string> skinConfigs;
    }
}