//
//  TodoEditViewController.swift
//  GardifyIOS
//
//  Created by Netzlab on 18.11.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class TodoEditViewController: UIViewController {

    var todoData: TodoModel?
    @IBOutlet weak var titleTextField: UITextField!
    
    @IBOutlet weak var descriptionTextArea: UITextView!
    
    @IBOutlet weak var noticeTextArea: UITextView!
    
    @IBOutlet weak var cancelButton: UIButton!
    
    @IBOutlet weak var saveButton: UIButton!
    
    var todoDelegate: todoOptionDelegate?
    
    override func viewDidLoad() {
        super.viewDidLoad()

        self.configurePadding()
        pageConfiguration()
        // Do any additional setup after loading the view.
    }
    
    func pageConfiguration(){
        
        cancelButton.setGrayButton()
        saveButton.setGreenButton()
        titleTextField.text = todoData?.Title
        descriptionTextArea.text = todoData?.Description
        descriptionTextArea.addBorderWidth()
        noticeTextArea.addBorderWidth()
        descriptionTextArea.addBorderRadiusSmall()
        noticeTextArea.addBorderRadiusSmall()
        noticeTextArea.text = todoData?.Notes
        
    }
    
    @IBAction func onCancel(_ sender: Any) {
        self.navigationController?.popViewController(animated: true)
    }
    
    @IBAction func onSave(_ sender: Any) {
        
        let params: [String: Any?] = [
            "CyclicId" : todoData?.CyclicId,
            "DateEnd" : todoData?.DateEnd,
            "DateStart" : todoData?.DateStart,
            "Description" : descriptionTextArea.text,

            "Id" : todoData?.Id,

            "Finished" : todoData?.Finished,
            "Ignored" : todoData?.Ignored,
            "UserId" : todoData?.UserId,
            "Deleted" : todoData?.Deleted,
            "Title" : titleTextField.text,
            "Notes" : noticeTextArea.text
        ]
        
        self.todoDelegate?.onEdit(params: params, todoId: self.todoData!.Id)
        
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        print("touched")
        
        view.endEditing(true)
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
