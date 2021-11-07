import { EventEmitter } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { CurrentUser } from "./community-page-new.component";

export class CommunityService {

  public currentUser: BehaviorSubject<CurrentUser> = new BehaviorSubject<CurrentUser>(null);

  public valueObs: BehaviorSubject<string> = new BehaviorSubject<string>(null);

}