//
//  SlideInTransition.swift
//  GardifyText
//
//  Created by Netzlab on 24.07.20.
//  Copyright © 2020 Netzlab. All rights reserved.
//

import UIKit

class SlideInTransition: NSObject, UIViewControllerAnimatedTransitioning {
    func transitionDuration(using transitionContext: UIViewControllerContextTransitioning?) -> TimeInterval {
        return 0.5
    }
    
    var isOpen = false
    
    func animateTransition(using transitionContext: UIViewControllerContextTransitioning) {
        guard let toController = transitionContext.viewController(forKey: .to)
            , let fromController = transitionContext.viewController(forKey: .from) else {
                return
        }
        let containerView = transitionContext.containerView
        let finalWidth = toController.view.bounds.width * 1
        
        
        let finalHeight = toController.view.bounds.height - 140
        
        if isOpen{
            containerView.addSubview(toController.view)
            
            toController.view.frame = CGRect(x: -finalWidth, y: 140, width: finalWidth, height: finalHeight)
        }
        
        let transform = {
            toController.view.transform = CGAffineTransform(translationX: finalWidth, y: 0)
        }
        
        let identity = {
            fromController.view.transform = .identity
        }
        
        let duration = transitionDuration(using: transitionContext)
        let isCancelled = transitionContext.transitionWasCancelled
        UIView.animate(withDuration: duration, animations: {
            self.isOpen ? transform() : identity()
        }) {(_) in
            transitionContext.completeTransition(!isCancelled)
        }
    }
    

}
