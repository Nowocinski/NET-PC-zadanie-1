using Core.Entities;

namespace Core.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact> AddAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(Guid id);
}
