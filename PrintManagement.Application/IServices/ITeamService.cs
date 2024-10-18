using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.IServices
{
    public interface ITeamService : IBaseRepository<Team>
    {
        Task<string> ChangeManager(int teamId, int newMangerId);
        Task<string> ChangeTeam(int newTeamId, int employeeId);
        Task<string> AddUserToTeam(int teamId, int userId);
    }
}
