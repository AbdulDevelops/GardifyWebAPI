//
//  ecoCategory.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

let ecoCategoryKeys: [Int: String] = [
    447: "Bienenfreundlich",
    320: "Vogelfreundlich",
    322: "Vogelfreundlich",
    321: "Insektenfreundlich",
    445: "Ökologisch_wertvoll",
    531: "Schmetterlingsfreundlich",
    530: "Heimische_Pflanze"
]

let ecoInformation: [String: String] = [
    "Bienenfreundlich" : "Eine bienenfreundliche Pflanze bietet Nektar und Pollen für Bienen, Hummeln und Wildbienen. Charakteristisch sind einfache „offene“ Blüten, die nicht durch Überzüchtung ihre Staubgefäße verloren haben.",
    "Insektenfreundlich" : "Pflanzen gelten als insektenfreundlich, wenn sie entweder Nektar und Pollen für Insekten bieten oder als Futterpflanze, beispielsweise für Schmetterlingsraupen und andere Insektenlarven, dienen.",
    "Ökologisch_wertvoll" : "Eine Pflanze ist ökologisch wertvoll, wenn sie zur heimischen Wildflora zählt und das ökologische Gleichgewicht besonders fördert. Solche Gewächse erhöhen allgemein die Artenvielfalt, indem sie mehrere wichtige Eigenschaften für andere Arten bieten (Futterquelle, Rückzugsort, Bodenstabilisierung).Auch bedrohte Wildarten der heimischen Flora werden als „ökologisch wertvoll“ eingestuft.",
    "Heimische_Pflanze" : "",
    "Schmetterlingsfreundlich" : "",
    "Vogelfreundlich" : "Vogelfreundliche Pflanzen bieten entweder Nahrung für Vögel in Form von Früchten oder sie eignen sich besonders gut als Schutz- und Nistplatz."
    
]

let textDropdownDispList:[String] = [
    "Pflanzen filtern",
    "Bienenfreundlich",
    "Insektenfreundlich",
    "Heimische Pflanzen",
    "Schmetterlingsfreundlich",
    "Ökologisch wertvoll",
    "Vogelfreundlich",
    "nicht frosthart",
    "frosthart bis -5 °C",
    "frosthart bis -10 °C",
    "voll frosthart",
    "bedingt giftig",
    "stark giftig"
]

let textDropdownKeysList:[String:String] = [
    "Pflanzen filtern" : "-",
    "Bienenfreundlich" : "Bienenfreundlich",
    "Insektenfreundlich" : "Insektenfreundlich",
    "Heimische Pflanzen" : "Heimische_Pflanze",
    "Schmetterlingsfreundlich" : "Schmetterlingsfreundlich",
    "Ökologisch wertvoll" : "Ökologisch_wertvoll",
    "Vogelfreundlich" : "Vogelfreundlich",
    "nicht frosthart" : "nicht frosthart",
    "frosthart bis -5 °C" : "frosthart bis -5°C",
    "frosthart bis -10 °C" : "frosthart bis -10°C",
    "voll frosthart" : "voll frosthart",
    "bedingt giftig" : "bedingt giftig",
    "stark giftig" : "stark giftig"
]

let filterCategoryKeys: [Int: String] = [
    262: "giftig",
    315: "bedingt giftig",
    561: "stark giftig",
    294: "nicht frosthart",
    295: "nicht frosthart",
    293: "frosthart bis -5°C",
    292: "frosthart bis -10°C",
    285: "voll frosthart",
    286: "voll frosthart",
    287: "voll frosthart",
    288: "voll frosthart",
    289: "voll frosthart",
    290: "voll frosthart",
    291: "voll frosthart"
]

func getEcoImage(key: Int) -> UIImage{
    
    return UIImage(named: ecoCategoryKeys[key] ?? "") ?? UIImage()
}

func getEcoImageString(key: String) -> UIImage{
    
    return UIImage(named: key) ?? UIImage()
}
