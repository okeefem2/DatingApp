import { Directive, Input, ViewContainerRef, TemplateRef } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective {

  private hasView = false;

  // Adding * on structural directive changes the element to an ng template

  @Input() set hasRoles(roles: string[]) {
    if (this.authService.checkRoles(roles) && !this.hasView) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (this.hasView) {
      this.viewContainerRef.clear();
      this.hasView = false;
    }
  }

  public constructor(private authService: AuthService,
                     private viewContainerRef: ViewContainerRef,
                     private templateRef: TemplateRef<any>) { }
}
