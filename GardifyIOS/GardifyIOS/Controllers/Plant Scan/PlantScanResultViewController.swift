//
//  PlantScanResultViewController.swift
//  GardifyText
//
//  Created by Netzlab on 29.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class PlantScanResultViewController: UIViewController {

    @IBOutlet weak var scanResultTableView: UITableView!
    var scanResultData: ScanResult?
    var scanResultDbImages: [UIImage] = []
    var scanResultPlantNetImages: [UIImage] = []
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.getAllImages()
        self.configurePadding()
        // Do any additional setup after loading the view.
    }
    
    func getAllImages(){

        if scanResultData == nil{
            return
        }
        

        
        var dbUrlListBefore = scanResultData?.PnResults.InDb?.reduce(into: [String]()){ output, input in
            var raw = input.Images![0].SrcAttr
            output.append(raw.toURL(false, false))
        }
//
//        let plantNetUrlListBefore = scanResultData?.PnResults.results.reduce(into: [String]()){ output, input in
//            let raw = input.images[0].link
//            output.append(raw)
//        }
//
 
//
        getImageFiles(files: dbUrlListBefore!){images in
            self.scanResultDbImages = images
            self.scanResultTableView.reloadData()
        }

//        getImageFiles(files: plantNetUrlListBefore!){images in
//            print("image request 2 success")
//            self.scanResultPlantNetImages = images
//            self.scanResultTableView.reloadData()
//        }
//
//
  
   
        
//        scanResultDbImages = Array.init(repeating: placeholderImage, count: (scanResultData?.PnResults.InDb!.count)!)
        

        
//        var cnt1 = 0
//        scanResultData?.PnResults.InDb!.forEach({ (result) in
//            var newUrl = result.Images![0].SrcAttr
//            var index = cnt1
//            NetworkManager().getImageFromUrl(urlString: newUrl.toURL(false, false)){ image in
////                self.scanResultDbImages.append(image)
//                self.scanResultDbImages[index] = image
//                    self.scanResultTableView.reloadData()
//            }
//            cnt1 += 1
//        })
        
            var placeholderImage = UIImage(named: "PlantPlaceholder")!
        
                scanResultPlantNetImages = Array.init(repeating: placeholderImage, count: (scanResultData?.PnResults.results.count)!)
        
        var cnt2 = 0
        scanResultData?.PnResults.results.forEach({ (result) in
            var index = cnt2
            NetworkManager().getImageFromUrl(urlString: result.images[0].link){ image in
                self.scanResultPlantNetImages[index] = image
                    self.scanResultTableView.reloadData()
            }
            cnt2 += 1
        })

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

extension PlantScanResultViewController: UITableViewDataSource{
    
    func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        switch section {
        case 0:
            return "Ähnliche Pflanzen aus der gardify Datenbank"
        default:
            return "Ähnliche Pflanzen aus PlantNet"
        }
    }
    
    func numberOfSections(in tableView: UITableView) -> Int {
        2
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        switch section {
        case 0:
            return scanResultData?.PnResults.InDb?.count ?? 0
        default:
            return scanResultData?.PnResults.results.count ?? 0
        }
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "scanResultCell", for: indexPath) as! ScanResultViewCell
        
        cell.viewController = self
        
        var nameLatin: String? = ""
        var nameGerman: String? = ""
        
        switch indexPath.section {
        case 0:
            let result = scanResultData?.PnResults.InDb![indexPath.row]
            nameLatin = result?.NameLatin
            nameGerman = result?.NameGerman
            cell.tag = result!.Id
            cell.isInDb = true
        default:
            let result = scanResultData?.PnResults.results[indexPath.row]
            nameLatin = result!.species.scientificNameWithoutAuthor
            nameGerman = result!.species.commonNames.first
            cell.isInDb = false

//            cell.imageView?.image = scanResultPlantNetImages[indexPath.row]
            
        }
        
        cell.onConfigure(nameLatin: nameLatin ?? "", nameGerman: nameGerman ?? "")
        cell.plantImage.image = nil
//        cell.textLabel?.text = "\(dataResult ?? "")"
        
        switch indexPath.section {
        case 0:
            if self.scanResultDbImages.count <= indexPath.row{
                return cell
            }
            cell.plantImage.image = scanResultDbImages[indexPath.row]
        default:
            if self.scanResultPlantNetImages.count <= indexPath.row{
                return cell
            }
            cell.plantImage.image = scanResultPlantNetImages[indexPath.row]
            
        }
        
        
        return cell
    }
}
