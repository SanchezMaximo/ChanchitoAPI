using Application.Interfaces;
using Application.DTOs;
using Domain.Interfaces;
using Domain.Entities;


namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly  IUserRepository userRepository;
        private readonly IAccountRepository accountRepository;
        public AccountService(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
        }
        
        

        public AccountResponse Create(CreateAccountRequest request)
        {
            var account = new Account(request.Name, request.Balance, request.Type, request.Currency, request.CustomerId);
            if (userRepository.GetCustomerById(request.CustomerId) is null)
                throw new KeyNotFoundException($"There is no customer with id {request.CustomerId}.");
            accountRepository.Add(account);
            accountRepository.SaveChanges();    
            
            return AccountResponse.From(account);
        }

        public AccountResponse? GetById(Guid id)
        {
            if(id == Guid.Empty)
                throw new ArgumentException("El id de la cuenta es obligatorio.", nameof(id));
            var account = accountRepository.GetById(id);
            if(account is null)
                throw new InvalidOperationException($"No existe una cuenta con el id {id}.");
        
            return AccountResponse.From(account);
        }

        public IReadOnlyList<AccountResponse> GetAll() =>
            accountRepository.GetAll().Select(AccountResponse.From).ToList();

        public AccountResponse DeleteById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El id de la cuenta es obligatorio.", nameof(id));
            var account = accountRepository.GetById(id);
            if (account is null)
                throw new InvalidOperationException($"No existe una cuenta con el id {id}.");
            accountRepository.Remove(account);
            accountRepository.SaveChanges();
            return AccountResponse.From(account);
        }   

        public AccountResponse Update(Guid id, UpdateAccountRequest request)
        {
            var account = accountRepository.GetById(id)
                ?? throw new KeyNotFoundException($"There is no account with id {id}.");
            
            account.Rename(request.Name);
            accountRepository.SaveChanges();
            return AccountResponse.From(account);
            

        }
    }
}

