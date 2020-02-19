using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBills.Domain.Entities;
using MyBills.Mvc.Models;
using MyBills.Services;

namespace MyBills.Mvc.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userService = new UserService();
            var userId = await userService.GetUserId(userName);
            var userDetails = await userService.GetUserDetailByUserId(userId);

            var userBillService = new UserBillService();
            var ubViewModel = new UserBillsViewModel
            {
                UserDetails = userDetails,
                UserBillSet = userBillService.GetBillsByUserIdConsolidated(userId)
            };

            ViewData["UsersFirstName"] = userDetails.FirstName;

            return View(ubViewModel);
        }

        // GET: Bills/Create
        public async Task<IActionResult> Create()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userService = new UserService();
            var userId = await userService.GetUserId(userName);

            var userDetails = await userService.GetUserDetailByUserId(userId);
            ViewData["UsersFirstName"] = userDetails.FirstName;

            var userBillService = new UserBillService();
            var billViewModel = new BillViewModel
            {
                Bill = new Bill(),
                RecurrenceTypeList = await userBillService.GetRecurrenceTypes()
            };

            return View(billViewModel);
        }

        // POST: Bills/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BillViewModel billViewModel)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userService = new UserService();
            var userId = await userService.GetUserId(userName);

            var userBillService = new UserBillService();
            var recModel = UserBillService.GetRecModel(billViewModel);
            var recSchedule = userBillService.GetRecSchedule(billViewModel.RecurrenceTypeId, recModel);
            var isSuccess = userBillService.CreateNewUserBill(userId, billViewModel.Bill, recModel, recSchedule);

            if (isSuccess) return RedirectToAction("Index");

            ViewBag.ErrorMessage = "There was an error creating your bill. Please try again.";
            billViewModel.RecurrenceTypeList = await userBillService.GetRecurrenceTypes();
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

            var userService = new UserService();
            var userId = await userService.GetUserId(userName);

            var userBillService = new UserBillService();
            var billToEdit = userBillService.GetUserBillByBillId(userId, billId);

            if (billToEdit == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var userDetails = await userService.GetUserDetailByUserId(userId);
            ViewData["UsersFirstName"] = userDetails.FirstName;

            return View(billToEdit);
        }

        // POST: Bills/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Bill bill)
        {
            if (ModelState.IsValid)
            {
                var userBillService = new UserBillService();
                var isSuccess = userBillService.UpdateUserBill(bill);

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

            var userService = new UserService();
            var userId = await userService.GetUserId(userName);

            var userBillService = new UserBillService();
            var billToDelete = userBillService.GetUserBillByBillId(userId, billId);

            if (billToDelete == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var userDetails = await userService.GetUserDetailByUserId(userId);
            ViewData["UsersFirstName"] = userDetails.FirstName;

            return View(billToDelete);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userService = new UserService();
            var userId = await userService.GetUserId(userName);

            var userBillService = new UserBillService();
            userBillService.DeleteUserBillByBillId(userId, id);

            return RedirectToAction("Index");
        }
    }
}