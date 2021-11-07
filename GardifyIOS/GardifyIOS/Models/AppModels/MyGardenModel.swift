//
//  MyGardenModel.swift
//  GardifyIOS
//
//  Created by Netzlab on 03.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct MyGardenModel: Codable, Hashable{
    static func == (lhs: MyGardenModel, rhs: MyGardenModel) -> Bool {
        return lhs.UserPlant.Id == rhs.UserPlant.Id && lhs.ListId == rhs.ListId
    }
    
    func hash(into hasher: inout Hasher) {
        hasher.combine(UserPlant.Id )
    }
    
    var UserPlant: GardenUserPlantModel
    let ListName: String?
    let ListId: Int
    let ListNames: [String]
    let ListIds: [Int]
}

struct GardenUserPlantModel: Codable{

    
    let Id, PlantId: Int
    let Description: String
    var Count: Int
    let Gardenid: Int
    let Name, NameLatin, CustomName: String
    var Notes: String?
    var CreatedNotes: String?
    let IsInPot: Bool
    let Badges: [badgeTagsModel]?
    let Images: [ImageFile]?
    let CyclicTodos: [CyclicTodosModel]?
}

struct GardenUserPlantEditModel: Codable{
    let Id: Int
    let Count: Int
    let Description, Name: String
    let Notes: String?
    let IsInPot: Bool
    let UserListId: Int
    
}

struct CyclicTodosModel: Codable{
    let Id: Int
    let Description: String
    let Title: String
    let Ignored: Bool
    let DateStart, DateEnd: String
}

struct MyGardenLightModel: Codable{
    let Id: Int
    let Name, Description: String
    let Images: [ImageFile]?
    let PlantsLight: [PlantsLightModel]?
}

struct PlantsLightModel: Codable{
    let Images: [ImageFile]?
    let PlantId: Int
    let Id: Int
//    let Todos: [TodosModel]?
//    let Name: String
//    let NameLatin: String?
//    let Count: Int
//    let GardenId: Int
//    let UserListId: Int
//    let Notes: String?
//    let Age: String?
//    let Description: String?
    
    let IsInPot: Bool
    
}

struct TodosModel: Codable{
    let Id: Int
    let UserId: String
    let ReferenceId: Int
    let Notification: Int
    let ReferenceType: Int
    let Precision: Int
    let Ignored: Bool
    let Finished: Bool
    let Index: Int
    let CyclicId: Int
    let Description: String
    
}

struct BadgesModel: Codable{
    let Id: Int
    let CategoryId: Int
    let Title: String?
}

struct ToDoDetailsModel: Codable{
    let Id: Int
    let Title, Description : String?
}


struct MyGardenEcoListModel: Codable{
    let Id, EcoCount: Int
    let Name: String
    let Description: String
    let Checked: Bool
    let EcoElementsImages: [ImageFile] = []
}

struct MyGardenDeviceListModel: Codable{
    let Id: Int
    let Name: String
    let isActive: Bool
    var notifyForWind, notifyForFrost: Bool
    let Note: String?
    let CreatedBy: String
    let EditedBy: String
    let UserDevListId: Int
    let Date: String
    let Gardenid: Int
    let AdminDevId: Int
    var Count: Int
    let EntryImages: [ImageFile]?
    let Todos: [TodoModel]?
    
}

struct AdminDeviceListModel: Codable{
    let Id: Int
    let Name: String
    let isActive, notifyForWind, notifyForFrost: Bool

    let DevicesImages: [ImageFile]?
    
}
