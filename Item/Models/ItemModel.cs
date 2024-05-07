using TestMediatR1.Player.Models;

namespace TestMediatR1.Item.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Effect { get; set; }
    }

    public class PlayerItemsModel
    {
        public List<ItemModel> Player1Items { get; set; }
        public List<ItemModel> Player2Items { get; set; }

        public PlayerItemsModel()
        {
            Player1Items = new List<ItemModel>();
            Player2Items = new List<ItemModel>();
        }
    }
}
