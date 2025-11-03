using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ContactRepository(ApplicationDbContext context) : IContactRepository
{
    public async Task<IEnumerable<Contact>> GetAllAsync() =>
        await context.Contacts
            .ToListAsync();

    public async Task<Contact> AddAsync(Contact contact)
    {
        // Check if email already exists
        var existingContact = await context.Contacts
            .FirstOrDefaultAsync(c => c.Email == contact.Email);
        
        if (existingContact != null)
            throw new InvalidOperationException("Contact with this email already exists");

        await context.Contacts.AddAsync(contact);
        await context.SaveChangesAsync();
        return contact;
    }

    public async Task UpdateAsync(Contact contact)
    {
        // Check if email already exists for a different contact
        var existingContact = await context.Contacts
            .FirstOrDefaultAsync(c => c.Email == contact.Email && c.Id != contact.Id);
        
        if (existingContact != null)
            throw new InvalidOperationException("Contact with this email already exists");

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
