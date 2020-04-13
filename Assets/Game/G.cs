using UnityEngine;

namespace Game
{
    public class G : MonoBehaviour
    {
        public static G self { get; private set; }
        public WindowManager window_manager { get; internal set; }
        public ResourceManager<Asset> asset_resource_manager { get; private set; }
        public ResourceManager<Personage> personage_resource_manager { get; private set; }
        public UserInventory user_inventory;
        public PersonageModel personage { get; private set; }

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            self = this;
            asset_resource_manager = new ResourceManager<Asset>();
            personage_resource_manager = new ResourceManager<Personage>();
            window_manager = new WindowManager();
            user_inventory = new UserInventory();
            personage = new PersonageModel(personage_resource_manager.GetResource("Human"));
            FillUserInventory();
            InitPersonage();
        }

        private void FillUserInventory()
        {
            user_inventory.Increase("Ax", 5);
            //user_inventory.Increase("Blade", 5);
            user_inventory.Increase("Bow", 1);
            //user_inventory.Increase("Shield", 5);
            user_inventory.Increase("Cap_1", 1);
            user_inventory.Increase("Helmet_1", 1);
            user_inventory.Increase("Shirt_1", 1);
            user_inventory.Increase("Shirt_2", 1);
            user_inventory.Increase("Jacket_1", 2);
            user_inventory.Increase("Leggings_1", 5);
            user_inventory.Increase("Leggings_1", 5);

            user_inventory.Increase("Apple_1", 51);
            user_inventory.Increase("Banana_1", 25);
            user_inventory.Increase("Beer_1", 47);
            user_inventory.Increase("Bread_1", 12);
            user_inventory.Increase("Canned_food_1", 2);
            user_inventory.Increase("Cheese_1", 98);
            user_inventory.Increase("Fish_1", 6);
            user_inventory.Increase("Kolbasa_1", 44);
            user_inventory.Increase("Peach_1", 109);
            user_inventory.Increase("Pear_1", 38);
            user_inventory.Increase("Pear_2", 82);
        }

        private void InitPersonage()
        {
            var ax = asset_resource_manager.GetResource("Ax");
            var blade = asset_resource_manager.GetResource("Blade");
            var shield = asset_resource_manager.GetResource("Shield");
            var bow = asset_resource_manager.GetResource("Bow");

            personage.PutOnItem(SlotType.WeaponRightArm, (DressableAsset)blade);
            personage.PutOnItem(SlotType.WeaponLeftArm, (DressableAsset)shield);
        }

        void Start()
        {
            personage.Instantiate();
        }

        void Update()
        {
            
        }
    }
}