import { Injectable } from '@angular/core';
import { ShopItem } from '../models/models';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { StatCounterService } from './stat-counter.service';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  private shopItems:ShopItem[];
  private itemsInCartSubject: BehaviorSubject<ShopItem[]> = new BehaviorSubject([]);
  private itemsInCart:any[] = [];
  cartLenght : number = 0;
  constructor(
    private statCounter: StatCounterService
  ) { 
    this.itemsInCartSubject.subscribe(_ => this.itemsInCart = _);
  }
  findAll():ShopItem[]{
    return this.shopItems;
  }
  findItem(id:number):Observable<ShopItem>{
    return of(this.shopItems.find(item =>item.Id===id))
  }

  public getItems(): Observable<ShopItem[]> {
    return this.itemsInCartSubject.asObservable();
  }
  public getTotalALmount():Observable<Number>{
    return this.itemsInCartSubject.pipe(map((item:any[])=>{
      return item.reduce((prev,curr:any)=>{
        return prev + curr.NormalPrice;
      },0);
    }))

  }
  public removeFromCart(item: any) {
    
    const currentItems = [...this.itemsInCart];
    
      const itemsWithoutRemoved = currentItems.filter(_ => _.Id !== item.Id);
      this.itemsInCartSubject.next(itemsWithoutRemoved);
    
    
    
      localStorage.setItem('winkelwagen', 
      JSON.stringify(this.itemsInCart));
     
    }
  
  
}
