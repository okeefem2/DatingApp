import { FormGroup } from '@angular/forms';

export class ConfirmPassword {
  public static confirmPassword(formGroup: FormGroup): any {
    return formGroup.value.password === formGroup.value.confirmPassword ? null : {'mismatch': true};
  }
}
