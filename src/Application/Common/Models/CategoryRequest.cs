using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class CategoryRequest
{
    // The Category's name
    public required string Name { get; set; }

    // The Category's description
    public required string Description { get; set; }
}
