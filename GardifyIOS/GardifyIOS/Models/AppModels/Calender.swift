//
//  Calender.swift
//  GardifyIOS
//
//  Created by Netzlab on 17.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

struct Day {
  // 1
  let date: Date
  // 2
  let number: String
  // 3
  let isSelected: Bool
  // 4
  let isWithinDisplayedMonth: Bool
}

struct MonthMetadata{
    let numberOfDays: Int
    let firstDay: Date
    let firstDayWeekday: Int
}
