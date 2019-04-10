using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CET322_HW5.Data;
using CET322_HW5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CET322_HW5.Controllers
{
	[Authorize]
	public class DepartmentsController : Controller
    {
        SchoolContext _context;

        #region Ctor
        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods
        public IList<Student> GetStudentsByDepartmentId(int id)
        {
            IList<Student> students;
            students = _context.Students.Where(x => x.DepartmentId == id).ToList();
            return students;

        }
		#endregion
		[AllowAnonymous]
		public IActionResult Detail(int id)
        {
            var department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
            if (department != null)
            {
                DepartmentModel departmentModel = new DepartmentModel();
                departmentModel.Name = department.Name;
                departmentModel.Students = GetStudentsByDepartmentId(id);
				departmentModel.Id = department.Id;
                return View(departmentModel);

            }
            else
                return NotFound();

        }
		[AllowAnonymous]
		#region List
		public IActionResult DepartmentList()
        {
            var departments = _context.Departments.ToList();
            var departmentsModel = new List<DepartmentModel>();
            foreach (var item in departments)
            {
                var model = new DepartmentModel
                {
                    Id = item.Id,
                    Name = item.Name
                };

                departmentsModel.Add(model);
            }

            return View(departmentsModel);
        }
        #endregion

        #region Create
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                var existingDepartment = _context.Departments.Where(x => x.Name == model.Name).FirstOrDefault();
                if (existingDepartment == null)
                {
                    Department newdepartment = new Department
                    {
                        Name = model.Name
                    };
                    _context.Departments.Add(newdepartment);
                    _context.SaveChanges();
                }
                return RedirectToAction("DepartmentList");
            }
            else
                return View(model);
        }
        #endregion

        #region Edit
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            var department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
            if (department == null)
            {
                return NotFound();
            }
            var model = new DepartmentModel
            {
                Id = department.Id,
                Name = department.Name

            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, DepartmentModel model)
        {
            var department = _context.Departments.Where(x => x.Id == model.Id).FirstOrDefault();

            if (!id.HasValue)
            {
                return BadRequest();
            }

            if (department == null)
            {
                return NotFound();
            }

            if (id != department.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid && department != null)
            {
                department.Name = model.Name;
                _context.Departments.Update(department);
                _context.SaveChanges();
                return RedirectToAction("Edit");
            }
            else
            {
                return View(department);

            }


        }
        #endregion

        #region Delete
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();

                return RedirectToAction("DepartmentList");
            }
            return NotFound();
        }

        #endregion
    }
}