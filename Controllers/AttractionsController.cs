using Microsoft.AspNetCore.Mvc;

public class AttractionsController : Controller
{
    private readonly TicketmasterService _ticketmasterService;

    public AttractionsController(TicketmasterService ticketmasterService)
    {
        _ticketmasterService = ticketmasterService;
    }

    public async Task<IActionResult> Index(string searchTerm = "Phish")
    {
        var attractions = await _ticketmasterService.SearchAttractionsAsync(searchTerm);
        ViewBag.SearchTerm = searchTerm; // Store the search term in ViewBag
        return View(attractions);
    }

    public async Task<IActionResult> Details(string id)
    {
        var (attraction, events) = await _ticketmasterService.GetAttractionDetailsAsync(id);
        ViewBag.Events = events; // Pass the events to the view
        return View(attraction);
    }

}
