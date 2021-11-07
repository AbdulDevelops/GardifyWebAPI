//
//  PlantDocDetailViewController.swift
//  GardifyText
//
//  Created by Netzlab on 27.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantDocDetailViewController: UIViewController , UITextFieldDelegate{

    var questionId: Int?
    
    @IBOutlet weak var docImage: UIImageView!
    
    @IBOutlet weak var outerView: UIView!
    @IBOutlet weak var dateText: UILabel!
    @IBOutlet weak var docTitleText: UILabel!
    
    @IBOutlet weak var notifImage: UIImageView!
    @IBOutlet weak var answerText: UILabel!
    @IBOutlet weak var moreInfoButton: UIButton!
    @IBOutlet weak var descriptionText: UILabel!
    
    @IBOutlet weak var frageLabel: UILabel!
    
    @IBOutlet weak var frageFirstAnswerLabel: UILabel!
    
    @IBOutlet weak var answerTableView: UITableView!
    
    @IBOutlet weak var answerTextView: UITextView!
    var descriptionDelegate: PlantDocDetailsDelegate?
    var plantDocData: PlantDoc?
    var plantDocAnswer: PlantDocDetailModel?
    
    @IBOutlet weak var answerButton: UIButton!
    var isDetailExtended: Bool = true

    @IBOutlet weak var submitAnswerButton: UIButton!
    @IBOutlet weak var imageCollectionView: UICollectionView!
    
    @IBOutlet weak var answerView: UIView!
    @IBOutlet weak var answerViewHeight: NSLayoutConstraint!
    var imageList: [UIImage] = []
    var selectedImage = 0

    var isAlertOn: Bool = false
    var isAnswerExtended = false
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.applyTheme()
        self.configurePadding()
        pageConfiguration()
        loadAnswer()
        // Do any additional setup after loading the view.
    }
    
    func getImage(){
        
//        let imageURL = (self.plantDocData?.Images![0].SrcAttr?.toURL(false, false, false))!
        
        let imageURLList = (self.plantDocData?.Images!.reduce(into: [String]()){output, input in
            var raw = input.SrcAttr
            output.append(raw!.toURL(false, false, false))
        })!
        
  
        getImageFiles(files: imageURLList){image in
            self.imageList = image
            
            self.docImage.image = self.imageList[0]
            
            self.imageCollectionView.reloadData()
        }
    }
    
    func pageConfiguration(){
        
        if plantDocData == nil{
            return
        }
        
        let data = self.plantDocData!
        
        getImage()
        
        answerText.text = "\(data.TotalAnswers) Antwort"
        dateText.text = data.PublishDate?.toDateString(output: "dd.MM.yyyy")
        docTitleText.text = data.Thema
        descriptionText.text = data.Description
        
//        self.backgroundColor = .clear
//        self.contentView.backgroundColor = .clear
        self.notifImage.tintColor = UIColor(named: "GardifyGreen")
        
        outerView.addBorderRadius()
        answerTableView.backgroundColor = .clear

        answerButton.setGreenButton()
        submitAnswerButton.setGreenButton()
        
        answerTextView.addBorderRadius()

    }
    
    func updateAnswerViewPop(){
        var alphaValue = 0
        
        if isAnswerExtended {
            alphaValue = 1
            
        }
        
        UIView.animate(withDuration: 0.25, animations: {
            self.view.layoutIfNeeded()
            self.answerView.alpha = CGFloat(alphaValue)
        })
    }
    
    @IBAction func onAnswerClicked(_ sender: Any) {
        
        isAnswerExtended = !isAnswerExtended
        updateAnswerViewPop()
    }
    
    
    func loadAnswer(){
        self.showSpinner(onView: self.view)
        
        if plantDocData == nil{
            return
        }
        
        DispatchQueue.main.async {
            NetworkManager().requestDataAsync(type: PlantDocDetailModel.self, APP_URL.PLANT_DOC_ROUTE + "//\(self.plantDocData!.QuestionId)/getEntry", printRequest: true){response in
                if !response.success{
                    self.ShowAlert(message: response.result as! String)
                    self.removeSpinner()
                    return
                }
                self.removeSpinner()
                self.plantDocAnswer = response.result as! PlantDocDetailModel
                self.updateAnswer()
       
            }
        }
        
        
    }
    
    @IBAction func onTouch(_ sender: Any) {
        print("touched")
        view.endEditing(true )
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("touched")
        view.endEditing(true )
    }
    
    func updateAnswer(){
        frageLabel.text = "Frage"
        
        frageFirstAnswerLabel.text = self.plantDocAnswer?.PlantDocViewModel.QuestionText

        self.answerTableView.reloadData()

    }
    
    func updateNotifMark(){
        if !isAlertOn{
            notifImage.image = nil
            return
        }
        
        notifImage.image = UIImage(systemName: "exclamationmark.circle.fill")

    }

    @IBAction func onSubmitAnswer(_ sender: Any) {
        
        
        isAnswerExtended = false
        updateAnswerViewPop()
        let params: [String: Any?] = [
            "AnswerText" : answerTextView.text,
            "AuthorId" : UserDefaultKeys.UserId.string(),
            "PlantDocEntryId": self.plantDocData?.QuestionId
        ]
        print("params is", params)
        
        
//        print(response)
        
        NetworkManager().requestDataAsync(type: Int.self, APP_URL.PLANT_DOC_ANSWER, params, method: .post ){response in

            self.viewDidLoad()
            print(response)

        }
    }
    
}


extension PlantDocDetailViewController: UITableViewDelegate, UITableViewDataSource{
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return (self.plantDocAnswer?.PlantDocAnswerList.count) ?? 0
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "plantDocAnswerCell", for: indexPath) as! PlantDocAnswerTableViewCell
        let data = self.plantDocAnswer?.PlantDocAnswerList[indexPath.row]
//        cell.parent = self
        cell.tableParent = self.answerTableView
        cell.dateLabel.text = data?.Date!.toDateString(output: "dd.MM.yyyy")
        cell.senderLabel.text = "Antwort von " + data!.AutorName
        cell.contentLabel.text = data?.AnswerText
        cell.outerView.backgroundColor = .systemBackground
        cell.adminImage.alpha = 0
        
        if data!.IsAdminAnswer{
            cell.senderLabel.text = "              " + cell.senderLabel.text!
            cell.adminImage.alpha = 1
            cell.outerView.backgroundColor = UIColor(named: "GardifyGreen")
            cell.dateLabel.textColor = .white
            cell.senderLabel.textColor = .white
            cell.contentLabel.textColor = .white
        }
        
        cell.configureCell()
        
        return cell
    }
    
    
}


extension PlantDocDetailViewController: UICollectionViewDelegate, UICollectionViewDataSource{
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return self.imageList.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "imageCell", for: indexPath) as! PlantDocImageCollectionViewCell
        
        cell.imageCell.image = self.imageList[indexPath.row]
        
        cell.imageCell.alpha = 0.5
        
        if selectedImage == indexPath.row{
            cell.imageCell.alpha = 1
        }
        
        return cell
    }
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        selectedImage = indexPath.row
        self.docImage.image = imageList[selectedImage]
        
        imageCollectionView.reloadData()
    }
    
    
}
