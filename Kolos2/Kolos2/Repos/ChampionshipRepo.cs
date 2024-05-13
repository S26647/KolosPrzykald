using System.Data.Common;
using System.Data.SqlClient;
using Kolos2.DTO;
using Kolos2.Models;

namespace Kolos2.Repos;

public class ChampionshipRepo : IChampionshipRepo
{
    private IConfiguration _configuration;

    public ChampionshipRepo(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Championship> GetChampionshipByIdAsync(int id)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Championship " +
                          "WHERE IdChampionship = @IdChampionShip";
        cmd.Parameters.AddWithValue("@IdChampionShip", id);

        using (var dr = await cmd.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                return new Championship
                {
                    IdChampionship = Int32.Parse(dr["IdChampionship"].ToString()),
                    OfficialName = dr["OfficialName"].ToString(),
                    Year = Int32.Parse(dr["Year"].ToString())
                };
            }
        }

        return null;
    }

    public async Task<IEnumerable<TeamWithScoreDTO>> GetTeamsWithScoresList(int id)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT * FROM Team " +
                          "JOIN Championship_Team ON Team.IdTeam = Championship_Team.IdTeam " +
                          "WHERE Championship_Team.IdChampionship = @IdChampionship";
        cmd.Parameters.AddWithValue("@IdChampionShip", id);

        var teamsList = new List<TeamWithScoreDTO>();

        using (var dr = await cmd.ExecuteReaderAsync())
        {
            while (await dr.ReadAsync())
            {
                float score = await GetScoreForTeam(Int32.Parse(dr["IdTeam"].ToString()));
                int idTeam = Int32.Parse(dr["IdTeam"].ToString());
                string teamName = dr["TeamName"].ToString();

                teamsList.Add(new TeamWithScoreDTO
                (
                    idTeam,
                    teamName,
                    score
                ));
            }
        }

        return teamsList;
    }

    public async Task<float> GetScoreForTeam(int idTeam)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT score FROM Championship_team " +
                          "WHERE IdTeam = @IdTeam";
        cmd.Parameters.AddWithValue("@IdTeam", idTeam);

        var dr = await cmd.ExecuteReaderAsync();
        await dr.ReadAsync();
        float score = float.Parse(dr["Score"].ToString());
        await dr.CloseAsync();

        return score;
    }

    public async Task<int> AssignPlayerToTeam(int IdPlayer, int IdTeam)
    {
        using SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;

        DbTransaction transaction = await con.BeginTransactionAsync();
        cmd.Transaction = (SqlTransaction)transaction;

        try
        {
            cmd.CommandText = "INSERT INTO Player_Team VALUES (@IdPlayer,@IdTeam,100,null); " +
                              "SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@IdPlayer", IdPlayer);
            cmd.Parameters.AddWithValue("@IdTeam", IdTeam);
            
            await transaction.CommitAsync();
            
            int insertedRecordId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return insertedRecordId;
        }
        catch (SqlException exception)
        {
            await transaction.RollbackAsync();
            throw exception;
        }
    }
    
    public async Task<bool> CheckIfAgeCorrect(int idPlayer, int idTeam)
    {
        using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();

        using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "SELECT dateOfBirth FROM Player WHERE IdPlayer = @IdPlayer";
        cmd.Parameters.AddWithValue("@IdPlayer", idPlayer);
        
        String dateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        var dr = await cmd.ExecuteReaderAsync();
        await dr.ReadAsync();
        DateTime dateOfBirth = Convert.ToDateTime(dr["DateOfBirth"].ToString());
        int year = dateOfBirth.Year;
        DateTime now = DateTime.Now;
        int yearnow = now.Year;
        int age = year - yearnow;
        await dr.CloseAsync();
        
        using var cmdMax = new SqlCommand();
        cmdMax.Connection = con;
        cmdMax.CommandText = "SELECT MaxAge FROM Team WHERE IdTeam = @IdTeam";
        cmdMax.Parameters.AddWithValue("@IdTeam", idTeam);
        
        var drMax = await cmd.ExecuteReaderAsync();
        await drMax.ReadAsync();
        int maxAge = Int32.Parse(drMax["MaxAge"].ToString());
        await drMax.CloseAsync();

        if (maxAge < age)
        {
            return false;
        }

        return true;
    }
}
