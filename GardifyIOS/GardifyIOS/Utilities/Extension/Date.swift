//
//  Date.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 31.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

extension Date{
    
    func toString(output: String = ISO_8601_DATE) -> String{
        let dateformatter = DateFormatter()
        dateformatter.dateFormat = output
        return dateformatter.string(from: self)
    }
    
    func dayNumberOfWeek() -> Int? {
        return Calendar.current.dateComponents([.weekday], from: self).weekday
    }
    
}
