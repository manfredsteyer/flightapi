using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ErrorController : ControllerBase
{

    [HttpGet]
    [HttpPost]
    [HttpPut]    
    [HttpDelete]
    [HttpPatch]
    public ActionResult Get(int code) {
        return this.StatusCode(code);
    }

}
