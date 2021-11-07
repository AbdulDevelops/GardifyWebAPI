//
//  PlantDoc.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct PlantDocs: Codable {
    let plantDocList: [PlantDoc]
}

struct PlantDoc: Codable{
    let AdminAllowsPublishment: Bool
    let Description: String?
    let HeadLine: String?
    var Images: [ImageFile]?
    let IsOwnFoto: Bool
    let PublishDate: String?
    let QuestionAuthorId: String?
    let QuestionId: Int
    let QuestionText: String?
    let Thema: String?
    let TotalAnswers: Int
    let UserAllowsPublishment: Bool
    let isEdited: Bool

}

struct PlantDocDetailModel: Codable{
    let PlantDocViewModel: PlantDoc
    let PlantDocAnswerList: [PlantDocAnswerModel]
}

struct PlantDocAnswerModel: Codable{
    let AnswerText, AutorName: String
    let Date : String?
    let AnswerId: Int
    let EnableToEdit: Bool
    let IsEdited: Bool
    let IsAdminAnswer: Bool
    
}

struct ImageFile: Codable{
    let AltAttr, Author, FullDescription, FullTitle: String?
    let Id: Int
    let License: String?
    let Sort: Int
    var SrcAttr: String?
    let TitleAttr: String?
    

}
