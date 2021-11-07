//
//  UserPlant.swift
//  GardifyIOS
//
//  Created by Netzlab on 03.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct UserPlantModel: Codable{
    let Id: Int
    let Name: String
    let Description: String?
    let GardenId: Int
    var ListSelected: Bool
    
}

struct UserPlantListModel: Codable{
    let UserPlantId, UserListId: Int
}
