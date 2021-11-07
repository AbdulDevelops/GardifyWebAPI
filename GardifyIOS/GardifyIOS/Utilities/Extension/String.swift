//
//  String.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation

let ISO_8601_DATE = "yyyy-MM-dd'T'HH:mm:ss.SSS"



let IMAGE_PLACEHOLDER = "https://gardify.de/intern/Images/gardify_Pflanzenbild_Platzhalter.svg"

extension String{
    
    func toDate(_ format: String = ISO_8601_DATE) -> Date?{
        let dateformatter = DateFormatter()
        dateformatter.dateFormat = format
        dateformatter.locale = Locale(identifier: "en_DE")
        guard let out = dateformatter.date(from: self) else{
            return nil
        }
        return out
    }
    
    func toDateString(_ format: String = ISO_8601_DATE, output: String) -> String{
        guard let dateValue = self.toDate(format) else{
            return self
        }
        
   
        let dateformatter = DateFormatter()
        dateformatter.dateFormat = output
        return dateformatter.string(from: dateValue)
    }
    
    mutating func toURL(_ small: Bool = true, _ autoscale: Bool = false, _ fromAdmin: Bool = true)-> String{
        if self == nil || self == "" || self.contains("Platzhalter"){
            return IMAGE_PLACEHOLDER
        }
        var baseUrl = ""
        var adminArea = false
        baseUrl = APP_URL.BASE_ROUTE_INTERN
        if !fromAdmin{
            baseUrl = APP_URL.BASE_ROUTE
        }
        
        if self.contains("PlantImages") || self.contains("ArticleImages") || self.contains("EventsImages")  ||  self.contains("EcoElementImages") {
            adminArea = true
        }
        if adminArea{
            var outputString = ""
             var textSplit = self.split(separator: "/")
            
            for (index, item) in textSplit.enumerated(){
                if index >= (textSplit.count - 1){
                    outputString += "250/" + item
                }
                else{
                    outputString +=  item + "/"
                }
                
            }
            return baseUrl + outputString
        }

        return baseUrl + self
        

    }
    
    static func getMonthString(month: Int) -> String{
        switch month {
        case 1:
            return "Jan"
        case 2:
            return "Feb"
        case 3:
            return "Mär"
        case 4:
            return "Apr"
        case 5:
            return "Mai"
        case 6:
            return "Jun"
        case 7:
            return "Jul"
        case 8:
            return "Aug"
        case 9:
            return "Sep"
        case 10:
            return "Okt"
        case 11:
            return "Nov"
        case 12:
            return "Dez"
        default:
            return ""
        }
    }
    
    static func getDayString(day: Int) -> String{
        switch day {
        case 2:
            return "Mo."
        case 3:
            return "Di."
        case 4:
            return "Mi."
        case 5:
            return "Dö."
        case 6:
            return "Fr."
        case 7:
            return "Sa."
        case 1:
            return "So."
        default:
            return ""
        }
    }
    
    static func getDayStringFull(day: Int) -> String{
        switch day {
        case 2:
            return "Montag"
        case 3:
            return "Dienstag"
        case 4:
            return "Mittwoch"
        case 5:
            return "Dönnerstag"
        case 6:
            return "Freitag"
        case 7:
            return "Samstag"
        case 1:
            return "Sonntag"
        default:
            return ""
        }
    }
    
    func removeItalicTag() -> String{
        
        let text = self.replacingOccurrences(of: "[k]", with: "").replacingOccurrences(of: "[/k]", with: "").replacingOccurrences(of: "[", with: "").replacingOccurrences(of: "]", with: "")
        return text
    }
    
    func slice(from: String, to: String) -> String? {

            return (range(of: from)?.upperBound).flatMap { substringFrom in
                (range(of: to, range: substringFrom..<endIndex)?.lowerBound).map { substringTo in
                    String(self[substringFrom..<substringTo])
                }
            }
        }
    
    func removeApostrophe() -> String{
        
        let text = self.split(separator: "\'")[0]
        return String(text).removeCharacter(separator: "‘").removeCharacter(separator: "×")
    }
    
    func removeUnusedChar() -> String{
        var text = self.components(separatedBy: "-")[0]
        text = text.components(separatedBy: "'")[0]
        text = text.components(separatedBy: "‘")[0]
        text = text.components(separatedBy: "‘")[0]

        return text
//        return self.replacingOccurrences(of: "-", with: "").replacingOccurrences(of: "'", with: "").replacingOccurrences(of: "‘", with: "").removeCharacter(separator: "’")
    }
    
    func removeCharacter(separator: Character) -> String{
        return String(self.split(separator: separator)[0])
    }
}
