import { Component, OnInit, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidationErrors } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { ConfirmPassword } from '../validators/confirm-password';
import { BsDatepickerConfig } from 'ngx-bootstrap';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  public registerForm: FormGroup;

  @Output() public register = new EventEmitter();
  @Output() public cancel = new EventEmitter();

  public bsDpConfig: Partial<BsDatepickerConfig>;

  constructor(private formBuilder: FormBuilder) { }

  public ngOnInit(): void {
    this.bsDpConfig = {
      containerClass: 'theme-orange'
    };
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.maxLength(32),
          Validators.minLength(8)
        ]
      ],
      confirmPassword: [
        '',
      ],
      gender: ['female'],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    }, {
      validator: [ConfirmPassword.confirmPassword]
    });
  }

  public onSubmit(): void {
    if (this.registerForm.valid) {
      this.register.emit(this.registerForm.value);
    }
  }

  public getControlErrors(controlName: string): ValidationErrors {
    return this.registerForm.get(controlName).errors;
  }

  public getControlTouched(controlName: string): boolean {
    return this.registerForm.get(controlName).touched;
  }
}
