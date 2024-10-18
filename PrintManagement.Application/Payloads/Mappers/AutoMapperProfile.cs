using AutoMapper;
using PrintManagement.Application.Payloads.Requests.Project;
using PrintManagement.Application.Payloads.Requests.Teams;
using PrintManagement.Application.Payloads.Requests.Users;
using PrintManagement.Application.Payloads.Respones.Users;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<Register, User>();
            CreateMap<User, UserInfor>();
            CreateMap<CreateTeam_Request, Team>();
            CreateMap<UpdateTeam_Request, Team>();
            CreateMap<CreateProject_Request, Project>();
        }
    }
}
