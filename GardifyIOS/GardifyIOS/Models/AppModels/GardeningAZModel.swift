//
//  GardeningAZModel.swift
//  GardifyIOS
//
//  Created by Netzlab on 04.09.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct GardeningAZModel: Codable {
    let Name: String
    let Description: String
    let Images: [ImageFile]?
}
