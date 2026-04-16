using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SlotsGameEngine.DTO.Shared.DataObjects;

namespace SlotsGameEngine.DTO.Client.Responses
{
    public class PlayResponse : BaseResponse
    {
        public PlayerRound Player { get; set; } = new();
        public GameRound Game { get; set; } = new();
        public List<Jackpot> Jackpots { get; set; } = new();
        public FeatureInfo? Feature { get; set; }
    }
}
