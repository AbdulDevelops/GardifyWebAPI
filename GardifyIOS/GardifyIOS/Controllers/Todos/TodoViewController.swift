//
//  TodoViewController.swift
//  GardifyIOS
//
//  Created by Team Netzlab on 31.08.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit


struct TodoCalenderModel {
    let date: String
    let title: String
    let description: String
    let id, CyclicId, ReferenceId: Int
    let type: ToDoType
    var isChecked, Deleted, Ignored: Bool
    var imageURL: String?
}

enum ToDoType {
    case todo
    case custom
    case diary
    case scan
    
    func getTodoColor() -> UIColor{
        switch self {
        case .todo:
            return rgb(190, 202, 59)
        case .custom:
            return rgb(58, 97, 136)
        case .diary:
            return rgb(201, 34, 252)
        case .scan:
            return rgb(255, 127, 35)
        default:
            return .white
        }
    }
}

protocol todoOptionDelegate {
    func onEdit(params: [String: Any?], todoId: Int)

}

extension TodoViewController: todoOptionDelegate{
    func onEdit(params: [String : Any?], todoId: Int) {
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.TO_DO_LIST+"\(todoId)/uploadTodo", params, method: .put, printRequest: true){response in
            
            DispatchQueue.global(qos: .background).async {
                self.getTodoData()
                
            }
            self.ShowBackAlert(message: "Deine Änderungen wurden gespeichert")
            
            
        }
    }
    
    
}

class TodoViewController: UIViewController {
    
    @IBOutlet weak var calenderView: UIView!
    
    @IBOutlet weak var monthLabel: UILabel!
    @IBOutlet weak var prevMonthButton: UIButton!
    @IBOutlet weak var nextMonthButton: UIButton!
    
    @IBOutlet weak var yearLabel: UILabel!
    @IBOutlet weak var prevYearButton: UIButton!
    @IBOutlet weak var nextYearButton: UIButton!
    
    @IBOutlet weak var todoTableView: UITableView!
    @IBOutlet weak var addTodoButtonContainer: UIView!
    
    @IBOutlet weak var createOwnTodoButton: UIButton!
    @IBOutlet weak var enterDiaryButton: UIButton!
    @IBOutlet weak var ecoScanButton: UIButton!
    
    @IBOutlet weak var bigCalenderView: UIView!
    @IBOutlet weak var normalCalenderHeight: NSLayoutConstraint!
    
    @IBOutlet weak var calenderModeButton: UIButton!
    @IBOutlet weak var bigCalenderHeight: NSLayoutConstraint!
    
    @IBOutlet weak var innerCalenderView: UIView!
    
    @IBOutlet weak var innerCalenderHeaderView: UIView!
    @IBOutlet weak var calenderCollectionView: UICollectionView!
    
    @IBOutlet weak var todoInfoLabel: UILabel!
    
    @IBOutlet weak var todoCustomInfoLabel: UILabel!
    @IBOutlet weak var diaryInfoLabel: UILabel!
    
    @IBOutlet weak var ecoScanInfoLabel: UILabel!
    @IBOutlet weak var infoPopUpView: UIView!
    
    var currentMonth: Int = 1
    var currentDay: Int = 0
    var currentYear: Int = 2020
    var isLargeCalenderMode: Bool = false
    var isInfoExtended = false
    
    var calenderDayList: [Day] = []
    var todoData: TodoMainModel?
    var diaryData: DiaryMainModel?
    var todoTableData: [TodoCalenderModel] = []
    
    var todoDateGroupData: [String: [TodoCalenderModel]] = [:]
    
    var todoSelectedList: String = ""
    var todoGroupList: [String] = []
    private let calendar = Calendar(identifier: .gregorian)
    
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.updateNavigationBar(isMain: false, "TO-DO", "KALENDER", "main_todoCalender")
        self.pageConfiguration()
        self.showSpinner(onView: self.view)
        
        DispatchQueue.global(qos: .background).async {
            self.setInitialCalender()
            
            DispatchQueue.main.async {
                self.updateCalenderContent()

            }
        }
        // Do any additional setup after loading the view.
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(false)
        self.updateNavigationBar(isMain: false, "TO-DO", "KALENDER", "main_todoCalender")
        
        
    }
    
    func reloadPage(){
        DispatchQueue.global(qos: .background).async {

            self.getTodoData()

            DispatchQueue.main.async {
                self.updateCalenderContent()

            }
        }
    }
    
    func pageConfiguration(){
        self.applyTheme()
        self.configurePadding()
        self.addTodoButtonContainer.isHidden = true
        self.addTodoButtonContainer.isUserInteractionEnabled = false
        
        self.calenderView.setWhiteButtonView()
        self.todoTableView.backgroundColor = .clear
        addTodoButtonContainer.setWhiteButtonView()
        ecoScanButton.setClearWhiteButton()
        createOwnTodoButton.setClearWhiteButton()
        enterDiaryButton.setClearWhiteButton()
        
        innerCalenderHeaderView.addBorderRadiusSmall()
        
        updateCalenderMode()
        popUpInfoConfiguration()
        calenderCollectionViewConfiguration()
        calenderCollectionView.reloadData()
    }
    
    func popUpInfoConfiguration(){
        
        infoPopUpView.addBorderRadius()
        
        todoInfoLabel.attributedText = getBoldText(firstText: "         To-Do ", secondText: "werden automatisch erzeugt, wenn Pflanzen in deinem Garten gespeichert werden.")
        todoCustomInfoLabel.attributedText = getBoldText(firstText: "         Eigenes To-Do ", secondText: "kannst du selbst anlegen. ")
        diaryInfoLabel.attributedText = getBoldText(firstText: "         Tagebucheintrag ", secondText: "kannst du selbst anlegen, werden aber nicht wiederholt.")
        ecoScanInfoLabel.attributedText = getBoldText(firstText: "         Ökoscan ", secondText: "hält den ökologischen Wert deines Gartens für diesen Tag fest. ")
    }
    
    @IBAction func onInfoClicked(_ sender: Any) {
        
        isInfoExtended = !isInfoExtended
        
        updateInfoPopUpModal()
    }
    
    func updateInfoPopUpModal(){
        var alphaValue = 0
        if isInfoExtended{
            alphaValue = 1
        }
        
        UIView.animate(withDuration: 0.5, animations: {
            self.view.layoutIfNeeded()
            
            self.infoPopUpView.alpha = CGFloat(alphaValue)
        })
    }
    
    
    func calenderCollectionViewConfiguration(){
        
        let layout = self.calenderCollectionView.collectionViewLayout as! UICollectionViewFlowLayout
        layout.minimumLineSpacing = 0
        layout.minimumInteritemSpacing = 0
        
        self.calenderCollectionView.translatesAutoresizingMaskIntoConstraints = false
        
        
    }
    
    
    func setInitialCalender(){
        
        let today = Date()
        //        currentDay = Int(today.toString(output: "d")) ?? 0
        //        monthLabel.text = today.toString(output: "MMM")
        //
        //        yearLabel.text = today.toString(output: "yyyy")
        currentYear = Int(today.toString(output: "yyyy")) ?? 0
        currentMonth = Int(today.toString(output: "M")) ?? 0
        
        DispatchQueue.main.async {
            self.updateDateLabel()
        }
        
        
        self.getTodoData()
        
        
    }
    
    func updateDateLabel(){
        
        monthLabel.text = String.getMonthString(month: currentMonth)
        yearLabel.text = "\(currentYear)"
        
    }
    
    func updateCalenderContent(){
        
        self.todoSelectedList = ""
        if currentDay > 0{
            self.todoSelectedList = self.todoGroupList.first(where: {$0.toDateString("yyyy-MM-dd'T'hh:mm:ss",output: "d") == "\(currentDay)"}) ?? ""
            
        }
        
        todoTableView.reloadData()
        
        calenderDayList = generateDaysInMonth()

        
        calenderCollectionView.reloadData()
    }
    
    func generateDaysInMonth() -> [Day] {
        // 2
        let currentDate = "\(currentYear)-\( currentMonth)-\((  1))-12:00".toDate("yyyy-M-d-hh:mm")!
        print(currentDate)
        
        guard let metadata = try? monthMetadata(for: currentDate) else {
            return []
            //            fatalError("An error occurred when generating the metadata for \(currentDate)")
        }
        print("metadata is", metadata)
        let numberOfDaysInMonth = metadata.numberOfDays
        let offsetInInitialRow = metadata.firstDayWeekday
        let firstDayOfMonth = metadata.firstDay
        
        // 3
        let days: [Day] = (1..<(numberOfDaysInMonth + offsetInInitialRow))
            .map { day in
                // 4
                let isWithinDisplayedMonth = day >= offsetInInitialRow
                // 5
                let dayOffset =
                    isWithinDisplayedMonth ?
                    day - offsetInInitialRow :
                    -(offsetInInitialRow - day)
                
                // 6
                return generateDay(
                    offsetBy: dayOffset,
                    for: firstDayOfMonth,
                    isWithinDisplayedMonth: isWithinDisplayedMonth,
                    currentDate: currentDate)
            }
        
        return days
    }
    
    func generateDay(
        offsetBy dayOffset: Int,
        for baseDate: Date,
        isWithinDisplayedMonth: Bool,
        currentDate: Date
    ) -> Day {
        let date = calendar.date(
            byAdding: .day,
            value: dayOffset,
            to: baseDate)
            ?? baseDate
        
        
        
        return Day(
            date: date,
            number: date.toString(output: "d"),
            isSelected: calendar.isDate(date, inSameDayAs: currentDate),
            isWithinDisplayedMonth: isWithinDisplayedMonth
        )
    }
    
    
    
    func updateCalenderMode(){
        if isLargeCalenderMode{
            normalCalenderHeight.constant = 325
            calenderModeButton.setImage(UIImage(named: "todo_calender"), for: .normal)
            innerCalenderView.alpha = 1
        }
        else{
            calenderModeButton.setImage(UIImage(named: "todo_calender_off"), for: .normal)
            normalCalenderHeight.constant = 50
            innerCalenderView.alpha = 0
        }
    }
    
    func resetCalenderDay(){
        self.currentDay = 0
        self.todoSelectedList = ""
    }
    
    func getTodoData(){
        let date = "\(currentYear)-\(currentMonth)-01T22:00:00.000".toDate()
   
        var dateComponent = DateComponents()
        dateComponent.day = -1
        let prevDate = Calendar.current.date(byAdding: dateComponent, to: date!)!
        let prevDateText =  prevDate.toString(output: "yyyy-MM-dd'T'hh:mm:ss.sss'Z'")
        
        let params: [String: Any] = [
            "period": "month",
            "startDate": prevDateText
        ]
        
        self.todoTableData = []
        self.todoGroupList = []
        
        NetworkManager().requestDataAsync(type: TodoMainModel.self, APP_URL.TO_DO_LIST + params.getRawBody(), printRequest: true){response in
            
            if !response.success{
                
                self.ShowAlert(message: response.result as! String)
                self.removeSpinner()
                print("stop spinner")

                return
            }
            
            self.todoData = response.result as! TodoMainModel
            
            self.fillFilteredTodoData()
            
            
        }
        
        NetworkManager().requestDataAsync(type: DiaryMainModel.self, APP_URL.DIARY_ROUTE + "?m=\(currentMonth)&y=\(currentYear)" ){response in
            if !response.success{
                
//                self.ShowAlert(message: response.result as! String)
                return
            }
            
            self.diaryData = response.result as! DiaryMainModel
            
            self.fillFilteredDiaryData()

        }
    }
    
    func fillFilteredTodoData(){
        guard let todoList = self.todoData?.TodoList else{
            self.removeSpinner()
            print("remove spinner")

            return
        }
        
        for todo in todoList{
            
            if self.todoTableData.contains(where: {$0.id == todo.Id}){
                continue
            }
            var currentType = ToDoType.todo
            switch todo.ReferenceType {
            case 1:
                currentType = ToDoType.todo
            case 8:
                currentType = ToDoType.diary
            case 20:
                currentType = ToDoType.scan
            case 12:
                currentType = ToDoType.custom
            default:
                currentType = ToDoType.todo
            }
            
            let newTodo = TodoCalenderModel(
                date: todo.DateStart,
                title: todo.Title,
                description: todo.Description,
                id: todo.Id,
                CyclicId: todo.CyclicId ?? -1,
                ReferenceId: todo.ReferenceId,
                type: currentType,
                isChecked: todo.Finished,
                Deleted: todo.Deleted,
                Ignored: todo.Ignored,
                imageURL: todo.EntryImages!.count > 0 ? todo.EntryImages![0].SrcAttr : nil
            )
            self.todoTableData.append(newTodo)
        }
        
        self.todoTableData = self.todoTableData.sorted(by: {$0.date < $1.date})
        
        DispatchQueue.main.async {
            self.todoTableViewSetting()

        }
    }
    
    func fillFilteredDiaryData(){
        guard let diaryList = self.diaryData?.ListEntries else{
            
            return
        }
        
        for diary in diaryList{
            
            if self.todoTableData.contains(where: {$0.id == diary.Id}){
                continue
            }
            var currentType = ToDoType.diary
    
            
            let newTodo = TodoCalenderModel(
                date: diary.Date,
                title: diary.Title,
                description: diary.Description,
                id: diary.Id,
                CyclicId: 0,
                ReferenceId: 0,
                type: currentType,
                isChecked: false,
                Deleted: false,
                Ignored: false,
                imageURL: diary.EntryImages!.count > 0 ? diary.EntryImages![0].SrcAttr?.components(separatedBy: "de\\")[1] : nil
            )
            self.todoTableData.append(newTodo)
        }
        
        self.todoTableData = self.todoTableData.sorted(by: {$0.date < $1.date})
        
        self.todoTableViewSetting()
    }
    
    func todoTableViewSetting(){
        
        self.todoDateGroupData = self.todoTableData.reduce(into: [String: [TodoCalenderModel]]()){output, input in
            
            if (output[input.date] == nil){
                output[input.date] = []
                self.todoGroupList.append(input.date)
            }
            
            output[input.date]?.append(input)
            
            
        }
        
        if self.todoGroupList.count == 0{
            self.todoGroupList.append("Du hast in diesem Monat keine To-Dos")
        }
        
        self.removeSpinner()
        print("remove spinner")

        self.calenderCollectionView.reloadData()
        self.todoTableView.reloadData()
    }
    @IBAction func clickAddButton(_ sender: UIButton) {
        if self.addTodoButtonContainer.isHidden == true{
            self.addTodoButtonContainer.isHidden = false
            self.addTodoButtonContainer.isUserInteractionEnabled = true
        }else if self.addTodoButtonContainer.isHidden == false{
            self.addTodoButtonContainer.isHidden = true
            self.addTodoButtonContainer.isUserInteractionEnabled = false
            
        }
    }
    @IBAction func clickEcoScanButton(_ sender: UIButton) {
        let ecoScanStoryBoard = UIStoryboard(name: "EcoScan", bundle: nil)
        let ecoScanViewController = ecoScanStoryBoard.instantiateViewController(identifier: "ecoScanView") as! EcoScanViewController
        self.present(ecoScanViewController, animated: true, completion: nil)
    }
    
    
    @IBAction func onCalenderModeClick(_ sender: Any) {
        
        isLargeCalenderMode = !isLargeCalenderMode
        
        updateCalenderMode()
    }
    
    @IBAction func onPrevMonth(_ sender: Any) {
        self.showSpinner(onView: self.view)

        
        print("start spinner")
        
        currentMonth -= 1
        
        if currentMonth < 1{
            currentMonth += 12
            currentYear -= 1
        }
        
        
        DispatchQueue.global(qos: .background).asyncAfter(deadline: .now() + 0.01) {
            self.getTodoData()
            
            DispatchQueue.main.async {
                self.resetCalenderDay()
                
                self.updateCalenderContent()
                self.updateDateLabel()
            }
        }
        
    }
    
    @IBAction func onNextMonth(_ sender: Any) {
        currentMonth += 1
        self.showSpinner(onView: self.view)

        
        if currentMonth > 12{
            currentMonth -= 12
            currentYear += 1
        }
        
        
        DispatchQueue.global(qos: .background).asyncAfter(deadline: .now() + 0.01) {
            self.getTodoData()
            
            DispatchQueue.main.async {
                self.resetCalenderDay()
                
                self.updateCalenderContent()
                self.updateDateLabel()
            }
            
            
        }
    }
    
    @IBAction func onPrevYear(_ sender: Any) {
        
        self.showSpinner(onView: self.view)
        
        currentYear -= 1
        
        
        
        DispatchQueue.global(qos: .background).asyncAfter(deadline: .now() + 0.01) {
            self.getTodoData()
            
            DispatchQueue.main.async {
                self.updateCalenderContent()
                self.resetCalenderDay()
                self.updateDateLabel()
            }
            
        }
    }
    
    @IBAction func onNextYear(_ sender: Any) {
        self.showSpinner(onView: self.view)
        
        currentYear += 1
        
        
        
        DispatchQueue.global(qos: .background).asyncAfter(deadline: .now() + 0.01) {
            self.getTodoData()
            
            DispatchQueue.main.async {
                self.updateCalenderContent()
                self.resetCalenderDay()
                self.updateDateLabel()
            }
            
        }
    }
    
    @IBAction func onInfoButtonClicked(_ sender: Any) {
        
    }
    
    func goToEditTodoPage(todoId: Int){
        
        performSegue(withIdentifier: "editTodo", sender: todoId)
    }
    
    
    
    func deleteSingleTodo(todoId: Int){
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.TO_DO_LIST+"\(todoId)/false", nil, method: .delete, printRequest: true){response in
            
            DispatchQueue.global(qos: .background).async {
                self.getTodoData()
                
            }
            
            self.ShowAlert(message: "Speichern")
        }
    }
    
    func deleteTodos(todoId: Int){
        
        NetworkManager().requestDataAsync(type: String.self, APP_URL.TO_DO_LIST+"\(todoId)/true", nil, method: .delete, printRequest: true){response in
            DispatchQueue.global(qos: .background).async {
                self.getTodoData()
                
            }
            self.ShowAlert(message: "Speichern")
        }
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        
        if segue.identifier == "editTodo"{
            guard let selectedTodo = self.todoData?.TodoList!.first(where: {$0.Id == sender as! Int}) else {
                return
            }
            
            let controller = segue.destination as! TodoEditViewController
            controller.todoDelegate = self
            controller.todoData = selectedTodo
        }
        
        if segue.identifier == "ownTodo"{
            let controller = segue.destination as! OwnTodoViewController
            
            controller.parentView = self
        }
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

extension TodoViewController: UITableViewDataSource, UITableViewDelegate{
    func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        
        let dateText = (currentDay == 0 ? self.todoGroupList[section] : self.todoSelectedList)
        
        let dateValue = dateText.toDate("yyyy-MM-dd'T'hh:mm:ss")
        let weekDay = String.getDayString(day: dateValue?.dayNumberOfWeek() ?? 0)
        return "ab \(weekDay) \(dateValue!.toString(output: "dd.MM"))"
    }
    
    func tableView(_ tableView: UITableView, viewForHeaderInSection section: Int) -> UIView? {
        let returnedView = UIView(frame: CGRect(x: 0, y: 10, width: tableView.bounds.width , height: 180)) //set these values as necessary
        returnedView.applyTheme()
        
        let label = UILabel(frame: CGRect(x: 0, y: -10, width: tableView.bounds.width   , height: 50))
        
        label.font = UIFont.systemFont(ofSize: 16, weight: .semibold)
        
        
        
        let dateText = (currentDay == 0 ? self.todoGroupList[section] : self.todoSelectedList)
        
        let dateValue = dateText.toDate("yyyy-MM-dd'T'hh:mm:ss")
        let weekDay = String.getDayString(day: dateValue?.dayNumberOfWeek() ?? 0)
        //        return "ab \(weekDay) \(dateValue!.toString(output: "dd.MM"))"
        if let dateText = dateValue?.toString(output: "dd.MM") {
            label.text = "ab \(weekDay) \(dateText)"

        }else{
            label.text = dateText
            label.adjustsFontSizeToFitWidth = true
            returnedView.addSubview(label)

            return returnedView
        }
        
        returnedView.addSubview(label)
        
//        let dateNumber = Int( (dateValue?.toString(output: "dd"))!) ?? 33
//        
//        let monthNumber = Int((dateValue?.toString(output: "MM"))!) ?? 13
//        
//        let todayMonth = Int(Date().toString(output: "MM"))!
//        
//        let todayDay = Int(Date().toString(output: "dd"))!
//        
        if (dateValue! >= Date()){
            return returnedView
        }
//        if section > 0{
//            return returnedView
//        }
        
        let imageView = UIImageView(frame: CGRect(x: returnedView.frame.width - 66, y: 5, width: 20, height: 20))
        
        imageView.image = UIImage(named: "todo_filled")
//        let imageView = UIImageView(image: UIImage(named: "todo_filled"))
        
//        imageView.translatesAutoresizingMaskIntoConstraints = false
//
        returnedView.addSubview(imageView)

        return returnedView
    }
    
    func numberOfSections(in tableView: UITableView) -> Int {
        if currentDay == 0{
            print("todo group count is", self.todoGroupList.count)
            return self.todoGroupList.count
        }
        
        
        return todoSelectedList != "" ? 1 : 0
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        if currentDay == 0{
            return self.todoDateGroupData[self.todoGroupList[section]]?.count ?? 0
            
        }
        
        return self.todoDateGroupData[self.todoSelectedList]?.count ?? 0
        
        
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        //        let cell = TodoViewCell()
        let cell = tableView.dequeueReusableCell(withIdentifier: "TodoCell", for: indexPath) as! TodoViewCell
        print("todo group is", self.todoGroupList, indexPath)
        let keyValue = currentDay == 0 ? self.todoGroupList[indexPath.section] : self.todoSelectedList
        
        let value = self.todoDateGroupData[keyValue]?[indexPath.row]
        
        
        cell.parentView = self
        cell.key = indexPath.section
        cell.index = indexPath.row
        cell.data = value
        cell.parent = self.todoTableView
        cell.isTop = false
        cell.isBottom = false
        
        print("index of ", indexPath, "total count:", self.todoDateGroupData[self.todoGroupList[indexPath.section]]!.count)
        if indexPath.row == 0{
            cell.isTop = true
            print("is top")
        }
        if (indexPath.row + 1) >= self.todoDateGroupData[self.todoGroupList[indexPath.section]]!.count{
            cell.isBottom = true
            print("is bot")
            
        }
        cell.innerImageView.image = nil
        cell.imageURL = ""
        
        if value!.imageURL != nil{
            var processedURL = value!.imageURL
            print("todo image is:")
            print(processedURL)
            
            var finalURL = ""
            if value?.type == ToDoType.todo{
                finalURL = processedURL!.toURL(false, false, true)
            }
            else{
                finalURL = processedURL!.toURL(false, false, false)
            }
            
            cell.imageURL = finalURL
            print(finalURL)
            cell.loadImage()
            
        }
        cell.onConfigure(type: value!.type, title: value!.title, id: value!.id, date: value!.date, desc: value!.description)
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        //        let value = self.todoDateGroupData[self.todoGroupList[indexPath.section]]?[indexPath.row]
        //        print("value is", value!)
        //        self.goToTodoDetails(todoId: value!.CyclicId)
        
    }
    
    
    
    
    
}

extension TodoViewController: UICollectionViewDelegate, UICollectionViewDataSource, UICollectionViewDelegateFlowLayout{
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return calenderDayList.count
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "calenderCell", for: indexPath) as! TodoCalenderCollectionViewCell
        
        var isSelected = false
        
        if "\(currentDay)" == calenderDayList[indexPath.row].number && calenderDayList[indexPath.row].isWithinDisplayedMonth{
            isSelected = true
        }
        
        let dateList = self.todoGroupList.reduce(into: [String]()){out, inp in
            out.append(inp.toDateString("yyyy-MM-dd'T'hh:mm:ss", output: "d"))
        }
        
        var isTodo = false
        if dateList.contains("\(calenderDayList[indexPath.row].number)") && calenderDayList[indexPath.row].isWithinDisplayedMonth{
            isTodo = true
        }
        
        cell.onConfigure(data: calenderDayList[indexPath.row], isSelected: isSelected , hasTodo: isTodo )
        //        cell.dateLabel.text = calenderDayList[indexPath.row].number
        return cell
    }
    
    func collectionView(_ collectionView: UICollectionView, layout collectionViewLayout: UICollectionViewLayout, sizeForItemAt indexPath: IndexPath) -> CGSize {
        
        
        let width = self.calenderCollectionView.frame.width / 7
        
        return CGSize(width: width, height: width * 0.8)
        
    }
    
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        
        if !calenderDayList[indexPath.row].isWithinDisplayedMonth{
            return
        }
        
        var selectedDay = Int(calenderDayList[indexPath.row].number) ?? 0
        
        if currentDay != 0 && selectedDay == currentDay{
            currentDay = 0
        }
        else{
            currentDay = selectedDay
            
        }
        
        
        updateCalenderContent()
        
        calenderCollectionView.reloadData()
    }
}

private extension TodoViewController {
    // 1
    func monthMetadata(for baseDate: Date) throws -> MonthMetadata {
        // 2
        guard
            let numberOfDaysInMonth = calendar.range(
                of: .day,
                in: .month,
                for: baseDate)?.count,
            let firstDayOfMonth = calendar.date(
                from: calendar.dateComponents([.year, .month], from: baseDate))
        else {
            // 3
            throw CalendarDataError.metadataGeneration
        }
        // 4
        let firstDayWeekday = calendar.component(.weekday, from: firstDayOfMonth)
        
        // 5
        return MonthMetadata(
            numberOfDays: numberOfDaysInMonth,
            firstDay: firstDayOfMonth,
            firstDayWeekday: (firstDayWeekday - 1) == 0 ? 7 : (firstDayWeekday - 1))
    }
    
    enum CalendarDataError: Error {
        case metadataGeneration
    }
}
