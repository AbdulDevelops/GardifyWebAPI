//
//  GardeningAZViewController.swift
//  
//
//  Created by Netzlab on 04.09.20.
//

import UIKit

class GardeningAZViewController: UIViewController {

    @IBOutlet weak var gardenAZTableView: UITableView!
    
    var gardenAZData: [GardeningAZModel] = []
    var gardenImageList: [UIImage] = []
    @IBOutlet weak var cardImage: UIImageView!
    @IBOutlet weak var listImage: UIImageView!
    @IBOutlet weak var askQuestionButton: UIButton!
    @IBOutlet weak var questionTextField: UITextView!
    
    var isListMode: Bool = false
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        self.applyTheme()
        self.showSpinner(onView: self.view)
        self.pageConfiguration()
        DispatchQueue.global(qos: .background).async {
            self.loadGardeningAZData()

        }
        // Do any additional setup after loading the view.
        
    }
    
    func pageConfiguration(){
        self.gardenAZTableView.backgroundColor = .clear
        
        self.askQuestionButton.setGreenButton()
        self.questionTextField.addBorderRadius()
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        
        self.updateNavigationBar(isMain: false, "GARDIFY", "GLOSSAR", "main_knowhow")

    }
    
    func updatePageMode(){
        if isListMode{
            listImage.image = UIImage(named: "doc_list_on")
            cardImage.image = UIImage(named: "doc_card_off")
        }
        else{
            listImage.image = UIImage(named: "doc_list_off")
            cardImage.image = UIImage(named: "doc_card_on")
        }
    }
    @IBAction func onListMode(_ sender: Any) {
        isListMode = true
        updatePageMode()
        self.gardenAZTableView.reloadData()
    }
    
    @IBAction func onCardMode(_ sender: Any) {
        isListMode = false
        updatePageMode()
        self.gardenAZTableView.reloadData()
    }
    func loadGardeningAZData(){
        
        NetworkManager().requestDataAsync(type: [GardeningAZModel].self, APP_URL.GARDENING_AZ_LIST, printRequest: true){response in
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                return
            }
            self.removeSpinner()
            
            
            self.gardenAZData = response.result as! [GardeningAZModel]
            
            DispatchQueue.main.async {
                self.getAllImages()
                self.reloadTableview()
            }
            

        }
    }
    
    func getAllImages(){
        
        let imageUrlList = gardenAZData.reduce(into: [String]()){output, input in
            var raw = input.Images![0].SrcAttr
            output.append((raw?.toURL(false, false))!)
        }
        
        print(imageUrlList)
        getImageFiles(files: imageUrlList){images in
            self.gardenImageList = images
            self.reloadTableview()
        }
        
    }
    
    func sendContactMessage(){
        let params : [String: Any?] = [
            "Email" : UserDefaultKeys.Email.string() ?? "",
            "Subject" : "GardeningAZ Thema",
            "Text" : questionTextField.text
        ]
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.ACCOUNT_CONTACT, params, method: .post){response in
            self.ShowAlert(message: "Deine Nachricht wurde versendet!")
        }
    }

    @IBAction func onMessageSend(_ sender: Any) {
        sendContactMessage()
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

extension GardeningAZViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return gardenAZData.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        if isListMode{
            let cell = tableView.dequeueReusableCell(withIdentifier: "gardenAZListCell", for: indexPath) as! GardeningAZListTableViewCell
            cell.parentTable = self.gardenAZTableView
            cell.titleLabel.text = gardenAZData[indexPath.row].Name
            cell.descText = gardenAZData[indexPath.row].Description
            cell.onConfigure()

            return cell
        }
        let cell = tableView.dequeueReusableCell(withIdentifier: "gardenAZCell", for: indexPath) as! GardeningAZTableViewCell
        
        cell.parentTable = self.gardenAZTableView
        cell.titleLabel.text = gardenAZData[indexPath.row].Name.capitalized
        if self.gardenImageList.count >= (gardenAZData.count){
            cell.gardeningImageView.image = self.gardenImageList[indexPath.row]
        }
        cell.gardeningDescription.text = gardenAZData[indexPath.row].Description
        cell.onConfigure()
        return cell
    }
    
    
    func reloadTableview(){
        self.gardenAZTableView.reloadData()
    }
}
