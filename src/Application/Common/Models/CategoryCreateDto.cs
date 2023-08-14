using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class CategoryCreateDto
{
    [Required()]
    public string? Name { get; set; }
    [Required()]
    public string? Description { get; set; }
}
