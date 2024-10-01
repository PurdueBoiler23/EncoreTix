using Newtonsoft.Json;
using EncoreTixApp.Models;

public class TicketmasterService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public TicketmasterService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<List<Attraction>> SearchAttractionsAsync(string searchTerm)
    {
        var response = await _httpClient.GetStringAsync($"https://app.ticketmaster.com/discovery/v2/attractions.json?keyword={searchTerm}&apikey={_apiKey}");
        dynamic data = JsonConvert.DeserializeObject(response);

        List<Attraction> attractions = new List<Attraction>();

        // Check if _embedded and attractions are not null
        if (data?._embedded != null && data._embedded.attractions != null)
        {
            foreach (var item in data._embedded.attractions)
            {
                attractions.Add(new Attraction
                {
                    Name = item.name,
                    Image = item.images[0]?.url,
                    Id = item.id
                });
            }
        }

        return attractions;
    }


    public async Task<(Attraction, List<Event>)> GetAttractionDetailsAsync(string id)
    {
        var response = await _httpClient.GetStringAsync($"https://app.ticketmaster.com/discovery/v2/attractions/{id}.json?apikey={_apiKey}");
        dynamic data = JsonConvert.DeserializeObject(response);

        var attraction = new Attraction
        {
            Name = data.name,
            Image = data.images[0]?.url,
            TwitterLink = (data.externalLinks?.twitter != null && data.externalLinks.twitter.Count > 0) ? data.externalLinks.twitter[0]?.url : null,
            YoutubeLink = (data.externalLinks?.youtube != null && data.externalLinks.youtube.Count > 0) ? data.externalLinks.youtube[0]?.url : null,
            SpotifyLink = (data.externalLinks?.spotify != null && data.externalLinks.spotify.Count > 0) ? data.externalLinks.spotify[0]?.url : null,
            WebsiteLink = (data.externalLinks?.homepage != null && data.externalLinks.homepage.Count > 0) ? data.externalLinks.homepage[0]?.url : null
        };

        // Fetch upcoming events
        var events = await GetUpcomingEventsAsync(id);

        return (attraction, events);
    }

    public async Task<List<Event>> GetUpcomingEventsAsync(string attractionId)
    {
        var response = await _httpClient.GetStringAsync($"https://app.ticketmaster.com/discovery/v2/events.json?attractionId={attractionId}&apikey={_apiKey}");
        dynamic data = JsonConvert.DeserializeObject(response);

        List<Event> events = new List<Event>();

        // Check if _embedded and events are not null
        if (data?._embedded != null && data._embedded.events != null)
        {
            foreach (var item in data._embedded.events)
            {
                events.Add(new Event
                {
                    Name = item.name,
                    Image = item.images[0]?.url,
                    Venue = String.Format("{0}, {1}, {2}", item._embedded.venues[0].name, item._embedded.venues[0].city.name, item._embedded.venues[0].state.stateCode),
                    LocalDate = item.dates.start.localDate
                });
            }
        }

        return events.OrderBy(e => e.LocalDate).ToList(); // return events in chronological order
    }


}
