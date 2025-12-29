using System.Text.Json;

namespace OctoBrief.Api.Services;

public class NewsSearchService : INewsSearchService
{
  private readonly ILogger<NewsSearchService> _logger;

  // News sources organized by Country -> Category
  // Structure: Country > Category > List of sources
  private static readonly Dictionary<string, Dictionary<string, List<NewsSource>>> CountryNewsSources = new()
  {
    ["usa"] = new()
    {
      ["technology"] = [
        new("TechCrunch", "https://techcrunch.com", false, "USA"),
        new("The Verge", "https://www.theverge.com", false, "USA"),
        new("Ars Technica", "https://arstechnica.com", false, "USA"),
        new("Wired", "https://www.wired.com", false, "USA"),
        new("Engadget", "https://www.engadget.com/big-tech", false, "USA"),
      ],
      ["science"] = [
        new("Scientific American", "https://www.scientificamerican.com", false, "USA"),
        new("Science Daily", "https://www.sciencedaily.com", false, "USA"),
        new("Live Science", "https://www.livescience.com", false, "USA"),
        new("Space.com", "https://www.space.com", false, "USA"),
      ],
      ["sports"] = [
        new("ESPN", "https://www.espn.com", false, "USA"),
        new("Bleacher Report", "https://bleacherreport.com", false, "USA"),
        new("Sports Illustrated", "https://www.si.com", false, "USA"),
        new("The Athletic", "https://theathletic.com", false, "USA"),
        new("CBS Sports", "https://www.cbssports.com", false, "USA"),
      ],
      ["media"] = [
        new("Variety", "https://variety.com", false, "USA"),
        new("Hollywood Reporter", "https://www.hollywoodreporter.com", false, "USA"),
        new("Deadline", "https://deadline.com", false, "USA"),
        new("Entertainment Weekly", "https://ew.com", false, "USA"),
        new("Rolling Stone", "https://www.rollingstone.com", false, "USA"),
      ],
      ["health"] = [
        new("WebMD", "https://www.webmd.com", false, "USA"),
        new("Healthline", "https://www.healthline.com", false, "USA"),
        new("STAT News", "https://www.statnews.com", false, "USA"),
        new("Health.com", "https://www.health.com", false, "USA"),
      ],
      ["climate"] = [
        new("Inside Climate News", "https://insideclimatenews.org", false, "USA"),
        new("Grist", "https://grist.org", false, "USA"),
        new("Yale Environment 360", "https://e360.yale.edu", false, "USA"),
      ],
      ["politics"] = [
        new("Politico", "https://www.politico.com", false, "USA"),
        new("The Hill", "https://thehill.com", false, "USA"),
        new("CNN Politics", "https://www.cnn.com/politics", false, "USA"),
        new("Fox News Politics", "https://www.foxnews.com/politics", false, "USA"),
      ],
      ["crypto"] = [
        new("The Block", "https://www.theblock.co", false, "USA"),
        new("Decrypt", "https://decrypt.co", false, "USA"),
        new("Bitcoin Magazine", "https://bitcoinmagazine.com", false, "USA"),
      ],
      ["gaming"] = [
        new("IGN", "https://www.ign.com", false, "USA"),
        new("GameSpot", "https://www.gamespot.com", false, "USA"),
        new("Kotaku", "https://kotaku.com", false, "USA"),
        new("Polygon", "https://www.polygon.com", false, "USA"),
        new("PC Gamer", "https://www.pcgamer.com", false, "USA"),
      ],
    },

    ["uk"] = new()
    {
      ["technology"] = [
        new("TechRadar UK", "https://www.techradar.com", false, "UK"),
        new("BBC Technology", "https://www.bbc.com/innovation/technology", false, "UK"),
        new("Silicon UK", "https://www.silicon.co.uk", false, "UK"),
      ],
      ["science"] = [
        new("New Scientist", "https://www.newscientist.com", false, "UK"),
        new("BBC Science", "https://www.bbc.com/news/science_and_environment", false, "UK"),
        new("The Guardian Science", "https://www.theguardian.com/science", false, "UK"),
      ],
      ["sports"] = [
        new("Sky Sports", "https://www.skysports.com", false, "UK"),
        new("BBC Sport", "https://www.bbc.com/sport", false, "UK"),
        new("The Athletic UK", "https://theathletic.com/uk", false, "UK"),
        new("talkSPORT", "https://talksport.com", false, "UK"),
      ],
      ["media"] = [
        new("Digital Spy", "https://www.digitalspy.com", false, "UK"),
        new("NME", "https://www.nme.com", false, "UK"),
        new("Radio Times", "https://www.radiotimes.com", false, "UK"),
        new("The Guardian Culture", "https://www.theguardian.com/culture", false, "UK"),
      ],
      ["health"] = [
        new("The Guardian Health", "https://www.theguardian.com/society/health", false, "UK"),
        new("BBC Health", "https://www.bbc.com/news/health", false, "UK"),
      ],
      ["climate"] = [
        new("Carbon Brief", "https://www.carbonbrief.org", false, "UK"),
        new("The Guardian Environment", "https://www.theguardian.com/environment", false, "UK"),
        new("Climate Home News", "https://www.climatechangenews.com", false, "UK"),
      ],
      ["politics"] = [
        new("The Guardian Politics", "https://www.theguardian.com/politics", false, "UK"),
        new("BBC Politics", "https://www.bbc.com/news/politics", false, "UK"),
        new("Sky News Politics", "https://news.sky.com/politics", false, "UK"),
      ],
      ["crypto"] = [
        new("City A.M. Crypto", "https://www.cityam.com/crypto", false, "UK"),
      ],
      ["gaming"] = [
        new("Eurogamer", "https://www.eurogamer.net", false, "UK"),
        new("Rock Paper Shotgun", "https://www.rockpapershotgun.com", false, "UK"),
        new("GamesRadar", "https://www.gamesradar.com", false, "UK"),
        new("Nintendo Life", "https://www.nintendolife.com", false, "UK"),
      ],
    },

    ["canada"] = new()
    {
      ["technology"] = [
        new("BetaKit", "https://betakit.com", false, "Canada"),
        new("MobileSyrup", "https://mobilesyrup.com", false, "Canada"),
      ],
      ["science"] = [
        new("CBC Science", "https://www.cbc.ca/news/science", false, "Canada"),
        new("Canadian Geographic", "https://canadiangeographic.ca", false, "Canada"),
        new("Science.gc.ca", "https://science.gc.ca/site/science/en", false, "Canada"),
      ],
      ["sports"] = [
        new("TSN", "https://www.tsn.ca", false, "Canada"),
        new("Sportsnet", "https://www.sportsnet.ca", false, "Canada"),
        new("CBC Sports", "https://www.cbc.ca/sports", false, "Canada"),
      ],
      ["media"] = [
        new("ET Canada", "https://etcanada.com", false, "Canada"),
        new("CBC Arts", "https://www.cbc.ca/arts", false, "Canada"),
        new("Toronto Star Entertainment", "https://www.thestar.com/entertainment", false, "Canada"),
      ],
      ["health"] = [
        new("CBC Health", "https://www.cbc.ca/news/health", false, "Canada"),
        new("Healthing.ca", "https://www.healthing.ca", false, "Canada"),
        new("Global News Health", "https://globalnews.ca/health", false, "Canada"),
      ],
      ["climate"] = [
        new("Canada's National Observer", "https://www.nationalobserver.com", false, "Canada"),
        new("The Narwhal", "https://thenarwhal.ca", false, "Canada"),
        new("CBC Climate", "https://www.cbc.ca/news/climate", false, "Canada"),
      ],
      ["politics"] = [
        new("CBC Politics", "https://www.cbc.ca/news/politics", false, "Canada"),
        new("Global News Politics", "https://globalnews.ca/politics", false, "Canada"),
        new("The Globe and Mail Politics", "https://www.theglobeandmail.com/politics", false, "Canada"),
        new("iPolitics", "https://www.ipolitics.ca", false, "Canada"),
      ],
      ["crypto"] = [
        new("BetaKit Crypto", "https://betakit.com/tag/cryptocurrency", false, "Canada"),
      ],
      ["gaming"] = [
        new("CGMagazine", "https://www.cgmagonline.com", false, "Canada"),
        new("MobileSyrup Gaming", "https://mobilesyrup.com/category/gaming", false, "Canada"),
      ],
    },

    ["germany"] = new()
    {
      ["technology"] = [
        new("Heise Online", "https://www.heise.de", false, "Germany"),
        new("Golem.de", "https://www.golem.de", false, "Germany"),
        new("t3n", "https://t3n.de", false, "Germany"),
      ],
      ["science"] = [
        new("Spektrum.de", "https://www.spektrum.de", false, "Germany"),
        new("Scinexx", "https://www.scinexx.de", false, "Germany"),
        new("Wissenschaft.de", "https://www.wissenschaft.de", false, "Germany"),
      ],
      ["sports"] = [
        new("Kicker", "https://www.kicker.de", false, "Germany"),
        new("Sport1", "https://www.sport1.de", false, "Germany"),
        new("Sportschau", "https://www.sportschau.de", false, "Germany"),
        new("Spox", "https://www.spox.com/de", false, "Germany"),
      ],
      ["media"] = [
        new("DWDL.de", "https://www.dwdl.de", false, "Germany"),
        new("Filmstarts", "https://www.filmstarts.de", false, "Germany"),
        new("Rolling Stone DE", "https://www.rollingstone.de", false, "Germany"),
      ],
      ["health"] = [
        new("Apotheken Umschau", "https://www.apotheken-umschau.de", false, "Germany"),
        new("Netdoktor", "https://www.netdoktor.de", false, "Germany"),
        new("Gesundheit.de", "https://www.gesundheit.de", false, "Germany"),
      ],
      ["climate"] = [
        new("Klimareporter", "https://www.klimareporter.de", false, "Germany"),
      ],
      ["politics"] = [
        new("Tagesschau", "https://www.tagesschau.de", false, "Germany"),
        new("Zeit Politik", "https://www.zeit.de/politik", false, "Germany"),
        new("Spiegel", "https://www.spiegel.de", false, "Germany"),
        new("FAZ Politik", "https://www.faz.net/aktuell/politik", false, "Germany"),
      ],
      ["crypto"] = [
        new("BTC-Echo", "https://www.btc-echo.de", false, "Germany"),
        new("Cointelegraph DE", "https://de.cointelegraph.com", false, "Germany"),
        new("Blocktrainer", "https://www.blocktrainer.de", false, "Germany"),
      ],
      ["gaming"] = [
        new("GameStar", "https://www.gamestar.de", false, "Germany"),
        new("4Players", "https://www.4players.de", false, "Germany"),
        new("Spieletipps", "https://www.spieletipps.de", false, "Germany"),
      ],
    },

    ["france"] = new()
    {
      ["technology"] = [
        new("01net", "https://www.01net.com", false, "France"),
        new("Frandroid", "https://www.frandroid.com", false, "France"),
        new("Les Numériques", "https://www.lesnumeriques.com", false, "France"),
      ],
      ["science"] = [
        new("Sciences et Avenir", "https://www.sciencesetavenir.fr", false, "France"),
        new("Futura Sciences", "https://www.futura-sciences.com", false, "France"),
        new("Pour la Science", "https://www.pourlascience.fr", false, "France"),
      ],
      ["sports"] = [
        new("RMC Sport", "https://rmcsport.bfmtv.com", false, "France"),
        new("Eurosport France", "https://www.eurosport.fr", false, "France"),
        new("So Foot", "https://www.sofoot.com", false, "France"),
      ],
      ["media"] = [
        new("Allociné", "https://www.allocine.fr", false, "France"),
        new("Première", "https://www.premiere.fr", false, "France"),
        new("Télérama", "https://www.telerama.fr", false, "France"),
      ],
      ["health"] = [
        new("Doctissimo", "https://www.doctissimo.fr", false, "France"),
        new("Santé Magazine", "https://www.santemagazine.fr", false, "France"),
        new("Top Santé", "https://www.topsante.com", false, "France"),
      ],
      ["climate"] = [
        new("Novethic", "https://www.novethic.fr", false, "France"),
        new("Vert", "https://vert.eco", false, "France"),
        new("Actu Environnement", "https://www.actu-environnement.com", false, "France"),
      ],
      ["politics"] = [
        new("France Info Politique", "https://www.francetvinfo.fr/politique", false, "France"),
        new("Le Monde Politique", "https://www.lemonde.fr/politique", false, "France"),
        new("Le Figaro Politique", "https://www.lefigaro.fr/politique", false, "France"),
      ],
      ["crypto"] = [
        new("Journal du Coin", "https://journalducoin.com", false, "France"),
        new("Cryptonaute", "https://cryptonaute.fr", false, "France"),
        new("Cointribune", "https://www.cointribune.com/fr", false, "France"),
      ],
      ["gaming"] = [
        new("Jeuxvideo.com", "https://www.jeuxvideo.com", false, "France"),
        new("Gamekult", "https://www.gamekult.com", false, "France"),
        new("JeuxActu", "https://www.jeuxactu.com", false, "France"),
        new("NoFrag", "https://nofrag.com", false, "France"),
      ],
    },

    ["italy"] = new()
    {
      ["technology"] = [
        new("Tom's Hardware Italia", "https://www.tomshw.it", false, "Italy"),
        new("HDblog", "https://www.hdblog.it", false, "Italy"),
        new("Punto Informatico", "https://www.punto-informatico.it", false, "Italy"),
      ],
      ["science"] = [
        new("Le Scienze", "https://www.lescienze.it", false, "Italy"),
        new("Focus", "https://www.focus.it", false, "Italy"),
        new("Galileo", "https://www.galileonet.it", false, "Italy"),
      ],
      ["sports"] = [
        new("Corriere dello Sport", "https://www.corrieredellosport.it", false, "Italy"),
        new("Tuttosport", "https://www.tuttosport.com", false, "Italy"),
        new("Sky Sport Italia", "https://sport.sky.it", false, "Italy"),
      ],
      ["media"] = [
        new("Coming Soon", "https://www.comingsoon.it", false, "Italy"),
        new("BadTaste", "https://www.badtaste.it", false, "Italy"),
        new("Rockol", "https://www.rockol.it", false, "Italy"),
      ],
      ["health"] = [
        new("Humanitas Salute", "https://www.humanitasalute.it", false, "Italy"),
        new("OK Salute", "https://www.ok-salute.it", false, "Italy"),
      ],
      ["climate"] = [
        new("Rinnovabili.it", "https://www.rinnovabili.it", false, "Italy"),
        new("Qualenergia", "https://www.qualenergia.it", false, "Italy"),
        new("GreenReport", "https://greenreport.it", false, "Italy"),
      ],
      ["politics"] = [
        new("La Repubblica Politica", "https://www.repubblica.it/politica", false, "Italy"),
        new("Il Post Politica", "https://www.ilpost.it/politica", false, "Italy"),
      ],
      ["crypto"] = [
        new("The Cryptonomist", "https://cryptonomist.ch/it", false, "Italy"),
        new("Criptovaluta.it", "https://www.criptovaluta.it", false, "Italy"),
        new("Cointelegraph IT", "https://it.cointelegraph.com", false, "Italy"),
      ],
      ["gaming"] = [
        new("Multiplayer.it", "https://multiplayer.it", false, "Italy"),
        new("SpazioGames", "https://www.spaziogames.it", false, "Italy"),
      ],
    },

    ["spain"] = new()
    {
      ["technology"] = [
        new("El Androide Libre", "https://elandroidelibre.elespanol.com", false, "Spain"),
        new("Hipertextual", "https://hipertextual.com", false, "Spain"),
      ],
      ["science"] = [
        new("Muy Interesante", "https://www.muyinteresante.es", false, "Spain"),
        new("National Geographic España", "https://www.nationalgeographic.com.es", false, "Spain"),
        new("Agencia SINC", "https://www.agenciasinc.es", false, "Spain"),
      ],
      ["sports"] = [
        new("Marca", "https://www.marca.com", false, "Spain"),
        new("AS", "https://as.com", false, "Spain"),
        new("Mundo Deportivo", "https://www.mundodeportivo.com", false, "Spain"),
        new("Sport", "https://www.sport.es", false, "Spain"),
      ],
      ["media"] = [
        new("Sensacine", "https://www.sensacine.com", false, "Spain"),
        new("eCartelera", "https://www.ecartelera.com", false, "Spain"),
        new("FormulaTV", "https://www.formulatv.com", false, "Spain"),
      ],
      ["health"] = [
        new("Cuídate Plus", "https://cuidateplus.marca.com", false, "Spain"),
        new("Infosalus", "https://www.infosalus.com", false, "Spain"),
      ],
      ["climate"] = [
        new("Climática", "https://www.climatica.lamarea.com", false, "Spain"),
        new("El Periódico Verde", "https://elperiodicodelaenergia.com", false, "Spain"),
      ],
      ["politics"] = [
        new("El Mundo España", "https://www.elmundo.es/espana.html", false, "Spain"),
        new("La Vanguardia Política", "https://www.lavanguardia.com/politica", false, "Spain"),
      ],
      ["crypto"] = [
        new("Cointelegraph ES", "https://es.cointelegraph.com", false, "Spain"),
        new("Bit2Me News", "https://news.bit2me.com", false, "Spain"),
      ],
      ["gaming"] = [
        new("3DJuegos", "https://www.3djuegos.com", false, "Spain"),
        new("Vandal", "https://vandal.elespanol.com", false, "Spain"),
        new("MeriStation", "https://as.com/meristation", false, "Spain"),
        new("Vida Extra", "https://www.vidaextra.com", false, "Spain"),
      ],
    },

    ["romania"] = new()
    {
      ["technology"] = [
        new("Playtech.ro", "https://playtech.ro", false, "Romania"),
        new("Go4IT", "https://www.go4it.ro", false, "Romania"),
        new("start-up.ro", "https://start-up.ro", false, "Romania"),
      ],
      ["science"] = [
        new("Descopera.ro", "https://www.descopera.ro", false, "Romania"),
        new("HotNews Știință", "https://hotnews.ro/c/science", false, "Romania"),
      ],
      ["sports"] = [
        new("GSP.ro", "https://www.gsp.ro", false, "Romania"),
        new("Prosport", "https://www.prosport.ro", false, "Romania"),
        new("Sport.ro", "https://www.sport.ro", false, "Romania"),
        new("Digisport", "https://www.digisport.ro", false, "Romania"),
      ],
      ["media"] = [
        new("Cinemagia", "https://www.cinemagia.ro", false, "Romania"),
      ],
      ["health"] = [
        new("ROmedic", "https://www.romedic.ro", false, "Romania"),
        new("Sfatul Medicului", "https://www.sfatulmedicului.ro", false, "Romania"),
        // new("Viata Medicala", "https://www.viata-medicala.ro", false, "Romania"),
      ],
      ["climate"] = [
        new("Greenpeace Romania", "https://www.greenpeace.org/romania", false, "Romania"),
      ],
      ["politics"] = [
        new("Digi24", "https://www.digi24.ro", false, "Romania"),
        new("Hotnews", "https://www.hotnews.ro", false, "Romania"),
        new("G4Media", "https://www.g4media.ro", false, "Romania"),
        new("Adevarul Politica", "https://adevarul.ro/politica", false, "Romania"),
      ],
      ["crypto"] = [
        new("Crypto.ro", "https://crypto.ro", false, "Romania"),
      ],
      ["gaming"] = [
        new("Go4Games", "https://www.go4games.ro", false, "Romania"),
        new("Need4Games", "https://need4games.ro", false, "Romania"),
      ],
    },

    ["poland"] = new()
    {
      ["technology"] = [
        new("Chip.pl", "https://www.chip.pl", false, "Poland"),
        new("Komputer Świat", "https://www.komputerswiat.pl", false, "Poland"),
        new("Benchmark.pl", "https://www.benchmark.pl", false, "Poland"),
      ],
      ["science"] = [
        new("Crazy Nauka", "https://www.crazynauka.pl", false, "Poland"),
        new("National Geographic Polska", "https://www.national-geographic.pl", false, "Poland"),
      ],
      ["sports"] = [
        new("Sport.pl", "https://www.sport.pl", false, "Poland"),
        new("WP SportoweFakty", "https://sportowefakty.wp.pl", false, "Poland"),
        new("Przegląd Sportowy", "https://www.przegladsportowy.pl", false, "Poland"),
        new("Interia Sport", "https://sport.interia.pl", false, "Poland"),
      ],
      ["media"] = [
        new("Filmweb", "https://www.filmweb.pl", false, "Poland"),
        new("Onet Kultura", "https://kultura.onet.pl", false, "Poland"),
        new("WP Film", "https://film.wp.pl", false, "Poland"),
      ],
      ["health"] = [
        new("Medonet", "https://www.medonet.pl", false, "Poland"),
        new("Poradnik Zdrowie", "https://www.poradnikzdrowie.pl", false, "Poland"),
      ],
      ["climate"] = [
        new("Zielona Interia", "https://zielona.interia.pl", false, "Poland"),
      ],
      ["politics"] = [
        new("TVN24", "https://tvn24.pl", false, "Poland"),
        new("Onet Wiadomości", "https://wiadomosci.onet.pl", false, "Poland"),
        new("Gazeta Wyborcza", "https://wyborcza.pl", false, "Poland"),
        new("Rzeczpospolita", "https://www.rp.pl", false, "Poland"),
      ],
      ["crypto"] = [
        new("Bitcoin.pl", "https://bitcoin.pl", false, "Poland"),
      ],
      ["gaming"] = [
        new("GRYOnline", "https://www.gry-online.pl", false, "Poland"),
        new("Gram.pl", "https://www.gram.pl", false, "Poland"),
      ],
    },
  };

  public NewsSearchService(ILogger<NewsSearchService> logger)
  {
    _logger = logger;
  }

  public async Task<NewsSearchResult> SearchNewsSourcesAsync(string topic, string country)
  {
    try
    {
      var sources = new List<NewsSource>();
      var countryKey = country.ToLowerInvariant().Trim();
      var topicKey = topic.ToLowerInvariant().Trim();

      _logger.LogInformation("Searching news sources for topic: {Topic}, country: {Country}", topic, country);

      // Global uses only English-speaking countries: USA, UK, Canada
      var globalCountries = new[] { "usa", "uk", "canada" };

      // Handle "all" topic - get all topics from the specified country
      if (topicKey == "all")
      {
        if (countryKey == "global")
        {
          // All topics + Global = get sources from USA, UK, Canada only
          foreach (var globalCountry in globalCountries)
          {
            if (CountryNewsSources.TryGetValue(globalCountry, out var countryData))
            {
              foreach (var topicSources in countryData.Values)
              {
                sources.AddRange(topicSources);
              }
            }
          }
        }
        else if (CountryNewsSources.TryGetValue(countryKey, out var countryTopics))
        {
          // All topics for a specific country
          foreach (var topicSources in countryTopics.Values)
          {
            sources.AddRange(topicSources);
          }
        }
      }
      // Handle "global" country - get specific topic from USA, UK, Canada
      else if (countryKey == "global")
      {
        foreach (var globalCountry in globalCountries)
        {
          if (CountryNewsSources.TryGetValue(globalCountry, out var countryData))
          {
            if (countryData.TryGetValue(topicKey, out var topicSources))
            {
              sources.AddRange(topicSources);
            }
          }
        }
      }
      // Specific country + specific topic
      else if (CountryNewsSources.TryGetValue(countryKey, out var countryTopics))
      {
        if (countryTopics.TryGetValue(topicKey, out var topicSources))
        {
          sources.AddRange(topicSources);
        }
      }

      // Remove duplicates and shuffle
      sources = sources
        .GroupBy(s => s.Url)
        .Select(g => g.First())
        .OrderBy(_ => Random.Shared.Next())
        .Take(5)
        .ToList();

      // Fallback if no sources found
      if (sources.Count == 0)
      {
        _logger.LogWarning("No sources found for topic: {Topic}, country: {Country}. Using USA technology as fallback.", topic, country);
        sources = CountryNewsSources["usa"]["technology"]
          .OrderBy(_ => Random.Shared.Next())
          .Take(5)
          .ToList();
      }

      _logger.LogInformation("Found {Count} news sources: {Sources}",
          sources.Count,
          string.Join(", ", sources.Select(s => s.Name)));

      return await Task.FromResult(new NewsSearchResult(true, sources));
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to search for news sources");
      return new NewsSearchResult(false, [], ex.Message);
    }
  }
}
