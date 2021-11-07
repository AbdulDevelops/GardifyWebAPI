//
//  Array.swift
//  GardifyIOS
//
//  Created by Netzlab on 10.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

extension Array where Element == Int{
    
    mutating func AddOrRemove(value: Int){
        if self.contains(value){
            self = self.filter{ $0 != value}

        }
        else{
            self.append(value)
        }
    }
}

extension Array where Element == String{
    
    mutating func AddOrRemove(value: String){
        if self.contains(value){
            self = self.filter{ $0 != value}
        }
        else{
            self.append(value)
        }
    }
}

extension Array where Element: Hashable {
    func removingDuplicates() -> [Element] {
        var addedDict = [Element: Bool]()
        
        return filter {
            addedDict.updateValue(true, forKey: $0) == nil
        }
    }
    
    mutating func removeDuplicates() {
        self = self.removingDuplicates()
    }
}

