//
//  ScannerResult.swift
//  GardifyText
//
//  Created by Netzlab on 28.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct ScanResult: Codable{
    let GPlants: String?
    let GImages: String?
    let PnResults: PnResultsModel
}

struct PnResultsModel: Codable{
    let InDb: [InDbModel]?
    let results: [Results]
}

struct InDbModel: Codable{
    let Id: Int
    let NameLatin, NameGerman, Description: String
    let IsInUserGarden: Bool
//    let GenusTaxon: String?
//    let PlantTags: String?
//    let PlantCharacteristics: String?
//    let PlantCharacteristicsOld: String?
    var Images: [ImageDb]?
//    let StatusMessage: String?
//    let Articles: String?
    let Published: Bool
    let Score: Int
    let ShopcartCounter: Int
    let PlantCount: Int
    let Points: Int
    let CurrentTodoCount: Int
    let NewMessages: Int
}

struct ImageDb: Codable{
    let Id: Int
    var FullTitle, FullDescription, TitleAttr, AltAttr, SrcAttr: String
    let Sort: Int
}

struct Results: Codable{
    let score: Double
    let species: Species
    let images: [ImageList]
}

struct ImageList: Codable{
    let link: String
    let image: ImageObject
}

struct ImageObject: Codable{
    let thumbnailLink: String
    let contextLink: String
}

struct Species: Codable {
    let commonNames: [String]
    let scientificNameWithoutAuthor: String
    let family, genus: Family
}

struct Family: Codable {
    let scientificNameWithoutAuthor: String
}
