using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class AppendixViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Описание")]
        public string Name { get; set; }
        [Display(Name = "Название файла")]
        public IFormFile FileName { get; set; }
    }

}
