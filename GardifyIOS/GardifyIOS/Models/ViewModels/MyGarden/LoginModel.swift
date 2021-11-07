//
//  LoginModel.swift
//  GardifyText
//
//  Created by Netzlab on 30.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct LoginModel: Codable {
    let Admin: Bool
    let Email: String
    let ExpiresUtc: String
    let Name: String
    let Token: String
    let UserId: String
}
