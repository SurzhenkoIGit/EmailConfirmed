using EmailConfirmed.Data;
using EmailConfirmed.Models.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmailConfirmed.Controllers
{
    [Authorize(Policy = "AspAdmin")]
    public class ClientsController : Controller
    {
        private readonly ClientService _service;
        private readonly ApplicationContext _context;
        public ClientsController(ClientService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string searchTerm, [FromQuery] string selectedClients, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var (clients, totalCount) = await _service.GetClients(searchTerm, selectedClients, pageNumber, pageSize);
            var viewModel = new ClientsViewModel
            {
                Clients = clients,
                PageNumb = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                SearchTerm = searchTerm,
                SelectedClients = selectedClients
            };

            GetSelectLists();
            ViewBag.PageSizeOptions = new SelectList(new List<int> { 2, 5, 10, 15, 20, 25 }, pageSize);
            return View(viewModel);  
        }

        [HttpGet]
        public IActionResult Create()
        {
            GetSelectLists();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ClientApp app)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateClientAsync(app);
                return RedirectToAction("Success", new { Id = app.Id });
            }
            return View(app);
        }

        public async Task<IActionResult> Success([FromRoute] int Id)
        {
            var client = await _service.GetClientById(Id);
            if (client == null)
                return NotFound();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(client);
            return View(client);
        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var employee = await _service.GetClientById(id);
            if (employee == null)
                return NotFound();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(employee);
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] ClientApp app)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateClient(app);
                TempData["Сообщение"] = $"Сотрудник под номером {app.Id} и с именем {app.FullName} успешно обновлен!";
                return RedirectToAction("List");
            }
            GetSelectLists();
            return View(app);
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            var client = await _service.GetClientById(Id);
            if (client == null)
                return NotFound();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(client);
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int Id)
        {
            var client = await _service.GetClientById(Id);
            if (client == null)
                return NotFound();

            await _service.DeleteClient(Id);
            TempData["Сообщение"] = $"Кредит под номером {client.Id} клиента {client.FullName} успешно удален!";
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int Id)
        {
            var client = await _service.GetClientById(Id);
            if (client == null)
                return NotFound();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView(client);
            return View(client);
        }

        private void GetSelectLists()
        {
            ViewBag.ClientTypeOptions = new SelectList(Enum.GetValues<ClientType>().Cast<ClientType>());
        }

        /*[HttpPost("Clients/{Id}/photo")]
        public async Task<IActionResult> AddPhoto(int Id, IFormFile photo)
        {
            try
            {
                await _service.AddPhoto(Id, photo);
                return Ok(new { Message = "Photo uploaded successfuly!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{Id}/photo")]
        public async Task<IActionResult> UpdatePhoto(int Id, IFormFile photo)
        {
            try
            {
                await _service.UpdatePhoto(Id, photo);
                return Ok(new { Message = "Photo updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{Id}/photo")]
        public async Task<IActionResult> DeletePhoto(int Id, IFormFile photo)
        {
            try
            {
                await _service.DeletePhoto(Id, photo);
                return Ok(new { Message = "Photo deleted successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }*/
    }
}
