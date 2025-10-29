using System.ComponentModel.DataAnnotations;

namespace RegistryApp.Domain.Enums {
    public enum Role {
        [Display(Name = "Пользователь")]
        User = 0,
        [Display(Name = "Админ")]
        Admin = 1,
    }

}
