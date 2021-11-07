//
//  Colors.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 19.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

let ColorsKey: [String: UIColor] = [
    "rot": .red,
    "weiß": .white,
    "beige": rgba(233, 215, 187, 1),
    "gelb": .yellow,
    "gelblich": rgb(255,255,153),
    "hellorange": .orange,
    "orange": rgb(255, 92, 32),
//    "oliv": rgba(233, 215, 187, 1),
    "blau": rgba(0, 81 , 255, 1),
    "petrol": rgba(20, 96, 126, 1),
    "grau": rgba(172, 172, 172, 1),
    "grün": .green,
//    "dunkelgrau": rgba(233, 215, 187, 1),
    "schwarz": .black,
    "braun": .brown,
    "pink": rgb(255, 111, 199),
    "apricot": rgb(245, 149, 86) ,
    "creme": rgb(231, 225, 204),
    "violett": rgb(189, 125, 241),
    "rosa": rgb(250, 57, 176),
    "purpur": rgb(226, 21, 245),
    "hellblau": rgb(83, 169, 226),
    "dunkelrot": rgb(175, 0, 0),
    "blaugrün": rgb(33, 201, 187),
    "gelbgrün": rgb(166, 190, 25),
    "rotbraun": rgb(119, 18, 18),
    "lachsfarben": rgb(236, 179, 179),
    "blauviolett": rgb(55, 78, 172),
    "silbrigweiß": rgb(204, 204, 204) ,
    "fliederfarben": rgb(192, 147, 199),

]



func rgb(_ r: Int, _ g: Int, _ b: Int) -> UIColor{
    return UIColor(displayP3Red: (CGFloat(r)/255), green: (CGFloat(g)/255), blue: (CGFloat(b)/255), alpha: 1)
}

func rgba(_ r: Int, _ g: Int, _ b: Int, _ a: CGFloat) -> UIColor{
    return UIColor(displayP3Red: (CGFloat(r)/255), green: (CGFloat(g)/255), blue: (CGFloat(b)/255), alpha: a)
}


extension UIColor{
    static var Background: UIColor{
        return UIColor(named: "Background")!
    }
    
    static var ViewBackground: UIColor{
        return UIColor(named: "ViewBackground")!
    }
}
