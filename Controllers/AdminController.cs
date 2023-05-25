using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UbertweakNfcReaderWeb.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet("test")]
    public ActionResult<string> Test()
    {
        return "Hello World!";
    }
}