//
//  UserInfo.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 10.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct UserInfo: Codable {
    let HouseNr: String
    let FirstName, LastName, UserName, City: String
    let Street, Zip, Country: String

}

struct UserImageInfo: Codable{
    let Images: [ImageFile]
}

struct UserSettingInfo: Codable{
    let UserId: String
    var ActiveStormAlert, ActiveFrostAlert, AlertByEmail, AlertByPush: Bool
    var FrostDegreeBuffer: Int
}

struct UserWarningModel: Codable{
    
    let Id: Int
    let NotificationType: Int
    let Title, Body: String
    let ConditionValue: Float
    let Date: String
    
}

struct WarningModel : Codable{
    let RelatedObjectId, ObjectType: Int
    var NotifyForWind, NotifyForFrost, IsInPot : Bool
    let Title, Text, RelatedObjectName : String?
    let AlertConditionValue : Decimal
    let Dismissed: Bool
}
