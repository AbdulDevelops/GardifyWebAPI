//
//  PlantNameSearchViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 21.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantNameSearchViewController: UIViewController, UITextFieldDelegate {
    @IBOutlet weak var nameTextField: UITextField!
    
    @IBOutlet weak var searchTableView: UITableView!
    var plantName: String = ""
    var plantNameList: [String] = []
    var plantNameFilter: [String] = []
    var plantDelegate: plantSearchDelegate?
    
    @IBOutlet weak var outerView: UIView!
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()

        
        nameTextField.text = plantName
        updateNameList()
        
        outerView.setWhiteButtonView()
        nameTextField.becomeFirstResponder()
        nameTextField.addTarget(self, action: #selector(textFieldChange), for: .editingChanged)
        // Do any additional setup after loading the view.
    }
    
    
    
    func updateNameList(){
        plantName = plantName.trimmingCharacters(in: .whitespacesAndNewlines)
        if nameTextField.text == nil || nameTextField.text == ""{
            self.plantNameFilter = []
            self.searchTableView.reloadData()
        }
        else{
            getPlantNameSearch()
//            self.plantNameFilter = self.plantNameList.filter({$0.lowercased().contains(plantName.lowercased())})
        }
        
        
        
    }
    
    func getPlantNameSearch(){
        
        let plantNameParam = plantName.replacingOccurrences(of: " ", with: "").lowercased().removeUnusedChar().trimmingCharacters(in: .whitespacesAndNewlines).addingPercentEncoding(withAllowedCharacters: .urlHostAllowed)
        let urlText = APP_URL.PLANT_SEARCH_API + "/getplantwithname?plantName=\(plantNameParam!)"
        
//        let url = URL(fileURLWithPath: urlText)
////        print(url?.absoluteString, APP_URL.PLANT_SEARCH_API + "/getplantwithname/\(plantName.lowercased().removeUnusedChar().trimmingCharacters(in: .whitespacesAndNewlines))")
////
//
//        if url == nil{
//            return
//        }
//        let finalUrl = url.absoluteString.components(separatedBy: "file:///")
        print("before:",urlText )
        NetworkManager().requestDataAsync(type: [String].self, urlText){response in
            
            if !response.success{
                self.plantNameFilter = []
                self.searchTableView.reloadData()

                return
            }
            
            self.plantNameFilter = response.result as! [String]
            
            self.searchTableView.reloadData()

        }
    }
    
    @objc func textFieldChange(){
        plantName = nameTextField.text ?? ""
//        plantDelegate?.updateNameSearch(name: self.nameTextField.text ?? "")

        updateNameList()
    }
    

    func textFieldShouldReturn(_ textField: UITextField) -> Bool {
        view.endEditing(true    )

        plantDelegate?.updateNameSearch(name: self.nameTextField.text ?? "")
        self.navigationController?.popViewController(animated: false)
        return true
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        let searchText = self.nameTextField.text?.lowercased().removeUnusedChar()
        
        plantDelegate?.updateNameSearch(name: searchText ?? "")
        view.endEditing(true    )
        self.navigationController?.popViewController(animated: false)

        
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

extension PlantNameSearchViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return plantNameFilter.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = UITableViewCell()
        
        cell.textLabel?.text = plantNameFilter[indexPath.row]
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        self.nameTextField.text = plantNameFilter[indexPath.row]
        let searchText = self.nameTextField.text?.lowercased().removeUnusedChar()
        
        plantDelegate?.updateNameSearch(name: searchText ?? "")
        self.navigationController?.popViewController(animated: false)

        
    }
    
    
    
}
