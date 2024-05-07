using MediatR;
using Microsoft.EntityFrameworkCore;
using TestMediatR1.Game;
using TestMediatR1.Item.Models;
using TestMediatR1.Item.Queries;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Lobby.Services
{
    public class GameService
    {
        private readonly IMediator _mediator;
        private readonly List<GameModel> _games;

        public GameService(IMediator mediator)
        {
            _mediator = mediator;
            _games = new List<GameModel>();
        }

        public async Task<bool> IsInGame(string name)
        {
            return _games.Any(game => game.Players.Any(player => player.Name == name));
        }

        public async Task<GameStartResponse> CreateGame(string gameId, List<PlayerModel> players)
        {
            GameModel game = new GameModel();
            game.GameId = gameId;
            game.Players = new List<PlayerProps>();
            game.Bullets = GenerateBullets();

            var health = Random.Shared.Next(3, 6);

            var query = new GetAllItemIdsQuery();
            var itemIds = await _mediator.Send(query);

            foreach (var player in players)
            {
                game.Players.Add(new PlayerProps
                {
                    Health = health,
                    ItemIds = await GenerateItemIds(3, itemIds),
                    Name = player.Username
                });
            }

            _games.Add(game);

            return (GameStartResponse)game;
        }

        public async Task<int[]> GenerateItemIds(int amount, int[] itemIds)
        {
            Random rdn = new();

            int[] result = new int[amount];

            for (int i = 0; i < amount; i++)
                result[i] = itemIds[rdn.Next(itemIds.Length)];

            return result;
        }

        public BulletsModel GenerateBullets()
        {
            Random random = new Random();

            int bulletCount = random.Next(0, 7);

            int blankCount = 6 - bulletCount;

            BulletsModel result = new BulletsModel
            {
                Bullets = bulletCount,
                Blanks = blankCount
            };

            return result;
        }

        //public void ProcessShoot(GameModel game, string playerName, int type, int consumable = 0)
        //{
        //    var player = game.Players.FirstOrDefault(p => p.Name == playerName);
        //    if (player == null)
        //        return;

        //    int damage = CalculateDamage(type, consumable);
        //    UpdateBullets(game.Bullets, type, consumable);
        //    UpdatePlayerHealth(player, damage);

        //    game.GameId = game.GameId;
        //}

        //private int CalculateDamage(int type, int consumable)
        //{
        //    return (type == 1 && consumable == 1) ? 2 : (type == 0 ? 0 : 1);
        //}

        //private void UpdateBullets(BulletsModel bullets, int type, int consumable)
        //{
        //    if (type == 1 && consumable == 1)
        //        bullets.Bullets -= 1;
        //    else if (type == 0)
        //        bullets.Blanks -= 1;
        //}

        //private void UpdatePlayerHealth(PlayerProps player, int damage)
        //{
        //    player.Health -= damage;
        //    if (player.Health < 0)
        //        player.Health = 0;
        //}
    }
}
