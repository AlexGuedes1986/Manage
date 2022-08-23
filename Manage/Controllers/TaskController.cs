using AutoMapper;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TaskController : Controller
    {
        public ITaskService _taskService { get; set; }
        private readonly IMapper _mapper;

        public TaskController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Tasks_Read([DataSourceRequest] DataSourceRequest request)
        {
            var allTasks = _taskService.GetAll();
            List<TaskBioTechVM> taskBiotechVMs = new List<TaskBioTechVM>();
            foreach (var task in allTasks)
            {
                var taskBioTechVM = _mapper.Map<TaskBioTechVM>(task);
                taskBioTechVM.FormattedTaskNumber = $"{task.TaskCodeParent.ToString("00")}-{task.TaskCodeSub.ToString("00")}";
                taskBiotechVMs.Add(taskBioTechVM);
            }
            var sortedTaskBioTechVMs = taskBiotechVMs.OrderBy(t => t.TaskCodeParent).ThenBy(t => t.TaskCodeSub);
            var obj = sortedTaskBioTechVMs.ToDataSourceResult(request);
            return Json(obj);
        }

        public IActionResult Create()
        {
            var allTaskCategories = _taskService.GetAll().Select(t => t.Category).Distinct();
            TaskBioTechVM taskBiotechVM = new TaskBioTechVM();
            taskBiotechVM.CategoriesList = allTaskCategories;
            return View(taskBiotechVM);
        }

        [HttpPost]
        public IActionResult Create(TaskBioTechVM model)
        {
            var allTasks = _taskService.GetAll();
            if (allTasks.Any(t => String.Equals(t.TaskCodeParent.ToString("00"), model.TaskCodeParent.ToString("00"))))
            {
                var sameTaskCodeParentTasks = allTasks.Where(t => String.Equals(t.TaskCodeParent.ToString("00"), model.TaskCodeParent.ToString("00")));
                foreach (var sameTaskCP in sameTaskCodeParentTasks)
                {
                    if (String.Equals(sameTaskCP.TaskCodeSub.ToString("00"), model.TaskCodeSub.ToString("00")))
                    {
                        ModelState.AddModelError("TaskCodeParent", "There's a similar combination of Task Code Parent and Task Code Sub registered for an existing task." +
                  " Please select a different combination");
                        break;
                    }
                }
            }
            if (ModelState.IsValid)
            {
                TaskBioTech taskBioTech = _mapper.Map<TaskBioTech>(model);
                _taskService.Create(taskBioTech);
                return RedirectToAction("Index");
            }
            //Something was bad, let's stay within the same view...
            model.CategoriesList = _taskService.GetAll().Select(t => t.Category).Distinct();
            return View(model);
        }

        public IActionResult EditTask(int id)
        {
            var task = _taskService.GetTaskById(id);
            TaskBioTechVM taskBioTechVM = _mapper.Map<TaskBioTechVM>(task);
            taskBioTechVM.CategoriesList = _taskService.GetAll().Select(t => t.Category).Distinct();
            return View(taskBioTechVM);
        }

        [HttpPost]
        public IActionResult EditTask(TaskBioTechVM model)
        {
            var allTasks = _taskService.GetAll();
            if (allTasks.Any(t => String.Equals(t.TaskCodeParent.ToString("00"), model.TaskCodeParent.ToString("00"))))         
            {
                var sameTaskCodeParentTasks = allTasks.Where(t => String.Equals(t.TaskCodeParent.ToString("00"), model.TaskCodeParent.ToString("00")));
                foreach (var sameTaskCP in sameTaskCodeParentTasks)
                {
                    if (String.Equals(sameTaskCP.TaskCodeSub.ToString("00"), model.TaskCodeSub.ToString("00")))
                    {
                        if (sameTaskCP.Id != model.Id)
                        {
                            ModelState.AddModelError("TaskCodeParent", "There's a similar combination of Task Code Parent and Task Code Sub registered for an existing task." +
               "Please select a different combination");
                            break;
                        }                     
                    }
                }
            }
            if (ModelState.IsValid)
            {
                TaskBioTech taskBioTech = _mapper.Map<TaskBioTech>(model);
                _taskService.UpdateTask(taskBioTech);
                return RedirectToAction("Index");
            }

            //Something was bad, let's stay within the same view...
            model.CategoriesList = _taskService.GetAll().Select(t => t.Category).Distinct();
            return View(model);
        }

        [AcceptVerbs("Post")]
        public IActionResult Task_Destroy([DataSourceRequest] DataSourceRequest request, TaskBioTech task)
        {
            if (task != null)
            {
                _taskService.Destroy(task);
            }
            return Json(new[] { task }.ToDataSourceResult(request, null));
        }
    }
}
