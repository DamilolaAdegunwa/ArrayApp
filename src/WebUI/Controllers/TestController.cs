using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;
public class TestController: ApiControllerBase
{
    // GET: TestController
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }

    // GET: TestController/Details/5
    [HttpGet("[action]/{id}")]
    public IActionResult Details(int id)
    {
        return Ok();
    }

    // GET: TestController/Create
    [HttpGet("[action]")]
    public IActionResult Create()
    {
        return Ok();
    }

    // POST: TestController/Create
    [HttpPost("[action]")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return Ok();
        }
    }

    // GET: TestController/Edit/5
    [HttpGet("[action]/{id}")]
    public IActionResult Edit(int id)
    {
        return Ok();
    }

    // POST: TestController/Edit/5
    [HttpPost("[action]/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return Ok();
        }
    }

    // GET: TestController/Delete/5
    [HttpGet("[action]/{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }

    // POST: TestController/Delete/5
    [HttpPost("[action]/{id}")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return Ok();
        }
    }
}
