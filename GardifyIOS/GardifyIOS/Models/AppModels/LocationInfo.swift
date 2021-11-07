//
//  LocationInfo.swift
//  GardifyIOS
//
//  Created by Rifat Hussain on 24.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

// MARK: - Welcome
struct LocationInfo: Codable {
    let id, street, zip, city: String
    let country: String
    let shopcartCounter, plantCount, points, currentTodoCount: Int
    let newMessages: Int

    enum CodingKeys: String, CodingKey {
        case id = "$id"
        case street = "Street"
        case zip = "Zip"
        case city = "City"
        case country = "Country"
        case shopcartCounter = "ShopcartCounter"
        case plantCount = "PlantCount"
        case points = "Points"
        case currentTodoCount = "CurrentTodoCount"
        case newMessages = "NewMessages"
    }
}
