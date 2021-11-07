//
//  videoModel.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 26.10.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct VideoModel: Codable{
    let Id, ViewCount: Int
    let YTLink, Title, Text: String
    let Tags: [String]
    let Date: String
    
    
}
