using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ContactRepository(ApplicationDbContext context) : IContactRepository
{
    public async Task<IEnumerable<Contact>> GetAllAsync() =>
        await context.Contacts
            .Include(c => c.Category)
            .Include(c => c.Subcategory)
            .ToListAsync();

    public async Task<Contact> AddAsync(Contact contact)
    {
        await context.Contacts.AddAsync(contact);
        await context.SaveChangesAsync();
        return contact;
    }

    public async Task UpdateAsync(Contact contact)
    {
        context.Contacts.Update(contact);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var contact = await context.Contacts.FindAsync(id);
        if (contact != null)
        {
            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
        }
    }
}
