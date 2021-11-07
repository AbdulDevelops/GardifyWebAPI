//
//  TodoDetailViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 31.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class TodoDetailViewController: UIViewController {

    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var todoTitleLabel: UILabel!
    @IBOutlet weak var todoDescLabel: UILabel!
    @IBOutlet weak var todoDateLabel: UILabel!
    @IBOutlet weak var todoPlantName: UILabel!
    
    var todoId: Int?
    var userPlantData: GardenUserPlantModel?
    
    override func viewDidLoad() {
        super.viewDidLoad()

        
        self.showSpinner(onView: self.view)
        
        pageConfiguration()
        
            self.getTodoDetail()

        
        
        // Do any additional setup after loading the view.
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(false)
        
        self.updateNavigationBar(isMain: false, "TO-DO", "", "main_todoCalender")
    }
    
    
    func pageConfiguration(){
        
        self.applyTheme()
        self.configurePadding()
        self.outerView.addBorderRadius()
        self.outerView.backgroundColor = .white
    }
    
    func getTodoDetail(){
        
        NetworkManager().requestDataAsync(type: TodoCyclicModel.self, APP_URL.TO_DO_CYCLIC_DETAIL+"\(todoId!)", printRequest: true){response in
            
            if !response.success{
                
                self.removeSpinner()
                return
            }
//            self.removeSpinner()
            
            let todo = response.result as! TodoCyclicModel
            self.getUserPlantDetail(refId: todo.ReferenceId)

            self.fillDetailsInfo(todo: todo)
            
        }
    }
    
    func getUserPlantDetail(refId: Int){
        
        NetworkManager().requestDataAsync(type: GardenUserPlantModel.self, APP_URL.MY_GARDEN_USER_PLANTS + "\(refId)"){response in
            
            if !response.success{
                
                self.removeSpinner()
//                self.ShowBackAlert(message: response.result as! String)
                return
            }
            self.removeSpinner()
            self.userPlantData = response.result as! GardenUserPlantModel

            self.todoPlantName.text = self.userPlantData?.Name
        }
    }
    
    @IBAction func goToPlantDetail(_ sender: Any) {
        if self.userPlantData == nil{
            return
        }
        self.goToPlantDetails(plantId: self.userPlantData!.PlantId)
    }
    
    func fillDetailsInfo(todo: TodoCyclicModel){
        
        self.todoTitleLabel.text = todo.Title
        self.todoDescLabel.text = todo.Description
        self.todoDateLabel.text = "zwischen \(todo.DateStart.toDateString("yyyy-MM-dd'T'hh:mm:ss", output: "dd.MM.")) und \(todo.DateEnd.toDateString("yyyy-MM-dd'T'hh:mm:ss", output: "dd.MM."))"
    }

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}
