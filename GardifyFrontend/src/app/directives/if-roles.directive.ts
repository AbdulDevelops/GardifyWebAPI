import { Directive, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { Role } from '../models/roles';
import { AuthService } from '../services/auth.service';

@Directive({
  // tslint:disable-next-line: directive-selector
  selector: '[ifRoles]'
})
export class IfRolesDirective implements OnInit, OnDestroy {
  private subs = new Subscription();
  @Input() public ifRoles: Role[];

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private auth: AuthService
  ) {}

  public ngOnInit(): void {
    // Remove element from DOM by default
    this.viewContainerRef.clear();

    this.subs.add(
      this.auth.roles().subscribe(roles => {
        // check if user is in role
        const idx = roles.findIndex((element) => this.ifRoles.indexOf(element) !== -1);
        if (idx >= 0) {
          // appends the ref element to DOM
          this.viewContainerRef.createEmbeddedView(this.templateRef);
        }
      })
    );
  }

  public ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
