
using System.Collections.Generic;

namespace AuFe.Models.Interface;


public interface IUsers
{
    public IEnumerable<User> Users { get; }

    public User this[long id] { get; }

    public void Add(User book);
    public void Update(User book);
    public void Remove(User book);

    public void SaveChanges();
}
