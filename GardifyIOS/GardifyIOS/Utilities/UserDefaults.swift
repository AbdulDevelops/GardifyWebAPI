//
//  UserDefaults.swift
//  GardifyText
//
//  Created by Netzlab on 30.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import Foundation
import UIKit

enum UserDefaultKeys: String{
    
    case Username, UserId, Email, ExpireDate, IsLoggedIn, JWT, plantSort, plantTotal, todoMonth, todoYear, darkmode, imageStorage, warningCount, warningNotif, lastNewsRead, lastVideoRead, lastWarningRead, plantSearchCategory, totalGardenArea, totalUsedArea
    var key: String{
        return rawValue
    }
    
    func set(_ data: Any?){
        UserDefaults.standard.set(data, forKey: key)
    }
    
    func any() -> Any?{
        UserDefaults.standard.object(forKey: key)
    }
    
    func string() -> String?{
        UserDefaults.standard.string(forKey: key)
    }
    
    func stringArr() -> [String]?{
        UserDefaults.standard.stringArray(forKey: key)
    }
    
    func bool() -> Bool{
        UserDefaults.standard.bool(forKey: key)
    }
    
    func int() -> Int{
        UserDefaults.standard.integer(forKey: key)
    }
    
    
}


func storeScanImage(image: UIImage){
    
    var currentImage = UserDefaultKeys.imageStorage.stringArr()
    let imageKey = Date().toString(output: "yyyyMMdd_hhmmss")
    if currentImage == nil {
        
        
        UserDefaultKeys.imageStorage.set([imageKey])
        store(image: image, forKey: imageKey)
        
    }
    else if currentImage!.count < 99{
        currentImage?.append(imageKey)
        
        UserDefaultKeys.imageStorage.set(currentImage)
        store(image: image, forKey: imageKey)
    }
//    else{
//        let removedImage = currentImage?.removeFirst()
//        removeImageFromStorage(forKey: removedImage!)
//        currentImage?.append(imageKey)
//
//        UserDefaultKeys.imageStorage.set(currentImage)
//        store(image: image, forKey: imageKey)
//    }
}


func getScanImage() -> [UIImage?]{
    var currentImage = UserDefaultKeys.imageStorage.stringArr()
    
    if currentImage == nil {
        
        
        return []
        
    }
    else{
        
        var output: [UIImage?] = []
        for image in currentImage!{
            output.append(retrieveImage(forKey: image))
        }
        
        return output
    }
}

func userLogOut(){
    
    UserDefaultKeys.IsLoggedIn.set(false)
    UserDefaultKeys.JWT.set(nil)
}

func updatePlantCount(completion: @escaping (Bool) -> Void){
    
    if !UserDefaultKeys.IsLoggedIn.bool(){
        completion(false)
        return
    }
    
    NetworkManager().requestDataAsync(type: PlantCountModel.self, APP_URL.USER_TOTAL_PLANT){response in
        if response.success{
            let result = response.result as! PlantCountModel
            UserDefaultKeys.plantSort.set(result.Sorts)
            UserDefaultKeys.plantTotal.set(result.Total)
            
            completion(true)
            return
        }
        
        completion(false)
        return
    }
    
}

func updateTodoCount(completion: @escaping (Bool) -> Void){
    if !UserDefaultKeys.IsLoggedIn.bool(){
        completion(false)
        return
    }
    
    NetworkManager().requestDataAsync(type: TodoCountModel.self, APP_URL.USER_TOTAL_TODOES){response in
        if response.success{
            let result = response.result as! TodoCountModel
            UserDefaultKeys.todoMonth.set(result.AllTodosOfTheMonth)
            UserDefaultKeys.todoYear.set(result.AllTodos)
            
            completion(true)
            return
        }
        
        completion(false)
        return
    }
}

func setLastWarningTime(){
//    UserDefaultKeys.lastWarningRead.set(Date().toString(output: "yyyy-MM-dd'T'HH:mm:ss'Z'"))
}

func updateWarningCount(completion: @escaping (Bool) -> Void){
    if !UserDefaultKeys.IsLoggedIn.bool(){
        completion(false)
        return
    }
    
    UserDefaultKeys.warningNotif.set(false)
    
    NetworkManager().requestDataAsync(type: [WarningModel].self, APP_URL.USER_WARNING_COUNT, printRequest : false){response in
        if response.success{
            let result = response.result as! [WarningModel]
            
            UserDefaultKeys.warningCount.set(result.filter({!$0.Dismissed}).count)
        
//            if UserDefaultKeys.lastWarningRead.string() == nil{
//                completion(true)
//                return
//            }
//            completion(true)

            let isAWarning = result.filter({!$0.Dismissed })
            

            if isAWarning.count < 1{
                UserDefaultKeys.warningNotif.set(false)

                completion(true)
                return
            }
//
            UserDefaultKeys.warningNotif.set(true)

            completion(true)
            return
        }
        
        completion(false)
        return
    }
}

func setLastVideoTime(){
    UserDefaultKeys.lastVideoRead.set(Date().toString(output: "yyyy-MM-dd'T'HH:mm:ss'Z'"))
}

func updateLastVideoTime(completion: @escaping (Int) -> Void){
    
    var videoListData: [VideoModel] = []

    NetworkManager().requestDataAsync(type: [VideoModel].self, APP_URL.VIDEO_LIST){response in
        
        if !response.success{
            completion(videoListData.count)
            return
        }
        
        videoListData = response.result as! [VideoModel]
        
        if UserDefaultKeys.lastVideoRead.string() == nil{
            completion(videoListData.count)
            return
        }
        
        videoListData = videoListData.filter({$0.Date > UserDefaultKeys.lastVideoRead.string()!})
        
        completion(videoListData.count)
        return
 
    }
}

func setLastNewsTime(){
    UserDefaultKeys.lastNewsRead.set(Date().toString(output: "yyyy-MM-dd'T'HH:mm:ss'Z'"))

}

func updateLastNewsTime(completion: @escaping (Int) -> Void){
    
    var newsListData: [NewsEntryModel] = []
    
    NetworkManager().requestDataAsync(type: NewsModel.self, APP_URL.NEWS_LIST+"?take=10", printRequest: false){response in
        
        if !response.success{
            completion(newsListData.count)

            return
        }
        
        let newsData = response.result as! NewsModel
        
//        if newsData.ListEntries == nil{
//            completion(newsListData.count)
//
//        }
        
        newsListData = newsData.ListEntries
        if UserDefaultKeys.lastNewsRead.string() == nil{
            completion(newsListData.count)
            return
        }
        print("last news is", UserDefaultKeys.lastNewsRead.string())
        newsListData = newsListData.filter({$0.Date > UserDefaultKeys.lastNewsRead.string()!})
        
        completion(newsListData.count)
        return

    }
    
}

func getPlantCount() ->  String{
    
    return "\(UserDefaultKeys.plantSort.int())/\(UserDefaultKeys.plantTotal.int())"
}
