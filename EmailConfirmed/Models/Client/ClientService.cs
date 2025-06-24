using EmailConfirmed.Data;
using Microsoft.EntityFrameworkCore;

namespace EmailConfirmed.Models.Client
{
    public class ClientService
    {
        private readonly ApplicationContext _context;

        public ClientService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddClients(List<ClientApp> clients)
        {
            if(clients == null || clients.Count == 0)
            {
                throw new ArgumentException("Список клиентов не может быть пустым.", nameof(clients));
            }

            await _context.Clients.AddRangeAsync(clients);
            await _context.SaveChangesAsync();
        }

        public async Task SeedData()
        {
            var clients = new List<ClientApp>
            {
                new ClientApp
                {
                    FullName = "Петров Иван Иванович",
                    BirthDate = new DateTime(1989, 02, 10),
                    PhoneNumber = "+7(913)245-68-90",
                    Email = "petrov_i@test.ru",
                    Gender = "Муж",
                    Type = ClientType.Потребительский,
                    LoanAmount = 2000,
                    LoanTerm = 2,
                    MonthSum = 3000
                },
                new ClientApp
                {
                    FullName = "Иванов Петр Петрович",
                    BirthDate = new DateTime(1993, 08, 21),
                    PhoneNumber = "+7(950)123-23-16",
                    Email = "ivanov_pp@gtail.com",
                    Gender = "Муж",
                    Type = ClientType.Ипотечный,
                    LoanAmount = 8000,
                    LoanTerm = 8,
                    MonthSum = 9000
                },
                new ClientApp
                {
                    FullName = "Сидоров Сидр Сидорович",
                    BirthDate = new DateTime(2002, 09, 15),
                    PhoneNumber = "+7(962)786-02-78",
                    Email = "sidr_forever@pivo.da",
                    Gender = "Муж",
                    Type = ClientType.Автокредитование,
                    LoanAmount = 5000,
                    LoanTerm = 6,
                    MonthSum = 5000
                }
            };

            await AddClients(clients);
        }

        private async Task<bool> HasClientsAsync()
        {
            return await _context.Clients.AnyAsync();
        }

        public async Task<(List<ClientApp> clientApps, int totalPages)> GetClients(string searchTerm, string selectedClients, int pageNumber, int pageSize)
        {
            bool hasClients = await HasClientsAsync();
            if (hasClients == false)
                await SeedData();
            
            var filterClients = _context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                filterClients = filterClients.Where(p => p.FullName.ToLower().Contains(searchTerm.ToLower()));

            if (!string.IsNullOrEmpty(selectedClients))
            {
                if (Enum.TryParse(selectedClients, out ClientType type))
                    filterClients = filterClients.Where(p => p.Type == type);
            }

            if (!await filterClients.AnyAsync())
            {
                return (new List<ClientApp>(), 0);
            }

            int totalCount = await filterClients.CountAsync();
            var clients = await filterClients.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return await Task.FromResult((filterClients.ToList(), totalCount));
            
        }

        public async Task<ClientApp?> GetClientById(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task CreateClientAsync(ClientApp app)
        {
            _context.Clients.Add(app);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClient(ClientApp app)
        {
            var existingClient = await GetClientById(app.Id);
            if(existingClient != null)
            {
                existingClient.FullName = app.FullName;
                existingClient.Email = app.Email;
                existingClient.BirthDate = app.BirthDate;
                existingClient.Gender = app.Gender;
                existingClient.PhoneNumber = app.PhoneNumber;
                existingClient.Type = app.Type;
                existingClient.LoanAmount = app.LoanAmount;
                existingClient.LoanTerm = app.LoanTerm;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteClient(int Id)
        {
            var client = await GetClientById(Id);
            if(client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
        }

        /*public async Task<bool> AddPhoto(int Id, IFormFile photo)
        {
            if(photo == null || photo.Length == 0)
                throw new ArgumentException("Photo cannot be null or empty.", nameof(photo));
            var client = await GetClientById(Id);
            if(client == null)
                throw new ArgumentException("Client not found!");

            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
            client.PhotoPath = fileName;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdatePhoto(int Id, IFormFile photo)
        {
            if(photo == null || photo.Length == 0)
                throw new ArgumentException("Photo cannot be null or empty.", nameof(photo));
            var client = await GetClientById(Id);
            if(client == null)
                throw new ArgumentException("Client not found");

            if (!string.IsNullOrEmpty(client.PhotoPath))
            {
                var oldFilePath = Path.Combine("wwwroot/images", client.PhotoPath);
                if(File.Exists(oldFilePath))
                    File.Delete(oldFilePath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
            client.PhotoPath= fileName;

            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return true; 
        }
        public async Task<bool> DeletePhoto(int Id, IFormFile photo)
        {
            if(photo == null || photo.Length == 0)
                throw new ArgumentException("Photo cannot be null or empty.", nameof(photo));
            var client = await GetClientById(Id);
            if(client == null)
                throw new ArgumentException("Client not found");
            if(!string.IsNullOrEmpty(client.PhotoPath))
            {
                var filePath = Path.Combine("wwwroot/images", client.PhotoPath);
                if(File.Exists(filePath))
                    File.Delete(filePath);
            }
            client.PhotoPath = null;
            _context.Update(client);
            await _context.SaveChangesAsync();
            return true;
        }*/
    }
}
