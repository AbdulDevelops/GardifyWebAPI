//
//  TodoCountModel.swift
//  GardifyIOS
//
//  Created by Netzlab on 24.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation


struct TodoCountModel: Codable {
    let Finished, Open, AllTodos, AllTodosOfTheMonth: Int
}

struct TodoMainModel: Codable{
    let TodoList: [TodoModel]?
    let StartDate, EndDate: String
    
}

struct DiaryMainModel: Codable{
    let ListEntries: [DiaryModel]?
    let DiaryType: Int
}

struct DiaryModel: Codable{
    let Id, DiaryType: Int
    let Title, Date: String
    let Description: String
    let EntryImages: [ImageFile]?
    
}

struct TodoModel: Codable{
    let Id, ReferenceId, ReferenceType: Int
    let CyclicId: Int?
    let Description, Title: String
    
    let Notes: String?
    let DateStart, DateEnd, UserId: String
    let Finished, Deleted, Ignored: Bool
    let EntryImages: [ImageFile]?
    
}

struct TodoCyclicModel: Codable{
    let Id, ReferenceId: Int
    let Description, Title: String
    
    let Notes: String?
    let DateStart, DateEnd, UserId: String
    let Finished: Bool
    
}
