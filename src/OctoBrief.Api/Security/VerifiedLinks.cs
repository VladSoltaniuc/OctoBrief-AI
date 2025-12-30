using OctoBrief.Api.Models;

namespace OctoBrief.Api.Security;

public static class VerifiedLinks
{
  public static readonly Dictionary<string, Dictionary<string, List<NewsSource>>> CountryNewsSources = new()
  {
    ["usa"] = new()
    {
      ["technology"] = [
        new("TechCrunch", "https://techcrunch.com"),
        new("The Verge", "https://www.theverge.com"),
        new("Ars Technica", "https://arstechnica.com"),
        new("Wired", "https://www.wired.com"),
        new("Engadget", "https://www.engadget.com/big-tech"),
      ],
      ["science"] = [
        new("Scientific American", "https://www.scientificamerican.com"),
        new("Science Daily", "https://www.sciencedaily.com"),
        new("Live Science", "https://www.livescience.com"),
        new("Space.com", "https://www.space.com"),
      ],
      ["sports"] = [
        new("ESPN", "https://www.espn.com"),
        new("Bleacher Report", "https://bleacherreport.com"),
        new("Sports Illustrated", "https://www.si.com"),
        new("The Athletic", "https://theathletic.com"),
        new("CBS Sports", "https://www.cbssports.com"),
      ],
      ["media"] = [
        new("Variety", "https://variety.com"),
        new("Hollywood Reporter", "https://www.hollywoodreporter.com"),
        new("Deadline", "https://deadline.com"),
        new("Entertainment Weekly", "https://ew.com"),
        new("Rolling Stone", "https://www.rollingstone.com"),
      ],
      ["health"] = [
        new("WebMD", "https://www.webmd.com"),
        new("Healthline", "https://www.healthline.com"),
        new("STAT News", "https://www.statnews.com"),
        new("Health.com", "https://www.health.com"),
      ],
      ["climate"] = [
        new("Inside Climate News", "https://insideclimatenews.org"),
        new("Grist", "https://grist.org"),
        new("Yale Environment 360", "https://e360.yale.edu"),
      ],
      ["politics"] = [
        new("Politico", "https://www.politico.com"),
        new("The Hill", "https://thehill.com"),
        new("CNN Politics", "https://www.cnn.com/politics"),
        new("Fox News Politics", "https://www.foxnews.com/politics"),
      ],
      ["crypto"] = [
        new("The Block", "https://www.theblock.co"),
        new("Decrypt", "https://decrypt.co"),
        new("Bitcoin Magazine", "https://bitcoinmagazine.com"),
      ],
      ["gaming"] = [
        new("IGN", "https://www.ign.com"),
        new("GameSpot", "https://www.gamespot.com"),
        new("Kotaku", "https://kotaku.com"),
        new("Polygon", "https://www.polygon.com"),
        new("PC Gamer", "https://www.pcgamer.com"),
      ],
    },

    ["uk"] = new()
    {
      ["technology"] = [
        new("TechRadar UK", "https://www.techradar.com"),
        new("BBC Technology", "https://www.bbc.com/innovation/technology"),
        new("Silicon UK", "https://www.silicon.co.uk"),
      ],
      ["science"] = [
        new("New Scientist", "https://www.newscientist.com"),
        new("BBC Science", "https://www.bbc.com/news/science_and_environment"),
        new("The Guardian Science", "https://www.theguardian.com/science"),
      ],
      ["sports"] = [
        new("Sky Sports", "https://www.skysports.com"),
        new("BBC Sport", "https://www.bbc.com/sport"),
        new("The Athletic UK", "https://theathletic.com/uk"),
        new("talkSPORT", "https://talksport.com"),
      ],
      ["media"] = [
        new("Digital Spy", "https://www.digitalspy.com"),
        new("NME", "https://www.nme.com"),
        new("Radio Times", "https://www.radiotimes.com"),
        new("The Guardian Culture", "https://www.theguardian.com/culture"),
      ],
      ["health"] = [
        new("The Guardian Health", "https://www.theguardian.com/society/health"),
        new("BBC Health", "https://www.bbc.com/news/health"),
      ],
      ["climate"] = [
        new("Carbon Brief", "https://www.carbonbrief.org"),
        new("The Guardian Environment", "https://www.theguardian.com/environment"),
        new("Climate Home News", "https://www.climatechangenews.com"),
      ],
      ["politics"] = [
        new("The Guardian Politics", "https://www.theguardian.com/politics"),
        new("BBC Politics", "https://www.bbc.com/news/politics"),
        new("Sky News Politics", "https://news.sky.com/politics"),
      ],
      ["crypto"] = [
        new("City A.M. Crypto", "https://www.cityam.com/crypto"),
      ],
      ["gaming"] = [
        new("Eurogamer", "https://www.eurogamer.net"),
        new("Rock Paper Shotgun", "https://www.rockpapershotgun.com"),
        new("GamesRadar", "https://www.gamesradar.com"),
        new("Nintendo Life", "https://www.nintendolife.com"),
      ],
    },

    ["canada"] = new()
    {
      ["technology"] = [
        new("BetaKit", "https://betakit.com"),
        new("MobileSyrup", "https://mobilesyrup.com"),
      ],
      ["science"] = [
        new("CBC Science", "https://www.cbc.ca/news/science"),
        new("Canadian Geographic", "https://canadiangeographic.ca"),
        new("Science.gc.ca", "https://science.gc.ca/site/science/en"),
      ],
      ["sports"] = [
        new("TSN", "https://www.tsn.ca"),
        new("Sportsnet", "https://www.sportsnet.ca"),
        new("CBC Sports", "https://www.cbc.ca/sports"),
      ],
      ["media"] = [
        new("ET Canada", "https://etcanada.com"),
        new("CBC Arts", "https://www.cbc.ca/arts"),
        new("Toronto Star Entertainment", "https://www.thestar.com/entertainment"),
      ],
      ["health"] = [
        new("CBC Health", "https://www.cbc.ca/news/health"),
        new("Healthing.ca", "https://www.healthing.ca"),
        new("Global News Health", "https://globalnews.ca/health"),
      ],
      ["climate"] = [
        new("Canada's National Observer", "https://www.nationalobserver.com"),
        new("The Narwhal", "https://thenarwhal.ca"),
        new("CBC Climate", "https://www.cbc.ca/news/climate"),
      ],
      ["politics"] = [
        new("CBC Politics", "https://www.cbc.ca/news/politics"),
        new("Global News Politics", "https://globalnews.ca/politics"),
        new("The Globe and Mail Politics", "https://www.theglobeandmail.com/politics"),
        new("iPolitics", "https://www.ipolitics.ca"),
      ],
      ["crypto"] = [
        new("BetaKit Crypto", "https://betakit.com/tag/cryptocurrency"),
      ],
      ["gaming"] = [
        new("CGMagazine", "https://www.cgmagonline.com"),
        new("MobileSyrup Gaming", "https://mobilesyrup.com/category/gaming"),
      ],
    },

    ["germany"] = new()
    {
      ["technology"] = [
        new("Heise Online", "https://www.heise.de"),
        new("Golem.de", "https://www.golem.de"),
        new("t3n", "https://t3n.de"),
      ],
      ["science"] = [
        new("Spektrum.de", "https://www.spektrum.de"),
        new("Scinexx", "https://www.scinexx.de"),
        new("Wissenschaft.de", "https://www.wissenschaft.de"),
      ],
      ["sports"] = [
        new("Kicker", "https://www.kicker.de"),
        new("Sport1", "https://www.sport1.de"),
        new("Sportschau", "https://www.sportschau.de"),
        new("Spox", "https://www.spox.com/de"),
      ],
      ["media"] = [
        new("DWDL.de", "https://www.dwdl.de"),
        new("Filmstarts", "https://www.filmstarts.de"),
        new("Rolling Stone DE", "https://www.rollingstone.de"),
      ],
      ["health"] = [
        new("Apotheken Umschau", "https://www.apotheken-umschau.de"),
        new("Netdoktor", "https://www.netdoktor.de"),
        new("Gesundheit.de", "https://www.gesundheit.de"),
      ],
      ["climate"] = [
        new("Klimareporter", "https://www.klimareporter.de"),
      ],
      ["politics"] = [
        new("Tagesschau", "https://www.tagesschau.de"),
        new("Zeit Politik", "https://www.zeit.de/politik"),
        new("Spiegel", "https://www.spiegel.de"),
        new("FAZ Politik", "https://www.faz.net/aktuell/politik"),
      ],
      ["crypto"] = [
        new("BTC-Echo", "https://www.btc-echo.de"),
        new("Cointelegraph DE", "https://de.cointelegraph.com"),
        new("Blocktrainer", "https://www.blocktrainer.de"),
      ],
      ["gaming"] = [
        new("GameStar", "https://www.gamestar.de"),
        new("4Players", "https://www.4players.de"),
        new("Spieletipps", "https://www.spieletipps.de"),
      ],
    },

    ["france"] = new()
    {
      ["technology"] = [
        new("01net", "https://www.01net.com"),
        new("Frandroid", "https://www.frandroid.com"),
        new("Les Numériques", "https://www.lesnumeriques.com"),
      ],
      ["science"] = [
        new("Sciences et Avenir", "https://www.sciencesetavenir.fr"),
        new("Futura Sciences", "https://www.futura-sciences.com"),
        new("Pour la Science", "https://www.pourlascience.fr"),
      ],
      ["sports"] = [
        new("RMC Sport", "https://rmcsport.bfmtv.com"),
        new("Eurosport France", "https://www.eurosport.fr"),
        new("So Foot", "https://www.sofoot.com"),
      ],
      ["media"] = [
        new("Allociné", "https://www.allocine.fr"),
        new("Première", "https://www.premiere.fr"),
        new("Télérama", "https://www.telerama.fr"),
      ],
      ["health"] = [
        new("Doctissimo", "https://www.doctissimo.fr"),
        new("Santé Magazine", "https://www.santemagazine.fr"),
        new("Top Santé", "https://www.topsante.com"),
      ],
      ["climate"] = [
        new("Novethic", "https://www.novethic.fr"),
        new("Vert", "https://vert.eco"),
        new("Actu Environnement", "https://www.actu-environnement.com"),
      ],
      ["politics"] = [
        new("France Info Politique", "https://www.francetvinfo.fr/politique"),
        new("Le Monde Politique", "https://www.lemonde.fr/politique"),
        new("Le Figaro Politique", "https://www.lefigaro.fr/politique"),
      ],
      ["crypto"] = [
        new("Journal du Coin", "https://journalducoin.com"),
        new("Cryptonaute", "https://cryptonaute.fr"),
        new("Cointribune", "https://www.cointribune.com/fr"),
      ],
      ["gaming"] = [
        new("Jeuxvideo.com", "https://www.jeuxvideo.com"),
        new("Gamekult", "https://www.gamekult.com"),
        new("JeuxActu", "https://www.jeuxactu.com"),
        new("NoFrag", "https://nofrag.com"),
      ],
    },

    ["italy"] = new()
    {
      ["technology"] = [
        new("Tom's Hardware Italia", "https://www.tomshw.it"),
        new("HDblog", "https://www.hdblog.it"),
        new("Punto Informatico", "https://www.punto-informatico.it"),
      ],
      ["science"] = [
        new("Le Scienze", "https://www.lescienze.it"),
        new("Focus", "https://www.focus.it"),
        new("Galileo", "https://www.galileonet.it"),
      ],
      ["sports"] = [
        new("Corriere dello Sport", "https://www.corrieredellosport.it"),
        new("Tuttosport", "https://www.tuttosport.com"),
        new("Sky Sport Italia", "https://sport.sky.it"),
      ],
      ["media"] = [
        new("Coming Soon", "https://www.comingsoon.it"),
        new("BadTaste", "https://www.badtaste.it"),
        new("Rockol", "https://www.rockol.it"),
      ],
      ["health"] = [
        new("Humanitas Salute", "https://www.humanitasalute.it"),
        new("OK Salute", "https://www.ok-salute.it"),
      ],
      ["climate"] = [
        new("Rinnovabili.it", "https://www.rinnovabili.it"),
        new("Qualenergia", "https://www.qualenergia.it"),
        new("GreenReport", "https://greenreport.it"),
      ],
      ["politics"] = [
        new("La Repubblica Politica", "https://www.repubblica.it/politica"),
        new("Il Post Politica", "https://www.ilpost.it/politica"),
      ],
      ["crypto"] = [
        new("The Cryptonomist", "https://cryptonomist.ch/it"),
        new("Criptovaluta.it", "https://www.criptovaluta.it"),
        new("Cointelegraph IT", "https://it.cointelegraph.com"),
      ],
      ["gaming"] = [
        new("Multiplayer.it", "https://multiplayer.it"),
        new("SpazioGames", "https://www.spaziogames.it"),
      ],
    },

    ["spain"] = new()
    {
      ["technology"] = [
        new("El Androide Libre", "https://elandroidelibre.elespanol.com"),
        new("Hipertextual", "https://hipertextual.com"),
      ],
      ["science"] = [
        new("Muy Interesante", "https://www.muyinteresante.es"),
        new("National Geographic España", "https://www.nationalgeographic.com.es"),
        new("Agencia SINC", "https://www.agenciasinc.es"),
      ],
      ["sports"] = [
        new("Marca", "https://www.marca.com"),
        new("AS", "https://as.com"),
        new("Mundo Deportivo", "https://www.mundodeportivo.com"),
        new("Sport", "https://www.sport.es"),
      ],
      ["media"] = [
        new("Sensacine", "https://www.sensacine.com"),
        new("eCartelera", "https://www.ecartelera.com"),
        new("FormulaTV", "https://www.formulatv.com"),
      ],
      ["health"] = [
        new("Cuídate Plus", "https://cuidateplus.marca.com"),
        new("Infosalus", "https://www.infosalus.com"),
      ],
      ["climate"] = [
        new("Climática", "https://www.climatica.lamarea.com"),
        new("El Periódico Verde", "https://elperiodicodelaenergia.com"),
      ],
      ["politics"] = [
        new("El Mundo España", "https://www.elmundo.es/espana.html"),
        new("La Vanguardia Política", "https://www.lavanguardia.com/politica"),
      ],
      ["crypto"] = [
        new("Cointelegraph ES", "https://es.cointelegraph.com"),
        new("Bit2Me News", "https://news.bit2me.com"),
      ],
      ["gaming"] = [
        new("3DJuegos", "https://www.3djuegos.com"),
        new("Vandal", "https://vandal.elespanol.com"),
        new("MeriStation", "https://as.com/meristation"),
        new("Vida Extra", "https://www.vidaextra.com"),
      ],
    },

    ["romania"] = new()
    {
      ["technology"] = [
        new("Playtech.ro", "https://playtech.ro"),
        new("Go4IT", "https://www.go4it.ro"),
        new("start-up.ro", "https://start-up.ro"),
      ],
      ["science"] = [
        new("Descopera.ro", "https://www.descopera.ro"),
        new("HotNews Știință", "https://hotnews.ro/c/science"),
      ],
      ["sports"] = [
        new("GSP.ro", "https://www.gsp.ro"),
        new("Prosport", "https://www.prosport.ro"),
        new("Sport.ro", "https://www.sport.ro"),
        new("Digisport", "https://www.digisport.ro"),
      ],
      ["media"] = [
        new("Cinemagia", "https://www.cinemagia.ro"),
      ],
      ["health"] = [
        new("ROmedic", "https://www.romedic.ro"),
        new("Sfatul Medicului", "https://www.sfatulmedicului.ro"),
      ],
      ["climate"] = [
        new("Greenpeace Romania", "https://www.greenpeace.org/romania"),
      ],
      ["politics"] = [
        new("Digi24", "https://www.digi24.ro"),
        new("Hotnews", "https://www.hotnews.ro"),
        new("G4Media", "https://www.g4media.ro"),
        new("Adevarul Politica", "https://adevarul.ro/politica"),
      ],
      ["crypto"] = [
        new("Crypto.ro", "https://crypto.ro"),
      ],
      ["gaming"] = [
        new("Go4Games", "https://www.go4games.ro"),
        new("Need4Games", "https://need4games.ro"),
      ],
    },

    ["poland"] = new()
    {
      ["technology"] = [
        new("Chip.pl", "https://www.chip.pl"),
        new("Komputer Świat", "https://www.komputerswiat.pl"),
        new("Benchmark.pl", "https://www.benchmark.pl"),
      ],
      ["science"] = [
        new("Crazy Nauka", "https://www.crazynauka.pl"),
        new("National Geographic Polska", "https://www.national-geographic.pl"),
      ],
      ["sports"] = [
        new("Sport.pl", "https://www.sport.pl"),
        new("WP SportoweFakty", "https://sportowefakty.wp.pl"),
        new("Przegląd Sportowy", "https://www.przegladsportowy.pl"),
        new("Interia Sport", "https://sport.interia.pl"),
      ],
      ["media"] = [
        new("Filmweb", "https://www.filmweb.pl"),
        new("Onet Kultura", "https://kultura.onet.pl"),
        new("WP Film", "https://film.wp.pl"),
      ],
      ["health"] = [
        new("Medonet", "https://www.medonet.pl"),
        new("Poradnik Zdrowie", "https://www.poradnikzdrowie.pl"),
      ],
      ["climate"] = [
        new("Zielona Interia", "https://zielona.interia.pl"),
      ],
      ["politics"] = [
        new("TVN24", "https://tvn24.pl"),
        new("Onet Wiadomości", "https://wiadomosci.onet.pl"),
        new("Gazeta Wyborcza", "https://wyborcza.pl"),
        new("Rzeczpospolita", "https://www.rp.pl"),
      ],
      ["crypto"] = [
        new("Bitcoin.pl", "https://bitcoin.pl"),
      ],
      ["gaming"] = [
        new("GRYOnline", "https://www.gry-online.pl"),
        new("Gram.pl", "https://www.gram.pl"),
      ],
    },
  };
}