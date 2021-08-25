using AuFe.Models.Interface;
using LiteDB;
using System.Collections.Generic;

namespace AuFe.Models;

public class UserContext : LiteDatabase, IUsers
{
    readonly ILiteCollection<User> users;
    #region Ctor
    /// <summary>
    /// Starts database using a connection string for file system database
    /// </summary>
    public UserContext(string connectionString, BsonMapper mapper = null) : this(new ConnectionString(connectionString), mapper) { }
    /// <summary>
    /// Starts database using a connection string for file system database
    /// </summary>
    public UserContext(ConnectionString connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
    {
        users = this.GetCollection<User>("users");
    }

    #endregion

    public User this[long id] { get => users.FindById(id); }

    public IEnumerable<User> Users => users.FindAll();

    public void Add(User user) => users.Insert(user);
    public void Update(User user) => users.Upsert(user);
    public void Remove(User user) => users.Delete(user.ID);

    public void SaveChanges() { }
}
