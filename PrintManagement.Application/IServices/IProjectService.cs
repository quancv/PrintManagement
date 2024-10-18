using PrintManagement.Application.Payloads.Requests.Project;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.IServices
{
    public interface IProjectService
    {
        Task<ResponeObject<Project>> Create(CreateProject_Request project);
    }
}
