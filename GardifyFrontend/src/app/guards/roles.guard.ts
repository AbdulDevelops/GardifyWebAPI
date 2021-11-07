import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { Role } from '../models/roles';

import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class RolesGuard implements CanActivate {
  roles: Role[];
  constructor(
    private router: Router,
    private auth: AuthService
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const inRole = this.auth.roles().pipe(map(roles => {
      const idx = roles.findIndex((element) => route.data.roles.indexOf(element) !== -1);
      if (idx >= 0) {
        return true;
      }
      // redirect to premium or alert 'Only premium'
      this.router.navigate(['/']);
      return false;
    }));

    return inRole;
  }
}
