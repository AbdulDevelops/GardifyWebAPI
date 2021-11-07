//
//  CommunityViewController.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 04.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

struct CommunityData{
    var title = String()
    var details = String()
    var url = String()
    var moderator = String()
    var dropdownImage = "Community_Pfeil_greun_auf"
    var isExtended = false
}

class CommunityViewController: UIViewController, UITableViewDelegate, UITableViewDataSource {
    
    @IBOutlet weak var communityScrollView: UIScrollView!
    @IBOutlet weak var communityContainerView: UIView!
    @IBOutlet weak var communityTableView: UITableView!
    @IBOutlet weak var communityTitlelLabel: UILabel!
    @IBOutlet weak var communityTitleDetails: UILabel!
    @IBOutlet weak var footerLabel: UILabel!
    
    @IBOutlet weak var detailIconImage: UIImageView!
    
    @IBOutlet weak var listIconImage: UIImageView!
    
    @IBOutlet var descriptionGesture: UITapGestureRecognizer!
    
    var isListMode  = true
    
    var communityDataArray = [CommunityData]()
    
    override func viewDidLoad(){
        
        self.configurePadding()
        communityTableView.delegate = self
        communityTableView.dataSource = self
        self.configurePage()
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        self.updateNavigationBar(isMain: false, "", "COMMUNITY", "main_community")
        
    }
    
    func configurePage(){
        
        self.applyTheme()
        
        
        
//        communityTableView.setWhiteButtonView()
        communityTableView.backgroundColor = .clear
        
        
        self.updateNavigationBar(isMain: false, "", "COMMUNITY", "main_community")
        
    
        
        //        var communityTitleAttributedString = NSMutableAttributedString(string: "Gern möchten wir euch hier das Gartenforum green-24.de unter https://green-24.de/forum/index.php ans Herz legen. Hier könnt ihr euch mit unzähligen Gartenfans zu allen wichtigen Themen austauschen.\n\ngardify®-User sind hier herzlich willkommen!")
        //
        //        communityTitleAttributedString.addAttribute(.link, value: "https://green-24.de/forum/index.php ", range: NSRange(location: 61, length: 35))
        
        let communityTitleAttributedString = getSeparateLinkText(fullText: "Gern möchten wir euch hier das Gartenforum green-24.de unter ans Herz legen, bis unser gardify-Community-Bereich freigegeben ist. Hier könnt ihr euch mit unzähligen Gartenfans zu allen wichtigen Themen austauschen. Aufgrund der Komplexität empfehlen wir die Nutzung auf einem großen Bildschirm oder Laptop. \n\ngardify®-User sind hier herzlich willkommen!", linkText: "https://green-24.de/forum/index.php")
        
        self.communityTitleDetails.attributedText = communityTitleAttributedString
        self.configureCommunityData()
        
//        let footerAttributedString = NSMutableAttributedString(string:"Wir bitten euch, die Regeln und Gepflogenheiten im Forum zu beachten. Regeln und Beiträge dieses Forums werden ausschließlich über Green-24.de gepflegt und administriert. Für die Inhalte übernehmen wir daher keinerlei Haftung.")
//        footerAttributedString.addAttribute(.link, value: "Green-24.de", range: NSRange(location: 131, length: 11))
        self.footerLabel.attributedText = getSeparateLinkText(fullText: "Wir bitten euch, die Regeln und Gepflogenheiten im Forum zu beachten. Regeln und Beiträge dieses Forums werden ausschließlich über Green-24.de gepflegt und administriert. Für die Inhalte übernehmen wir daher keinerlei Haftung.", linkText: "Green-24.de")
        
    }
    
    @IBAction func onTapDesc(_ sender: Any) {
        
        let oriText = "Gern möchten wir euch hier das Gartenforum green-24.de unter https://green-24.de/forum/index.php ans Herz legen. Hier könnt ihr euch mit unzähligen Gartenfans zu allen wichtigen Themen austauschen.\n\ngardify®-User sind hier herzlich willkommen!"
        
        let linkRange = (oriText as NSString).range(of: "Gern möchten wir euch hier das")
        
        if descriptionGesture.didTapAttributedTextInLabel(label: self.communityTitleDetails, inRange: linkRange) {
            if let url = URL(string: "https://green-24.de/forum/index.php"), UIApplication.shared.canOpenURL(url) {
                UIApplication.shared.openURL(url)
            }
        }  else {
            print("Tapped none")
        }
        
    }
    
    
    
    @IBAction func onTapDescriptionLink(_ sender: Any) {
        
    }
    
    func getSeparateLinkText(fullText: String, linkText: String) -> NSMutableAttributedString{
        
        //        let attributedString = NSMutableAttributedString(string:firstText)
        //
        //
        //        let linkString =  NSMutableAttributedString(string:linkText)
        
        let text = fullText
        let range = (text as NSString).range(of: linkText)
        let attributedString = NSMutableAttributedString(string:text)
        
        //        attributedString.addAttribute(NSAttributedString.Key.link, value: "Green-24.de" , range: range)
        attributedString.addAttribute(NSAttributedString.Key.foregroundColor, value: UIColor(named: "GardifyGreen")! , range: range)
        attributedString.addAttribute(NSAttributedString.Key.underlineColor, value: UIColor(named: "GardifyGreen")! , range: range)
        
        attributedString.addAttribute(NSAttributedString.Key.underlineStyle, value: NSUnderlineStyle.single.rawValue , range: range)
        
        
        return attributedString
        //        attributedString.append(normalString)
        
    }
    
    func configureCommunityData(){
        self.communityDataArray =
            [CommunityData(title: "Garten- und Pflanzen-News",
                         details: "Berichte und Erfahrungen aus der Pflanzen- und Gartenwelt. News und Ratschläge aus der grünen Redaktion.",
                         url: "https://green-24.de/forum/garten-und-pflanzen-news-f13.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team, Frank",
                         dropdownImage: "Community_Pfeil_gold_auf"),
             
           
           CommunityData(title: "Pflanzen-Magazin",
                         details: "Das Pflanzen-Magazin mit umfangreichen Pflanzenbeschreibungen, Anleitungen sowie Tipps rund um die Welt der Pflanzen.",
                         url: "https://green-24.de/forum/pflanzen-magazin-f44.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, Mara23, Rose23611, Wollschweber, Redaktion Magazin"),
           
           CommunityData(title: "Pflanzen und Botanik",
                         details: "Pflanzen und Botanik? Das Thema Pflanze mit allen Details ... Wissenswertes über Gartenpflanzen (Bäume, Sträucher und Stauden), Zimmerpflanzen (Palmen, Tropenpflanzen und exotische Pflanzen), Nutzpflanzen (Früchte, Obst und Gemüse), Wildpflanzen, Sukkulenten und Pilze",
                         url: "https://green-24.de/forum/pflanzen-und-botanik-f1.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team"),
           
           CommunityData(title: "Pflanzenkrankheiten und Schädlinge",
                         details: "Krankheit oder Schädling? Braune Blätter, kleine Tiere, was tun ... Neben den verbreiteten Schädlingen wie Blattläusen, Spinnmilben, Wollläusen, Schmierläusen, Schildläusen, Thripse, Weißen Fliegen und Trauermücken gibt es auch häufige Pilzerkrankungen wie echten und falschen Mehltau, Rost und Schimmel, die zu Flecken und Schäden an der Pflanze führen. Neben den chemischen Mitteln wie Insektiziden und Fungiziden gibt es auch oft gute Hausmittel zur Bekämpfung der Krankheiten oder Schädlinge. Ein optimaler Standort bezüglich Licht und Boden, die richtige Erde oder ein neues Substrat sowie regelmäßiges Düngen können eine Pflanze stärken und weniger anfällig gegen Schädlingsbefall und Krankheiten machen.",
                         url: "https://green-24.de/forum/pflanzenkrankheiten-schaedlinge-f10.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team,Frank"),
           
           CommunityData(title: "Pflegen, Schneiden und Veredeln",
                         details: "Richtige Pflanzenpflege, Schneiden und Veredeln … Die richtige Pflege von Pflanzen umfasst das Düngen (welchen Dünger und wie oft düngen), Schneiden (wie schneiden und wann wird geschnitten), Wässern (wieviel Wasser und wie oft gießen), Standort der Pflanze (wieviel Licht oder Schatten), Boden (welche Erde oder Substrate), Überwinterung (wie überwintern und bei welchen Temperaturen, winterhart oder nicht), Veredelung (welche Technik zum Veredeln, Okulieren, Anplatten oder Pfropfen).",
                         url: "green-24.de/forum/pflegen-schneiden-veredeln-f23.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team"),
           
           CommunityData(title: "Pflanzenbestimmung und Pflanzensuche",
                         details: "Pflanzen bestimmen? Wie heißt diese Pflanze? Bestimmung von Pflanzen ganz einfach ... Pflanzennamen werden in der Fachsprache oft aus dem Lateinischen abgeleitet und bezeichnen den botanischen Namen. Der botanische Name setzt sich aus Gattung, Art und Sorte zusammen. Die meisten Pflanzen besitzen auch einen deutschen Namen, der sich teilweise aus der lateinischen Übersetzung oder aus dem Volksmund, aber auch aus den Eigenschaften einer Pflanze ergeben hat. Eine Bestimmung ist oft über Bilder von Blättern, Blüten, Früchten, Trieben oder Fotos gesamter Pflanzen am einfachsten.",
                         url: "https://green-24.de/forum/pflanzenbestimmung-pflanzensuche-f16.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team"),
           
           CommunityData(title: "Pflanzenlexikon und Steckbriefe",
                         details: "Das Pflanzenlexikon – Steckbriefe, Portraits und Pflegehinweise ... In unserem Lexikon der Pflanzen sind die Pflanzenarten alphabetisch sortiert. Es werden alle Pflanzengattungen wie Gartenpflanzen, Zimmerpflanzen und exotische Pflanzen aufgeführt. Zu fast jeder Pflanze gibt es Angaben zum Schneiden (Rückschnitt und Erziehungsschnitt), Wässern (wie viel Wasser und wie oft), Düngen (welcher Dünger und wann muss gedüngt werden), Standort (sonnig, halbschattig oder im Schatten, Licht und Ansprüche), Erde (welcher Boden oder welches Substrat), Krankheiten (bekannte Schädlinge und Erkrankungen, Mittel wie Insektizide und Fungizide sowie Maßnahmen zur Bekämpfung).",
                         url: "https://green-24.de/forum/pflanzenlexikon-steckbriefe-f31.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team"),
           
           CommunityData(title: "Beliebte Pflanzen und Erfahrungen",
                         details: "Beliebte Pflanzen, eigene Erfahrungen, Bilder, Entwicklung und Wünsche ... Die bekanntesten oder beliebtesten Pflanzenarten mit Bildern (Fotos von Blüten und Blättern), individuellen Infos zur Pflege, Haltung, Vermehrung und Entwicklung. Welche Erde, welcher Dünger und wie oft, wie viel Licht, Entwicklungsstadien, Tipps und Tricks, Arten und Sorten, Überwinterung und eigene Erfahrungen.",
                         url: "https://green-24.de/forum/beliebte-pflanzen-erfahrungen-f51.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team"),
           
           CommunityData(title: "Samen, Anzucht und Vermehrung",
                         details: "Vermehrung, Anzucht und Aufzucht von Pflanzen ... Vermehrungsarten (wie kann ich eine Pflanze vermehren), Saatgut (in welcher Erde, wie zur Keimung bringen, Samen ernten), Aussaat (wann aussäen, Temperatur zum Keimen, wie viel Licht und Luftfeuchtigkeit, welches Wasser und wie oft gießen), Pikieren (wann teilen, Wurzeln trennen und vereinzeln, wie und wann umpflanzen), Stecklinge (wann schneiden und wie anschneiden), Anzucht (welches Substrat, im Gewächshaus oder Freiland), Düngen von jungen Pflanzen, Blüte und Frucht (wann kommt das erste Blatt, die erste Blüte oder Früchte).",
                         url: "https://green-24.de/forum/samen-anzucht-vermehrung-f5.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_dunkelturkis_auf"),
           
           CommunityData(title: "Anleitungen Saatgut, Samen und Sorten",
                         details: "Anleitungen zur Aussaat von Samen, Saatgutbeschreibungen sowie Hilfe zur Pflege während der Anzucht der jungen Pflanzen ... Falls Du Bilder, Erfahrungen mit Erden, Substraten und weitere Tipps zu den einzelnen Sämereien hast, so freuen wir uns auf Deine Angaben. Melde Dich zum Schreiben an.",
                         url: "https://green-24.de/forum/anleitungen-saatgut-samen-sorten-f37.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber",
                         dropdownImage: "Community_Pfeil_dunkelturkis_auf"),
           
           CommunityData(title: "Keimzeiten und Wartezeiten",
                         details: "Die Dauer bis zum Keimen, Bewurzelung und Fruchtbildung bei Samen, Stecklingen und Rhizomen ... Wann keimt es, wann bilden sich Wurzeln bei Saatgut, Steckling und Rhizom? Zeiten für die Aussaat, Bedingungen und Lichtverhältnisse sowie genutzte Substrate, Erden und Zuschlagsstoffe.",
                         url: "https://green-24.de/forum/keimzeiten-wartezeiten-f19.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_dunkelturkis_auf"),
           
           CommunityData(title: "Natur und Umwelt",
                         details: "Alles von Biologie bis Zytologie, Natur und Umwelt, neue Energien, Ökologie ... Im Forum für Natur und Umwelt werden viele Themen von Biologie bis zur Zytologie behandelt, dazu neue Energien und Ökologie.",
                         url: "https://green-24.de/forum/natur-26amp-umwelt-f2.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_hellblau_auf"),
           
           CommunityData(title: "Gartenplanung und Gartengestaltung",
                         details: "Planung und Gestaltung, Ideen und Technik, Beispiele, wie wird es gemacht ... Von der Gartenplanung bis zur fertigen Gartengestaltung mit Beispielen zur Entstehung und Integration technischer Ideen. Steuerungstechnik, Automatisierung und Gartenroboter, Bewässerungsanlagen und Rasenroboter. Ob Neugestaltung, Umgestaltung, moderne oder historische Gartenanlagen.",
                         url: "https://green-24.de/forum/garten-und-pflanzen-news-f13.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_greungelb_auf"),
           
           CommunityData(title: "Teichbau und Wassergarten",
                         details: "Ich baue einen Teich, Springbrunnen, Wasserspiel ... Wasser im Garten ist schon seit Jahrhunderten ein Element in der Gestaltung. Ob als Zierteich, Fischteich, Schwimmteich oder Brunnenanlage. Einen besonderen Reiz macht bewegtes Wasser in Form von Quellen, Fontänen, Wasserfällen oder Bachläufen aus. Hier gibt es Rat und Tipps zu Bau, Technik und der Gestaltung eigener Wasserwelten.",
                         url: "https://green-24.de/forum/teichbau-wassergarten-f22.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_greungelb_auf"),
           
           CommunityData(title: "Tiere und Tierwelt",
                         details: "Alles über Hunde, Katzen, Vögel, Fische, Reptilien, Amphibien, Insekten ... Die Fauna ist ein sehr umfangreiches Thema. Hier gibt es überwiegend Infos zu den heimischen Tierarten und beliebten Haustierarten. Fragen und Antworten zu Haltung, Pflege, Gesundheit und Ernährung sowie tierische Impressionen.",
                         url: "https://green-24.de/forum/tiere-und-tierwelt-f33.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_hellblau_auf"),
           
           CommunityData(title: "Tipps und Tricks",
                         details: "Tipps und Tricks für Haus und Garten... Hilfe und Beispiele für die alltäglichen Projekte, kleine und große Probleme mit der Natur und Technik.",
                         url: "https://green-24.de/forum/tipps-und-tricks-f3.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_rosa_auf"),
           
           CommunityData(title: "New GREENeration",
                         details: "Ob Frage, Sachbericht, Referat, Aufsatz oder Studium ... Alles wird grün werden ... In den Fachbereichen der Biologie und in der Ausbildung zum Gärtner ist die Botanik ein umfassendes Thema. Wir helfen bei Lösungsansätzen und Fachbegriffen.",
                         url: "https://green-24.de/forum/new-greeneration-f27.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_lila_auf"),
           
           CommunityData(title: "Treffpunkt und Stammtisch",
                         details: "Für alle Garten- und Pflanzenfreunde. Gute Gespräche führen, nette Leute treffen ... Wer Freude an Pflanzen und am Gärtnern hat, ist in unserem Treffpunkt herzlich willkommen. Hier treffen sich nette Leute und tauschen sich aus.",
                         url: "https://green-24.de/forum/treffpunkt-stammtisch-f11.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_dunkelblau_auf"),
           
           CommunityData(title: "INFOS ZUM FORUM",
                         details: "Regeln, Neuigkeiten zum Forum, Hilfe, sowie Kurzinfos für alle nicht registrierten User. Und der Prämien-Shop. Bitte lesen... Um alle Funktionen unserer Foren zu verstehen gibt es hier die Anleitungen und den Support für die wichtigsten Fragen zur Bedienung und Technik.",
                         url: "https://green-24.de/forum/infos-zum-forum-f12.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_grau_auf"),
           
           CommunityData(title: "Wohnen und Einrichten",
                         details: "Bauen, umbauen, einrichten und dekorieren. Über Häuser, Wohnungen, Technik und Design...\nAuf der Suche nach Gemütlichkeit mögen die meisten Menschen zunächst an ihre eigenen vier Wände denken. Umso wichtiger, dass die eigene Wohnung oder das eigene Haus so eingerichtet ist, dass es der persönlichen Lebensart entspricht.\nGemütlichkeit mit Deko-Ideen\nWer gerne zweckmäßig lebt wird es schwer haben, Gemütlichkeit in das Wohnzimmer oder das Schlafzimmer zu zaubern. Das Einrichten ist nicht immer eine Frage von Nützlichkeit. Kleine Objekte wie Vasen oder Bilderrahmen lockern jeden Raum auf. Auch im Schlafzimmer verleihen persönliche Gegenstände dem Raum ein individuelles Design. Man sollte beim Einrichten darauf achten, die Gegenstände und Möbel möglichst miteinander harmonierend auszuwählen. Für entsprechende Wohnideen stehen in jedem Möbelhaus auch immer Experten zur Verfügung, die über Farben und Materialien gut Bescheid wissen.\nGemütlichkeit durch Pflanzen\nInsbesondere im Sommer tut es dem eigenen Wohlbefinden gut, viel Zeit im Freien zu verbringen. Natürlich ist das nur dann möglich, wenn man über einen Balkon oder eine Terrasse verfügt. Wer das Glück hat und einen Wintergarten sein Eigen nennt, kann sogar die kalten Tage im Grünen verbringen. Die Trends beim Wohnen entsprechen immer mehr dem Verlangen nach Natürlichkeit. Pflanzen spielen dabei tatsächlich nicht nur auf dem Balkon eine große Rolle. Auch im Haus oder in der Wohnung lockern sie die Atmosphäre auf und vermitteln Wohnlichkeit. In der Küche lassen sich durch selbst angepflanzte Kräuter auf der Fensterbank schnell Trends und Nützlichkeit verbinden.\nDie Gegebenheiten nutzen\nNicht jede Wohnung oder jedes Haus ist mit einer besonderen und dankbaren Architektur gesegnet. Die Kunst besteht darin, so kreativ zu sein, dass selbst unglückliche Gegebenheiten gemütlich und individuell werden. Wer das Glück hat und das Bauen selbst übernimmt, sollte hinsichtlich der Architektur auch eine später veränderte Lebensart berücksichtigen.",
                         url: "https://green-24.de/forum/wohnen-einrichten-f54.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_rosa_auf"),
           
           CommunityData(title: "Basteln, Deko und Co.",
                         details: "Do it yourself, Deko, Basteln, Werkeln und Co. Was man alles basteln und werkeln kann, wird in diesem Forum berichtet. Do it yourself mit vielen Beispielen und Tipps. Deko für Haus und Garten.",
                         url: "https://green-24.de/forum/basteln-deko-co-f14.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_rosa_auf"),
           
           CommunityData(title: "Kochen, Backen und Rezepte",
                         details: "Kochen mit Zutaten aus Garten und Natur, Pflanzen, Früchte, Kräuter- und Salatgerichte ... Natürliches Kochen und Backen mit schönen Rezepten und Zutaten aus Garten und Natur. Früchte, ausgefallene Kräuter und Salate.",
                         url: "https://green-24.de/forum/kochen-backen-rezepte-f25.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_mint_auf"),
           
           CommunityData(title: "Termine, Reisen und Urlaub",
                         details: "Termine, Messen, Garten- und Pflanzenreisen, Gärten und Parks, Reiseziele, Reiseberichte ... Die schönsten Reiseziele für Garten- und Pflanzenfreunde. Termine zu Ausstellungen und Messen mit Reiseberichten.",
                         url: "https://green-24.de/forum/termine-reisen-urlaub-f28.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_grau_auf"),
           
           CommunityData(title: "Bücher und Literatur",
                         details: "Buchvorstellungen und Rezensionen zu Büchern, Empfehlungen ... Ausgefallene und lesenswerte Bücher, schöne Literatur über Pflanzen und Gärten mit Buchvorstellungen und Rezensionen.",
                         url: "https://green-24.de/forum/buecher-literatur-f35.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_lila_auf"),
           
           CommunityData(title: "Dokumentationen und Berichte in den Medien",
                         details: "Filme und Videos, Berichte und Dokumentationen über Pflanzen, Gärten und die Natur... Filme über Gärten, Pflanzen und Natur in Form von Berichten und Dokumentationen. Videos zu aktuellen Themen.",
                         url: "https://green-24.de/forum/filme-videos-tv-f40.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber",
                         dropdownImage: "Community_Pfeil_lila_auf"),
           
           CommunityData(title: "Aus aller Welt und Off Topic",
                         details: "Für alle Themen und Beiträge, auch mit nicht grünen Inhalten ... Das Forum für alle anderen Themen wird auch Off Topic genannt. Hier ist Platz für alles, was sonst noch nicht thematisch passt.",
                         url: "https://green-24.de/forum/aus-aller-welt-off-topic-f20.html",
                         moderator: "Moderatoren: gudrun, Roadrunner, daylily, Mara23, Elfensusi, Rose23611, Wollschweber, Green-Team",
                         dropdownImage: "Community_Pfeil_grau_auf"),
           
           CommunityData(title: "Pressemitteilungen aus der Pflanzen- und Gartenwelt",
                         details: "News, Vorstellungen und Pressemitteilungen über Firmen und Produkte ... Infos und Pressemitteilungen aus der Pflanzen- und Gartenwelt, über Firmen und Produkte.",
                         url: "https://green-24.de/forum/presse-mitteilungen-pflanzen-und-gartenwelt-f42.html",
                         moderator: "Moderatoren: Roadrunner",
                         dropdownImage: "Community_Pfeil_gold_auf")
                                   
        ]
    }
    
    @IBAction func onDetailClicked(_ sender: Any) {
        for c in 0...(self.communityDataArray.count-1){
            self.communityDataArray[c].isExtended = true
        }
        self.detailIconImage.image = UIImage(named: "List_View_Dark")
        self.listIconImage.image = UIImage(named: "Gartenwissen_A_Z_Switchbutton_Liste_off")
        
        self.communityTableView.reloadData()
    }
    @IBAction func onListClicked(_ sender: Any) {
        configureCommunityData()
        self.communityTableView.reloadData()
        self.detailIconImage.image = UIImage(named: "List_View_Light")
        self.listIconImage.image = UIImage(named: "Gartenwissen_A_Z_Switchbutton_Liste_on")

    }
    
    func updateItemExtendStatus(index: Int, status: Bool){
        guard self.communityDataArray[index] != nil else{
            return
        }
        
        self.communityDataArray[index].isExtended = status
        
        
    }
    
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.communityDataArray.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        
        
        let cell = tableView.dequeueReusableCell(withIdentifier: "communityViewCell", for: indexPath) as! CommunityTableViewCell
        cell.delegate = self
        cell.parent = self
        cell.index = indexPath.row
        cell.clearBackground()
        
        cell.configureCell(community: self.communityDataArray[indexPath.row])
        return cell
        
    }
}


