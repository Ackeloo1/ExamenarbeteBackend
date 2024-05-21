using MediatR;
using TestMediatR1.Game.Models;
using TestMediatR1.Item.Queries;
using TestMediatR1.Player.Models;

namespace TestMediatR1.Game.Services
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
            return _games.Any(game => game.Players!.Any(player => player.Name == name));
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

            //Player startvärden
            foreach (var player in players)
            {
                var rdn = new Random();
                var numOfItems = rdn.Next(0, 3);
                game.Players.Add(new PlayerProps
                {
                    Health = health,
                    ItemIds = await GenerateItemIds(numOfItems, itemIds),
                    Name = player.Username
                });
            }

            _games.Add(game);

            return (GameStartResponse)game;
        }

        //Generera item id's när applikationen startas,
        //sedan ska items hämtas ur databas.
        public async Task<int[]> GenerateItemIds(int amount, int[] itemIds)
        {
            Random rdn = new();

            int[] result = new int[amount];

            if (itemIds.Length == 0)
                return result;

            for (int i = 0; i < amount; i++)
                result[i] = itemIds[rdn.Next(itemIds.Length)];

            return result;
        }

        //Generera random mängd kulor och blanka, sammanlagt max från 3 till 6 totalt
        public BulletsModel GenerateBullets()
        {
            Random random = new Random();

            // Generera random total från 3 till 6
            int totalCount = random.Next(3, 7);

            // generera random bullet count inom totalCount range
            int bulletCount = random.Next(0, totalCount + 1);

            // räkna ut antalet blanka
            int blankCount = totalCount - bulletCount;

            BulletsModel result = new BulletsModel
            {
                Bullets = bulletCount,
                Blanks = blankCount
            };

            return result;
        }
    }
}
