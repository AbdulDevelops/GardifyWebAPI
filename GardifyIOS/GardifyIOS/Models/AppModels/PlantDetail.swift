//
//  PlantDetail.swift
//  GardifyIOS
//
//  Created by Netzlab on 03.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation


struct PlantDetailModel: Codable{
    let Id: Int
    let NameLatin, NameGerman, Description: String
    let IsInUserGarden: Bool
    
    let Images: [ImageFile]?
    let Family: String?
    let Herkunft: String?
    let PlantGroups: [PlantGroupsModel]?
    let Colors: [String]?
    let Badges: [BadgesModel]?
    let PlantTags: [BadgesModel]?
    let TodoTemplates: [ToDoDetailsModel]?
}

struct PlantGroupsModel: Codable{
    let Name: String
    let Id: Int
}

struct PlantSiblingDetailModel: Codable{
    let Id : Int
    let NameLatin, NameGerman : String?
    let Images : [ImageFile]?
}
