using AutoMapper;
using Microsoft.AspNetCore.Http;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Project;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.Enumerates;
using PrintManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IBaseRepository<Project> _baseProjectRepository;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<Customer> _baseCustomerRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        public ProjectService(IBaseRepository<Project> baseProjectRepository, IBaseRepository<User> baseUserRepository, IBaseRepository<Customer> baseCustomerRepository,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _baseProjectRepository = baseProjectRepository;
            _baseUserRepository = baseUserRepository;
            _baseCustomerRepository = baseCustomerRepository;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ResponeObject<Project>> Create(CreateProject_Request project)
        {
            var currentUser = _contextAccessor.HttpContext.User;
            var currentTeamId = currentUser.FindFirst("TeamId")?.Value;
            try
            {
                if (!currentUser.Identity.IsAuthenticated)
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "Unauthorize",
                        Data = null
                    };
                }
                if (!currentUser.IsInRole(ConstantEnums.Role.Employee.ToString()))
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "You are not have Employee Permission",
                        Data = null
                    };
                }
                if (currentTeamId == null)
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "You are not in Sale Team",
                        Data = null
                    };
                }
                if (project == null)
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Project must be not null",
                        Data = null
                    };
                }
                var employee = _baseUserRepository.GetAsync(x => x.Id == project.EmployeeId);
                if (employee == null)
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = $"Not Found Employee with Id = {project.EmployeeId}",
                        Data = null
                    };
                }
                var customer = _baseCustomerRepository.GetAsync(x => x.Id == project.CustomerId);
                if (customer == null)
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = $"Not Found Customer with Id = {project.CustomerId}",
                        Data = null
                    };
                }
                if (!currentTeamId.Equals("1"))
                {
                    return new ResponeObject<Project>
                    {
                        Status = StatusCodes.Status406NotAcceptable,
                        Message = $"Not accepted, only employees can create",
                        Data = null
                    };
                }
                await _baseProjectRepository.CreateAsync(_mapper.Map<Project>(project));
                return new ResponeObject<Project>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Create Project Success",
                    Data = _mapper.Map<Project>(project)
                };
                
            }
            catch (Exception ex)
            {
                return new ResponeObject<Project>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Error: " + ex.Message,
                    Data = null
                };
            }

        }
    }
}
