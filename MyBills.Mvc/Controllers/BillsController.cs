using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBills.Domain.Entities;
using MyBills.Mvc.Models;
using MyBills.Services;
using System.Security.Claims;

namespace MyBills.Mvc.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserBillService _userBillService;

        public BillsController(IUserService userService, IUserBillService userBillService)
        {
            _userService = userService;
            _userBillService = userBillService;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var userId = await _userService.GetUserId(userName);
            var userDetails = await _userService.GetUserDetailByUserId(userId);
            
            var ubViewModel = new UserBillsViewModel
            {
                UserDetails = userDetails,
                UserBillSet = await _userBillService.GetBillsByUserIdConsolidatedAsync(userId)
            };

            ViewData["UsersFirstName"] = userDetails.FirstName;

            return View(ubViewModel);
        }

        // GET: Bills/Create
        public async Task<IActionResult> Create()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var userId = await _userService.GetUserId(userName);

            var userDetails = await _userService.GetUserDetailByUserId(userId);
            ViewData["UsersFirstName"] = userDetails.FirstName;
            
            var billViewModel = new BillViewModel
            {
                Bill = new Bill(),
                RecurrenceTypeList = await _userBillService.GetRecurrenceTypes()
            };

            return View(billViewModel);
        }

        // POST: Bills/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillViewModel billViewModel)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);            
            var userId = await _userService.GetUserId(userName);
            var recModel = UserBillService.GetRecModel(billViewModel);
            var recSchedule = await _userBillService.GetRecScheduleAsync(billViewModel.RecurrenceTypeId, recModel);
            var isSuccess = await _userBillService.CreateNewUserBillAsync(userId, billViewModel.Bill, recModel, recSchedule);

            if (isSuccess) return RedirectToAction("Index");

            ViewBag.ErrorMessage = "There was an error creating your bill. Please try again.";
            billViewModel.RecurrenceTypeList = await _userBillService.GetRecurrenceTypes();
            return View(billViewModel);
        }

        // GET: Bills/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var billId = id.GetValueOrDefault();
            if (billId == 0)
            {
                return StatusCode(400);
            }
            
            var userId = await _userService.GetUserId(userName);
            var billToEdit = await _userBillService.GetUserBillByBillIdAsync(userId, billId);

            if (billToEdit == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var userDetails = await _userService.GetUserDetailByUserId(userId);
            ViewData["UsersFirstName"] = userDetails.FirstName;

            return View(billToEdit);
        }

        // POST: Bills/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Bill bill)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _userBillService.UpdateUserBillAsync(bill);

                if (isSuccess) return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = "There was an error creating the bill. Please try again.";
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<ActionResult>  Delete(int? id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var billId = id.GetValueOrDefault();
            if (billId == 0)
            {
                return StatusCode(400);
            }
            
            var userId = await _userService.GetUserId(userName);
            var billToDelete = await _userBillService.GetUserBillByBillIdAsync(userId, billId);

            if (billToDelete == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var userDetails = await _userService.GetUserDetailByUserId(userId);
            ViewData["UsersFirstName"] = userDetails.FirstName;

            return View(billToDelete);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userId = await _userService.GetUserId(userName);
            
            await _userBillService.DeleteUserBillByBillIdAsync(userId, id);

            return RedirectToAction("Index");
        }
    }
}