using Kolos2.DTO;
using Kolos2.Models;

namespace Kolos2.Repos;

public interface IChampionshipRepo
{
    Task<Championship> GetChampionshipByIdAsync(int id);
    Task<IEnumerable<TeamWithScoreDTO>> GetTeamsWithScoresList(int id);
    Task<float> GetScoreForTeam(int idTeam);



    Task<int> AssignPlayerToTeam(int IdPlayer, int IdTeam);

    Task<bool> CheckIfAgeCorrect(int idPlayer, int idTeam);
}