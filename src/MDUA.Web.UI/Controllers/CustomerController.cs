using MDUA.Entities;
using MDUA.Facade.Interface;
using MDUA.Web.UI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

public class CustomerController : BaseController
{
    private readonly ICustomerFacade _customerFacade;
    private readonly IUserLoginFacade _userLoginFacade;

    public CustomerController(ICustomerFacade customerFacade, IUserLoginFacade userLoginFacade)
    {
        _customerFacade = customerFacade;
        _userLoginFacade = userLoginFacade;
    }

    // In CustomerController.cs

    [HttpGet]
    [Route("customer/all")]
    public IActionResult CustomerList()
    {
        if (!HasPermission("Customer.View")) return HandleAccessDenied();

        var userId = CurrentUserId; // Returns int?
        if (userId == null) return RedirectToAction("Login", "Account");
        
      //  if (!HasPermission("Customer.View"))
           // return RedirectToAction("AccessDenied", "Account");

        var customers = _customerFacade.GetAllCustomersForAdmin();
        return View(customers); // Returns the list to CustomerList.cshtml
    }

    [HttpGet]
    [Route("customer/get-orders-partial/{customerId}")]
    public IActionResult GetCustomerOrdersPartial(int customerId)
    {
        if (!HasPermission("Customer.Orders")) return HandleAccessDenied();

        var userId = CurrentUserId;
        if (userId == null) return Unauthorized();

        // 1. Get CompanyId securely from Claims
        var companyIdClaim = User.FindFirstValue("CompanyId");

        // Safety check and conversion must happen outside the method call
        if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
        {
            return Forbid("Missing company context.");
        }

        try
        {
            // 2. Call Facade, passing the non-nullable 'companyId'
            var orders = _customerFacade.GetCustomerOrders(customerId, companyId); // ✅ FIXED CALL
            Console.WriteLine($"DEBUG: Orders count in controller = {orders?.Count ?? -1}");

            ViewBag.CustomerId = customerId;
            return PartialView("_CustomerOrdersPartial", orders);
        }
        catch (Exception ex)
        {
            // ... error handling ...
            Console.WriteLine($"Error loading orders for Customer {customerId}: {ex.Message}");
            return StatusCode(500, new { message = $"Server failed to load orders. Error: {ex.Message}" });
        }
    }

    [HttpGet]
    [Route("customer/get-addresses-partial/{customerId}")]
    public IActionResult GetCustomerAddressesPartial(int customerId)
    {
        if (!HasPermission("Customer.Addresses")) return HandleAccessDenied();

        var userId = CurrentUserId;
        if (userId == null) return Unauthorized();

        try
        {
            var addresses = _customerFacade.GetCustomerAddresses(customerId);
            ViewBag.CustomerId = customerId;
            return PartialView("_CustomerAddressesPartial", addresses);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading addresses for Customer {customerId}: {ex.Message}");
            return StatusCode(500, new { message = $"Server failed to load addresses. Error: {ex.Message}" });
        }
    }

    // You must implement the Details action for the sidebar links to work
    [HttpGet]
    [Route("customer/details/{id}")]
    public IActionResult Details(int id)
    {
        if (!HasPermission("Customer.Details")) return HandleAccessDenied();

        var customer = _customerFacade.GetCustomerDetails(id);
        if (customer == null) return NotFound();
        return View(customer);
    }
	
	    // Inside CustomerController.cs

    [HttpGet]
    public IActionResult GetDetails(int id)
    {
        if (!HasPermission("Customer.Details")) return HandleAccessDenied();

        // Call the facade method which uses the GetCustomerById stored procedure
        Customer customer = _customerFacade.GetCustomerDetailsById(id);
        if (customer == null)
        {
            return NotFound();
        }

        // Return the full Customer entity object as JSON
        // The JavaScript client will receive this JSON object and render it.
        return Ok(customer);
    }

    [HttpGet]
    [Route("customer/get-details-partial/{id}")]
    public IActionResult GetDetailsPartial(int id)
    {
        if (!HasPermission("Customer.Details")) return HandleAccessDenied();

        if (id <= 0)
        {
            return BadRequest("Invalid Customer ID.");
        }

        try
        {
            // 1. Fetch the full customer entity using the Facade
            Customer customer = _customerFacade.GetCustomerDetailsById(id);

            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            // 2. Pass the Customer entity to the partial view for rendering
            // NOTE: The partial view will use the Customer entity as its Model.
            return PartialView("_CustomerDetailsPartial", customer);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}