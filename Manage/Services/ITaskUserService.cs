using BioTech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public interface ITaskUserService
    {
        void Create(TaskUser taskUser);
        List<TaskUser> GetAll();
        void Update(TaskUser taskUser);
        TaskUser GetById(int id);
        TaskUser GetByTaskExtensionIdAndUserId(int taskExtensionId, string userId);
        void Remove(TaskUser taskUser);
        void HideFromTimesheet(int taskUserId);
        IEnumerable<TaskUser> GetTaskUsersByTaskExtensionId(int taskExtensionId);
    }
}
