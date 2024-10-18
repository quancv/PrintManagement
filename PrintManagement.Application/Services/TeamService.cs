using Microsoft.AspNetCore.Http;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.Enumerates;
using PrintManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly IBaseRepository<Team> _baseTeamRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public TeamService(IBaseRepository<Team> baseTeamRepository,IBaseRepository<User> baseUserRepository , IHttpContextAccessor contextAccessor)
        {
            _baseTeamRepository = baseTeamRepository;
            _baseUserRepository = baseUserRepository;
            _contextAccessor = contextAccessor; 
        }

        public async Task<string> AddUserToTeam(int userId, int teamId)
        {
            var user = await _baseUserRepository.GetAsync(u => u.Id == userId);
            if (user == null)
            {
                return "User is not exist";
            }
            if(user.TeamId != null)
            {
                return "User is in another team";
            }
            var team = await _baseTeamRepository.GetAsync(t => t.Id == teamId);
            if (team == null)
            {
                return "Team is not exist";
            }
            user.TeamId = teamId;
            await _baseUserRepository.UpdateAsync(user);
            team.NumberOfMember++;
            await _baseTeamRepository.UpdateAsync(team);
            return $"Add User With Id = {userId} To Team With Id = {teamId} Success";
        }

        public async Task<string> ChangeManager(int teamId, int newMangerId)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            try
            {
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return "You are Unauthorization";
                }
                if (!currentUser.IsInRole(ConstantEnums.Role.Admin.ToString()))
                {
                    return "You are not have Admin Permission";
                }
                var team = await _baseTeamRepository.GetAsync(x => x.Id == teamId);
                if (team == null)
                {
                    return $"Does not exist team with Id = {teamId}";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == newMangerId);
                if (user == null)
                {
                    return $"Does not exist manager with Id = {newMangerId}";
                }
                team.ManagerId = newMangerId;
                await _baseTeamRepository.UpdateAsync(team);
                user.TeamId = teamId;
                await _baseUserRepository.UpdateAsync(user);
                return $"Change Manager Success, Team {teamId} with new manager Id is {newMangerId}";

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public async Task<string> ChangeTeam(int newTeamId, int employeeId)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            try
            {
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return "You are Unauthorization";
                }
                if (!currentUser.IsInRole(ConstantEnums.Role.Admin.ToString()))
                {
                    return "You are not have Admin Permission";
                }
                var team = await _baseTeamRepository.GetAsync(x => x.Id == newTeamId);
                if (team == null)
                {
                    return $"Does not exist team with Id = {newTeamId}";
                }
                var user = await _baseUserRepository.GetAsync(x => x.Id == employeeId);
                if (user == null)
                {
                    return $"Does not exist employee with Id = {employeeId}";
                }
                user.TeamId = newTeamId;
                await _baseUserRepository.UpdateAsync(user);
                return $"Change Team Success, Employee with Id = {employeeId} with new team Id is {newTeamId}";

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public async Task<int> CountAsync(Expression<Func<Team, bool>> expression = null) 
        {
            return await _baseTeamRepository.CountAsync(expression);
        }

        public virtual async Task<Team> CreateAsync(Team entity)
        {
            return await _baseTeamRepository.CreateAsync(entity); ;
        }

        public async Task<string> DeleteAsync(int Id)
        {
            await _baseTeamRepository.DeleteAsync(Id);
            var lstUser = await _baseUserRepository.GetAllAsync(x => x.TeamId == Id);
            if(lstUser.Count() > 0)
            {
                foreach (var user in lstUser)
                {
                    user.TeamId = null;
                    await _baseUserRepository.UpdateAsync(user);
                }
            }
            
            return $"Delete Success Team with Id = {Id}";
        }

        public async Task<IQueryable<Team>> GetAllAsync(Expression<Func<Team, bool>> expression = null)
        {
            return await _baseTeamRepository.GetAllAsync(expression);
        }

        public async Task<Team> GetAsync(Expression<Func<Team, bool>> expression)
        {
            return await _baseTeamRepository.GetAsync(expression);
        }

        public virtual async Task<Team> UpdateAsync(Team entity)
        {
            var manager = _baseUserRepository.GetAsync(x => x.Id == entity.ManagerId);
            if (manager == null)
            {
                return null;
            }
            await _baseTeamRepository.UpdateAsync(entity);
            return entity;
        }
    }
}
