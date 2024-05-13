using Kolos2.DTO;
using Kolos2.Exceptions;
using Kolos2.Models;
using Kolos2.Repos;

namespace Kolos2.Services;

public class ChampionShipService : IChampionShipService
{
    private readonly IChampionshipRepo _championshipRepo;

    public ChampionShipService(IChampionshipRepo championshipRepo)
    {
        _championshipRepo = championshipRepo;
    }
    
    public async Task<TeamScoreResponseDTO> GetInfoForTeamInChampionships(int id)
    {
        Championship championship = await _championshipRepo.GetChampionshipByIdAsync(id);
        IEnumerable<TeamWithScoreDTO> teamsList = await _championshipRepo.GetTeamsWithScoresList(id);

        TeamScoreResponseDTO returngowno = new TeamScoreResponseDTO(championship, teamsList);
        return returngowno;
    }

    public async Task<int> AssingPlayerToTeamAsync(int idPlayer, int idTeam)
    {
        if (!await _championshipRepo.CheckIfAgeCorrect(idPlayer, idTeam))
        {
            throw new WrongAgeException();
        }
        
        int id = await _championshipRepo.AssignPlayerToTeam(idPlayer, idTeam);

        return id;
    }
}