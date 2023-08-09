using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayApp.Application.Common.Models;
public class ApiResponse<T>
{
    public ApiResponse()
    {

    }
    public ApiResponse(string code, string description, T data)
    {
        Code = code;
        Description = description;
        Data = data;
    }
    public string Code { get; set; }
    public string Description { get; set; }
    public T Data { get; set; }
}
