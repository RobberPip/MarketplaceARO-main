using RegistryApp.Domain.Enums;

namespace RegistryApp.Domain.Models;

public class User {
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email {
        get { return valueEmail; }
        set {
            valueEmail = value;
            flagEmail = true;
        }
    }
    public string Password {
        get { return valuePassword; }
        set {
            valuePassword = value;
            flagPassword = true;
        }
    }
    public Role? Role { get; set; }

    /// <summary>
    /// Данные флаги нужны для того, чтобы мы могли присваивать null значения полям в БД
    /// </summary>
    #region flags
    private string valueEmail;
    public bool flagEmail = false;
    private string valuePassword;
    public bool flagPassword = false;
    #endregion
    public User() { }
    public User(long id, string name, string email, string password, Role _role) {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        Role = _role;
    }
}