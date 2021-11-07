//
//  PlantSearch.swift
//  GardifyIOS
//
//  Created by Netzlab on 04.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct PlantSearch: Codable{
    let Plants: [PlantDetailModel]?
}

struct searchCategoryModel: Codable{
    let Title: String
    let Id: Int
}

struct searchCategoryStorageModel: Encodable{
    var data: [searchCategoryModel]?
}

struct badgeTagsModel: Codable {
    let Id: Int
    let CategoryId: Int
    let Title: String?
}

struct plantGroupMainModel: Codable{
    let Groups: [plantGroupModel]?
}

struct plantGroupModel: Codable{
    let Id: Int
    let Name: String
}
