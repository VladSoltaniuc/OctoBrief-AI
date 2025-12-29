// Quick test script to validate news source selection
// Run with: dotnet script test-sources.cs

var countries = new[] { "global", "usa", "uk", "canada", "germany", "france", "italy", "spain", "romania", "poland" };
var topics = new[] { "all", "ai", "media", "health", "climate", "politics" };

var CountryNewsSources = new Dictionary<string, Dictionary<string, List<(string Name, string Url)>>>
{
  ["usa"] = new()
  {
    ["technology"] = new() { ("TechCrunch", "https://techcrunch.com"), ("The Verge", "https://www.theverge.com"), ("Ars Technica", "https://arstechnica.com"), ("Wired", "https://www.wired.com"), ("Engadget", "https://www.engadget.com") },
    ["ai"] = new() { ("MIT Technology Review", "https://www.technologyreview.com"), ("VentureBeat AI", "https://venturebeat.com/ai"), ("TechCrunch AI", "https://techcrunch.com/category/artificial-intelligence"), ("Wired AI", "https://www.wired.com/tag/artificial-intelligence") },
    ["business"] = new() { ("Wall Street Journal", "https://www.wsj.com"), ("Forbes", "https://www.forbes.com"), ("CNBC", "https://www.cnbc.com"), ("Business Insider", "https://www.businessinsider.com"), ("Bloomberg", "https://www.bloomberg.com") },
    ["science"] = new() { ("Scientific American", "https://www.scientificamerican.com"), ("Science Daily", "https://www.sciencedaily.com"), ("Live Science", "https://www.livescience.com"), ("Space.com", "https://www.space.com") },
    ["sports"] = new() { ("ESPN", "https://www.espn.com"), ("Bleacher Report", "https://bleacherreport.com"), ("Sports Illustrated", "https://www.si.com"), ("The Athletic", "https://theathletic.com"), ("CBS Sports", "https://www.cbssports.com") },
    ["media"] = new() { ("Variety", "https://variety.com"), ("Hollywood Reporter", "https://www.hollywoodreporter.com"), ("Deadline", "https://deadline.com"), ("Entertainment Weekly", "https://ew.com"), ("Rolling Stone", "https://www.rollingstone.com") },
    ["health"] = new() { ("WebMD", "https://www.webmd.com"), ("Healthline", "https://www.healthline.com"), ("STAT News", "https://www.statnews.com"), ("Health.com", "https://www.health.com") },
    ["climate"] = new() { ("Inside Climate News", "https://insideclimatenews.org"), ("Grist", "https://grist.org"), ("Yale Environment 360", "https://e360.yale.edu"), ("Climate Central", "https://www.climatecentral.org") },
    ["politics"] = new() { ("Politico", "https://www.politico.com"), ("The Hill", "https://thehill.com"), ("Axios", "https://www.axios.com"), ("CNN Politics", "https://www.cnn.com/politics"), ("Fox News Politics", "https://www.foxnews.com/politics") },
    ["crypto"] = new() { ("CoinDesk", "https://www.coindesk.com"), ("The Block", "https://www.theblock.co"), ("Decrypt", "https://decrypt.co"), ("Bitcoin Magazine", "https://bitcoinmagazine.com") },
    ["gaming"] = new() { ("IGN", "https://www.ign.com"), ("GameSpot", "https://www.gamespot.com"), ("Kotaku", "https://kotaku.com"), ("Polygon", "https://www.polygon.com"), ("PC Gamer", "https://www.pcgamer.com") },
  },
  ["uk"] = new()
  {
    ["technology"] = new() { ("The Register", "https://www.theregister.com"), ("TechRadar", "https://www.techradar.com"), ("Wired UK", "https://www.wired.co.uk"), ("BBC Technology", "https://www.bbc.com/news/technology") },
    ["ai"] = new() { ("The Guardian AI", "https://www.theguardian.com/technology/artificialintelligenceai"), ("BBC AI", "https://www.bbc.com/news/topics/ce1qrvleleqt"), ("New Scientist AI", "https://www.newscientist.com/subject/technology") },
    ["business"] = new() { ("Financial Times", "https://www.ft.com"), ("The Economist", "https://www.economist.com"), ("City A.M.", "https://www.cityam.com"), ("This is Money", "https://www.thisismoney.co.uk") },
    ["science"] = new() { ("New Scientist", "https://www.newscientist.com"), ("Nature", "https://www.nature.com/news"), ("BBC Science", "https://www.bbc.com/news/science_and_environment"), ("The Guardian Science", "https://www.theguardian.com/science") },
    ["sports"] = new() { ("Sky Sports", "https://www.skysports.com"), ("BBC Sport", "https://www.bbc.com/sport"), ("The Athletic UK", "https://theathletic.com/uk"), ("talkSPORT", "https://talksport.com") },
    ["media"] = new() { ("Digital Spy", "https://www.digitalspy.com"), ("NME", "https://www.nme.com"), ("Radio Times", "https://www.radiotimes.com"), ("The Guardian Culture", "https://www.theguardian.com/culture") },
    ["health"] = new() { ("NHS News", "https://www.nhs.uk/news"), ("The Guardian Health", "https://www.theguardian.com/society/health"), ("BBC Health", "https://www.bbc.com/news/health") },
    ["climate"] = new() { ("Carbon Brief", "https://www.carbonbrief.org"), ("The Guardian Environment", "https://www.theguardian.com/environment"), ("Climate Home News", "https://www.climatechangenews.com") },
    ["politics"] = new() { ("The Guardian Politics", "https://www.theguardian.com/politics"), ("BBC Politics", "https://www.bbc.com/news/politics"), ("Sky News Politics", "https://news.sky.com/politics"), ("Politico EU", "https://www.politico.eu") },
    ["crypto"] = new() { ("CoinDesk", "https://www.coindesk.com"), ("Cointelegraph", "https://cointelegraph.com"), ("City A.M. Crypto", "https://www.cityam.com/crypto") },
    ["gaming"] = new() { ("Eurogamer", "https://www.eurogamer.net"), ("Rock Paper Shotgun", "https://www.rockpapershotgun.com"), ("GamesRadar", "https://www.gamesradar.com"), ("Nintendo Life", "https://www.nintendolife.com") },
  },
  ["canada"] = new()
  {
    ["technology"] = new() { ("BetaKit", "https://betakit.com"), ("IT World Canada", "https://www.itworldcanada.com"), ("MobileSyrup", "https://mobilesyrup.com"), ("Daily Hive Tech", "https://dailyhive.com/tech") },
    ["ai"] = new() { ("The Logic", "https://thelogic.co"), ("BetaKit AI", "https://betakit.com/tag/artificial-intelligence"), ("IT World Canada", "https://www.itworldcanada.com") },
    ["business"] = new() { ("Financial Post", "https://financialpost.com"), ("BNN Bloomberg", "https://www.bnnbloomberg.ca"), ("The Globe and Mail Business", "https://www.theglobeandmail.com/business"), ("CBC Business", "https://www.cbc.ca/news/business") },
    ["science"] = new() { ("CBC Science", "https://www.cbc.ca/news/science"), ("Canadian Geographic", "https://canadiangeographic.ca"), ("Science.gc.ca", "https://www.science.gc.ca") },
    ["sports"] = new() { ("TSN", "https://www.tsn.ca"), ("Sportsnet", "https://www.sportsnet.ca"), ("The Athletic Canada", "https://theathletic.com/canada"), ("CBC Sports", "https://www.cbc.ca/sports") },
    ["media"] = new() { ("ET Canada", "https://etcanada.com"), ("CBC Arts", "https://www.cbc.ca/arts"), ("Toronto Star Entertainment", "https://www.thestar.com/entertainment") },
    ["health"] = new() { ("CBC Health", "https://www.cbc.ca/news/health"), ("Healthing.ca", "https://www.healthing.ca"), ("Global News Health", "https://globalnews.ca/health") },
    ["climate"] = new() { ("Canada's National Observer", "https://www.nationalobserver.com"), ("The Narwhal", "https://thenarwhal.ca"), ("CBC Climate", "https://www.cbc.ca/news/climate") },
    ["politics"] = new() { ("CBC Politics", "https://www.cbc.ca/news/politics"), ("Global News Politics", "https://globalnews.ca/politics"), ("The Globe and Mail Politics", "https://www.theglobeandmail.com/politics"), ("iPolitics", "https://www.ipolitics.ca") },
    ["crypto"] = new() { ("CoinDesk", "https://www.coindesk.com"), ("Cointelegraph", "https://cointelegraph.com"), ("BetaKit Crypto", "https://betakit.com/tag/cryptocurrency") },
    ["gaming"] = new() { ("CGMagazine", "https://www.cgmagonline.com"), ("Daily Hive Gaming", "https://dailyhive.com/gaming"), ("MobileSyrup Gaming", "https://mobilesyrup.com/category/gaming") },
  },
  ["germany"] = new()
  {
    ["technology"] = new() { ("Heise Online", "https://www.heise.de"), ("Golem.de", "https://www.golem.de"), ("t3n", "https://t3n.de"), ("Chip.de", "https://www.chip.de") },
    ["ai"] = new() { ("Heise KI", "https://www.heise.de/thema/Kuenstliche-Intelligenz"), ("The Decoder", "https://the-decoder.com"), ("Golem AI", "https://www.golem.de/specials/ki") },
    ["business"] = new() { ("Handelsblatt", "https://www.handelsblatt.com"), ("Wirtschaftswoche", "https://www.wiwo.de"), ("Manager Magazin", "https://www.manager-magazin.de"), ("Finanzen.net", "https://www.finanzen.net") },
    ["science"] = new() { ("Spektrum.de", "https://www.spektrum.de"), ("Scinexx", "https://www.scinexx.de"), ("Wissenschaft.de", "https://www.wissenschaft.de") },
    ["sports"] = new() { ("Kicker", "https://www.kicker.de"), ("Sport1", "https://www.sport1.de"), ("Sportschau", "https://www.sportschau.de"), ("Spox", "https://www.spox.com/de") },
    ["media"] = new() { ("DWDL.de", "https://www.dwdl.de"), ("Filmstarts", "https://www.filmstarts.de"), ("Rolling Stone DE", "https://www.rollingstone.de") },
    ["health"] = new() { ("Apotheken Umschau", "https://www.apotheken-umschau.de"), ("Netdoktor", "https://www.netdoktor.de"), ("Gesundheit.de", "https://www.gesundheit.de") },
    ["climate"] = new() { ("Klimareporter", "https://www.klimareporter.de"), ("Utopia", "https://utopia.de"), ("Spiegel Klimakrise", "https://www.spiegel.de/thema/klimakrise") },
    ["politics"] = new() { ("Tagesschau", "https://www.tagesschau.de"), ("Zeit Politik", "https://www.zeit.de/politik"), ("Spiegel Politik", "https://www.spiegel.de/politik"), ("FAZ Politik", "https://www.faz.net/aktuell/politik") },
    ["crypto"] = new() { ("BTC-Echo", "https://www.btc-echo.de"), ("Cointelegraph DE", "https://de.cointelegraph.com"), ("Blocktrainer", "https://www.blocktrainer.de") },
    ["gaming"] = new() { ("GameStar", "https://www.gamestar.de"), ("PC Games", "https://www.pcgames.de"), ("4Players", "https://www.4players.de"), ("Spieletipps", "https://www.spieletipps.de") },
  },
  ["france"] = new()
  {
    ["technology"] = new() { ("01net", "https://www.01net.com"), ("Numerama", "https://www.numerama.com"), ("Frandroid", "https://www.frandroid.com"), ("Les Numériques", "https://www.lesnumeriques.com") },
    ["ai"] = new() { ("L'Usine Digitale", "https://www.usine-digitale.fr"), ("ActuIA", "https://www.actuia.com"), ("Siècle Digital", "https://siecledigital.fr") },
    ["business"] = new() { ("Les Echos", "https://www.lesechos.fr"), ("La Tribune", "https://www.latribune.fr"), ("Capital", "https://www.capital.fr"), ("BFM Business", "https://www.bfmtv.com/economie") },
    ["science"] = new() { ("Sciences et Avenir", "https://www.sciencesetavenir.fr"), ("Futura Sciences", "https://www.futura-sciences.com"), ("Pour la Science", "https://www.pourlascience.fr") },
    ["sports"] = new() { ("L'Équipe", "https://www.lequipe.fr"), ("RMC Sport", "https://rmcsport.bfmtv.com"), ("Eurosport France", "https://www.eurosport.fr"), ("So Foot", "https://www.sofoot.com") },
    ["media"] = new() { ("Allociné", "https://www.allocine.fr"), ("Première", "https://www.premiere.fr"), ("Télérama", "https://www.telerama.fr") },
    ["health"] = new() { ("Doctissimo", "https://www.doctissimo.fr"), ("Santé Magazine", "https://www.santemagazine.fr"), ("Top Santé", "https://www.topsante.com") },
    ["climate"] = new() { ("Reporterre", "https://reporterre.net"), ("Novethic", "https://www.novethic.fr"), ("Vert", "https://vert.eco") },
    ["politics"] = new() { ("France Info Politique", "https://www.francetvinfo.fr/politique"), ("Le Monde Politique", "https://www.lemonde.fr/politique"), ("Libération Politique", "https://www.liberation.fr/politique") },
    ["crypto"] = new() { ("Journal du Coin", "https://journalducoin.com"), ("Cryptonaute", "https://cryptonaute.fr"), ("Cointribune", "https://www.cointribune.com/fr") },
    ["gaming"] = new() { ("Jeuxvideo.com", "https://www.jeuxvideo.com"), ("Gamekult", "https://www.gamekult.com"), ("JeuxActu", "https://www.jeuxactu.com"), ("NoFrag", "https://nofrag.com") },
  },
  ["italy"] = new()
  {
    ["technology"] = new() { ("Tom's Hardware Italia", "https://www.tomshw.it"), ("HDblog", "https://www.hdblog.it"), ("Punto Informatico", "https://www.punto-informatico.it"), ("Wired Italia", "https://www.wired.it") },
    ["ai"] = new() { ("AI4Business", "https://www.ai4business.it"), ("Wired Italia AI", "https://www.wired.it/topic/intelligenza-artificiale"), ("Agenda Digitale", "https://www.agendadigitale.eu") },
    ["business"] = new() { ("Il Sole 24 Ore", "https://www.ilsole24ore.com"), ("Milano Finanza", "https://www.milanofinanza.it"), ("Affari Italiani", "https://www.affaritaliani.it/economia") },
    ["science"] = new() { ("Le Scienze", "https://www.lescienze.it"), ("Focus", "https://www.focus.it"), ("Galileo", "https://www.galileonet.it") },
    ["sports"] = new() { ("Gazzetta dello Sport", "https://www.gazzetta.it"), ("Corriere dello Sport", "https://www.corrieredellosport.it"), ("Tuttosport", "https://www.tuttosport.com"), ("Sky Sport Italia", "https://sport.sky.it") },
    ["media"] = new() { ("Coming Soon", "https://www.comingsoon.it"), ("BadTaste", "https://www.badtaste.it"), ("Rockol", "https://www.rockol.it") },
    ["health"] = new() { ("Corriere Salute", "https://www.corriere.it/salute"), ("Humanitas Salute", "https://www.humanitasalute.it"), ("OK Salute", "https://www.ok-salute.it") },
    ["climate"] = new() { ("Rinnovabili.it", "https://www.rinnovabili.it"), ("Qualenergia", "https://www.qualenergia.it"), ("GreenReport", "https://greenreport.it") },
    ["politics"] = new() { ("ANSA Politica", "https://www.ansa.it/sito/notizie/politica"), ("La Repubblica Politica", "https://www.repubblica.it/politica"), ("Il Post Politica", "https://www.ilpost.it/politica") },
    ["crypto"] = new() { ("The Cryptonomist", "https://cryptonomist.ch/it"), ("Criptovaluta.it", "https://www.criptovaluta.it"), ("Cointelegraph IT", "https://it.cointelegraph.com") },
    ["gaming"] = new() { ("Multiplayer.it", "https://multiplayer.it"), ("Everyeye.it", "https://www.everyeye.it"), ("SpazioGames", "https://www.spaziogames.it"), ("IGN Italia", "https://it.ign.com") },
  },
  ["spain"] = new()
  {
    ["technology"] = new() { ("Xataka", "https://www.xataka.com"), ("Genbeta", "https://www.genbeta.com"), ("El Androide Libre", "https://elandroidelibre.elespanol.com"), ("Hipertextual", "https://hipertextual.com") },
    ["ai"] = new() { ("Xataka IA", "https://www.xataka.com/tag/inteligencia-artificial"), ("MIT Technology Review ES", "https://www.technologyreview.es"), ("Computer Hoy IA", "https://computerhoy.com/tag/inteligencia-artificial") },
    ["business"] = new() { ("Expansión", "https://www.expansion.com"), ("Cinco Días", "https://cincodias.elpais.com"), ("El Economista", "https://www.eleconomista.es") },
    ["science"] = new() { ("Muy Interesante", "https://www.muyinteresante.es"), ("National Geographic España", "https://www.nationalgeographic.com.es"), ("Agencia SINC", "https://www.agenciasinc.es") },
    ["sports"] = new() { ("Marca", "https://www.marca.com"), ("AS", "https://as.com"), ("Mundo Deportivo", "https://www.mundodeportivo.com"), ("Sport", "https://www.sport.es") },
    ["media"] = new() { ("Sensacine", "https://www.sensacine.com"), ("eCartelera", "https://www.ecartelera.com"), ("FormulaTV", "https://www.formulatv.com") },
    ["health"] = new() { ("Cuídate Plus", "https://cuidateplus.marca.com"), ("WebConsultas", "https://www.webconsultas.com"), ("Infosalus", "https://www.infosalus.com") },
    ["climate"] = new() { ("Climática", "https://www.climatica.lamarea.com"), ("Ecologistas en Acción", "https://www.ecologistasenaccion.org"), ("El Periódico Verde", "https://elperiodicodelaenergia.com") },
    ["politics"] = new() { ("El País Política", "https://elpais.com/espana/"), ("El Mundo España", "https://www.elmundo.es/espana.html"), ("La Vanguardia Política", "https://www.lavanguardia.com/politica") },
    ["crypto"] = new() { ("Cointelegraph ES", "https://es.cointelegraph.com"), ("Bit2Me News", "https://news.bit2me.com"), ("Observatorio Blockchain", "https://observatorioblockchain.com") },
    ["gaming"] = new() { ("3DJuegos", "https://www.3djuegos.com"), ("Vandal", "https://vandal.elespanol.com"), ("MeriStation", "https://as.com/meristation"), ("Vida Extra", "https://www.vidaextra.com") },
  },
  ["romania"] = new()
  {
    ["technology"] = new() { ("Playtech.ro", "https://playtech.ro"), ("Go4IT", "https://www.go4it.ro"), ("start-up.ro", "https://start-up.ro"), ("Techcafe", "https://techcafe.ro") },
    ["ai"] = new() { ("Playtech AI", "https://playtech.ro/tag/inteligenta-artificiala"), ("start-up.ro", "https://start-up.ro"), ("Wall-Street.ro Tech", "https://www.wall-street.ro/tag/tehnologie") },
    ["business"] = new() { ("Ziarul Financiar", "https://www.zf.ro"), ("Wall-Street.ro", "https://www.wall-street.ro"), ("Business Magazin", "https://www.businessmagazin.ro"), ("Economica.net", "https://www.economica.net") },
    ["science"] = new() { ("Descopera.ro", "https://www.descopera.ro"), ("Science Alert RO", "https://www.sciencealert.ro"), ("National Geographic RO", "https://www.natgeo.ro") },
    ["sports"] = new() { ("GSP.ro", "https://www.gsp.ro"), ("Prosport", "https://www.prosport.ro"), ("Sport.ro", "https://www.sport.ro"), ("Digisport", "https://www.digisport.ro") },
    ["media"] = new() { ("Cinemagia", "https://www.cinemagia.ro"), ("Adevarul Cultura", "https://adevarul.ro/cultura"), ("Observator Entertainment", "https://observator.tv/show") },
    ["health"] = new() { ("ROmedic", "https://www.romedic.ro"), ("Sfatul Medicului", "https://www.sfatulmedicului.ro"), ("Viata Medicala", "https://www.viata-medicala.ro") },
    ["climate"] = new() { ("Greenpeace Romania", "https://www.greenpeace.org/romania"), ("Economica Green", "https://www.economica.net/verde"), ("Digi24 Mediu", "https://www.digi24.ro/stiri/externe/mediu") },
    ["politics"] = new() { ("Digi24", "https://www.digi24.ro"), ("Hotnews", "https://www.hotnews.ro"), ("G4Media", "https://www.g4media.ro"), ("Adevarul", "https://adevarul.ro/politica") },
    ["crypto"] = new() { ("Bitcoin Romania", "https://bitcoinromania.ro"), ("CryptoCoin.ro", "https://cryptocoin.ro"), ("Criptomonede.ro", "https://criptomonede.ro"), ("Ziarul Financiar Crypto", "https://www.zf.ro/burse-fonduri-mutuale/criptomonede"), ("Wall-Street Crypto", "https://www.wall-street.ro/tag/criptomonede") },
    ["gaming"] = new() { ("Games-ede.ro", "https://games-ede.ro"), ("GamesLine", "https://www.games-line.ro"), ("PC Garage", "https://www.pcgarage.ro/blog") },
  },
  ["poland"] = new()
  {
    ["technology"] = new() { ("Antyweb", "https://antyweb.pl"), ("Chip.pl", "https://www.chip.pl"), ("Komputer Świat", "https://www.komputerswiat.pl"), ("Benchmark.pl", "https://www.benchmark.pl") },
    ["ai"] = new() { ("Antyweb AI", "https://antyweb.pl/tag/sztuczna-inteligencja"), ("ITwiz", "https://itwiz.pl"), ("Computerworld.pl", "https://www.computerworld.pl") },
    ["business"] = new() { ("Money.pl", "https://www.money.pl"), ("Bankier.pl", "https://www.bankier.pl"), ("Puls Biznesu", "https://www.pb.pl"), ("Business Insider Polska", "https://businessinsider.com.pl") },
    ["science"] = new() { ("Nauka w Polsce", "https://naukawpolsce.pl"), ("Crazy Nauka", "https://www.crazynauka.pl"), ("National Geographic Polska", "https://www.national-geographic.pl") },
    ["sports"] = new() { ("Sport.pl", "https://www.sport.pl"), ("WP SportoweFakty", "https://sportowefakty.wp.pl"), ("Przegląd Sportowy", "https://www.przegladsportowy.pl"), ("Interia Sport", "https://sport.interia.pl") },
    ["media"] = new() { ("Filmweb", "https://www.filmweb.pl"), ("Onet Kultura", "https://kultura.onet.pl"), ("WP Film", "https://film.wp.pl") },
    ["health"] = new() { ("Medonet", "https://www.medonet.pl"), ("Poradnik Zdrowie", "https://www.poradnikzdrowie.pl"), ("Zdrowie PAP", "https://zdrowie.pap.pl") },
    ["climate"] = new() { ("Zielona Interia", "https://zielona.interia.pl"), ("High Tech", "https://hightech.fm"), ("Wyborcza Green", "https://wyborcza.pl/0,173236.html") },
    ["politics"] = new() { ("TVN24", "https://tvn24.pl"), ("Onet Wiadomości", "https://wiadomosci.onet.pl"), ("Gazeta Wyborcza", "https://wyborcza.pl"), ("Rzeczpospolita", "https://www.rp.pl") },
    ["crypto"] = new() { ("Bitcoin.pl", "https://bitcoin.pl"), ("Kryptowaluty.pl", "https://kryptowaluty.pl"), ("CryptoNews Polska", "https://cryptonews.pl") },
    ["gaming"] = new() { ("GRYOnline", "https://www.gry-online.pl"), ("CD-Action", "https://www.cdaction.pl"), ("PPE.pl", "https://ppe.pl"), ("Gram.pl", "https://www.gram.pl") },
  },
};

Console.WriteLine("=== NEWS SOURCE VALIDATION REPORT ===\n");

foreach (var countryKey in countries)
{
  Console.WriteLine($"\n{'='.ToString().PadLeft(60, '=')}");
  Console.WriteLine($"COUNTRY: {countryKey.ToUpper()}");
  Console.WriteLine($"{'='.ToString().PadLeft(60, '=')}");

  foreach (var topicKey in topics)
  {
    Console.WriteLine($"\n  Topic: {topicKey}");
    Console.WriteLine($"  {'-'.ToString().PadLeft(40, '-')}");

    var sources = new List<(string Name, string Url)>();

    if (topicKey == "all")
    {
      if (countryKey == "global")
      {
        foreach (var countryData in CountryNewsSources.Values)
        {
          foreach (var topicSources in countryData.Values)
          {
            sources.AddRange(topicSources);
          }
        }
      }
      else if (CountryNewsSources.TryGetValue(countryKey, out var countryTopics))
      {
        foreach (var topicSources in countryTopics.Values)
        {
          sources.AddRange(topicSources);
        }
      }
    }
    else if (countryKey == "global")
    {
      foreach (var countryData in CountryNewsSources.Values)
      {
        if (countryData.TryGetValue(topicKey, out var topicSources))
        {
          sources.AddRange(topicSources);
        }
      }
    }
    else if (CountryNewsSources.TryGetValue(countryKey, out var countryTopics))
    {
      if (countryTopics.TryGetValue(topicKey, out var topicSources))
      {
        sources.AddRange(topicSources);
      }
    }

    // Remove duplicates
    sources = sources.GroupBy(s => s.Url).Select(g => g.First()).ToList();

    if (sources.Count == 0)
    {
      Console.WriteLine($"  ⚠️  NO SOURCES FOUND!");
    }
    else
    {
      Console.WriteLine($"  ✓ Found {sources.Count} sources (showing first 5):");
      foreach (var source in sources.Take(5))
      {
        Console.WriteLine($"    - {source.Name}: {source.Url}");
      }
    }
  }
}

Console.WriteLine("\n\n=== SUMMARY ===");
Console.WriteLine($"Countries tested: {countries.Length}");
Console.WriteLine($"Topics tested: {topics.Length}");
Console.WriteLine($"Total combinations: {countries.Length * topics.Length}");
