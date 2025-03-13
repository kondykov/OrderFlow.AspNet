using Microsoft.AspNetCore.Mvc;
using OrderFlow.Shared.Filters;

namespace OrderFlow.Shared.Extensions;

[ApiController]
[TypeFilter<ApiExceptionFilter>]
public class ApiController : Controller
{
}