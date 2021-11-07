//
//  Networking.swift
//  GardifyText
//
//  Created by Netzlab on 30.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import Alamofire
import Alamofire_Synchronous
import UIKit

private let validStatusCodes = 200..<300

struct RequestModel{
    var success: Bool
    var result: Any?
}

struct MessageModel: Codable{
    let Message: String
}

class NetworkManager{
    func _getTokenHeaders() -> HTTPHeaders{
        var headers: HTTPHeaders = [:]
        
        if !UserDefaultKeys.IsLoggedIn.bool(){
            return headers
        }
        
        guard let token = UserDefaultKeys.JWT.string() else{
            return headers
        }
        
        headers["Authorization"] = "Bearer \(token)"
        return headers
    }
    
    func _prepareRequest(_ method: HTTPMethod = .get, _ url: String, _ parameters: Parameters? = nil, encoding: ParameterEncoding) ->  DataRequest
    {
        let headers = _getTokenHeaders()
        
        return Alamofire.request(url, method: method, parameters: parameters, encoding: encoding , headers: headers)
    }
    
    func requestJsonAsync<T: Encodable>(type: T.Type,_ url: String, _ body: Any? = nil, method: String, printRequest: Bool = false, encoding: ParameterEncoding = JSONEncoding.default, completion: @escaping (RequestModel) -> Void){
        
        var output : RequestModel = RequestModel(success: false, result: "")
        let headers = _getTokenHeaders()

        var apiRequest = URLRequest(url: URL(string: url)!)
        
        apiRequest.addValue("application/json", forHTTPHeaderField: "Content-Type")
        apiRequest.addValue(headers["Authorization"]!, forHTTPHeaderField: "Authorization")
        apiRequest.httpMethod = method
        
        let jsonEncoder = JSONEncoder()
        let jsonData = try? jsonEncoder.encode(body as! T)
//        let json = String(data: jsonData, encoding: String.Encoding.utf16)
//        let data = try? JSONSerialization.data(withJSONObject: body)
        
        let jsonString = String(data: jsonData!, encoding: String.Encoding.utf8)
//         let jsonString = "[9,11]"
//
        apiRequest.httpBody = jsonString!.data(using: .utf8)
        
        print(apiRequest.httpBody)
        let request = Alamofire.request(apiRequest)
        
        print(request.responseJSON())
//        completion(output)

        request.responseData{response in

            print(response.request, response.result)
            completion(output)

        }

    }
    
    func requestRawAsync<T:Decodable>(type: T.Type,_ url: String, _ body: Any? = nil, method: String, printRequest: Bool = false, encoding: ParameterEncoding = JSONEncoding.default, completion: @escaping (RequestModel) -> Void){
        
        var output : RequestModel = RequestModel(success: false, result: "")
        let headers = _getTokenHeaders()

        var apiRequest = URLRequest(url: URL(string: url)!)
        
        apiRequest.addValue("application/json", forHTTPHeaderField: "Content-Type")
        apiRequest.addValue(headers["Authorization"]!, forHTTPHeaderField: "Authorization")
        apiRequest.httpMethod = method
        
      
        let data = try? JSONSerialization.data(withJSONObject: body)
        
        let jsonString = String(data: data!, encoding: String.Encoding.utf8)
//         let jsonString = "[9,11]"
//
        apiRequest.httpBody = jsonString!.data(using: .utf8)
        
        print(apiRequest.httpBody)
        let request = Alamofire.request(apiRequest)
        
        request.responseData{response in
            
            print(response.request, response.result)
            completion(output)

        }

        
//
//        request.responseData{ response in
//
//            if printRequest{
//                print(request.request)
//            }
//            switch response.result{
//            case .failure(let error):
//                output.result = "There is network Problem"
//            case .success(let data):
//                if !validStatusCodes.contains(response.response!.statusCode){
//                    do {
//                        let errorData = try JSONDecoder().decode(MessageModel.self, from: data)
//                        output.result = errorData.Message
//
//                    }
//                    catch let error{
//                        output.result = "There is network Problem"
//                    }
//                }
//                else{
//                    do {
//                        let elements = try JSONDecoder().decode(T.self, from: data)
//                        output.success = true
//                        output.result = elements
//                    }
//                    catch let error{
//                        output.result = "There is network Problem"
//                    }
//                }
//
//            }
//            DispatchQueue.main.async {
//                completion(output)
//            }
//        }
    }
    
    
    func requestData<T:Decodable>(type: T.Type,_ url: String, _ parameters: Parameters? = nil, encoding: ParameterEncoding = JSONEncoding.default, method: HTTPMethod = .get) -> RequestModel{
        
        var output : RequestModel = RequestModel(success: false, result: "")
        let request = _prepareRequest(method, url, parameters, encoding: encoding)
        let response = request.responseData()

        switch response.result{
        case .failure(let error):
            output.result = error
        case .success(let data):
            if !validStatusCodes.contains(response.response!.statusCode){
                do {
                    let errorData = try JSONDecoder().decode(MessageModel.self, from: data)
                    output.result = errorData.Message
                    
                }
                catch let error{
                    output.result = error
                }
            }
            else{
                do {
                    let elements = try JSONDecoder().decode(T.self, from: data)
                    output.success = true
                    output.result = elements
                }
                catch let error{
                    output.result = error
                }
            }
            
        }
        
        
        
        
        return output
    }
    
    func requestDataAsync<T:Decodable>(type: T.Type,_ url: String, _ parameters: Parameters? = nil, method: HTTPMethod = .get, ignoreContent: Bool = false, failParse: Bool = false , printRequest: Bool = false ,  encoding: ParameterEncoding = JSONEncoding.default, completion: @escaping (RequestModel) -> Void){
        
        var output : RequestModel = RequestModel(success: false, result: "")
        let request = _prepareRequest(method, url, parameters, encoding: encoding)
        
        if printRequest{
            print(request.responseJSON())
        }
        request.responseData{ response in
            
            if printRequest{
                print(request.request)
            }
            switch response.result{
            case .failure(let error):
                print(url)

                print("error network is: ", error)
                output.result = error.localizedDescription


                
            case .success(let data):
              
                
               if !validStatusCodes.contains(response.response!.statusCode){
                    do {
                        let errorData = try JSONDecoder().decode(MessageModel.self, from: data)
                        output.success = false
                        output.result = errorData.Message
                        
                    }
                    catch let error{
                        print(url)
                        print("error network is: ", error)
                        output.success = false

                        output.result = error
                    }
                }
                else{
                    if ignoreContent{
                        output.success = true
                        output.result = "success"
                    }
                    else{
                        do {
                            let elements = try JSONDecoder().decode(T.self, from: data)
                            output.success = true
                            output.result = elements
                        }
                        catch let error{
                            print(url)

                            print("error network is: ", error)
                            output.result = error
                        }
                    }
                   
                }
                
            }
            DispatchQueue.main.async {
                completion(output)
            }
        }
    }
    

    
    func getImageFromUrl(urlString: String, completion: @escaping (UIImage) -> (Void)){
        Alamofire.request(urlString).response{ response in
            let image = UIImage(named: "PlantPlaceholder")
            
            guard let data = response.data else{
                completion( image!)
                return
            }
            
            guard let outimage = UIImage(data: data) else{
                completion( image!)
                return
            }
            completion(outimage)
            
        }
    }
    
    func getImageFromUrlSync(urlString: String) -> UIImage{
        
        
        
        let response = Alamofire.request(urlString).response()
        guard let data = response.data else{
            let image = UIImage(named: "PlantPlaceholder")
            return image!
        }
        return UIImage(data: data, scale: 1)!
        
    }
    
}

