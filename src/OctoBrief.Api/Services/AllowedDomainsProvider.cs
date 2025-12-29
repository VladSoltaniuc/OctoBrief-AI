namespace OctoBrief.Api.Services;

public static class AllowedDomainsProvider
{
  private static readonly HashSet<string> AllowedDomains = new(StringComparer.OrdinalIgnoreCase)
  {
    // USA
    "techcrunch.com", "theverge.com", "arstechnica.com", "wired.com", "engadget.com",
    "scientificamerican.com", "sciencedaily.com", "livescience.com", "space.com",
    "espn.com", "bleacherreport.com", "si.com", "theathletic.com", "cbssports.com",
    "variety.com", "hollywoodreporter.com", "deadline.com", "ew.com", "rollingstone.com",
    "webmd.com", "healthline.com", "statnews.com", "health.com",
    "insideclimatenews.org", "grist.org", "e360.yale.edu", "climatecentral.org",
    "politico.com", "thehill.com", "axios.com", "cnn.com", "foxnews.com",
    "coindesk.com", "theblock.co", "decrypt.co", "bitcoinmagazine.com",
    "ign.com", "gamespot.com", "kotaku.com", "polygon.com", "pcgamer.com",
    
    // UK
    "theregister.com", "techradar.com", "wired.co.uk", "bbc.com", "bbc.co.uk",
    "newscientist.com", "nature.com", "theguardian.com",
    "skysports.com", "talksport.com",
    "digitalspy.com", "nme.com", "radiotimes.com",
    "nhs.uk",
    "carbonbrief.org", "climatechangenews.com",
    "news.sky.com", "politico.eu",
    "cityam.com",
    "eurogamer.net", "rockpapershotgun.com", "gamesradar.com", "nintendolife.com",
    
    // Canada
    "betakit.com", "itworldcanada.com", "mobilesyrup.com", "dailyhive.com",
    "cbc.ca", "canadiangeographic.ca", "science.gc.ca",
    "tsn.ca", "sportsnet.ca",
    "etcanada.com", "thestar.com",
    "healthing.ca", "globalnews.ca",
    "nationalobserver.com", "thenarwhal.ca",
    "theglobeandmail.com", "ipolitics.ca",
    "cgmagonline.com",
    
    // Germany
    "heise.de", "golem.de", "t3n.de", "chip.de",
    "spektrum.de", "scinexx.de", "wissenschaft.de",
    "kicker.de", "sport1.de", "sportschau.de", "spox.com",
    "dwdl.de", "filmstarts.de", "rollingstone.de",
    "apotheken-umschau.de", "netdoktor.de", "gesundheit.de",
    "klimareporter.de", "utopia.de", "spiegel.de",
    "tagesschau.de", "zeit.de", "faz.net",
    "btc-echo.de", "de.cointelegraph.com", "blocktrainer.de",
    "gamestar.de", "pcgames.de", "4players.de", "spieletipps.de",
    
    // France
    "01net.com", "numerama.com", "frandroid.com", "lesnumeriques.com",
    "sciencesetavenir.fr", "futura-sciences.com", "pourlascience.fr",
    "lequipe.fr", "rmcsport.bfmtv.com", "eurosport.fr", "sofoot.com",
    "allocine.fr", "premiere.fr", "telerama.fr",
    "doctissimo.fr", "santemagazine.fr", "topsante.com",
    "novethic.fr", "vert.eco", "actu-environnement.com",
    "francetvinfo.fr", "lemonde.fr", "lefigaro.fr",
    "journalducoin.com", "cryptonaute.fr", "cointribune.com",
    "jeuxvideo.com", "gamekult.com", "jeuxactu.com", "nofrag.com",
    
    // Italy
    "tomshw.it", "hdblog.it", "punto-informatico.it", "wired.it",
    "lescienze.it", "focus.it", "galileonet.it",
    "gazzetta.it", "corrieredellosport.it", "tuttosport.com", "sport.sky.it",
    "comingsoon.it", "badtaste.it", "rockol.it",
    "corriere.it", "humanitasalute.it", "ok-salute.it",
    "rinnovabili.it", "qualenergia.it", "greenreport.it",
    "ansa.it", "repubblica.it", "ilpost.it",
    "cryptonomist.ch", "criptovaluta.it", "it.cointelegraph.com",
    "multiplayer.it", "everyeye.it", "spaziogames.it", "it.ign.com",
    
    // Spain
    "xataka.com", "genbeta.com", "elandroidelibre.elespanol.com", "hipertextual.com",
    "muyinteresante.es", "nationalgeographic.com.es", "agenciasinc.es",
    "marca.com", "as.com", "mundodeportivo.com", "sport.es",
    "sensacine.com", "ecartelera.com", "formulatv.com",
    "cuidateplus.marca.com", "webconsultas.com", "infosalus.com",
    "climatica.lamarea.com", "ecologistasenaccion.org", "elperiodicodelaenergia.com",
    "elpais.com", "elmundo.es", "lavanguardia.com",
    "es.cointelegraph.com", "news.bit2me.com", "observatorioblockchain.com",
    "3djuegos.com", "vandal.elespanol.com", "meristation.as.com", "vidaextra.com",
    
    // Romania
    "playtech.ro", "go4it.ro", "start-up.ro", "techcafe.ro",
    "descopera.ro", "sciencealert.ro", "natgeo.ro",
    "gsp.ro", "prosport.ro", "sport.ro", "digisport.ro",
    "cinemagia.ro", "adevarul.ro", "observator.tv",
    "romedic.ro", "sfatulmedicului.ro", "viata-medicala.ro",
    "greenpeace.org", "digi24.ro",
    "hotnews.ro", "g4media.ro",
    "crypto.ro",
    "go4games.ro", "need4games.ro",
    
    // Poland
    "antyweb.pl", "chip.pl", "komputerswiat.pl", "benchmark.pl",
    "naukawpolsce.pl", "crazynauka.pl", "national-geographic.pl",
    "sport.pl", "sportowefakty.wp.pl", "przegladsportowy.pl", "sport.interia.pl",
    "filmweb.pl", "kultura.onet.pl", "film.wp.pl",
    "medonet.pl", "poradnikzdrowie.pl", "zdrowie.pap.pl",
    "zielona.interia.pl", "hightech.fm", "wyborcza.pl",
    "tvn24.pl", "wiadomosci.onet.pl", "rp.pl",
    "bitcoin.pl", "kryptowaluty.pl", "cryptonews.pl",
    "gry-online.pl", "cdaction.pl", "ppe.pl", "gram.pl",
    
    // Common TLDs
    "cointelegraph.com"
  };

  public static HashSet<string> GetAllowedDomains()
  {
    return new HashSet<string>(AllowedDomains, StringComparer.OrdinalIgnoreCase);
  }

  public static bool IsAllowedDomain(string domain)
  {
    var normalizedDomain = NormalizeDomain(domain);
    return AllowedDomains.Contains(normalizedDomain);
  }

  public static string NormalizeDomain(string domain)
  {
    if (string.IsNullOrWhiteSpace(domain)) return string.Empty;
    return domain.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
      ? domain[4..]
      : domain;
  }

  public static int Count => AllowedDomains.Count;
}
