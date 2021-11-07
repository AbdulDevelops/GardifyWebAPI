//
//  Parameters.swift
//  GardifyIOS
//
//  Created by Netzlab on 04.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import Alamofire

extension Parameters{
    
    func getRawBody() -> String{
        var output: [String] = []
        for param in self{
            print("params value is", param.value)
            var values = "\(param.key)=\(param.value as? String ?? String(describing: param.value ?? "").addingPercentEncoding(withAllowedCharacters: .urlHostAllowed)!)"
            
            print("final value is", values, values)
//            if values.contains(" "){
//                values = values.replacingOccurrences(of: " ", with: "%20")
//            }
            output.append(values.addingPercentEncoding(withAllowedCharacters: .urlHostAllowed)!)
        }
        return "?\(output.joined(separator: "&"))"
    }
}
