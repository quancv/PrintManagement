using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using PrintManagement.Application.Constants;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Teams;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Domain.Entities;
using System.Security.Claims;

namespace PrintManagement.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route(Constant.DefaultRoute.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;
        public TeamController(ITeamService teamService, IMapper mapper)
        {
            _teamService = teamService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _teamService.GetAllAsync());
        }
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _teamService.GetAsync(x => x.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTeam_Request entity)
        {
            try
            {
                return Ok(await _teamService.CreateAsync(_mapper.Map<Team>(entity)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> AddUserToTeam(int userId, int teamId)
        {
            return Ok(await _teamService.AddUserToTeam(userId, teamId));
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateTeam_Request entity)
        {
            var team = _mapper.Map<Team>(entity);
            team.UpdateTime = DateTime.Now;
            return Ok(await _teamService.UpdateAsync(team));
        }


        [HttpPut]
        public async Task<IActionResult> ChangeManager(int teamId, int newMangerId)
        {
            return Ok(await _teamService.ChangeManager(teamId, newMangerId));
        }

        [HttpPut]
        public async Task<IActionResult> ChangeTeam(int newTeamId, int employeeId)
        {
            return Ok(await _teamService.ChangeTeam(newTeamId, employeeId));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _teamService.DeleteAsync(id));
        }
        

    }
}
