using System.ComponentModel.DataAnnotations;

namespace EmailConfirmed.Models.Client
{
    public class ClientApp
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Укажите ФИО!")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Неверная дата рождения!")]
        [Display(Name = "День рождения")]
        [DataType(DataType.Date, ErrorMessage = "Неверный формат")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Адрес почты неверный!")]
        [EmailAddress(ErrorMessage = "Проверьте адрес электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пол!")]
        [StringLength(3, ErrorMessage = "Укажите МУЖ или ЖЕН")]
        [Display(Name = "Пол")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Неверный номер телефона")]
        [Phone(ErrorMessage = "Проверьте правильность ввода номера телефона")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Ошибка типа кредита")]
        [Display(Name = "Тип кредита")]
        public ClientType Type { get; set; }

        [Required]
        [Display(Name = "Сумма кредита")]
        [Range(1000, 10000, ErrorMessage = "Сумма кредита должна составлять от 1000 до 10000 рублей!")]
        public decimal LoanAmount { get; set; }

        [Required]
        [Display(Name = "Срок кредита")]
        [Range(1, 60, ErrorMessage = "Срок кредита должен составлять от 1 до 60 месяцев!")]
        public int LoanTerm { get; set; }

        [Required]
        [Display(Name = "Ежемесячный доход")]
        [Range(1000, 10000, ErrorMessage = "Ежемесячный доход должен составлять от 1000 рублей!")]
        public decimal MonthSum { get; set; }
    }
}
