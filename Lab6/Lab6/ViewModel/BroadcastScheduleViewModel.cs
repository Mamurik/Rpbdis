using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab6.ViewModels
{
    public class BroadcastScheduleViewModel
    {
        [Display(Name = "Код расписания")]
        [Key]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Сотрудник обязателен.")]
        [ForeignKey("Employee")]
        [Display(Name = "Код сотрудника")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Запись обязательна.")]
        [ForeignKey("Record")]
        [Display(Name = "Код записи")]
        public int RecordId { get; set; }

        [Display(Name = "Сотрудник")]
        public string EmployeeName { get; set; } = null!;

        [Display(Name = "Название записи")]
        public string RecordTitle { get; set; } = null!;

        [Required(ErrorMessage = "Дата трансляции обязательна.")]
        [Display(Name = "Дата трансляции")]
        public DateTime BroadcastDate { get; set; }
    }
}