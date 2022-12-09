﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using First_Project.Areas.Student.Models;
using First_Project.Data;
using First_Project.Helpers;
using First_Project.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace First_Project.Areas.Student.Controllers
{
    [Area("Student")]
    public class StudentInfoController : Controller
    {
        private readonly IStudentService studentService;

        public StudentInfoController(IStudentService _student)
        {
            this.studentService = _student;
        }
        public async Task<IActionResult> Section()
        {
            StudentDetailsViewModel model = new StudentDetailsViewModel
            {
                sectionsInfo = await studentService.GetSection(),
            };
            return View(model);
        }
      
      
        [HttpPost]
        public async Task<IActionResult> Section([FromForm] StudentDetailsViewModel model)
        {
            var data = new Section
            {
                Id=model.sectionsId,
                NameBN = model.NameBN,
                NameEN = model.NameEN,
            };

            await studentService.SaveSection(data);     
            return RedirectToAction(nameof(Section));
        }


        public async Task<IActionResult> StudentInfo(int id)
        {
            StudentDetailsViewModel model = new StudentDetailsViewModel
            {
                sectionsInfo = await studentService.GetSection(),
                studentsInfo = await studentService.GetStudent(),
                studentsInfos = await studentService.GetStudentInfobyId(id),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> StudentInfo([FromForm] StudentDetailsViewModel model)
        {
            string imagePath = string.Empty;
            string fileName = "";
            if (model.imgUrl != null)
            {
                string message = FileSave.SaveImage(out fileName, model.imgUrl);
            }
            else
            {
                fileName = model.url;
            }

            var studentInfo = new StudentInfo
            {
                Id=model.studentId,
                Name = model.Name,
                sectionId = model.sectionId,
                Roll = model.Roll,
                Address = model.Address,
                url=fileName,
                isActive=model.isActive,
                gender=model.gender
            };
            await studentService.SaveStudent(studentInfo);
            return RedirectToAction("StudentInfoList");
        }
        public async Task<IActionResult>StudentInfoList()
        {
            var data = new StudentDetailsViewModel
            {
                studentsInfo = await studentService.GetStudent(),
            };
            return View(data);
        }


        [HttpGet]
        public async Task<IActionResult> ResultSheet()
        {
            StudentDetailsViewModel model = new StudentDetailsViewModel
            {
                sectionsInfo = await studentService.GetSection(),
                studentsInfo = await studentService.GetStudent(),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResultSheet([FromForm] StudentDetailsViewModel model)
        {
            if (model.childId != null)
            {
                for (int i = 0; i < model.childId.Length; i++)
                {
                    var result = new Resultsheet
                    {
                        Id = (int)model.childId[i],
                        Subject = model.Subject[i],
                        TotalMarks = model.TotalMarks[i],
                        Grade = model.Grade[i],
                        studentInfoId = model.studentId
                    };
                    await studentService.SaveResult(result);
                }
            }

            return RedirectToAction("ResultSheet");
        }
        public async Task<JsonResult> GetStudentDetails(int id)
        {
            var data = await studentService.GetStudentDetails(id);
            return Json(data);
        }
        public async Task<JsonResult> GetResultSheet(int id)
        {
            var data = new StudentDetailsViewModel
            {
                resultSheets = await studentService.GetResultSheet(id),
            };
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> StudentInfoActivateStatus(int Id, int status)
        {
            var data= await studentService.UpdateStudentActiveStatus(Id, status);
            return RedirectToAction("StudentInfoList");
        }

    }
}

