using Core.Entities;

namespace Core.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetAllByUserIdAsync(Guid userId);
    Task<Contact> AddAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(Guid id);
}
