using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.IRepositories;
using PrintManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static PrintManagement.Domain.Enumerates.ConstantEnums;
namespace PrintManagement.Infrastructure.Repositories
{
    public class TeamRepository : BaseRepository<Team>
    {
        public TeamRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<Team> CreateAsync(Team team)
        {
            try
            {
                var manager = await _context.Users.FindAsync(team.ManagerId);

                if (manager == null)
                {
                    throw new Exception("Manager Is Not Exist");
                }
                if (manager.UserStatus == UserStatusEnum.UnActivated)
                {
                    throw new Exception("Manager Is UnActivated");
                }
                _context.Teams.Add(team);
                await _context.CommitChangeAsync();
                return team;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }

        public override async Task<Team> UpdateAsync(Team entity)
        {
            try
            {
                var manager = await _context.Users.FindAsync(entity.ManagerId);
                if (manager == null)
                {
                    throw new Exception("Manager Is Not Exist");
                }
                if (manager.UserStatus == UserStatusEnum.UnActivated)
                {
                    throw new Exception("Manager Is UnActivated");
                }
                _context.Teams.Update(entity);
                await _context.CommitChangeAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
        }


    }
}
